using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using KsxEventTracker.Registration.Models;
using KsxEventTracker.Domain.Aggregates.Registration;
using KsxEventTracker.Domain.Messages.Commands;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace KsxEventTracker.Registration.Controllers
{
    public partial class RegistrationController : Controller
    {
        private readonly IAuthenticateRegistration access;
        private readonly RegistrationOptions options;
        private readonly RegistrationOrchestrator orchestrator;
        private readonly ILogger<RegistrationController> logger;

        public RegistrationController(
            IAuthenticateRegistration access, 
            IOptions<RegistrationOptions> optionsAccessor,
            RegistrationOrchestrator orchestrator,
            ILogger<RegistrationController> logger)
        {
            this.access = access;
            this.options = optionsAccessor.Value;
            this.orchestrator = orchestrator;
            this.logger = logger;
        }

        public async Task<IActionResult> Register(string happening, [Bind(Prefix = "s")] string memberOverride)
        {
            var status = await access.AuthenticateRequest(happening, memberOverride);
            switch (status)
            {
                case RegistrationAccessResult.NotYetOpen:
                    return View("RegistrationNotYetOpen");
                case RegistrationAccessResult.AlreadyClosed:
                    return View("RegistrationAlreadyClosed");
                case RegistrationAccessResult.DoesNotExist:
                    return NotFound();
                default:
                    break;
            }

            ViewBag.MemberOverride = memberOverride;
            return View("Register", new RegistrationViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            string happening, 
            [Bind(Prefix = "s")] string memberOverride,
            RegistrationViewModel model)
        {
            var status = await access.AuthenticateRequest(happening, memberOverride);
            switch (status)
            {
                case RegistrationAccessResult.NotYetOpen:
                    return View("RegistrationNotYetOpen");
                case RegistrationAccessResult.AlreadyClosed:
                    return View("RegistrationAlreadyClosed");
                case RegistrationAccessResult.DoesNotExist:
                    return NotFound();
                default:
                    break;
            }

            // TODO: Check, that given nickname is unique (or just let it be and add suffix automatically)

            if (!ModelState.IsValid)
            {
                return View("Register", model);
            }

            Guid registrationId = Guid.NewGuid();
            logger.LogInformation("Received a new valid registration form. Assigning id " + registrationId);

            string emailValidationUrl = Url.Action(
                "Confirm",
                "Registration",
                new { happening = happening, id = registrationId },
                Request.Scheme);

            var command = new RegistrationCommand
                              {
                                  Id = registrationId,
                                  Happening = happening,
                                  Time = DateTime.UtcNow,
                                  Email = model.Email.Trim(),
                                  Mobile = model.Mobile.Trim(),
                                  Firstname = model.Firstname.Trim(),
                                  Lastname = model.Lastname.Trim(),
                                  Nickname = model.Nickname.Trim(),
                                  BeenThere = model.BeenThere,
                                  IsMember = !string.IsNullOrWhiteSpace(memberOverride),
                                  Info = model.Info,
                                  EmailValidationUrl = emailValidationUrl
                              };

            var result = await orchestrator.Register(command);
            switch (result)
            {
                case RegistrationResult.Duplicate:
                    return View("Duplicate");
                case RegistrationResult.InternalError:
                    return View("Error");
                case RegistrationResult.OK:
                default:
                    logger.LogInformation(registrationId + ": Registration command sent to queue without errors, redirecting client.");
                    return RedirectToAction("WaitingForConfirmation");
            }
        }

        public IActionResult WaitingForConfirmation()
        {
            return View();
        }

        public async Task<IActionResult> Confirm(string happening, Guid id)
        {
            logger.LogInformation(id + ": Received a new registration confirmation request.");

            var result = await orchestrator.Confirm(happening, id);
            logger.LogInformation(id + ": Confirm result is " + result.ToString("g"));

            switch (result)
            {
                case ConfirmResult.Success:
                case ConfirmResult.AlreadyConfirmed:
                    return View("Success");

                case ConfirmResult.NotFound:
                    return NotFound();

                default:
                    return View("Fail");
            }
        }
    }
}
