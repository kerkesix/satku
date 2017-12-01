/// <reference path="_references.ts" />

module Ksx {
    export class ViewModelBase {
        loaded: KnockoutObservable<boolean> = ko.observable(false);
        sammyContext: any;

        constructor (public domroot: string) { }

        public onload(context: any): void {
            if (this.loaded()) {
                return;
            }

            this.sammyContext = context;
            this.applyBindings();
        }

        applyBindings() {
            ko.applyBindings(this, document.getElementById(this.domroot + "-layout"));
            this.loaded(true);
        }
    }
}
