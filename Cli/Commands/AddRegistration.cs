namespace KsxEventTracker.Cli.Commands
{
    using System;
    using System.Threading.Tasks;
    using Domain.Repositories;
    using Newtonsoft.Json;

    using static System.Console;

    public class AddRegistration : ICliCommandHandler
    {
        const string sampleEntity =
            "\"{ Email:'foo@bar.com', Mobile:'040 1234566', Firstname:'John', Lastname:'Doe',Nickname:'walker',BeenThere:false,IsMember:true,Info:'whatever' }\"";

        public string Usage => "toEnvironment happening dataAsJson";
        public string Help => $"Adds a new registration row. E.g. AddRegistration production satkuxiv {sampleEntity}";

        public Task Execute(params string[] args)
        {
            if (args.Length != 3)
            {
                return Fail("Invalid arguments\r\n" + Usage);
            }

            var env = args[0];
            var partition = args[1];
            var serializedData = args[2];

            Write("Parsing and validating registration object... ");
            var registration = JsonConvert.DeserializeObject<RegistrationEntity>(serializedData);

            WriteLine("done.");
            WriteLine($"Adding registration row to env:{env} with partition key {partition}.");
            
            var repository = new RegistrationRepository(new EnvironmentClient(env).Options);

            registration.PartitionKey = partition;
            registration.Timestamp = DateTime.UtcNow;
            registration.ConfirmedAt = DateTime.UtcNow;
            registration.RowKey = Guid.NewGuid().ToString();
            
            if (string.IsNullOrWhiteSpace(registration.Nickname))
            {
                registration.Nickname = $"{registration.Lastname} {registration.Firstname}";
            }

            var result = repository.Add(registration);
            Task.WaitAll(result);

            var statusCode = result.Result.HttpStatusCode;

            return statusCode == 204 ? Task.FromResult(0) : Fail($"Insert returned HTTP status {statusCode}.");
        }

        private Task Fail(string message)
        {
            Console.WriteLine(message);
            return Task.FromResult(-1);
        }
    }
}