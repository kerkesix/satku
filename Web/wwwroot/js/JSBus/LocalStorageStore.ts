/// <reference path="TransportInterfaces.ts" />
/// <reference path="LocalStorageQueue.ts" />
module JSBus {

    // TODO: Create fail counter to avoid trying infinitely
    // TODO: Consider using separate delayPolicy class to specify times, like new IncreasingDelay(5, 200)

    export class LocalStorageStore implements IStore {
        public containerName: string = 'default';

        private _outgoing: LocalStorageQueue;
        private _sent: LocalStorageQueue;
        private _retry: LocalStorageQueue;

        initQueues() {
            if (!this._outgoing) {
                this._outgoing = new LocalStorageQueue(this.containerName + "." + "outgoing");
                this._sent = new LocalStorageQueue(this.containerName + "." + "sent");
                this._retry = new LocalStorageQueue(this.containerName + "." + "retry");

                // Also init timers to transfer stuff from queue to queue

                // Retry failed sendings when message is older than 30 s. Check every 10 s.
                setInterval(this.moveBackToOutgoing.bind(this, this.retry, 30000), 10000);

                // Move sent back to outgoing if there is no ack from server after 60 s. Check every 10 s
                setInterval(this.moveBackToOutgoing.bind(this, this.sent, 60000), 10000);
            }
        }

        get outgoing(): LocalStorageQueue {
            this.initQueues();
            return this._outgoing;
        }

        get sent(): LocalStorageQueue {
            this.initQueues();
            return this._sent;
        }

        get retry(): LocalStorageQueue {
            this.initQueues();
            return this._retry;
        }

        add(message: IMessage) {
            // Convert message id to string to prevent comparison 
            // problems later on (the format on the wire is string)
            this.outgoing.add(message.id.toString(), message);
        }

        ack(id: string) {
            // Consider adding this message to an audit log

            // Should only remove from sent queue, but it is possible that ack comes from server before message 
            // has been moved from outgoing to sent queue. Therefore remove from both queues, just in case.
            this.outgoing.remove(id);
            this.sent.remove(id);
        }

        sendAll(sendCallback: (message: IMessage) => Promise<string>): Promise<any> {
            return new Promise((resolve, reject) => {
                let msgs = this.outgoing.all();
                let expect = msgs.length;
    
                if (expect === 0) {
                    resolve();
                }
                else {
                    msgs.forEach((m: IMessage) => {
                        // Callback returns a promise with message id as value
                        sendCallback(m)
                            .then(id => this.markSent(id))
                            .catch(err => {
                                console.log("Error sending message, delaying:", m, err);
                                this.delay(m.id);
                            })
                            .then(() => {
                                expect--;
                                if (expect === 0) {
                                    resolve();
                                }
                            });
                    });
                }
            });
        }

        private markSent(id: string) {
            this.moveMessageFromOutgoingTo(this.sent, id);
        }

        private delay(id: string) {
            this.moveMessageFromOutgoingTo(this.retry, id);
        }

        private moveMessageFromOutgoingTo(queue: LocalStorageQueue, id: string) {
            var m = this.outgoing.remove(id);

            if (m) {
                // Add operation date for automatic queue to queue moves
                queue.add(m.id, m, new Date());
            }
        }

        private moveBackToOutgoing(queue: LocalStorageQueue, cutOffMS: number) {
            var move = queue.removeWhereLastOperationOlderThan(cutOffMS);

            for (var i = move.length - 1; i >= 0; i--) {
                console.log("Moving message %s %s --> %s", move[i].id, queue.name, this.outgoing.name);
                this.outgoing.add(move[i].id, move[i]);
            }
        }
    }
}