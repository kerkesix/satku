var Messaging;
(function (Messaging) {
    var Queues = (function () {
        function Queues() {
        }
        Queues.create = function (topic) {
            var q = $.Topic(topic);
            q.subscribe(function (data) { return console.log(topic, data); });
            return q;
        };
        Queues.error = Queues.create("message/error");
        Queues.warning = Queues.create("message/warning");
        Queues.info = Queues.create("message/info");
        Queues.success = Queues.create("message/success");
        Queues.navigated = Queues.create("navigation/viewchanged");
        Queues.referenceData = Queues.create("referencedata/refresh");
        Queues.eventsFromServer = Queues.create("event/received");
        Queues.commandAck = Queues.create("command/ack");
        Queues.checkpointForScanRequest = Queues.create("readings/checkpointforscanrequest");
        Queues.checkpointForScanResponse = Queues.create("readings/checkpointforscanresponse");
        return Queues;
    }());
    Messaging.Queues = Queues;
})(Messaging || (Messaging = {}));
//# sourceMappingURL=Queues.js.map