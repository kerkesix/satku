/// <reference path="../_references.ts" />
module Ksx {
    export interface ILinkPersonDto {
        personId: string;
        name: string;
        email: string;
        phone: string;
    }

    export class LinkPerson {
        Id: string;
        Name: string;
        Phone: string;
        Email: string;

        constructor(dto: ILinkPersonDto) {
            this.Id = dto.personId;
            this.Email = dto.email;
            this.Phone = dto.phone;
            this.Name = dto.name;
        }
    }
}