/// <reference path="../_references.ts" />
module Ksx {
    export interface IRegistrationDto {
        id: string;
        timestamp: string;
        confirmedAt: string;
        lastname: string;
        firstname: string;
        displayName: string;
        email: string;
        phone: string;
        isMember: boolean;
        beenThere: boolean;
        info: string;
        newPersonId: string;
        linkedToPerson: string;
    }

    export class RegistrationRow {
        id: KnockoutObservable<string>;
        personId: string;

        time: KnockoutObservable<string>;
        confirmedAt: KnockoutObservable<string>;
        lastname: KnockoutObservable<string>;
        firstname: KnockoutObservable<string>;
        displayName: KnockoutObservable<string>;
        email: KnockoutObservable<string>;
        phone: KnockoutObservable<string>;
        isMember: KnockoutObservable<boolean>;
        beenThere: KnockoutObservable<boolean>;
        info: KnockoutObservable<string>;
        linkedToPerson: KnockoutObservable<string>;

        showName: KnockoutComputed<string>;
        availablePeople: KnockoutObservableArray<LinkPerson>;
        selectedPerson: KnockoutObservable<LinkPerson> = ko.observable<LinkPerson>();

        link() {
            var selected = this.selectedPerson();

            var linkToPerson = selected ? this.linkToExistingPerson(selected.Id) : this.linkToNewPerson(),
                linkCommand: any = Ksx.Bus.createCommand("LinkPersonToHappening", {
                    PersonId: linkToPerson,
                    HappeningId: Ksx.Routes.currentHappening,
                    RegistrationId: this.id()
                });

            // Update UI
            this.linkedToPerson(linkToPerson);

            // Force linkCommand to have timestamp later than createPersonCommand
            // to avoid replay problems
            linkCommand.timestamp = Ksx.Time.AddMs(Ksx.Time.ServerTime, 1000);

            // Delay sending somewhat to avoid constant ordering problems at server side
            setTimeout(() => Ksx.Bus.bus.send(linkCommand), 500);

            Messaging.Queues.info.publish((selected ? "Päivitetään" : "Luodaan")
                + " henkilöä " + linkCommand.PersonId
                + " ja sidotaan satkuun " + linkCommand.HappeningId + ".");

            return false;
        }

        unlink() {
            var unlinkCommand : any = Ksx.Bus.createCommand("UnlinkPersonFromHappening", {
                PersonId: this.linkedToPerson(),
                HappeningId: Ksx.Routes.currentHappening,
                RegistrationId: this.id()
            });

            Ksx.Bus.bus.send(unlinkCommand);
            this.linkedToPerson(null);

            Messaging.Queues.info.publish("Poistetaan linkitystä " + unlinkCommand.PersonId
                + " -> " + unlinkCommand.HappeningId + "...");
        }

        private linkToNewPerson(): string {
            var props = ko.toJS(this),
                createPersonCommand = Ksx.Bus.createCommand("CreatePerson", props);

            Ksx.Bus.bus.send(createPersonCommand);
            return this.personId;
        }

        private linkToExistingPerson(personId: string): string {
            var props = ko.toJS(this);
            props.PersonId = personId;

            Ksx.Bus.bus.send(Ksx.Bus.createCommand("UpdateContactInformation", props));
            return personId;
        }

        constructor(dto: IRegistrationDto, availablePeople: LinkPerson[]) {
            this.id = ko.observable(dto.id);
            this.personId = dto.newPersonId;

            this.time = ko.observable(dto.timestamp);
            this.confirmedAt = ko.observable(dto.confirmedAt);
            this.lastname = ko.observable(dto.lastname);
            this.firstname = ko.observable(dto.firstname);
            this.displayName = ko.observable(dto.displayName);
            this.email = ko.observable(dto.email);
            this.phone = ko.observable(dto.phone);
            this.isMember = ko.observable(dto.isMember);
            this.beenThere = ko.observable(dto.beenThere);
            this.info = ko.observable(dto.info);
            this.linkedToPerson = ko.observable(dto.linkedToPerson);

            this.showName = ko.pureComputed(() => {
                var fullname = this.lastname() + " " + this.firstname();
                return fullname === this.displayName() ? fullname : fullname + " (" + this.displayName() + ")";
            });

            this.availablePeople = ko.observableArray(availablePeople);

            // Listen to server side events to be able to remove people from list 
            // when they are linked. 
            // TODO: Should also add people back on PersonUnLinkedFromHappening.
            // TODO: Currently this does not update the view in real time
            Messaging.Queues.eventsFromServer.subscribe(e => {
                if (e.name === 'PersonLinkedToHappening') {
                    this.availablePeople.remove(a => a.Id === e.data.personId);
                }
            });
        }
    }
}