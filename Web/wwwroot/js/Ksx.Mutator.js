var Ksx;
(function (Ksx) {
    var Mutator = (function () {
        function Mutator() {
        }
        Mutator.DtoToKnockoutObservableModel = function (target, dto) {
            var m, e, what = Object.prototype.toString;
            for (m in dto || {}) {
                if (dto.hasOwnProperty(m)) {
                    e = dto[m];
                    target[m] = what.call(e) === "[object Array]" ? ko.observableArray(e) : ko.observable(e);
                }
            }
        };
        return Mutator;
    }());
    Ksx.Mutator = Mutator;
})(Ksx || (Ksx = {}));
//# sourceMappingURL=Ksx.Mutator.js.map