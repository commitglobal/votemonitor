interface PushMessageModel {
    id: string;
    title: string;
    body: string;
    sender: string;
    sentAt: Date;
    numberOfTargetedObservers: number;
}
interface PushMessageReceiverModel{
    id: string;
    name: string;
}

interface PushMessageDetailedModel {
    id: string;
    title: string;
    body: string;
    sender: string;
    sentAt: Date;
    receivers: PushMessageReceiverModel[];
}
