namespace KsxEventTracker.Cli.Commands
{
    using Microsoft.WindowsAzure.Storage.Table;

    public class AllEventsOfType : QueryEventsBase, ICliCommandHandler
    {
        public string Usage => "targetEnvironment eventType";
        public string Help => "Queries all events of certain type from given environment";
    

        public AllEventsOfType()
            : base(
                "Must give event type as second argument.", 
                "Querying {0}-events from environment {1}.",
                GenerateQueryFilter)
        {
        }

        private static string GenerateQueryFilter(string queryArgument)
        {
            return TableQuery.GenerateFilterCondition(
                "DataType",
                QueryComparisons.Equal,
                queryArgument.Contains(".") ? queryArgument : "KsxEventTracker.Domain.Messages.Events." + queryArgument);
        }
    }
}