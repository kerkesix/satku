namespace Web.Models
{
    using System;

    public interface IReadingCalculation
    {
        string Id { get; set; }

        DateTime Timestamp { get; set; }
    }
}