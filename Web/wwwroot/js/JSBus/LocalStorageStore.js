var JSBus;
(function (JSBus) {
    var LocalStorageStore = (function () {
        function LocalStorageStore() {
            this.containerName = 'default';
        }
        LocalStorageStore.prototype.initQueues = function () {
            if (!this._outgoing) {
                this._outgoing = new JSBus.LocalStorageQueue(this.containerName + "." + "outgoing");
                this._sent = new JSBus.LocalStorageQueue(this.containerName + "." + "sent");
                this._retry = new JSBus.LocalStorageQueue(this.containerName + "." + "retry");
                setInterval(this.moveBackToOutgoing.bind(this, this.retry, 30000), 10000);
                setInterval(this.moveBackToOutgoing.bind(this, this.sent, 60000), 10000);
            }
        };
        Object.defineProperty(LocalStorageStore.prototype, "outgoing", {
            get: function () {
                this.initQueues();
                return this._outgoing;
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(LocalStorageStore.prototype, "sent", {
            get: function () {
                this.initQueues();
                return this._sent;
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(LocalStorageStore.prototype, "retry", {
            get: function () {
                this.initQueues();
                return this._retry;
            },
            enumerable: true,
            configurable: true
        });
        LocalStorageStore.prototype.add = function (message) {
            this.outgoing.add(message.id.toString(), message);
        };
        LocalStorageStore.prototype.ack = function (id) {
            this.outgoing.remove(id);
            this.sent.remove(id);
        };
        LocalStorageStore.prototype.sendAll = function (sendCallback) {
            var _this = this;
            return new Promise(function (resolve, reject) {
                var msgs = _this.outgoing.all();
                var expect = msgs.length;
                if (expect === 0) {
                    resolve();
                }
                else {
                    msgs.forEach(function (m) {
                        sendCallback(m)
                            .then(function (id) { return _this.markSent(id); })
                            .catch(function (err) {
                            console.log("Error sending message, delaying:", m, err);
                            _this.delay(m.id);
                        })
                            .then(function () {
                            expect--;
                            if (expect === 0) {
                                resolve();
                            }
                        });
                    });
                }
            });
        };
        LocalStorageStore.prototype.markSent = function (id) {
            this.moveMessageFromOutgoingTo(this.sent, id);
        };
        LocalStorageStore.prototype.delay = function (id) {
            this.moveMessageFromOutgoingTo(this.retry, id);
        };
        LocalStorageStore.prototype.moveMessageFromOutgoingTo = function (queue, id) {
            var m = this.outgoing.remove(id);
            if (m) {
                queue.add(m.id, m, new Date());
            }
        };
        LocalStorageStore.prototype.moveBackToOutgoing = function (queue, cutOffMS) {
            var move = queue.removeWhereLastOperationOlderThan(cutOffMS);
            for (var i = move.length - 1; i >= 0; i--) {
                console.log("Moving message %s %s --> %s", move[i].id, queue.name, this.outgoing.name);
                this.outgoing.add(move[i].id, move[i]);
            }
        };
        return LocalStorageStore;
    }());
    JSBus.LocalStorageStore = LocalStorageStore;
})(JSBus || (JSBus = {}));
//# sourceMappingURL=LocalStorageStore.js.map