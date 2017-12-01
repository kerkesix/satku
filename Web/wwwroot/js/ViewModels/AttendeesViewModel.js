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
    var Attendees;
    (function (Attendees) {
        var AttendeesViewModel = (function (_super) {
            __extends(AttendeesViewModel, _super);
            function AttendeesViewModel(data) {
                var _this = _super.call(this, "attendees") || this;
                _this.selected = ko.observable();
                _this.filterText = ko.observable('');
                var self = _this, route = {
                    urlMap: ["/attendees/:id", "/attendees"],
                    navigationUrl: "/attendees",
                    icon: "fa-group",
                    order: 5,
                    text: "Kävelijät",
                    active: false,
                    root: _this.domroot,
                    showOnNavigation: true,
                    onViewOpen: self.onload.bind(_this)
                };
                Ksx.Routes.register(_this.domroot, route);
                self.filterText.subscribe(self.filter.bind(self));
                self.selected.subscribe(function (attendee) {
                    self.previousSelected = attendee;
                }, self, "beforeChange");
                self.selected.subscribe(function (attendee) {
                    if (self.previousSelected !== attendee && self.sammyContext) {
                        var newUrl = Ksx.Routes.url("attendees", null, attendee ? attendee.id : null);
                        self.sammyContext.redirect(newUrl);
                        Ksx.Track.pageView(newUrl);
                    }
                });
                Messaging.Queues.referenceData.subscribe(function (e) {
                    if (e.name === "checkpoints") {
                        self.checkpointNames = ko.observableArray(e.data.map(function (c) { return c.name; }));
                    }
                });
                data.done(self.loadData.bind(_this));
                return _this;
            }
            AttendeesViewModel.prototype.onload = function (context) {
                var _this = this;
                var id = context.params["id"];
                this.sammyContext = context;
                if (!this.attendees) {
                    if (AttendeesViewModel.retries < 100) {
                        AttendeesViewModel.retries++;
                        setTimeout(function () { return _this.onload(context); }, 100);
                    }
                    return;
                }
                this.selectAttendee(id);
                if (this.loaded()) {
                    return;
                }
                this.applyBindings();
            };
            AttendeesViewModel.prototype.filterSubmit = function (elem) {
                this.filter(this.filterText());
            };
            AttendeesViewModel.prototype.selectAttendee = function (id) {
                var found;
                this.attendees().some(function (elem) {
                    if (elem.id === id) {
                        found = elem;
                        return true;
                    }
                    return false;
                });
                this.selected(found);
            };
            AttendeesViewModel.prototype.loadData = function (inputAttendees) {
                var a = Object.getOwnPropertyNames(inputAttendees).map(function (p) { return inputAttendees[p]; });
                this.attendees = ko.observableArray(a);
                this.applyBindings();
            };
            AttendeesViewModel.prototype.filter = function (s) {
                if (!s) {
                    this.attendees().forEach(function (elem) {
                        elem.filtered(false);
                    });
                    return;
                }
                var rule = new RegExp("^[\\w\\s]*" + s + "[\\w\\s]*$", "i");
                this.attendees().forEach(function (elem) {
                    elem.filtered(!elem.name.match(rule));
                });
            };
            AttendeesViewModel.retries = 0;
            return AttendeesViewModel;
        }(Ksx.ViewModelBase));
        Attendees.AttendeesViewModel = AttendeesViewModel;
    })(Attendees = Ksx.Attendees || (Ksx.Attendees = {}));
})(Ksx || (Ksx = {}));
//# sourceMappingURL=AttendeesViewModel.js.map