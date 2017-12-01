/// <reference path="TransportInterfaces.ts" />
/// <reference path="LocalStorageStore.ts" />

interface IBus {
    send(message: any): void;
    subscribe(onMessageArrived: (message: any) => {}, filter: any);
}

module JSBus {

    export class Bus implements IBus {

        store: IStore = new LocalStorageStore();
        pending: IMessage[] = [];
        sendTimer: number = 0;

        constructor(
            name: string,
            public sendTransport: ISendTransport,
            public subscribeTransport: ISubscribeTransport) {

            // To speed up enumeration and allow multiple simultaneous 
            // Busses, use one local storage container per bus. 
            this.store.containerName = name;

            // Subscribe to ack messages (2 phase commit)
            this.subscribeTransport.ack(id => this.store.ack(id));

            // Begin send loop
            this.sendMessages();

            // Start message sending when browser is back online 
            window.addEventListener("online", () => this.sendMessages());
        }

        send(message: any) {
            // Validate
            if (!message) {
                return;
            }

            // If ID property is missing generate it.
            if (!message.id) {
                message.id = (new Date()).getTime();
            }

            this.store.add(message);

            // If send loop is not ongoing, start it
            this.sendMessages();
        }

        subscribe(onMessageArrived: (message: any) => {}, filter: any) {

            if (!Bus.isFunc(onMessageArrived)) {
                throw new Error('Given subscribe callback must be a function');
            }

            // If given filter is just the message type name or nothing,
            // create a new filter function
            if (!Bus.isFunc(filter)) {
                var eventName = filter;
                filter = message => !eventName || message.Name === eventName || message.name === eventName;
            }

            this.subscribeTransport.receive(receivedMessage => {
                if (filter(receivedMessage)) {
                    // Message passed filter, forward to handler
                    onMessageArrived(receivedMessage);
                }
            });
        }

        sendMessages() {
            // Do nothing if timer is running or if we are offline
            if (this.sendTimer || (typeof navigator.onLine !== "undefined" && !navigator.onLine)) {
                return;
            }

            console.log("Starting message sending loop");
            this.sendMessagesCore();
        }

        sendMessagesCore() {
            // Send, and only after it is completely done schedule next sending
            this.store.sendAll(msg => this.sendTransport.send(msg))
                // This promise always resolves, never rejects
                .then(() => {
                    // Consider pausing timer if there are no new messages
                    this.sendTimer = setTimeout(() => this.sendMessagesCore(), 200);
                });
        }

        static isFunc(f) {
            return typeof (f) === 'function';
        }
    }
}