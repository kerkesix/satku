(function () {
    ko.bindingHandlers.modal = {
        init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
            var popupSelector = ko.utils.unwrapObservable(valueAccessor()),
                popup = $(popupSelector),
                allBindings = allBindingsAccessor(),
                onOpenMethod = allBindings.onOpen,

                // Create child binding context --> modal can use $root etc.
                childContext = bindingContext.createChildContext(viewModel);

            $(element).click(function (event) {
                // Some views are loaded asynchronously, if that is the case 
                // re-check the dom.
                if (!popup.length) {
                    popup = $(popupSelector);
                }

                var popupDom = popup.get(0);
                
                // Close all existing modals
                $(".modal").modal('hide');

                // Apply bindings only when clicked. First clean as by default 
                // bindings can be applied only once per element.

                // cleanNode does not work correctly with foreach, 
                // see https://github.com/knockout/knockout/issues/912
                // Therefore also using own attribute for further cleaning.
                ko.cleanNode(popup.get(0));
                popup.find("[data-cleanup]").empty();

                // If give view model contains onModalOpen function, call that
                if (onOpenMethod) {
                    var callOnOpen = viewModel[onOpenMethod];
                    if (typeof callOnOpen === "function") {
                        callOnOpen.apply(viewModel);
                    }
                }

                // Using applybindings multiple times on same element is 
                // not the preferred approach, but this seems to work.
                ko.applyBindings(childContext, popupDom);

                // If modal has already been constructed, just open, 
                // otherwise create and open
                popup.modal({ show: true });

                // Log the open event
                Ksx.Track.pageEvent("popup", "open", popupDom.id);

                event.preventDefault();
            });

            // When any link inside the modal is clicked, close modal
            popup.on("click", "a", function () {
                popup.modal("hide");
            });
        }
    };
})();