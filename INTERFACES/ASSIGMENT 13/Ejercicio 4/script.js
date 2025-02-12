let astronaut = document.getElementById("astronauta");
let finish = false;

function move() {
    if (finish) {
        astronauta.style.transition = "transform 5s linear";
        astronauta.style.transform = "translateX(0px)";
    } else {
        astronauta.style.transition = "transform 5s linear";
        astronauta.style.transform = "translateX(1000px)";
    }
    finish = !finish;
}
