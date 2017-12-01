namespace KsxEventTracker.Cli.Commands
{
    using System;
    using System.Threading.Tasks;

    public class DeleteTable: ICliCommandHandler
    {
        public string Usage => "targetEnvironment:tableName [--allow-production]";
        public string Help => "Deletes a given table from given environment.";
          

        public async Task Execute(params string[] args)
        {
            var t = new TableTarget(args.Length > 0 ? args[0] : null);
            bool allowProductionDelete = args.Length > 1 && args[1].Equals("--allow-production");

            if (!t.Valid)
            {
                Console.Out.WriteLine("Invalid arguments");
                Console.Out.WriteLine(Usage);
                return;
            }

            // Ensure, that we are not accidentally deleting production tables
            if (!allowProductionDelete && t.Environment.Equals(EnvironmentClient.ProductionSuffix, StringComparison.OrdinalIgnoreCase))
            {
                Console.Out.WriteLine("Cannot delete a table from production.");
                return;
            }

            Console.Out.WriteLine("Deleting table {0} from environment {1}.", t.TableName, t.Environment);

            var targetTable = new EnvironmentClient(t.Environment).GetTable(t.TableName);

            if (!await targetTable.ExistsAsync())
            {
                Console.Out.WriteLine("Target table does not exist, exiting.");
                return;
            }

            await targetTable.DeleteAsync();
        }
    }
}