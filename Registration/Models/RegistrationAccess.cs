namespace KsxEventTracker.Registration.Models
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using KsxEventTracker.Domain.Repositories;
    using Microsoft.Extensions.Options;

    public class RegistrationAccess : IAuthenticateRegistration
    {
        private readonly RegistrationRepository repository;

        public RegistrationAccess(IOptions<RegistrationOptions> optionsAccessor, RegistrationRepository registrationRepository)
        {
            Options = optionsAccessor.Value;
            repository = registrationRepository;
        }

        public RegistrationOptions Options { get; }

        public async Task<RegistrationAccessResult> AuthenticateRequest(string happening, string secret)
        {
            if (Options.Happening.Id != happening)
            {
                return RegistrationAccessResult.DoesNotExist;
            }

            // Check whether override secret matches - do not care about max attendee count on this situation
            if (!string.IsNullOrEmpty(secret) && Options.Happening.OverrideSecret.Equals(secret, StringComparison.OrdinalIgnoreCase))
            {
                return RegistrationAccessResult.OpenOverride;
            }

            // Normal case, check whether the event is open
            if (DateTime.UtcNow < Options.Happening.From.ToUniversalTime() ) 
            {
                return RegistrationAccessResult.NotYetOpen;
            }

            if (Options.Happening.To.ToUniversalTime() < DateTime.UtcNow) {
                return RegistrationAccessResult.AlreadyClosed;
            }

            // Finally check max attendee count
            var all = await this.repository.All(happening);
            var count = all.Count(r => r.ConfirmedAt != null);
            
            if (count >= this.Options.Happening.MaxAttendees) 
            {
                return RegistrationAccessResult.AlreadyClosed;
            }

            return RegistrationAccessResult.Open;
        }
    }

    public enum RegistrationAccessResult
    {
        Open,
        OpenOverride,
        DoesNotExist,
        NotYetOpen,
        AlreadyClosed
    }

    public interface IAuthenticateRegistration
    {
        Task<RegistrationAccessResult> AuthenticateRequest(string happening, string secret);
    }
}