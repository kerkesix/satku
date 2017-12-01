/// <reference path="Messaging/jquery.topic.ts" />
/// <reference path="Messaging/Queues.ts" />
/// <reference path="Ksx.Routes.ts" />
/// <reference path="Ksx.Mutator.ts" />
/// <reference path="Ksx.Time.ts" />
/// <reference path="Ksx.Track.ts" />
/// <reference path="Ksx.Bus.ts" />
/// <reference path="ViewModelBase.ts" />

interface INProgressStatic {
    start();
    done();
}

declare var NProgress: INProgressStatic;