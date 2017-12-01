namespace Web.QueryModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using KsxEventTracker.Domain.Messages.Events;

    public class EditPeople: List<Person>
    {
        public void HandleCreate(PersonCreated e) => Add(new Person(e)); 
  
        public void HandleUpdate(PersonContactInformationUpdated e)
        {
            var person = this.FirstOrDefault(
                m => m.PersonId.Equals(e.PersonId, StringComparison.OrdinalIgnoreCase));

            if (person != null)
            {
                person.UpdateContactInformation(e);
            }
        }

        public void HandleLinkedToHappening(PersonLinkedToHappening e)
        {
            var person = this.FirstOrDefault(
                m => m.PersonId.Equals(e.PersonId, StringComparison.OrdinalIgnoreCase));

            // Theoretically it could happen that this message comes before the PersonCreated.
            if (person == null)
            {
                throw new InvalidOperationException("Person does not exist yet, cannot link person to happening.");
            }

            person.HappeningsAttended.Add(e.HappeningId);
        }

        public void HandleUnlinkedFromHappening(PersonUnlinkedFromHappening e)
        {
            var person = this.FirstOrDefault(
                m => m.PersonId.Equals(e.PersonId, StringComparison.OrdinalIgnoreCase));

            // Theoretically it could happen that this message comes before the PersonCreated.
            if (person == null)
            {
                throw new InvalidOperationException("Person does not exist yet, cannot unlink person from happening.");
            }

            person.HappeningsAttended.Remove(e.HappeningId);
        }

        public void HandleDelete(PersonDeleted e)
        {
            RemoveAll(
                m => m.PersonId.Equals(e.PersonId, StringComparison.OrdinalIgnoreCase));
        }
    }
}