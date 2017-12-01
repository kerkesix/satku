/// <reference path="TransportInterfaces.ts" />
module JSBus {

    export class LocalStorageQueue {
        constructor(public name: string) {
            if (!LocalStorageQueue.hasStorage) {
                throw new Error("Local storage is not enabled in this browser.");
            }
        }

        private get queue(): MessageContainer[] {
            var items = JSON.parse(localStorage.getItem(this.name));
            return <MessageContainer[]>items || [];
        }

        private set queue(q: MessageContainer[]) {
            if (!q) {
                return;
            }
            localStorage.setItem(this.name, JSON.stringify(q));
        }

        add(id: string, message: IMessage, operationDate?: Date) {
            var q = this.queue,
                m = new MessageContainer(id, message);

            if (operationDate) {
                // use MS format as this is used only for queu to queue 
                // transfers where comparison is done based on milliseconds
                m.lastOperationAt = operationDate.getTime();
            }

            q.push(m);

            // Save altered queue back to localstorage
            this.queue = q;
        }

        remove(id: string): IMessage {
            // Removes an item and returns it (to be placed on any other store)
            var matching = this.removeWhenMatches(m => m.id == id);
            return matching.length ? matching[0] : null;   
        }

        all(): IMessage[]{
            var q = this.queue;
            return q.map(this.removeMessageContainer);
        }

        removeWhereLastOperationOlderThan(ms: number): IMessage[]{
            var cutOff = Date.now() - ms;

            return this.removeWhenMatches(m => m.lastOperationAt < cutOff); 
        }

        private removeWhenMatches(operator: (m: MessageContainer) => boolean): IMessage[] {
            var q = this.queue,
                matching = [];

            var len = q.length;
            while (len--) {
                if (operator(q[len])) {
                    // match, store for return & remove
                    matching.push(q[len]);
                    q.splice(len, 1);
                }
            }

            // Store queue back to localstorage with items removed
            if (matching.length) {
                this.queue = q;
            }

            return matching.map(this.removeMessageContainer);
        }

        private removeMessageContainer(m: MessageContainer) {
            return m.message
        }

        static hasStorage() {
            try {
                var m = "a";
                localStorage.setItem(m, m);
                localStorage.removeItem(m);
                return true;
            } catch (e) {
                return false;
            }
        }
    }

    class MessageContainer {
        public timestamp: Date = new Date();
        public lastOperationAt: number;

        constructor(public id: string, public message: IMessage) {
        }
    }
}