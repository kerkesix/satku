namespace Web.QueryModels
{
    using System.Collections.Generic;

    public class EditHappenings: List<Happening>
    {
    }

    public class Happening
    {
        public string Key { get; set; }

        public bool IsDefault { get; set; }

        public int CheckpointCount { get; set; }

        public string Path { get; set; }
    }
}