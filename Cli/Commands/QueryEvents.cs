namespace KsxEventTracker.Cli.Commands
{
    public class QueryEvents : QueryEventsBase, ICliCommandHandler
    {
        public string Usage => "targetEnvironment query";
        public string Help => "Queries all events that match query, e.g. PartitionKey eq 'foo' and EventType eq 'bar'";

        public QueryEvents()
            : base(
                "Must give query as second argument.",
                "Querying {0} from environment {1}.",
                GenerateQueryFilter)
        {
        }

        private static string GenerateQueryFilter(string queryArgument)
        {
            return queryArgument;
        }
    }
}