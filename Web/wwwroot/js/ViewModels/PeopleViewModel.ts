/// <reference path="../_references.ts" />
/// <reference path="../ViewModelBase.ts" />
/// <reference path="../DataClient.ts" />
module Ksx {
    export interface IPersonDto {
        id: string
        // Not defining any properties as all are dynamically mapped 1:1 to Person class
    }

    export class Person {
        personId: KnockoutObservable<string>;
        nfcId: KnockoutObservable<string> = ko.observable(""); // TODO: Remove, this is needed because there is no data yet
        lastname: KnockoutObservable<string>;
        firstname: KnockoutObservable<string>;
        displayName: KnockoutObservable<string>;
        email: KnockoutObservable<string>;
        twitter: KnockoutObservable<string>;
        phone: KnockoutObservable<string>;
        happeningsAttended: KnockoutObservableArray<string>;
        happeningsCompleted: KnockoutObservableArray<string>;
        info: KnockoutObservable<string>;

        isLinkedTo(happening: string): boolean {
            return this.happeningsAttended().some(h => h === happening);
        }

        constructor(dto: IPersonDto) {
            Ksx.Mutator.DtoToKnockoutObservableModel(this, dto);
            this.personId = ko.observable(dto.id);
        }
    }

    export class PeopleViewModel extends ViewModelBase {
        static instance: PeopleViewModel;

        people: KnockoutObservableArray<Person>;

        editorPerson: Person;

        onlyCurrent: KnockoutObservable<boolean> = ko.observable(true);

        // Load data from server
        public refreshData(): JQueryPromise<any> {
            var dataLoaded: JQueryDeferred<any> = $.Deferred();
            Ksx.DataClient.people()
                .done(data => {
                    var mapped : Person[] = (data || []).map((dto: IPersonDto) => {
                        return new Person(dto);
                    });

                    this.people = ko.observableArray(mapped);
                    dataLoaded.resolve();
                });

            return dataLoaded.promise();
        }

        public onload(context: any): void {
            if (this.loaded()) {
                return;
            }

            this.refreshData().then(
                () => this.applyBindings(),
                () => Messaging.Queues.error.publish("Henkilöiden lataus palvelimelta epäonnistui."));
        }

        public deletePerson(person: Person) {
            var command = Ksx.Bus.createCommand("DeletePerson", {
                personId: person.personId()
            });

            Ksx.Bus.bus.send(command);
            this.people.remove(person);

            Messaging.Queues.success.publish("Henkilö '" + person.lastname() + " " + person.firstname() + "' poistettu.");
            this.closeEditor();
        }

        public initializeEditor(person: Person): Person {
            this.editorPerson = person;
            return person;
        }

        public updateInformation() {
            var p = ko.toJS(this.editorPerson),
                command = Ksx.Bus.createCommand("UpdateContactInformation", p);

            Ksx.Bus.bus.send(command);

            // Update information in table row
            var person = this.people().filter(m => m.personId() === this.editorPerson.personId())[0];
            person.nfcId(p.nfcId);
            person.lastname(p.lastname);
            person.firstname(p.firstname);
            person.displayName(p.displayName);
            person.email(p.email);
            person.twitter(p.twitter);
            person.phone(p.phone);

            Messaging.Queues.info.publish("Tallennetaan henkilön " + p.displayName + " tietoja...");
            this.closeEditor();
            Ksx.Track.pageEvent("people", "update", person.personId());
        }

        private closeEditor() {
            // Close modal window
            // HACK: Should find a MVVM way to do this
            (<any>$(".modal")).modal('hide');
        }

        constructor() {
            super("people");

            var self = this,
                route: IRoute = {
                    urlMap: ["/edit/people"],
                    order: 90,
                    text: "Henkilöt",
                    icon: "fa-th-list",
                    active: false,
                    tag: "admin",
                    root: this.domroot,
                    showOnNavigation: true,
                    onViewOpen: self.onload.bind(this)
                };

            // Even though this page does not use happening information, register it under /hapening/ path
            // as this prevents problems detecting happening server side when refreshing page.
            Ksx.Routes.register(this.domroot, route);
        }
    }

    // Expose model to outside world for debugging purposes
    PeopleViewModel.instance = new PeopleViewModel();
}