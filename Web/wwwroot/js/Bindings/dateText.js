(function () {
    ko.bindingHandlers.dateText = {
        init: function (element, valueAccessor, allBindingsAccessor) {
            var valueUnwrapped = ko.utils.unwrapObservable(valueAccessor()),
                allBindings = allBindingsAccessor(),
                format = allBindings.dateFormat,
                interceptor = ko.computed(function() {
                    return Ksx.Time.formatDateTime(valueUnwrapped, format === 'full', true);
                });

            ko.applyBindingsToNode(element, { text: interceptor });
        }
    };
    
    ko.virtualElements.allowedBindings.dateText = true;
})();