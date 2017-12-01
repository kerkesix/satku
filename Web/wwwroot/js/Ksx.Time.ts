module Ksx {

    export class Time {

        static get Offset(): number {
            return JSON.parse(localStorage.getItem("offset"));
        }

        static set Offset(val: number) {
            localStorage.setItem("offset", JSON.stringify(val));
        }

        static get ServerTime() {
            return Time.ToServerTime(new Date());
        }

        static ToServerTime(date) {
            return Time.AddMs(date, Time.Offset);
        }

        // Give either time only, or date and time.
        static ToServerTimeFromInputDate(inputDate, inputTime?) {
            var parts, year, month, date, result, t;

            if (typeof inputTime === 'undefined') {
                inputTime = inputDate;
                t = Time.ServerTime;
                year = t.getFullYear();
                month = t.getMonth();
                date = t.getDate();
            }
            else {
                // Parse given input date
                parts = inputDate.split(".");

                if (parts.length !== 3) {
                    throw new Error("Invalid input date");
                }

                date = parseInt(parts[0]);
                month = parseInt(parts[1]) - 1;
                year = parseInt(parts[2]);
            }

            if (!inputTime) {
                throw new Error("Invalid input time");
            }

            parts = inputTime.split(".");

            if (parts.length !== 2) {
                parts = inputDate.split(":");
            }

            if (parts.length !== 2) {
                throw new Error("Invalid input date");
            }

            result = new Date(year, month, date, parts[0], parts[1]);

            return Time.AddMs(result, Time.Offset);
        }

        static AddMs(date, ms) {
            // Create new date to avoid changing reference
            var d = new Date();
            d.setTime(date.getTime() + ms);
            return d;
        }

        public static formatDate(ms) {
            return Time.formatDateTime(ms, true, false);
        }

        public static formatTime(ms) {
            return Time.formatDateTime(ms, false);
        }

        public static formatDateTime (ms, includeDate: boolean = true, includeTime: boolean = true) {
            var s = "";

            if (!ms) {
                return s;
            }
            
            var d = new Date(ms);

            if (includeDate) {
                s = d.getDate() + "." + (d.getMonth() + 1) + "." + d.getFullYear() + " ";
            }

            if (includeTime) {
                s += d.getHours() + "." + (d.getMinutes() < 10 ? "0" + d.getMinutes() : "" + d.getMinutes());
            }

            return s.trim();
        }
    }
}

// Wire up a special handler to parse server time from response headers.
$(document).ajaxSuccess(function (event, xhr) {
    var headerDate = xhr.getResponseHeader('Date');

    if (headerDate) {
        var localMsUtc = (new Date()).getTime();
        var serverMsUtc = (new Date(Date.parse(headerDate))).getTime();
        var offset = serverMsUtc - localMsUtc;

        Ksx.Time.Offset = offset;
    }
});