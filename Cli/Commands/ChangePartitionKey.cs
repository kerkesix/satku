namespace KsxEventTracker.Cli.Commands
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.WindowsAzure.Storage.Table;

    public class ChangePartitionKey : ICliCommandHandler
    {
        public string Usage => "fromEnvironment:tableName id oldpartition newpartition";
        public string Help => "Changes row's partition key by copying and deleting the row.";
           

        public async Task Execute(params string[] args)
        {
            var parsedArguments = ChangePartitionKeyArguments.Parse(args);

            if (!parsedArguments.Valid)
            {
                Console.WriteLine("Must give source table, row id, and both current and new partition keys.");
                return;
            }

            Console.Out.WriteLine("Changing partition key to {4} for row {0}:{1} {2}-{3}.",
                parsedArguments.Source.Environment,
                parsedArguments.Source.TableName, 
                parsedArguments.Partition,
                parsedArguments.Id,
                parsedArguments.NewPartition);

            var sourceTable = new EnvironmentClient(parsedArguments.Source.Environment).GetTable(parsedArguments.Source.TableName);

            if (!await sourceTable.ExistsAsync())
            {
                Console.Out.WriteLine("Source table does not exist, exiting.");
                return;
            }

            var retrieveRow = TableOperation.Retrieve(parsedArguments.Partition, parsedArguments.Id);

            // TODO: Could use some error handling
            var rowToChange = (DynamicTableEntity) (await sourceTable.ExecuteAsync(retrieveRow)).Result;

            await sourceTable.ExecuteAsync(TableOperation.Delete(rowToChange)).ContinueWith(
                (Task t) => {
                    rowToChange.PartitionKey = parsedArguments.NewPartition;
                    return sourceTable.ExecuteAsync(TableOperation.Insert(rowToChange));
                });
        }
    }

    internal class ChangePartitionKeyArguments
    {
        private ChangePartitionKeyArguments()
        {
        }

        public bool Valid
        {
            get
            {
                return Source.Valid && !string.IsNullOrWhiteSpace(Id)
                    && !string.IsNullOrWhiteSpace(Partition) && !string.IsNullOrWhiteSpace(NewPartition);
            }
        }

        public TableTarget Source { get; private set; }

        public string Id { get; private set; }

        public string Partition { get; private set; }

        public string NewPartition { get; private set; }

        public static ChangePartitionKeyArguments Parse(string[] args)
        {
            if (args.Length < 4)
            {
                return new ChangePartitionKeyArguments();
            }

            return new ChangePartitionKeyArguments
                   {
                       Source = new TableTarget(args[0]),
                       Id = args[1],
                       Partition = args[2],
                       NewPartition = args[3]
                   };
        }
    }
}