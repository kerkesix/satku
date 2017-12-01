namespace Web.QueryModels
{
    // TODO: Might want to use Lazy values to handle situation where initialization is still ongoing when first request comes in
    public static class QueryModelRepository
    {
        public static EditHappenings EditHappenings { get; } = new EditHappenings();
        public static Routing Routing { get; } = new Routing();
        public static EditPeople EditPeople { get; } = new EditPeople();
        public static Dashboard Dashboard { get; } = new Dashboard();
        public static LinkPeople LinkPeople { get; } = new LinkPeople();
        public static CsvDataFiles Csv { get; } = new CsvDataFiles();

        public static bool IsInitialized => EditHappenings != null; 
    }
}