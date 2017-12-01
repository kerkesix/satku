// Extend jQuery to understand our new function
// TODO: Replace with postal.js to get rid of jquery reference
interface ITopic {
    publish(...arguments: any[]);
    subscribe(...handler: any[]);
    unsubscribe(...handler: any[]);
}

interface JQueryStatic {
    Topic(topic: string): ITopic;
}

(function () {
    var topics = {};

    jQuery.Topic = id => {
        var callbacks,
            topic = id && topics[id];

        if (!topic) {
            callbacks = jQuery.Callbacks();
            topic = {
                publish: callbacks.fire,
                subscribe: callbacks.add,
                unsubscribe: callbacks.remove
            };
            if (id) {
                topics[id] = topic;
            }
        }
        return topic;
    };
})();