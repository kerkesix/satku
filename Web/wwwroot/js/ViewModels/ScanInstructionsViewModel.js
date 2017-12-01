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
    var ScanInstructionsViewModel = (function (_super) {
        __extends(ScanInstructionsViewModel, _super);
        function ScanInstructionsViewModel() {
            var _this = _super.call(this, "scaninstructions") || this;
            var self = _this, route = {
                urlMap: ["/scaninstructions"],
                order: 94,
                text: "Luentaohjeet",
                icon: "fa-info-sign",
                active: false,
                root: _this.domroot,
                showOnNavigation: false,
                onViewOpen: self.onload.bind(_this)
            };
            Ksx.Routes.register(_this.domroot, route);
            return _this;
        }
        return ScanInstructionsViewModel;
    }(Ksx.ViewModelBase));
    Ksx.ScanInstructionsViewModel = ScanInstructionsViewModel;
    ScanInstructionsViewModel.instance = new ScanInstructionsViewModel();
})(Ksx || (Ksx = {}));
//# sourceMappingURL=ScanInstructionsViewModel.js.map