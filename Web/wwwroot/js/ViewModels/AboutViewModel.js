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
    var AboutViewModel = (function (_super) {
        __extends(AboutViewModel, _super);
        function AboutViewModel() {
            var _this = _super.call(this, "about") || this;
            var self = _this, route = {
                urlMap: ["/about"],
                order: 90,
                text: "Mikä?",
                icon: "fa-info-circle",
                active: false,
                root: _this.domroot,
                showOnNavigation: true,
                onViewOpen: self.onload.bind(_this)
            };
            Ksx.Routes.register(_this.domroot, route);
            return _this;
        }
        return AboutViewModel;
    }(Ksx.ViewModelBase));
    Ksx.AboutViewModel = AboutViewModel;
    AboutViewModel.instance = new AboutViewModel();
})(Ksx || (Ksx = {}));
//# sourceMappingURL=AboutViewModel.js.map