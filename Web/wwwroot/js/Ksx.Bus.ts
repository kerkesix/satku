/// <reference path="JSBus/Bus.ts" />
/// <reference path="JSBus/SignalRSendTransport.ts" />
/// <reference path="JSBus/SignalRSubscribeTransport.ts" />
/// <reference path="Messaging/Queues.ts" />
/// <reference path="Ksx.Time.ts" />

declare var signalR: any;

module Ksx {
    export class Bus {
        private static readonly lazyInitialize: JQueryDeferred<any> = $.Deferred();

        public static bus: JSBus.Bus;
        public static readonly initialize: JQueryPromise<any> = Bus.lazyInitialize.promise();

        // Can't use constructor as we need static init. This is called below.
        static init() {
            // Connection extracted to function to be able to reconnect
            var connect = () => {
                let connection = new signalR.HubConnection('/commandbus', {
                    transport: signalR.TransportType.WebSockets,
                    logger: new signalR.ConsoleLogger(signalR.LogLevel.Information)
                });

                // Errors and warnings
                connection.on('error', Messaging.Queues.error.publish);
                connection.on('warning', Messaging.Queues.warning.publish);

                // Events from server
                connection.on('publishClientEvent', Messaging.Queues.eventsFromServer.publish);

                // Subscribe transport must be initialized before SignalR connection is started
                var sendTransport = new JSBus.SignalRSendTransport(connection),
                    subscribeTransport = new JSBus.SignalRSubscribeTransport(connection);

                // Wire up reconnect
                connection.onclose(e => {
                    console.log("Disconnected", e);
                    console.log("Reconnecting...");
                    connect();
                });

                connection.start()
                    .then(() => {
                        Bus.bus = new JSBus.Bus("commands", sendTransport, subscribeTransport);

                        // Route ack messages as they are needed in scan log
                        sendTransport.notifyWhenSent(Messaging.Queues.commandAck.publish);
                        Bus.lazyInitialize.resolve(Bus.bus);
                    },
                    () => {
                        Messaging.Queues.error.publish("Yhteyden ottaminen palvelimelle epäonnistui");
                        // Try again in 10 seconds
                        setTimeout(() => connect(), 10000);
                    });
            }

            connect();
        }

        static s4() {
            return Math.floor((1 + Math.random()) * 0x10000)
                .toString(16)
                .substring(1);
        }

        // This is needed in bus usage, thus introduce it here. Source:
        // http://stackoverflow.com/questions/105034/how-to-create-a-guid-uuid-in-javascript
        static guid() {
            return Bus.s4() + Bus.s4() + '-' + Bus.s4() + '-' + Bus.s4() + '-' + Bus.s4() + '-' + Bus.s4() + Bus.s4() + Bus.s4();
        };

        static createCommand(name, properties) {
            var command = {
                id: Bus.guid(),
                name: name,
                timestamp: Ksx.Time.ServerTime
            };

            for (var p in properties || []) {
                // Ignore reserved properties
                if (properties.hasOwnProperty(p) && p !== 'Id' && p !== 'id' && p !== 'Name' && p !== 'name') {
                    command[p] = properties[p];
                }
            }

            return command;
        };
    }

    Bus.init();
}