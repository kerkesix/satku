namespace KsxEventTracker.Cli
{
    using System;

    public class ArgsHelper
    {
        public static bool GetTargetEnvironmentFromArgs(string[] args, out string target, bool allowProduction = false)
        {
            if (args.Length == 0)
            {
                Console.Out.WriteLine("Must give target environment as first argument.");
                target = null;
                return false;
            }

            target = args[0];

            if (!allowProduction && target.Equals(EnvironmentClient.ProductionSuffix, StringComparison.OrdinalIgnoreCase))
            {
                Console.Out.WriteLine("Cannot use production as target environment.");
                return false;
            }

            return true;
        }
    }
}