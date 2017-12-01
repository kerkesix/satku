/// <reference path="../../lib/qunit/qunit/qunit.js" />
/// <reference path="../../lib/jquery/dist/jquery.js" />
/// <reference path="Barcode.js" />
var f = function (number) {
        if (console) {
            console.log("Received", number);
        }
    };

var keyEvent = Barcode.simulatedKeyEvent;

module("Barcode reader");

test("Namespace exists", function () {
    ok(Barcode, "Namespace && constructor should be defined");
});

test("Constructor throws if callback not given or invalid", function () {
    expect(2);
    throws(function () {
        Barcode();
    }, "Should throw with null input");
    throws(function () {
        Barcode("foo");
    }, "Should throw with non-function input");
});

test("Constructor throws if expected length given but invalid", function () {
    expect(1);
    throws(function () {
        Barcode(f, "foo");
    }, "Should throw with NaN input");
});

test("Constructor sets callback function for later use", function () {
    var target = new Barcode(f);
    strictEqual(target._callback, f);
});

test("Key is stored into buffer", function () {
    var target = new Barcode(f);
    target.readKey(keyEvent(48));
    equal(target._buffer, "0");
});

test("Non-numeric key is not stored into buffer, clears buffer", function () {
    var target = new Barcode(f);
    target.readKey(keyEvent(47));
    notEqual(target._buffer, "/");
    equal(target._buffer, "", "Should have cleared buffer");
});

test("Non-numeric key clears buffer", function () {
    var target = new Barcode(f);
    target._buffer = "111";
    target.readKey(keyEvent(47));
    equal(target._buffer, "", "Should have cleared buffer");
});

test("Return key clears buffer, does not trigger action on incomplete buffer", function () {
    expect(2);
    var result,
        callback = function (number) {
            result = number;
        },
        target = new Barcode(callback);

    target._buffer = "111";
    target.readKey(keyEvent(13));
    equal(target._buffer, "", "Should have cleared buffer");
    ok(!result, "Should not have triggered callback");
});

test("Return key with full barcode in buffer triggers action", function () {
    expect(2);
    var code = "123456789123",
        result,
        callback = function (number) {
            result = number;
        },
        target = new Barcode(callback);

    target._buffer = code;
    target.readKey(keyEvent(13));

    equal(result, code, "Should have triggered callback");
    equal(target._buffer, "", "Should have cleared buffer");
});

test("Expected barcode length can be given in constructor", function () {
    expect(2);
    var code = "123",
        result,
        callback = function (number) {
            result = number;
        },
        target = new Barcode(callback, code.length);

    // Instead of directly setting buffer, simulate repetitive keyups
    // target._buffer = code;
    for (var i = 0, len = code.length; i < len; i++) {
        target.readKey(keyEvent(code.charCodeAt(i)));
    }

    target.readKey(keyEvent(13));

    equal(result, code, "Should have triggered callback");
    equal(target._buffer, "", "Should have cleared buffer");
});

asyncTest("Buffer cleared if not complete in 1 s", 1, function () {
    var target = new Barcode(f);
    target._buffer = "111";

    // Read one more key, this triggers the timeout
    target.readKey(keyEvent(49));

    setTimeout(function () {
        ok(!target._buffer, "Should have cleared buffer");
        start();
    }, 1001);
});

test("Alphanumeric bar code is accepted", 1, function () {
    var code = "6GM1J2NA",
        result,
        callback = function (number) {
            result = number;
        },
        target = new Barcode(callback, 8);

    // Instead of directly setting buffer, simulate repetitive keyups
    // target._buffer = code;
    for (var i = 0, len = code.length; i < len; i++) {
        target.readKey(keyEvent(code.charCodeAt(i)));
    }

    target.readKey(keyEvent(13));

    equal(result, code);
});

test("Shift keys are ignored", 1, function () {
    var code = "6GM1J2NA",
        result,
        callback = function (number) {
            result = number;
        },
        target = new Barcode(callback, 8);

    // Instead of directly setting buffer, simulate repetitive keyups
    // target._buffer = code;
    for (var i = 0, len = code.length; i < len; i++) {
        // Some bar codes & bar code readers send shift before capital letters
        target.readKey(keyEvent(16));
        target.readKey(keyEvent(code.charCodeAt(i)));
    }

    target.readKey(keyEvent(13));

    equal(result, code);
});

test("Simulated barcode reading triggers action", function () {
    expect(1);
    var code = "123456789123",
        result,
        callback = function (number) {
            result = number;
        },
        target = new Barcode(callback);

    target.simulateReading(code);

    equal(result, code, "Should have triggered callback");
});