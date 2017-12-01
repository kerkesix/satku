namespace Web.Logic
{
    using System.Collections.Generic;

    using Web.Models;

    public interface IAttendeeDataFilter
    {
        void HideAttendeeNames(IEnumerable<IAttendee> attendees);

        bool HideAttendeeName(IAttendee attendee, int index = 0);

        void HideContactDetails(IContactInfo contactInfo);
    }
}