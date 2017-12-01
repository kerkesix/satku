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
    var SystemViewModel = (function (_super) {
        __extends(SystemViewModel, _super);
        function SystemViewModel() {
            var _this = _super.call(this, "system") || this;
            var self = _this, route = {
                urlMap: ["/edit/system"],
                order: 99,
                text: "Järjestelmä",
                icon: "fa-cogs",
                active: false,
                tag: "admin",
                root: _this.domroot,
                showOnNavigation: true,
                onViewOpen: self.onload.bind(_this)
            };
            Ksx.Routes.register(_this.domroot, route);
            return _this;
        }
        SystemViewModel.prototype.refresh = function () {
            Ksx.DataClient.refresh().done(function () {
                Ksx.Track.pageEvent("system", "restart", "");
                setTimeout(function () { window.location.href = "/"; }, 4000);
                Messaging.Queues.success.publish("Järjestelmä käynnistetty uudelleen., sinut ohjataan etusivulle...");
            })
                .fail(Messaging.Queues.error.publish.bind(this, "Järjestelmän käynnistäminen epäonnistui"));
        };
        return SystemViewModel;
    }(Ksx.ViewModelBase));
    Ksx.SystemViewModel = SystemViewModel;
    SystemViewModel.instance = new SystemViewModel();
})(Ksx || (Ksx = {}));
//# sourceMappingURL=SystemViewModel.js.map