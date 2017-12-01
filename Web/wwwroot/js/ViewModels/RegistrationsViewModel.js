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
    var RegistrationsViewModel = (function (_super) {
        __extends(RegistrationsViewModel, _super);
        function RegistrationsViewModel() {
            var _this = _super.call(this, "registrations") || this;
            var self = _this, route = {
                urlMap: ["/edit/registrations"],
                order: 90,
                text: "Ilmoittautumiset",
                icon: "fa-random",
                active: false,
                tag: "admin",
                root: _this.domroot,
                showOnNavigation: true,
                onViewOpen: self.onload.bind(_this)
            };
            Ksx.Routes.register(_this.domroot, route);
            return _this;
        }
        RegistrationsViewModel.prototype.remove = function (item) {
            var removeCommand = Ksx.Bus.createCommand("DeleteRegistration", {
                happeningId: Ksx.Routes.currentHappening,
                registrationId: item.id()
            });
            Ksx.Bus.bus.send(removeCommand);
            this.rows.remove(item);
            $(".modal").modal('hide');
        };
        RegistrationsViewModel.prototype.onload = function (context) {
            var _this = this;
            if (this.loaded()) {
                return;
            }
            $.when(Ksx.DataClient.registrations(), Ksx.DataClient.linkPeople())
                .done(function (ajaxResponseArrayRegistrations, ajaxResponseArrayPeople) {
                var mappedPeople = (ajaxResponseArrayPeople[0] || [])
                    .map(function (dto) { return new Ksx.LinkPerson(dto); });
                var mappedRegistrations = (ajaxResponseArrayRegistrations[0] || [])
                    .map(function (dto) { return new Ksx.RegistrationRow(dto, mappedPeople); });
                _this.rows = ko.observableArray(mappedRegistrations);
                _this.applyBindings();
            });
        };
        return RegistrationsViewModel;
    }(Ksx.ViewModelBase));
    Ksx.RegistrationsViewModel = RegistrationsViewModel;
    RegistrationsViewModel.instance = new RegistrationsViewModel();
})(Ksx || (Ksx = {}));
//# sourceMappingURL=RegistrationsViewModel.js.map