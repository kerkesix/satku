namespace KsxEventTracker.Cli.Commands.UploadDataCommand.Legacy
{
    using System;
    using System.Collections.Generic;

    using KsxEventTracker.Cli.Commands.UploadDataCommand.Legacy.Models;
    using KsxEventTracker.Domain.Aggregates.Checkpoint;

    using Checkpoint = KsxEventTracker.Cli.Commands.UploadDataCommand.Legacy.Models.Checkpoint;

    public class CheckpointsParser : ParserBase<Checkpoint>
    {
        // List of known checkpoint locations. Hack workaround to prevent parsing 
        // SQL Server hexadecimal locations. 
        private readonly Dictionary<string, SimpleGeography> knownLocations = new Dictionary<string, SimpleGeography>
        {
            // Porras
            { "0xE6100000010C149E0A9759604E40A314DF164DEF3740", new SimpleGeography(60.75273407, 23.93477004) },

            // Lopen raja
            { "0xE6100000010C00D1E5300C614E4061530860DA0B3840", new SimpleGeography(60.75818454, 24.04630089) },

            // Kusema
            { "0xE6100000010C3B545392F55C4E405B95C82EE62D3840", new SimpleGeography(60.72624425, 24.17929356) },

            // Pilpala
            { "0xE6100000010C22B899238E4F4E408A7867C437493840", new SimpleGeography(60.62152524, 24.28600719) },

            // Karkkila
            { "0xE6100000010CB58D3F51D9464E407FB407731E343840", new SimpleGeography(60.553507, 24.20358962) },

            // Vanjärvi
            { "0xE6100000010C367967407F3C4E401A749DA1133A3840", new SimpleGeography(60.47263341, 24.22686205) },

            // Vihti 
            { "0xE6100000010C8E3911F569354E4054A997E7DA523840", new SimpleGeography(60.41729606, 24.32365272) },

            // Hyvinkää 40
            { "0xE6100000010CA3D30C0CCD304E4072B2056B39693840", new SimpleGeography(60.38125754, 24.41103238) },

            // Lauri
            { "0xE6100000010C7938C30507314E403AF6A22C32923840", new SimpleGeography(60.38302681, 24.5710781) },

            // Velskola
            { "0xE6100000010C5C4B54B129284E408C0BAC883FA43840", new SimpleGeography(60.31377236, 24.64159445) },

            // Pirttimäki
            { "0xE6100000010CA9CAEF1B7C224E40CC99ABEEAAA63840", new SimpleGeography(60.26941251, 24.65104572) }
        };

        public CheckpointsParser(string csvRaw) :
            base(csvRaw, 8)
        {
        }

        public override Checkpoint ParseRow()
        {
            // Id;HappeningId;CheckpointType;DistanceFromPreviousCheckpoint;DistanceFromStart;Name;Order;Location
            return new Checkpoint
            {
                Id = this.Next<Guid>(),
                HappeningId = this.Next(),
                CheckpointType = this.Next<CheckpointType>(),
                DistanceFromPreviousCheckpoint = this.Next<decimal>(),
                DistanceFromStart = this.Next<decimal>(),
                Name = this.Next(),
                Order = this.Next<int>(),
                Location = this.ParseLocation(this.Next())
            };
        }

        private SimpleGeography ParseLocation(string sqlHexString)
        {
            if (string.IsNullOrWhiteSpace(sqlHexString))
            {
                return null;
            }

            string key = sqlHexString.Trim();

            return this.knownLocations.ContainsKey(key) ? this.knownLocations[key] : null;
        }
    }
}