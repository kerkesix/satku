var JSBus;
(function (JSBus) {
    var LocalStorageQueue = (function () {
        function LocalStorageQueue(name) {
            this.name = name;
            if (!LocalStorageQueue.hasStorage) {
                throw new Error("Local storage is not enabled in this browser.");
            }
        }
        Object.defineProperty(LocalStorageQueue.prototype, "queue", {
            get: function () {
                var items = JSON.parse(localStorage.getItem(this.name));
                return items || [];
            },
            set: function (q) {
                if (!q) {
                    return;
                }
                localStorage.setItem(this.name, JSON.stringify(q));
            },
            enumerable: true,
            configurable: true
        });
        LocalStorageQueue.prototype.add = function (id, message, operationDate) {
            var q = this.queue, m = new MessageContainer(id, message);
            if (operationDate) {
                m.lastOperationAt = operationDate.getTime();
            }
            q.push(m);
            this.queue = q;
        };
        LocalStorageQueue.prototype.remove = function (id) {
            var matching = this.removeWhenMatches(function (m) { return m.id == id; });
            return matching.length ? matching[0] : null;
        };
        LocalStorageQueue.prototype.all = function () {
            var q = this.queue;
            return q.map(this.removeMessageContainer);
        };
        LocalStorageQueue.prototype.removeWhereLastOperationOlderThan = function (ms) {
            var cutOff = Date.now() - ms;
            return this.removeWhenMatches(function (m) { return m.lastOperationAt < cutOff; });
        };
        LocalStorageQueue.prototype.removeWhenMatches = function (operator) {
            var q = this.queue, matching = [];
            var len = q.length;
            while (len--) {
                if (operator(q[len])) {
                    matching.push(q[len]);
                    q.splice(len, 1);
                }
            }
            if (matching.length) {
                this.queue = q;
            }
            return matching.map(this.removeMessageContainer);
        };
        LocalStorageQueue.prototype.removeMessageContainer = function (m) {
            return m.message;
        };
        LocalStorageQueue.hasStorage = function () {
            try {
                var m = "a";
                localStorage.setItem(m, m);
                localStorage.removeItem(m);
                return true;
            }
            catch (e) {
                return false;
            }
        };
        return LocalStorageQueue;
    }());
    JSBus.LocalStorageQueue = LocalStorageQueue;
    var MessageContainer = (function () {
        function MessageContainer(id, message) {
            this.id = id;
            this.message = message;
            this.timestamp = new Date();
        }
        return MessageContainer;
    }());
})(JSBus || (JSBus = {}));
//# sourceMappingURL=LocalStorageQueue.js.map