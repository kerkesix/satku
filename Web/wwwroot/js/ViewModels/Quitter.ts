/// <reference path="../_references.ts" />
/// <reference path="Dtos.ts" />

interface IQuitter {
    id: string;
    time: any;
    description: string;
    walkedSinceLast: number;
}

module Ksx {
    export class Quitter implements IQuitter  {
        id: string;
        time: any;
        description: string;
        walkedSinceLast: number;

        constructor (data: IQuitterDto, attendeeId: string) { 
            var self = this;

            // These are on the wire:
            self.time = Date.parse(data.t);
            self.description = data.d;
            self.walkedSinceLast = data.wsl;

            // These are set from the outside based on context
            self.id = attendeeId;
        }
    }
}