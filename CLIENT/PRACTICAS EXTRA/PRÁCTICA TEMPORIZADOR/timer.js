"use strict";

let seconds = 0;
let minutes = 0;
let hours = 0;
let interval;
let h3 = document.getElementById('timer');

function timer() {
    seconds++;
    if (seconds == 60) {
        seconds = 0;
        minutes++;
    }

    if (minutes == 60) {
        minutes = 0;
        hours++;
    }

    // console.log(`${hours < 10 ? "0" + hours : hours}:
    //             ${minutes < 10 ? "0" + minutes : minutes}:
    //             ${seconds < 10 ? "0" + seconds : seconds}`);

    h3.innerHTML = `${hours < 10 ? "0" + hours : hours}:`+
                    `${minutes < 10 ? "0" + minutes : minutes}:`+
                    `${seconds < 10 ? "0" + seconds : seconds}`;
}

let iniBtn = document.getElementById("start");
iniBtn.addEventListener("click", () => {
    if (!interval) {
        interval = setInterval(timer, 1000);
    }
});

let pauseBtn = document.getElementById("pause");
pauseBtn.addEventListener("click", () => {
    clearInterval(interval);
    interval = undefined;
});

let resetBtn = document.getElementById("reset");
resetBtn.addEventListener("click", () => {
    seconds = 0;
    minutes = 0;
    hours = 0;
    h3.innerHTML = "00:00:00";
    clearInterval(interval);
    interval = undefined;
});    

