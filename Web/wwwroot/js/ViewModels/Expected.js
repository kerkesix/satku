var Ksx;
(function (Ksx) {
    var Expected = (function () {
        function Expected(data) {
            var self = this;
            self.id = data.id;
            self.expectedAt = ko.observable(data.t);
        }
        return Expected;
    }());
    Ksx.Expected = Expected;
})(Ksx || (Ksx = {}));
//# sourceMappingURL=Expected.js.map