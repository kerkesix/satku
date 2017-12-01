// Since there is no UI to create checkpoints ATM, use this JS in Chrome dev tools.
let hid = "satkuxvi";
[
{ "happeningId": hid, "order": 1,  "checkpointType": 0, "checkpointName": "Crossfit 8000",    "latitude": 60.1583781,    "longitude": 24.6411699,   "distanceFromPrevious": 0.0,    "distanceFromStart": 0.0 },
{ "happeningId": hid, "order": 2,  "checkpointType": 1, "checkpointName": "Hölmölä",          "latitude": 60.2225441,    "longitude": 24.6693438,   "distanceFromPrevious": 12.5,   "distanceFromStart": 12.5 },
{ "happeningId": hid, "order": 3,  "checkpointType": 1, "checkpointName": "Gunnars",          "latitude": 60.2735067999, "longitude": 24.6560776,   "distanceFromPrevious": 9.1,    "distanceFromStart": 21.6 },
{ "happeningId": hid, "order": 4,  "checkpointType": 1, "checkpointName": "Near Lepsaemae",   "latitude": 60.389198,     "longitude": 24.645102,    "distanceFromPrevious": 14.3,   "distanceFromStart": 35.9 },
{ "happeningId": hid, "order": 5,  "checkpointType": 1, "checkpointName": "Puliväli",         "latitude": 60.4112037,    "longitude": 24.4510764,   "distanceFromPrevious": 14.2,   "distanceFromStart": 50.1 },
{ "happeningId": hid, "order": 6,  "checkpointType": 2, "checkpointName": "Vihdinnurkka",     "latitude": 60.4194218,    "longitude": 24.3512178,   "distanceFromPrevious": 5.8,    "distanceFromStart": 55.9 },
{ "happeningId": hid, "order": 7,  "checkpointType": 1, "checkpointName": "Hyvinkää 40",      "latitude": 60.3812346,    "longitude": 24.4109559,   "distanceFromPrevious": 6.5,    "distanceFromStart": 62.4 },
{ "happeningId": hid, "order": 8,  "checkpointType": 1, "checkpointName": "Kurjoo",           "latitude": 60.3100747,    "longitude": 24.4027162,   "distanceFromPrevious": 10.4,   "distanceFromStart": 72.8 },
{ "happeningId": hid, "order": 9,  "checkpointType": 1, "checkpointName": "Veikkola",         "latitude": 60.2699714999, "longitude": 24.4410503,   "distanceFromPrevious": 7.6,    "distanceFromStart": 80.4 },
{ "happeningId": hid, "order": 10, "checkpointType": 1, "checkpointName": "Vikatauko",        "latitude": 60.1996218,    "longitude": 24.5550871,   "distanceFromPrevious": 11.9,   "distanceFromStart": 92.3 },
{ "happeningId": hid, "order": 11, "checkpointType": 3, "checkpointName": "Crossfit 8000",    "latitude": 60.1583781,    "longitude": 24.6411699,   "distanceFromPrevious": 7.7,    "distanceFromStart": 100.0 }
]
.map(e => Ksx.Bus.createCommand("CreateCheckpoint", e))
.forEach(e => Ksx.Bus.bus.send(e));