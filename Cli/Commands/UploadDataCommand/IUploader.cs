namespace KsxEventTracker.Cli.Commands.UploadDataCommand
{
    using System.Threading.Tasks;

    internal interface IUploader
    {
        Task Upload(string environment);
    }
}