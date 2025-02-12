const texto = document.getElementById("texto");

texto.addEventListener("mouseenter", () => {
    texto.style.fontSize = "30px";
});

texto.addEventListener("mouseleave", () => {
    texto.style.fontSize = "12px";
});