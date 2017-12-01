module Ksx {

     export class Track {

         static pageView(path: string) {
             var appInsights: any = window["appInsights"];
             var ga: any = window["ga"];

             if (typeof appInsights != "undefined") {
                 appInsights.trackPageView(window.document.title, path);
             }

             if (typeof ga != "undefined") {
                 ga("send", "pageview", path);
             }
         }

         static pageEvent(category: string, action: string, label: string) {
             var appInsights: any = window["appInsights"];
             var ga: any = window["ga"];

             if (typeof appInsights != "undefined") {
                 appInsights.trackEvent(category + "/" + action + "/" + label);
             }

             if (typeof ga != "undefined") {
                 ga("send", "event", category, action, label);
             }
         }

     }
 }