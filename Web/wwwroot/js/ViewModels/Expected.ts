/// <reference path="../_references.ts" />
/// <reference path="Dtos.ts" />

interface IExpected {
    id: string;
    expectedAt: KnockoutObservable<string>;
}

module Ksx {
    export class Expected implements IExpected  {
        id: string;
        expectedAt: KnockoutObservable<string>;

        constructor (data: IExpectedDto) { 
            var self = this;

            self.id = data.id;
            self.expectedAt = ko.observable(data.t);
        }
    }
}