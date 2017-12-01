var JSBus;
(function (JSBus) {
    var SignalRSubscribeTransport = (function () {
        function SignalRSubscribeTransport(hub) {
            var _this = this;
            this.receiveCallbacks = [];
            this.ackCallbacks = [];
            hub.on('ack', function (id) {
                console.log("Received ack from server", id);
                _this.ackCallbacks.forEach(function (fn) { return fn(id); });
            });
            hub.on('onEvent', function (message) {
                console.log("Received event from server", message);
                _this.receiveCallbacks.forEach(function (fn) { return fn(message); });
            });
        }
        SignalRSubscribeTransport.prototype.receive = function (handler) {
            this.receiveCallbacks.push(handler);
        };
        SignalRSubscribeTransport.prototype.ack = function (handler) {
            this.ackCallbacks.push(handler);
        };
        return SignalRSubscribeTransport;
    }());
    JSBus.SignalRSubscribeTransport = SignalRSubscribeTransport;
})(JSBus || (JSBus = {}));
//# sourceMappingURL=SignalRSubscribeTransport.js.map