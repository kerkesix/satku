namespace KsxEventTracker.Cli.Commands
{
    using Microsoft.WindowsAzure.Storage.Table;

    public class AllEventsForPartitionKey : QueryEventsBase, ICliCommandHandler
    {
        public string Usage => "targetEnvironment partitionKey";
        public string Help => "Queries all events with a single partition key from given environment";

        public AllEventsForPartitionKey()
            : base(
                "Must give partition key as second argument.",
                "Querying all events for partition {0} from environment {1}.",
                GenerateQueryFilter)
        {
        }

        private static string GenerateQueryFilter(string queryArgument)
        {
            return TableQuery.GenerateFilterCondition(
                "PartitionKey",
                QueryComparisons.Equal,
                queryArgument);
        }
    }
}