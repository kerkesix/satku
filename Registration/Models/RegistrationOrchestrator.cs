namespace KsxEventTracker.Registration.Models
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using System.Linq;

    using KsxEventTracker.Domain.Aggregates.Registration;
    using KsxEventTracker.Domain.Messages.Commands;
    using KsxEventTracker.Domain.Repositories;

    using SendGrid;
    using SendGrid.Helpers.Mail;
    using Microsoft.Extensions.Options;
    using Microsoft.Extensions.Logging;

    public enum RegistrationResult
    {
        Duplicate,
        InternalError,
        OK
    }

    public class RegistrationOrchestrator
    {
        private readonly RegistrationRepository repository;
        private EmailOptions options { get; }
        private ILogger<RegistrationOrchestrator> logger;


        public RegistrationOrchestrator(
            IOptions<RegistrationOptions> registrationOptionsAccessor,
            RegistrationRepository registrationRepository,
            ILogger<RegistrationOrchestrator> logger)
        {
            options = registrationOptionsAccessor.Value.Email;

            // TODO: Now integration is done via this DB, should move the admin UI also on this site to avoid
            repository = registrationRepository;
            this.logger = logger;
        }

        public async Task<RegistrationResult> Register(RegistrationCommand cmd)
        {
            void log(string format, params object[] args)
            {
                logger.LogInformation("Register {0} ({1}): {2}", cmd.Id, cmd.Email, string.Format(format, args));
            }

            log("Starting registration sequence");

            // Duplicate detection based on email (used to be on the queue, now do dummily here)
            var all = await repository.All(cmd.Happening);
            if (all.Any(r => r.Email.Equals(cmd.Email, StringComparison.OrdinalIgnoreCase)))
            {
                logger.LogWarning("Register {0} ({1}): Duplicate registration detected", cmd.Id, cmd.Email);
                return RegistrationResult.Duplicate;
            }

            var registrationEntity = new RegistrationEntity
            {
                PartitionKey = cmd.Happening,
                RowKey = cmd.Id.ToString(),
                Email = cmd.Email.ToLower(),
                Mobile = cmd.Mobile,
                Firstname = cmd.Firstname,
                Lastname = cmd.Lastname,
                Nickname = cmd.Nickname,
                Timestamp = cmd.Time,
                BeenThere = cmd.BeenThere,
                IsMember = cmd.IsMember,
                Info = cmd.Info
            };

            var result = await repository.Add(registrationEntity);
            if (result.HttpStatusCode >= 300)
            {
                logger.LogError("Register {0} ({1}): Persisting registration to DB failed with status code {2}", cmd.Id, cmd.Email, result.HttpStatusCode);
                return RegistrationResult.InternalError;
            }

            log("Registration persisted, HTTP Status code is {0}.", result.HttpStatusCode);

            // Send verification email - as this is not part of transaction doing this with queues would be pedant, but all of that was removed to reduce complexity.
            log("Sending verification email");
            logger.LogDebug("Using email API key {0}", options.APIKey);

            var msg = MailHelper.CreateSingleEmail(
                from: new EmailAddress(options.FromAddress, options.FromDisplayName),
                to: new EmailAddress(cmd.Email, cmd.Firstname + " " + cmd.Lastname),
                subject: options.ConfirmationSubject,
                plainTextContent: string.Format(CultureInfo.CurrentCulture, options.ConfirmationTemplate, cmd.EmailValidationUrl),
                htmlContent: null);

            var response = await new SendGridClient(options.APIKey).SendEmailAsync(msg);
            if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
            {
                logger.LogError("Register {0} ({1}): Email sending failed, SendGrid status code was {2}.", cmd.Id, cmd.Email, response.StatusCode);
                return RegistrationResult.InternalError;
            }

            log("Email sent, SendGrid status code was {0}.", response.StatusCode);
            return RegistrationResult.OK;
        }

        public async Task<ConfirmResult> Confirm(string happening, Guid id)
        {
            var result = await this.repository.Confirm(happening, id);

            if (result.Item1 == ConfirmResult.Success)
            {
                var entity = result.Item2;

                void log(string format, params object[] args)
                {
                    logger.LogInformation("Confirm {0} ({1}): {2}", entity.RowKey, entity.Email, string.Format(format, args));
                }


                log("Sending registration completed -email.");

                // Send email with payment information etc.
                string emailTemplate = options.CompletedTemplate;

                var from = new EmailAddress(options.FromAddress, options.FromDisplayName);
                var subject = string.Format(CultureInfo.CurrentCulture, options.CompletedSubject, DateTime.Now.Year);
                var to = new EmailAddress(entity.Email, entity.Firstname + " " + entity.Lastname);
                var plainTextContent = string.Format(
                                        CultureInfo.CurrentCulture,
                                        emailTemplate,
                                        entity.Mobile,
                                        entity.Nickname);
                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, null);

                var response = await new SendGridClient(options.APIKey).SendEmailAsync(msg);
                if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
                {
                    logger.LogError("Confirm {0} ({1}): Email sending failed, SendGrid status code was {2}.", entity.RowKey, entity.Email, response.StatusCode);
                }
                else
                {
                    log("Email sent, SendGrid status code was {0}.", response.StatusCode);
                }
            }

            return result.Item1;
        }
    }
}
