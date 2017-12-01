/// <reference path="../_references.ts" />
/// <reference path="../ViewModelBase.ts" />
/// <reference path="../DataClient.ts" />
/// <reference path="RegistrationRow.ts" />
/// <reference path="LinkPerson.ts" />
module Ksx {
    export class RegistrationsViewModel extends ViewModelBase {
        static instance: RegistrationsViewModel;

        rows: KnockoutObservableArray<RegistrationRow>;

        remove(item) {
            var removeCommand = Ksx.Bus.createCommand("DeleteRegistration", {
                happeningId: Ksx.Routes.currentHappening,
                registrationId: item.id()
            });

            Ksx.Bus.bus.send(removeCommand);

            // Delete
            this.rows.remove(item);

            // Close the popup
            // TODO: This is not a correct MVVM way to do this
            (<any>$(".modal")).modal('hide');
        }

        public onload(context: any): void {
            if (this.loaded()) {
                return;
            }

            // Load data from server
            $.when(Ksx.DataClient.registrations(), Ksx.DataClient.linkPeople())
                .done((ajaxResponseArrayRegistrations, ajaxResponseArrayPeople) => {
                    var mappedPeople = (ajaxResponseArrayPeople[0] || [])
                        .map((dto: ILinkPersonDto) => new LinkPerson(dto));

                    var mappedRegistrations: RegistrationRow[] = (ajaxResponseArrayRegistrations[0] || [])
                        .map((dto: IRegistrationDto) => new RegistrationRow(dto, mappedPeople));

                    this.rows = ko.observableArray(mappedRegistrations);

                    this.applyBindings();
                });
        }

        constructor() {
            super("registrations");

            var self = this,
                route: IRoute = {
                    urlMap: ["/edit/registrations"],
                    order: 90,
                    text: "Ilmoittautumiset",
                    icon: "fa-random",
                    active: false,
                    tag: "admin",
                    root: this.domroot,
                    showOnNavigation: true,
                    onViewOpen: self.onload.bind(this)
                };

            Ksx.Routes.register(this.domroot, route);
        }
    }

    // Expose model to outside world for debugging purposes
    RegistrationsViewModel.instance = new RegistrationsViewModel();
}