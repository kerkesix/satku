interface IQuitterDto {
    t: any;
    d: string;
    wsl: number;
}

interface IVisitDto {
    a: string;
    i: string;
    o: string;
    quit: IQuitterDto;
}

interface IExpectedDto {
    t: string;
    id: string;
}

interface IAttendeeDto {
    a: number;
    id: string;
    nfc: string;
    name: string;
    phone: string;
    dstr: number;
    hash: string;
}

interface ICheckpointDto {
    id: string;
    name: string;
    distanceFromStart: number;
    distanceFromPrevious: number;
    visits: IVisitDto[];
    waitingFor: any;
    avgSpeed: number;
    avgTime: number;
    latitude: string;
    longitude: string;
    checkpointType: number;
}

interface IReportDto {
    checkpoints: ICheckpointDto[];
}
{ }
