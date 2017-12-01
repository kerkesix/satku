namespace KsxEventTracker.Cli.Commands
{
    using System.Threading.Tasks;

    internal interface ICliCommandHandler
    {
        string Usage { get; }
        string Help { get; }

        Task Execute(params string[] args);
    }
}