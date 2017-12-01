namespace Web.QueryModels
{
    using System.Collections.Generic;

    public class LinkPeople: List<LinkPerson>
    {
    }

    public class LinkPerson
    {
        public string PersonId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public Dictionary<string, string> Happenings { get; } = new Dictionary<string, string>();
    }
}