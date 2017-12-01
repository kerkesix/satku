/// <reference path="TransportInterfaces.ts" />
module JSBus {
    export class SignalRSubscribeTransport implements ISubscribeTransport {

        private receiveCallbacks: ((message: IMessage) => void)[] = [];
        private ackCallbacks: ((id: string) => void)[] = [];

        constructor(hub: any) {
            // Extend signalR client side hub with methods that server will call. 
            hub.on('ack', id => {
                console.log("Received ack from server", id);
                this.ackCallbacks.forEach(fn => fn(id));
            });

            hub.on('onEvent', message => {
                console.log("Received event from server", message);
                this.receiveCallbacks.forEach(fn => fn(message));
            });
        }

        receive(handler: (message: IMessage) => void ) {
            this.receiveCallbacks.push(handler);
        }

        ack(handler: (id: string) => void ) {
            this.ackCallbacks.push(handler);
        }
    }
}