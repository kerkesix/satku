var Ksx;
(function (Ksx) {
    var RegistrationRow = (function () {
        function RegistrationRow(dto, availablePeople) {
            var _this = this;
            this.selectedPerson = ko.observable();
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
            this.showName = ko.pureComputed(function () {
                var fullname = _this.lastname() + " " + _this.firstname();
                return fullname === _this.displayName() ? fullname : fullname + " (" + _this.displayName() + ")";
            });
            this.availablePeople = ko.observableArray(availablePeople);
            Messaging.Queues.eventsFromServer.subscribe(function (e) {
                if (e.name === 'PersonLinkedToHappening') {
                    _this.availablePeople.remove(function (a) { return a.Id === e.data.personId; });
                }
            });
        }
        RegistrationRow.prototype.link = function () {
            var selected = this.selectedPerson();
            var linkToPerson = selected ? this.linkToExistingPerson(selected.Id) : this.linkToNewPerson(), linkCommand = Ksx.Bus.createCommand("LinkPersonToHappening", {
                PersonId: linkToPerson,
                HappeningId: Ksx.Routes.currentHappening,
                RegistrationId: this.id()
            });
            this.linkedToPerson(linkToPerson);
            linkCommand.timestamp = Ksx.Time.AddMs(Ksx.Time.ServerTime, 1000);
            setTimeout(function () { return Ksx.Bus.bus.send(linkCommand); }, 500);
            Messaging.Queues.info.publish((selected ? "Päivitetään" : "Luodaan")
                + " henkilöä " + linkCommand.PersonId
                + " ja sidotaan satkuun " + linkCommand.HappeningId + ".");
            return false;
        };
        RegistrationRow.prototype.unlink = function () {
            var unlinkCommand = Ksx.Bus.createCommand("UnlinkPersonFromHappening", {
                PersonId: this.linkedToPerson(),
                HappeningId: Ksx.Routes.currentHappening,
                RegistrationId: this.id()
            });
            Ksx.Bus.bus.send(unlinkCommand);
            this.linkedToPerson(null);
            Messaging.Queues.info.publish("Poistetaan linkitystä " + unlinkCommand.PersonId
                + " -> " + unlinkCommand.HappeningId + "...");
        };
        RegistrationRow.prototype.linkToNewPerson = function () {
            var props = ko.toJS(this), createPersonCommand = Ksx.Bus.createCommand("CreatePerson", props);
            Ksx.Bus.bus.send(createPersonCommand);
            return this.personId;
        };
        RegistrationRow.prototype.linkToExistingPerson = function (personId) {
            var props = ko.toJS(this);
            props.PersonId = personId;
            Ksx.Bus.bus.send(Ksx.Bus.createCommand("UpdateContactInformation", props));
            return personId;
        };
        return RegistrationRow;
    }());
    Ksx.RegistrationRow = RegistrationRow;
})(Ksx || (Ksx = {}));
//# sourceMappingURL=RegistrationRow.js.map