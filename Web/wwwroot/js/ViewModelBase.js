var Ksx;
(function (Ksx) {
    var ViewModelBase = (function () {
        function ViewModelBase(domroot) {
            this.domroot = domroot;
            this.loaded = ko.observable(false);
        }
        ViewModelBase.prototype.onload = function (context) {
            if (this.loaded()) {
                return;
            }
            this.sammyContext = context;
            this.applyBindings();
        };
        ViewModelBase.prototype.applyBindings = function () {
            ko.applyBindings(this, document.getElementById(this.domroot + "-layout"));
            this.loaded(true);
        };
        return ViewModelBase;
    }());
    Ksx.ViewModelBase = ViewModelBase;
})(Ksx || (Ksx = {}));
//# sourceMappingURL=ViewModelBase.js.map