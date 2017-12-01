namespace KsxEventTracker.Cli.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using KsxEventTracker.Domain.Repositories;

    using Microsoft.WindowsAzure.Storage.Table;

    public class QueryEventsBase
    {
        private readonly string queryArgumentMissingError;

        private readonly string queryingInfo;

        private readonly Func<string, string> query;

        public QueryEventsBase(string queryArgumentMissingError, string queryingInfo, Func<string, string> query)
        {
            this.queryArgumentMissingError = queryArgumentMissingError;
            this.queryingInfo = queryingInfo;
            this.query = query;
        }

        public async Task Execute(params string[] args)
        {
            var stopWatch = Stopwatch.StartNew();

            string target;
            if (!ArgsHelper.GetTargetEnvironmentFromArgs(args, out target, true))
            {
                return;
            }

            string queryArg = args.Length >= 2 ? args[1] : null;
            if (string.IsNullOrWhiteSpace(queryArg))
            {
                Console.Out.WriteLine(this.queryArgumentMissingError);
                return;
            }

            Console.Out.WriteLine(this.queryingInfo, queryArg, target);

            var targetTable = new EnvironmentClient(target).GetTable("events");
            var exists = await targetTable.ExistsAsync();

            if (!exists)
            {
                Console.Out.WriteLine("Event store does not exist, exiting.");
                return;
            }


            var query = new TableQuery<EventEntity>().Where(this.query(queryArg));
            TableQuerySegment<EventEntity> querySegment = null;
            var foundEntities = new List<EventEntity>();
            
            while(querySegment == null || querySegment.ContinuationToken != null)
            {
                querySegment = await targetTable.ExecuteQuerySegmentedAsync(
                    query, querySegment != null ? querySegment.ContinuationToken : null);
                foundEntities.AddRange(querySegment);
            }

            foreach (var e in foundEntities)
            {
                Console.Out.WriteLine(
                    "PartitionKey:   {0}\r\n"
                    + "RowKey:         {1}\r\n"
                    + "DataType:       {2}\r\n"
                    + "EventTimeStamp: {3}\r\n"
                    + "{4}\r\n",
                    e.PartitionKey,
                    e.RowKey,
                    e.DataType,
                    e.EventTimestamp,
                    e.Data);
            }

            Console.Out.WriteLine("Search complete, found {0:n0} events in {1:n1} seconds.",
                foundEntities.Count,
                stopWatch.Elapsed.TotalSeconds);

            return;
        }
    }
}