namespace KsxEventTracker.Cli.Commands.UploadDataCommand.Legacy.Models
{
    public class SimpleGeography
    {
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }

        public SimpleGeography(double latitude, double longitude)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
        }
    }
}