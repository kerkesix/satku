var JSBus;
(function (JSBus) {
    var Bus = (function () {
        function Bus(name, sendTransport, subscribeTransport) {
            var _this = this;
            this.sendTransport = sendTransport;
            this.subscribeTransport = subscribeTransport;
            this.store = new JSBus.LocalStorageStore();
            this.pending = [];
            this.sendTimer = 0;
            this.store.containerName = name;
            this.subscribeTransport.ack(function (id) { return _this.store.ack(id); });
            this.sendMessages();
            window.addEventListener("online", function () { return _this.sendMessages(); });
        }
        Bus.prototype.send = function (message) {
            if (!message) {
                return;
            }
            if (!message.id) {
                message.id = (new Date()).getTime();
            }
            this.store.add(message);
            this.sendMessages();
        };
        Bus.prototype.subscribe = function (onMessageArrived, filter) {
            if (!Bus.isFunc(onMessageArrived)) {
                throw new Error('Given subscribe callback must be a function');
            }
            if (!Bus.isFunc(filter)) {
                var eventName = filter;
                filter = function (message) { return !eventName || message.Name === eventName || message.name === eventName; };
            }
            this.subscribeTransport.receive(function (receivedMessage) {
                if (filter(receivedMessage)) {
                    onMessageArrived(receivedMessage);
                }
            });
        };
        Bus.prototype.sendMessages = function () {
            if (this.sendTimer || (typeof navigator.onLine !== "undefined" && !navigator.onLine)) {
                return;
            }
            console.log("Starting message sending loop");
            this.sendMessagesCore();
        };
        Bus.prototype.sendMessagesCore = function () {
            var _this = this;
            this.store.sendAll(function (msg) { return _this.sendTransport.send(msg); })
                .then(function () {
                _this.sendTimer = setTimeout(function () { return _this.sendMessagesCore(); }, 200);
            });
        };
        Bus.isFunc = function (f) {
            return typeof (f) === 'function';
        };
        return Bus;
    }());
    JSBus.Bus = Bus;
})(JSBus || (JSBus = {}));
//# sourceMappingURL=Bus.js.map