namespace KsxEventTracker.Domain.Aggregates.Checkpoint
{
    /// <summary>
    /// Defines different types of endpoints.
    /// </summary>
    public enum CheckpointType
    {
        /// <summary>
        /// First checkpoint.
        /// </summary>
        Start = 0,

        /// <summary>
        /// Checkpoint that has two readings: checking in and out. 
        /// </summary>
        Pitstop = 1,

        /// <summary>
        /// Checkpoint that is a simple passthrough checkpoint, only one reading.
        /// </summary>
        Passthrough = 2,

        /// <summary>
        /// Last checkpoint.
        /// </summary>
        Finish = 3
    }
}