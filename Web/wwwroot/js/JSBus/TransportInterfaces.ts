interface ISendTransport {
    send(message: IMessage): Promise<string>;
}

interface ISubscribeTransport {
    receive(handler: (message: IMessage) => void);
    ack(handler: (id: string) => void);
}

interface IMessage {
    id: string;
    name: string;
}

interface IStore {
    containerName: string;
    add(message: IMessage);
    sendAll(sendCallback: (IMessage) => Promise<any>);

    // Acknowledges that message has arrived target server, should remove 
    // message from outgoing store.
    ack(id: string);
}