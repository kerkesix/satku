namespace Web.Hubs
{
    using System;

    public interface IGenericJSCommand
    {
        string Id { get; }

        string Name { get; }
        DateTimeOffset Timestamp { get; set; }
    }
}