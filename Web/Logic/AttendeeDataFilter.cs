namespace Web.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Security.Principal;

    using Web.Models;

    public class AttendeeDataFilter : IAttendeeDataFilter
    {
        private readonly IPrincipal user;

        public AttendeeDataFilter(IPrincipal user) => this.user = user ?? throw new ArgumentNullException(nameof(user));

        public void HideAttendeeNames(IEnumerable<IAttendee> attendees)
        {
            if (user.IsContributor())
            {
                return;
            }

            int index = 1;

            foreach (IAttendee a in attendees)
            {
                bool hidden = this.HideAttendeeName(a, index);

                if (hidden)
                {
                    index++;
                }
            }
        }

        public bool HideAttendeeName(IAttendee attendee, int index = 0)
        {
            if (attendee == null)
            {
                return false;
            }

            bool hide = !this.user.IsContributor() && !attendee.MayShowNameToPublic;

            if (hide)
            {
                attendee.Name = index == 0 ?
                    "Anonyymi Kävelijä" :
                    string.Format(CultureInfo.CurrentCulture, "Kävelijä {0:N0}", index);
            }

            return hide;
        }

        public void HideContactDetails(IContactInfo contactInfo)
        {
            if (contactInfo != null && !this.user.IsContributor())
            {
                contactInfo.Phone = null;
            }
        }
    }
}