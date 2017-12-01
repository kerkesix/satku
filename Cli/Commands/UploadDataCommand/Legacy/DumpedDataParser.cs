namespace KsxEventTracker.Cli.Commands.UploadDataCommand.Legacy
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    using KsxEventTracker.Cli.Commands.UploadDataCommand.Legacy.Models;

    public class DumpedDataParser
    {
        public DumpedDataParser()
        {
            this.Happenings = this.ParseEntities<Happening, HappeningsParser>("Happenings.csv");
            this.Checkpoints = this.ParseEntities<Checkpoint, CheckpointsParser>("Checkpoints.csv");
            this.People = this.ParseEntities<Person, PeopleParser>("People.csv");
            this.Attendees = this.ParseEntities<Attendee, AttendeesParser>("Attendees.csv");
            this.Readings = this.ParseEntities<AttendeeExtended<Reading>, ReadingsParser>("Readings.csv");
            this.Quitters = this.ParseEntities<AttendeeExtended<Quitter>, QuittersParser>("Quitters.csv");
        }

        public IList<Happening> Happenings { get; private set; }

        public IList<Checkpoint> Checkpoints { get; private set; }

        public IList<Person> People { get; private set; }

        public IList<Attendee> Attendees { get; private set; }

        public IList<AttendeeExtended<Reading>> Readings { get; private set; }

        public IList<AttendeeExtended<Quitter>> Quitters { get; private set; }

        private List<T> ParseEntities<T, TK>(string fileName) where TK : IParser<T>
        {
            string raw = this.GetEmbeddedResource(fileName);
            var parser = (TK)Activator.CreateInstance(typeof(TK), raw);
            return new List<T>(parser.Rows);
        }

        private string GetEmbeddedResource(string resourceName)
        {
            using (
                var stream = typeof(DumpedDataParser).GetTypeInfo().Assembly.GetManifestResourceStream(this.GetType().Namespace + "." + resourceName))
            {
                if (stream == null)
                {
                    throw new Exception("Resource " + resourceName + " not found");
                }

                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}