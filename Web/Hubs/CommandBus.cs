using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using KsxEventTracker.Domain.Messages;
using KsxEventTracker.Domain.Messages.Commands;
using KsxEventTracker.Domain.Messages.Handlers;
using KsxEventTracker.Domain.Repositories;
using Web.Logic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace Web.Hubs
{
    public class CommandBus : Hub
    {
        public const string PublicGroupName = "public";
        public const string AuthenticatedGroupName = "authenticated";

        private readonly ILogger logger;

        private readonly CommandRepository commandRepository;

        private readonly CommandTypes commands = new CommandTypes();

        private readonly CommandHandlerFactory commandHandlerFactory;
        private readonly List<string> userMutex = new List<string>();
        private readonly TelemetryClient tc = new TelemetryClient();


        public CommandBus(ILoggerFactory loggerFactory, CommandRepository repository, CommandHandlerFactory handlerFactory)
        {
            this.logger = loggerFactory.CreateLogger("Realtime");
            this.commandRepository = repository;
            this.commandHandlerFactory = handlerFactory;
        }

        private string UsersGroupName
        {
            get
            {
                return Context.User.IsContributor() ? AuthenticatedGroupName : PublicGroupName;
            }
        }

        public override async Task OnConnectedAsync()
        {
            this.logger.LogInformation("{0}: joined group {1}", Context.ConnectionId, this.UsersGroupName);
            await Groups.AddAsync(Context.ConnectionId, this.UsersGroupName);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            this.logger.LogInformation("{0}: left group {1} with err {2}", Context.ConnectionId, this.UsersGroupName, exception);
            await Groups.RemoveAsync(Context.ConnectionId, this.UsersGroupName);
            await base.OnDisconnectedAsync(exception);
        }

        [Authorize]
        public async Task Command(object command)
        {
            var telemetryProps = new Dictionary<string, string>{
                    { "connection", this.Context.ConnectionId},
                    { "user", this.Context.User.Identity.Name }
                };

            // In this method only one message for the user can be running at the time. Not very efficient nor 
            // reliable way of doing this, but with the low volumes of this app this should be more than fine.
            if (this.userMutex.Contains(this.Context.ConnectionId))
            {
                this.logger.LogError($"Simultaneous commands for user {this.Context.ConnectionId}");
                tc.TrackEvent("parallelcommand", telemetryProps);
                await Clients.Client(Context.ConnectionId).InvokeAsync("error", "Palvelimella käsitellään vielä edellistä pyyntöäsi, uutta pyyntöä ei voida vastaanottaa");
                return;
            }

            this.userMutex.Add(this.Context.ConnectionId);

            try
            {
                // Can't use built in model mapping as it is too limited for receiving various 
                // kinds of commands at the same endpoint. 
                // We know that the incoming model is always JSON object:
                var jsonObject = (JObject)command;

                // All commands have these base properties, this is 
                // the way to get exact type for next conversion.
                var jsCommand = jsonObject.ToObject<GenericJSCommand>();

                this.logger.LogInformation($"{jsCommand.Id}:{jsCommand.Name} Received");

                telemetryProps.Add("type", jsCommand.Name);
                tc.TrackEvent("command", telemetryProps);

                Type targetCommandType = this.commands.CreateCommandTypeFromCommandName(jsCommand.Name);

                if (targetCommandType == null)
                {
                    this.logger.LogError($"{jsCommand.Id}:{jsCommand.Name} Could not find command type");
                    tc.TrackEvent("commandtypenotfound", telemetryProps);

                    // Acknowledge, as the message was received but could not be handled properly - this is a 
                    // configuration error that won't be fixed by retrying --> ack the message
                    await Clients.Client(Context.ConnectionId).InvokeAsync("ack", jsCommand.Id);
                    await Clients.Client(Context.ConnectionId).InvokeAsync("error", $"Toimintoa ei löytynyt, toimintoa {jsCommand.Name} ei voitu suorittaa.");
                    return;
                }

                var baseCommand = (ICommand)jsonObject.ToObject(targetCommandType);

                // Copy message properties to command context
                baseCommand.Context.User = Context.User.Identity.Name;
                baseCommand.Context["ConnectionId"] = Context.ConnectionId;
                baseCommand.Context["Timestamp"] = jsCommand.Timestamp;
                baseCommand.Context["MessageType"] = targetCommandType.FullName;

                if (await commandRepository.Contains(baseCommand))
                {
                    // This command was already in the data store, exit
                    this.logger.LogWarning($"{jsCommand.Id}:{jsCommand.Name} Already handled, skipping");
                    await Clients.Client(Context.ConnectionId).InvokeAsync("ack", baseCommand.Id);
                    return;
                }

                // Process the command, fail if no one handled it
                // Reflection based call must know the exact type of the handler, thus RedirectToWhen-logic 
                // is in each command handler separately. Not pretty but works.
                // Only one handler should handle a command --> exit loop on first match
                var handled = commandHandlerFactory.All.Any(handler => handler.TryExecute((ICommand)baseCommand));
                if (!handled)
                {
                    var s = $"{jsCommand.Id}:{jsCommand.Name} Handler not found for command type {targetCommandType.FullName}";
                    this.logger.LogError(s);
                    this.tc.TrackEvent("handlernotfound", telemetryProps);
                    throw new InvalidOperationException(s);
                }

                // Save command to repository
                await commandRepository.Add(baseCommand, baseCommand.Context.User);

                // Tell client that this command has been received, no need to resend. This was more useful when all
                // operations were async and went through cloud queues.
                await Clients.Client(Context.ConnectionId).InvokeAsync("ack", baseCommand.Id);
            }
            catch (Exception e)
            {
                this.tc.TrackException(e, telemetryProps);
                this.logger.LogError("Dispatching command failed: " + e.ToString());

                try
                {
                    await Clients.Client(Context.ConnectionId).InvokeAsync("error", "Toiminto kaatui tuntemattomaan virheeseen.");
                }
                catch
                {
                    this.logger.LogError("Sending error to SignalR client failed");
                }
            }
            finally
            {
                this.userMutex.Remove(this.Context.ConnectionId);
            }
        }
    }
}