/// <reference path="_references.ts" />
module Ksx {

    export class DataClient {

        private static url(action: string) {
            return "/api/" + Ksx.Routes.currentHappening + "/" + action;
        }

        private static dataLoadError() {
            Messaging.Queues.error.publish("Tietojen haku palvelimelta epäonnistui.");
        }

        private static server(url: string, verb? : string, payload?): JQueryPromise<any> {
            // Show spinner
            NProgress.start();

             var promise = $.ajax({
                dataType: "json",
                cache: false,
                url: url,
                headers: { "accept": "JSON" },
                type: verb,
                data: payload
            });

            // Report error to user
            promise.fail(this.dataLoadError);

            // Hide spinner
            promise.always(NProgress.done);
            return promise;
        }

        static status(): JQueryPromise<any> {
            return DataClient.server(DataClient.url("dashboardQuery"));
        }

        static registrations(): JQueryPromise<any> {
            return DataClient.server(DataClient.url("registrationQuery"));
        }

        static happenings(): JQueryPromise<any> {
            return DataClient.server(DataClient.url("happeningQuery"));
        }

        static people(): JQueryPromise<any> {
            return DataClient.server(DataClient.url("peopleQuery"));
        }

        static linkPeople(): JQueryPromise<any> {
            return DataClient.server(DataClient.url("linkPeopleQuery"));
        }

        static refresh(): JQueryPromise<any> {
            var refreshUrl = DataClient.url("system");

            return $.ajax({
                type: "DELETE",
                url: refreshUrl,
                headers: { "accept": "JSON" }
            });            
        }
    }
}