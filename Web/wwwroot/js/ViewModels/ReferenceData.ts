module Ksx {
    // Stores last published reference data values to be used by 
    // classes that cannot listen to queue based refreshes (e.g. 
    // data has already been published when the script is loaded). 
    export class ReferenceData {
        static attendees: any[];
        static attendeesMap: any;
        static nfcMap: any;
        static checkpoints: any[];
        static quitAttendees: string[] = [];
        static completedAttendees: string[] = [];
    }

    Messaging.Queues.referenceData.subscribe(e => {
        if (e.name === "attendees") {
            ReferenceData.attendees = e.data;
            ReferenceData.attendeesMap = {};

            e.data.forEach(d => ReferenceData.attendeesMap[d.id] = d.name);
        }

        if (e.name === "checkpoints") {
            ReferenceData.checkpoints = e.data;
        }

        if (e.name === "nfc") {
            ReferenceData.nfcMap = {};
            e.data.forEach(a => ReferenceData.nfcMap[a.nfc] = a.id);
        }

        // TODO: Store quitters
        //if (e.name === "quitters") {
        //}

        // TODO: Store completed
        //if (e.name === "completed") {
        //}
    });

    // Update reference data based on some server events
    Messaging.Queues.eventsFromServer.subscribe(e => {
        var index;

        // Add to completed
        if (e.name === 'AttendeeScannedAtFinishCheckpoint') {
            ReferenceData.completedAttendees.push(e.data.personId);
        }

        // Keep quits in sync
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

}