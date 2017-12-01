var Ksx;
(function (Ksx) {
    var Quitter = (function () {
        function Quitter(data, attendeeId) {
            var self = this;
            self.time = Date.parse(data.t);
            self.description = data.d;
            self.walkedSinceLast = data.wsl;
            self.id = attendeeId;
        }
        return Quitter;
    }());
    Ksx.Quitter = Quitter;
})(Ksx || (Ksx = {}));
//# sourceMappingURL=Quitter.js.map