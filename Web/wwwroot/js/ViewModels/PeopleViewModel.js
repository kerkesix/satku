var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var Ksx;
(function (Ksx) {
    var Person = (function () {
        function Person(dto) {
            this.nfcId = ko.observable("");
            Ksx.Mutator.DtoToKnockoutObservableModel(this, dto);
            this.personId = ko.observable(dto.id);
        }
        Person.prototype.isLinkedTo = function (happening) {
            return this.happeningsAttended().some(function (h) { return h === happening; });
        };
        return Person;
    }());
    Ksx.Person = Person;
    var PeopleViewModel = (function (_super) {
        __extends(PeopleViewModel, _super);
        function PeopleViewModel() {
            var _this = _super.call(this, "people") || this;
            _this.onlyCurrent = ko.observable(true);
            var self = _this, route = {
                urlMap: ["/edit/people"],
                order: 90,
                text: "Henkilöt",
                icon: "fa-th-list",
                active: false,
                tag: "admin",
                root: _this.domroot,
                showOnNavigation: true,
                onViewOpen: self.onload.bind(_this)
            };
            Ksx.Routes.register(_this.domroot, route);
            return _this;
        }
        PeopleViewModel.prototype.refreshData = function () {
            var _this = this;
            var dataLoaded = $.Deferred();
            Ksx.DataClient.people()
                .done(function (data) {
                var mapped = (data || []).map(function (dto) {
                    return new Person(dto);
                });
                _this.people = ko.observableArray(mapped);
                dataLoaded.resolve();
            });
            return dataLoaded.promise();
        };
        PeopleViewModel.prototype.onload = function (context) {
            var _this = this;
            if (this.loaded()) {
                return;
            }
            this.refreshData().then(function () { return _this.applyBindings(); }, function () { return Messaging.Queues.error.publish("Henkilöiden lataus palvelimelta epäonnistui."); });
        };
        PeopleViewModel.prototype.deletePerson = function (person) {
            var command = Ksx.Bus.createCommand("DeletePerson", {
                personId: person.personId()
            });
            Ksx.Bus.bus.send(command);
            this.people.remove(person);
            Messaging.Queues.success.publish("Henkilö '" + person.lastname() + " " + person.firstname() + "' poistettu.");
            this.closeEditor();
        };
        PeopleViewModel.prototype.initializeEditor = function (person) {
            this.editorPerson = person;
            return person;
        };
        PeopleViewModel.prototype.updateInformation = function () {
            var _this = this;
            var p = ko.toJS(this.editorPerson), command = Ksx.Bus.createCommand("UpdateContactInformation", p);
            Ksx.Bus.bus.send(command);
            var person = this.people().filter(function (m) { return m.personId() === _this.editorPerson.personId(); })[0];
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
        };
        PeopleViewModel.prototype.closeEditor = function () {
            $(".modal").modal('hide');
        };
        return PeopleViewModel;
    }(Ksx.ViewModelBase));
    Ksx.PeopleViewModel = PeopleViewModel;
    PeopleViewModel.instance = new PeopleViewModel();
})(Ksx || (Ksx = {}));
//# sourceMappingURL=PeopleViewModel.js.map