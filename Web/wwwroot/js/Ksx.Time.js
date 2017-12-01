var Ksx;
(function (Ksx) {
    var Time = (function () {
        function Time() {
        }
        Object.defineProperty(Time, "Offset", {
            get: function () {
                return JSON.parse(localStorage.getItem("offset"));
            },
            set: function (val) {
                localStorage.setItem("offset", JSON.stringify(val));
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(Time, "ServerTime", {
            get: function () {
                return Time.ToServerTime(new Date());
            },
            enumerable: true,
            configurable: true
        });
        Time.ToServerTime = function (date) {
            return Time.AddMs(date, Time.Offset);
        };
        Time.ToServerTimeFromInputDate = function (inputDate, inputTime) {
            var parts, year, month, date, result, t;
            if (typeof inputTime === 'undefined') {
                inputTime = inputDate;
                t = Time.ServerTime;
                year = t.getFullYear();
                month = t.getMonth();
                date = t.getDate();
            }
            else {
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
        };
        Time.AddMs = function (date, ms) {
            var d = new Date();
            d.setTime(date.getTime() + ms);
            return d;
        };
        Time.formatDate = function (ms) {
            return Time.formatDateTime(ms, true, false);
        };
        Time.formatTime = function (ms) {
            return Time.formatDateTime(ms, false);
        };
        Time.formatDateTime = function (ms, includeDate, includeTime) {
            if (includeDate === void 0) { includeDate = true; }
            if (includeTime === void 0) { includeTime = true; }
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
        };
        return Time;
    }());
    Ksx.Time = Time;
})(Ksx || (Ksx = {}));
$(document).ajaxSuccess(function (event, xhr) {
    var headerDate = xhr.getResponseHeader('Date');
    if (headerDate) {
        var localMsUtc = (new Date()).getTime();
        var serverMsUtc = (new Date(Date.parse(headerDate))).getTime();
        var offset = serverMsUtc - localMsUtc;
        Ksx.Time.Offset = offset;
    }
});
//# sourceMappingURL=Ksx.Time.js.map