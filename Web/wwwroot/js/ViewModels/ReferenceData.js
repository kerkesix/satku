var Ksx;
(function (Ksx) {
    var ReferenceData = (function () {
        function ReferenceData() {
        }
        ReferenceData.quitAttendees = [];
        ReferenceData.completedAttendees = [];
        return ReferenceData;
    }());
    Ksx.ReferenceData = ReferenceData;
    Messaging.Queues.referenceData.subscribe(function (e) {
        if (e.name === "attendees") {
            ReferenceData.attendees = e.data;
            ReferenceData.attendeesMap = {};
            e.data.forEach(function (d) { return ReferenceData.attendeesMap[d.id] = d.name; });
        }
        if (e.name === "checkpoints") {
            ReferenceData.checkpoints = e.data;
        }
        if (e.name === "nfc") {
            ReferenceData.nfcMap = {};
            e.data.forEach(function (a) { return ReferenceData.nfcMap[a.nfc] = a.id; });
        }
    });
    Messaging.Queues.eventsFromServer.subscribe(function (e) {
        var index;
        if (e.name === 'AttendeeScannedAtFinishCheckpoint') {
            ReferenceData.completedAttendees.push(e.data.personId);
        }
        if (e.name === 'AttendeeQuit') {
            ReferenceData.quitAttendees.push(e.data.personId);
        }
        if (e.name === 'AttendeeQuitRemoved') {
            index = ReferenceData.quitAttendees.indexOf(e.data.personId);
            if (index) {
                ReferenceData.quitAttendees.splice(index, 1);
            }
        }
    });
})(Ksx || (Ksx = {}));
//# sourceMappingURL=ReferenceData.js.map