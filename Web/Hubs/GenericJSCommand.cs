namespace Web.Hubs
{
    using System;

    public class GenericJSCommand : IGenericJSCommand
    {
        public string Id { get; set; }

        public string Name { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}