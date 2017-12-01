var Ksx;
(function (Ksx) {
    var DataClient = (function () {
        function DataClient() {
        }
        DataClient.url = function (action) {
            return "/api/" + Ksx.Routes.currentHappening + "/" + action;
        };
        DataClient.dataLoadError = function () {
            Messaging.Queues.error.publish("Tietojen haku palvelimelta epäonnistui.");
        };
        DataClient.server = function (url, verb, payload) {
            NProgress.start();
            var promise = $.ajax({
                dataType: "json",
                cache: false,
                url: url,
                headers: { "accept": "JSON" },
                type: verb,
                data: payload
            });
            promise.fail(this.dataLoadError);
            promise.always(NProgress.done);
            return promise;
        };
        DataClient.status = function () {
            return DataClient.server(DataClient.url("dashboardQuery"));
        };
        DataClient.registrations = function () {
            return DataClient.server(DataClient.url("registrationQuery"));
        };
        DataClient.happenings = function () {
            return DataClient.server(DataClient.url("happeningQuery"));
        };
        DataClient.people = function () {
            return DataClient.server(DataClient.url("peopleQuery"));
        };
        DataClient.linkPeople = function () {
            return DataClient.server(DataClient.url("linkPeopleQuery"));
        };
        DataClient.refresh = function () {
            var refreshUrl = DataClient.url("system");
            return $.ajax({
                type: "DELETE",
                url: refreshUrl,
                headers: { "accept": "JSON" }
            });
        };
        return DataClient;
    }());
    Ksx.DataClient = DataClient;
})(Ksx || (Ksx = {}));
//# sourceMappingURL=DataClient.js.map