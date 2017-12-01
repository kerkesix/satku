namespace Web.QueryModels
{
    using System.Collections.Generic;

    public class Dashboard
    {
        public Dictionary<string, HappeningDashboard> Happenings { get; private set; }
        public EditPeople People { get; private set; }

        public Dashboard()
        {
            this.Happenings = new Dictionary<string, HappeningDashboard>();
            this.People = new EditPeople();
        }
    }
}