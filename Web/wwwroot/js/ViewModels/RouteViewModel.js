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
    var RouteViewModel = (function (_super) {
        __extends(RouteViewModel, _super);
        function RouteViewModel() {
            var _this = _super.call(this, "route") || this;
            var self = _this, route = {
                urlMap: ["/route"],
                order: 50,
                text: "Reitti",
                icon: "fa-map-marker",
                active: false,
                root: _this.domroot,
                showOnNavigation: true,
                onViewOpen: self.onload.bind(_this)
            };
            Ksx.Routes.register(_this.domroot, route);
            return _this;
        }
        return RouteViewModel;
    }(Ksx.ViewModelBase));
    Ksx.RouteViewModel = RouteViewModel;
    RouteViewModel.instance = new RouteViewModel();
})(Ksx || (Ksx = {}));
//# sourceMappingURL=RouteViewModel.js.map