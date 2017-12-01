namespace KsxEventTracker.Domain.Repositories
{
    using System.Threading.Tasks;

    using KsxEventTracker.Domain.Messages.Commands;

    using Microsoft.WindowsAzure.Storage.Table;

    public class CommandRepository: AzureTableStorageRepositoryBase
    {
        private const string PartitionKey = "ClientCommand";

        public CommandRepository(AzureTableStorageOptions options)
            : base("commands", options)
        {
        }

        /// <summary>
        /// Checks whether given command has already been stored.
        /// </summary>
        /// <param name="command">The command to check.</param>
        /// <returns>True, if exists.</returns>
        public async Task<bool> Contains(ICommand command)
        {
            var retrieveResult = await this.Table.ExecuteAsync(TableOperation.Retrieve<CommandEntity>(PartitionKey, command.Id));
            return retrieveResult.Result != null;
        }

        public async Task Add(ICommand command, string userName)
        {
            // TODO: Use happening id as part of the partition key for better queries
            // + DateTime.UtcNow.ToString("yyyy-MM-dd");

            var commandEntity = new CommandEntity(
                PartitionKey, 
                command.Id, 
                userName,
                command);

            await this.Table.ExecuteAsync(TableOperation.Insert(commandEntity));
        }
    }
}