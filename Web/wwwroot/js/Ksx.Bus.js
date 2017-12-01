var Ksx;
(function (Ksx) {
    var Bus = (function () {
        function Bus() {
        }
        Bus.init = function () {
            var connect = function () {
                var connection = new signalR.HubConnection('/commandbus', {
                    transport: signalR.TransportType.WebSockets,
                    logger: new signalR.ConsoleLogger(signalR.LogLevel.Information)
                });
                connection.on('error', Messaging.Queues.error.publish);
                connection.on('warning', Messaging.Queues.warning.publish);
                connection.on('publishClientEvent', Messaging.Queues.eventsFromServer.publish);
                var sendTransport = new JSBus.SignalRSendTransport(connection), subscribeTransport = new JSBus.SignalRSubscribeTransport(connection);
                connection.onclose(function (e) {
                    console.log("Disconnected", e);
                    console.log("Reconnecting...");
                    connect();
                });
                connection.start()
                    .then(function () {
                    Bus.bus = new JSBus.Bus("commands", sendTransport, subscribeTransport);
                    sendTransport.notifyWhenSent(Messaging.Queues.commandAck.publish);
                    Bus.lazyInitialize.resolve(Bus.bus);
                }, function () {
                    Messaging.Queues.error.publish("Yhteyden ottaminen palvelimelle epäonnistui");
                    setTimeout(function () { return connect(); }, 10000);
                });
            };
            connect();
        };
        Bus.s4 = function () {
            return Math.floor((1 + Math.random()) * 0x10000)
                .toString(16)
                .substring(1);
        };
        Bus.guid = function () {
            return Bus.s4() + Bus.s4() + '-' + Bus.s4() + '-' + Bus.s4() + '-' + Bus.s4() + '-' + Bus.s4() + Bus.s4() + Bus.s4();
        };
        ;
        Bus.createCommand = function (name, properties) {
            var command = {
                id: Bus.guid(),
                name: name,
                timestamp: Ksx.Time.ServerTime
            };
            for (var p in properties || []) {
                if (properties.hasOwnProperty(p) && p !== 'Id' && p !== 'id' && p !== 'Name' && p !== 'name') {
                    command[p] = properties[p];
                }
            }
            return command;
        };
        ;
        Bus.lazyInitialize = $.Deferred();
        Bus.initialize = Bus.lazyInitialize.promise();
        return Bus;
    }());
    Ksx.Bus = Bus;
    Bus.init();
})(Ksx || (Ksx = {}));
//# sourceMappingURL=Ksx.Bus.js.map