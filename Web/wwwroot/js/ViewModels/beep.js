var Beep = function (frequency, length) {

    this.length = isNaN(length) ? 250 : length;
    this.frequency = isNaN(frequency) ? 1700 : frequency;

    this.audioCtx = new (window.AudioContext || window.webkitAudioContext)();
};

Beep.prototype.play = function (times) {
    var that = this;
    var length = this.length;
 
    // Not supported
    if (!this.audioCtx) {
        return;
    }

    var oscillator = this.audioCtx.createOscillator();
    var gainNode = this.audioCtx.createGain();
    oscillator.connect(gainNode);
    gainNode.connect(this.audioCtx.destination);

    // 'square', 'sawtooth', 'triangle' and 'custom'
    oscillator.type = 'square';
    oscillator.frequency.value = this.frequency;
    oscillator.start();

    setTimeout(function () {
        oscillator.stop();

        if (times > 1) {
            times--;
            setTimeout(function () { that.play(times); }, 150);
        }
    }, this.length);
};