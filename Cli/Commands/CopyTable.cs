namespace KsxEventTracker.Cli.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.WindowsAzure.Storage.Table;

    public class CopyTable : ICliCommandHandler
    {
        public string Usage => "fromEnvironment:tableName toEnvironment:tableName";
        public string Help => "Copies a given table from source environment to target environment. E.g. CopyTable production:events qa:events-backup";
           

        public async Task Execute(params string[] args)
        {
            var stopWatch = Stopwatch.StartNew();

            var parsedArguments = TableCopyArguments.Parse(args);

            if (!parsedArguments.Valid)
            {
                Console.WriteLine("Must give source and target, both in a form environmentName:TableName.");
                return;
            }

            Console.Out.WriteLine("Copying {0} from {1} to {2} at {3}.",
                parsedArguments.Source.TableName,
                parsedArguments.Source.Environment,
                parsedArguments.Target.TableName,
                parsedArguments.Target.Environment);

            var sourceTable = new EnvironmentClient(parsedArguments.Source.Environment).GetTable(parsedArguments.Source.TableName);
            var targetTable = new EnvironmentClient(parsedArguments.Target.Environment).GetTable(parsedArguments.Target.TableName);

            if (!await sourceTable.ExistsAsync())
            {
                Console.Out.WriteLine("Source table does not exist, exiting.");
                return;
            }

            await targetTable.CreateIfNotExistsAsync();

            var allQuery = new TableQuery<DynamicTableEntity>();
            TableQuerySegment<DynamicTableEntity> querySegment = null;
            var sourceEntities = new List<DynamicTableEntity>();
            
            while(querySegment == null || querySegment.ContinuationToken != null)
            {
                querySegment = await sourceTable.ExecuteQuerySegmentedAsync(
                    allQuery, querySegment != null ? querySegment.ContinuationToken : null);
                sourceEntities.AddRange(querySegment);
            }

            var batches = new List<TableBatchOperation>();
            var groupedByPartitionKey = sourceEntities.GroupBy(
                p => p.PartitionKey,
                e => e,
                (key, list) => new KeyValuePair<string, IEnumerable<DynamicTableEntity>>(key, list));

            foreach (var g in groupedByPartitionKey)
            {
                const int BatchSize = 100;
                int i = 1;

                // If batch size is more than 100, chunk it as it is the max sixe for azure storage batch operation
                foreach (var chunk in g.Value.Chunk(BatchSize))
                {
                    Console.Out.WriteLine(g.Key + " - Batch " + i);
                    var batch = new TableBatchOperation();

                    foreach (var entity in chunk)
                    {
                        batch.Insert(entity);
                    }

                    batches.Add(batch);
                    i++;
                }
            }

            Console.Out.WriteLine("This might take a while...");
            var waits = batches.Select(targetTable.ExecuteBatchAsync).Cast<Task>().ToArray();

            Task.WaitAll(waits, TimeSpan.FromHours(1));

            Console.Out.WriteLine("\r\nCopying successful, copied {0:n0} rows in {1:n1} seconds.",
                sourceEntities.Count,
                stopWatch.Elapsed.TotalSeconds);
            return;

        }
    }

    internal class TableCopyArguments
    {
        private TableCopyArguments()
        {
        }

        public bool Valid
        {
            get
            {
                return Source.Valid && Target.Valid
                    // Overriding equals would be more idiomatic C# but takes much more lines
                       && !(Source.Environment.Equals(Target.Environment, StringComparison.OrdinalIgnoreCase)
                            && Source.TableName.Equals(Target.TableName, StringComparison.OrdinalIgnoreCase));
            }
        }

        public TableTarget Source { get; private set; }

        public TableTarget Target { get; private set; }

        public static TableCopyArguments Parse(string[] args)
        {
            if (args.Length < 2)
            {
                return new TableCopyArguments();
            }

            return new TableCopyArguments
                   {
                       Source = new TableTarget(args[0]),
                       Target = new TableTarget(args[1])
                   };
        }
    }

    internal class TableTarget
    {
        public TableTarget()
        {
        }

        public TableTarget(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return;
            }

            var parts = s.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
            {
                return;
            }

            this.Environment = parts[0].Trim();
            this.TableName = parts[1].Trim();
        }

        public string TableName { get; private set; }

        public string Environment { get; private set; }

        public bool Valid
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.TableName) && !string.IsNullOrWhiteSpace(Environment);
            }
        }
    }

    internal static class EnumerableExtensions
    {
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunksize)
        {
            while (source.Any())
            {
                yield return source.Take(chunksize);
                source = source.Skip(chunksize);
            }
        }
    }
}