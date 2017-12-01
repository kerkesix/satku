/// <reference path="jquery.Topic.ts" />
module Messaging {
    export class Queues {
        static create = (topic: string) => {
            var q = $.Topic(topic);

            // Bind tracing method
            q.subscribe(data => console.log(topic, data));
            return q;
        };

        static error: ITopic = Queues.create("message/error");
        static warning: ITopic = Queues.create("message/warning");
        static info: ITopic = Queues.create("message/info");
        static success: ITopic = Queues.create("message/success");
        static navigated: ITopic = Queues.create("navigation/viewchanged");
        static referenceData: ITopic = Queues.create("referencedata/refresh");
        static eventsFromServer: ITopic = Queues.create("event/received");
        static commandAck: ITopic = Queues.create("command/ack");

        static checkpointForScanRequest: ITopic = Queues.create("readings/checkpointforscanrequest");
        static checkpointForScanResponse: ITopic = Queues.create("readings/checkpointforscanresponse");
    }
}