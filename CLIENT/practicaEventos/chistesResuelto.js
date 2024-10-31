let chistes = [
    {
	enunciado: "¿Qué le dice un bit a otro?",
	respuesta: "Nos vemos en el bus."
    },
    {
	enunciado: "¿Qué es un terapeuta?",
	respuesta: "1024 GigaPeutas."
    },
    {
	enunciado: "¿Cuántos programadores hacen falta para cambiar una bombilla?",
	respuesta: "Ninguno, porque es un problema de hardware."
    }
]

// Elemento <div> en el que se mostrarÃ¡ el listado
// Debe estar creado en el documento HTML que ejecute el script
let contenedor = document.getElementById("contenedor");

// FunciÃ³n constructora para crear los manejadores de eventos de los botones
function Manejador() {
    // MÃ©todo 'handleEvent'
    this.handleEvent = function(evento) {
	// la propiedad objetoChiste del objeto harÃ¡ referencia al objeto chiste
	alert(this.objetoChiste.respuesta);
    }
}

// Recorremos el array de chistes
for (let chiste of chistes) {
    console.log(chistes);
    // Creamos un <div> para cada chiste
    let divChiste = document.createElement("div");

    // Creamos un botÃ³n para cada chiste
    let boton = document.createElement("button");
    boton.textContent = "Ver respuesta";

    // Creamos un objeto manejador de eventos a partir del prototipo
    let manejadorBoton = new Manejador();

    // Creamos la propiedad 'objetoChiste' en el manejador
    // para que apunte al objeto 'chiste'
    manejadorBoton.objetoChiste = chiste;

    // AÃ±adimos el objeto manejador de eventos al botÃ³n
    boton.addEventListener("click", manejadorBoton);

    // AÃ±adimos el enunciado al <div>
    divChiste.append(chiste.enunciado);

    // AÃ±adimos el botÃ³n al <div>
    divChiste.append(boton);

    // AÃ±adimos el <div> al contenedor
    contenedor.append(divChiste);
}