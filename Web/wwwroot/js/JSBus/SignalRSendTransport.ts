/// <reference path="TransportInterfaces.ts" />
module JSBus {
    export class SignalRSendTransport implements ISendTransport {

        private notifySentCallbacks: ((message: IMessage) => void)[] = [];

        constructor(public hub: any) { }

        send(message: IMessage): Promise<string> {
            // Use own promise instead of SignalR provided promise as we need to resolve with the message ID
            return new Promise((resolve, reject) => {
                console.log("Sending via SignalR", message);

                if (this.hub === undefined) {
                    console.log("Hub is undefined");
                    reject();
                }
    
                this.hub.invoke('Command', message)
                    .then(() => { 
                        this.notifySentCallbacks.forEach(fn => fn(message));
                        resolve(message.id);
                    }, reject);
            });
        }

        notifyWhenSent(callback: (message: IMessage) => void) {
            this.notifySentCallbacks.push(callback);
        }
    }
}
