/// <summary>Barcode reader class. Takes event input in 
/// and triggers a callback when full barcode is received. 
/// Automatically flushes buffer when full barcode is not 
/// received within one second. </summary>
var Barcode = function (callback, expectedLength) {
    var length = 12;

    if (!callback || typeof callback !== 'function') {
        throw new Error("Must specify a callback function "
        + "that is called when barcode is complete.");
    }

    if (typeof expectedLength !== 'undefined') {
        if (expectedLength && isNaN(expectedLength)) {
            throw new Error("Given expected barcode length must be a number.");
        }

        length = expectedLength;
    }

    this._callback = callback;
    this._buffer = "";
    this._validate = new RegExp("\\w{" + length + "}");
    this._timeout = null;

    // Bind to document keyup
    document.addEventListener("keyup", this.readKey.bind(this), false);
};

Barcode.prototype.readKey = function (event) {
    var that = this;

    // Ignore shift keys
    if (event.keyCode === 16) {
        return;
    }

    // Return key is sent by barcode reader as the last key
    if (event.keyCode === 13) {
        // proceed only if there is full barcode in the buffer
        if (this._buffer && this._buffer.match(this._validate)) {
            this._callback.call(this, this._buffer);
        }
    }

    // Accepted alphanumeric characters 0..9 + a..z 
    if (event.keyCode >= 48 && event.keyCode <= 90) {
        this._buffer += String.fromCharCode(event.keyCode);

        // In case user typed with the keyboard, clean the value
        if (this._timeout) {
            window.clearTimeout(this._timeout);
        }
        
        this._timeout = setTimeout(function () {
            that._buffer = "";
        }, 50);
    }
    else {
        // Invalid character or return, clear buffer
        this._buffer = "";
    }
};

Barcode.prototype.simulateReading = function (code) {
    for (var i = 0; i < code.length; i++) {
        this.readKey(Barcode.simulatedKeyEvent(code.charCodeAt(i)));
    }

    this.readKey(Barcode.simulatedKeyEvent(13));
};

Barcode.simulatedKeyEvent = function (charCode) {
    // Simulate keyup event, add properties if needed
    return {
        keyCode: charCode
    };
};