var JSBus;
(function (JSBus) {
    var SignalRSendTransport = (function () {
        function SignalRSendTransport(hub) {
            this.hub = hub;
            this.notifySentCallbacks = [];
        }
        SignalRSendTransport.prototype.send = function (message) {
            var _this = this;
            return new Promise(function (resolve, reject) {
                console.log("Sending via SignalR", message);
                if (_this.hub === undefined) {
                    console.log("Hub is undefined");
                    reject();
                }
                _this.hub.invoke('Command', message)
                    .then(function () {
                    _this.notifySentCallbacks.forEach(function (fn) { return fn(message); });
                    resolve(message.id);
                }, reject);
            });
        };
        SignalRSendTransport.prototype.notifyWhenSent = function (callback) {
            this.notifySentCallbacks.push(callback);
        };
        return SignalRSendTransport;
    }());
    JSBus.SignalRSendTransport = SignalRSendTransport;
})(JSBus || (JSBus = {}));
//# sourceMappingURL=SignalRSendTransport.js.map