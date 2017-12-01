module Ksx {
    export class Mutator {
        public static DtoToKnockoutObservableModel(target, dto) {
            var m, e, what = Object.prototype.toString;

            for (m in dto || {}) {
                if (dto.hasOwnProperty(m)) {
                    e = dto[m];
                    target[m] = what.call(e) === "[object Array]" ? <any>ko.observableArray(e) : <any>ko.observable(e);
                }
            }
        }
    }
}