namespace Web.QueryModels
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class CsvDataFiles: Dictionary<string, CsvDataFile>
    {
        public EditPeople People { get; private set; }
        public Dictionary<string, CsvCheckpoint> Checkpoints { get; private set; } 

        public CsvDataFiles()
        {
            this.People = new EditPeople();
            this.Checkpoints = new Dictionary<string, CsvCheckpoint>();
        }
    }

    public class CsvCheckpoint
    {
        public string Name { get; set; }
        public decimal DistanceFromPrevious { get; set; }
    }

    public class CsvDataFile
    {
        public Dictionary<string, CsvScanRow> ScanRows { get; private set; }

        public CsvDataFile()
        {
            this.ScanRows = new Dictionary<string, CsvScanRow>();
        }

        public string GenerateFile()
        {
            var file = new StringBuilder();
                
            // Write headers
            file.AppendLine("Tapahtuma;LP id;LP;Aika;Luenta;Henkilö;Henkilön nimi");

            // Write content
            foreach (var row in ScanRows.Values)
            {
                file.AppendFormat(
                    "{0};{1};\"{2}\";{3};\"{4}\";{5};\"{6}\"",
                    row.HappeningId,
                    row.CheckpointId,
                    row.CheckpointName,
                    row.Timestamp,
                    row.Text,
                    row.PersonId,
                    row.PersonName);
                file.AppendLine();
            }

            return file.ToString();
        }
    }

    public class CsvScanRow
    {
        public string HappeningId { get; set; }
        public string CheckpointId { get; set; }
        public string CheckpointName { get; set; }
        public string PersonId { get; set; }
        public string PersonName { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string Text { get; set; }
    }
}