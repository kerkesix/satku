namespace KsxEventTracker.Cli.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.WindowsAzure.Storage.Table;

    public class DeleteRows : ICliCommandHandler
    {
        public string Usage => "targetEnvironment:tableName partitionkey rowid[,rowid2,rowid,...]";
        public string Help => "Deletes given rows from given environment and table";


        public async Task Execute(params string[] args)
        {
            var parsed = DeleteRowsArguments.Parse(args);

            if (!parsed.Valid)
            {
                Console.Out.WriteLine("Must give target table, partitionkey, and rowid(s).");
                return;                
            }

            Console.Out.WriteLine(
                "Deleting rows \r\n{0}\r\n from environment table {1}:{2}.", 
                string.Join("\r\n", parsed.RowIds), 
                parsed.Target.Environment,
                parsed.Target.TableName);

            var targetTable = new EnvironmentClient(parsed.Target.Environment)
                .GetTable(parsed.Target.TableName);

            if (!await targetTable.ExistsAsync())
            {
                Console.Out.WriteLine("Target table does not exist, exiting.");
                return;
            }

            var deleteOperations = parsed.RowIds
                .Select(rowId => new DynamicTableEntity(parsed.PartitionKey, rowId, "*", new Dictionary<string, EntityProperty>()))
                .Select(entity => targetTable.ExecuteAsync(TableOperation.Delete(entity))).ToList();

            await Task.WhenAll(deleteOperations);
        }
    }

    internal class DeleteRowsArguments
    {
        private DeleteRowsArguments()
        {
        }

        public bool Valid
        {
            get
            {
                return Target.Valid && !string.IsNullOrWhiteSpace(PartitionKey) && RowIds.Any();
            }
        }

        public TableTarget Target { get; private set; }

        public string PartitionKey { get; set; }
        public IEnumerable<string> RowIds { get; set; }


        public static DeleteRowsArguments Parse(string[] args)
        {
            if (args.Length < 2)
            {
                return new DeleteRowsArguments();
            }

            return new DeleteRowsArguments
            {
                Target = new TableTarget(args[0]),
                PartitionKey = args[1],
                RowIds = args[2].Split(",".ToCharArray())
            };
        }
    }
}