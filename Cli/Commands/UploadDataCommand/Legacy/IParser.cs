namespace KsxEventTracker.Cli.Commands.UploadDataCommand.Legacy
{
    using System.Collections.Generic;

    public interface IParser<out T>
    {
        IEnumerable<T> Rows { get; }
    }
}