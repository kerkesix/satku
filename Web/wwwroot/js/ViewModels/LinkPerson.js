var Ksx;
(function (Ksx) {
    var LinkPerson = (function () {
        function LinkPerson(dto) {
            this.Id = dto.personId;
            this.Email = dto.email;
            this.Phone = dto.phone;
            this.Name = dto.name;
        }
        return LinkPerson;
    }());
    Ksx.LinkPerson = LinkPerson;
})(Ksx || (Ksx = {}));
//# sourceMappingURL=LinkPerson.js.map