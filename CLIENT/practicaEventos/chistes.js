let chistes = [
  {
    enunciado: "¿Qué le dice un bit a otro?",
    respuesta: "Nos vemos en el bus.",
  },
  {
    enunciado: "¿Qué es un terapeuta?",
    respuesta: "1024 GigaPeutas.",
  },
  {
    enunciado: "¿Cuántos programadores hacen falta para cambiar una bombilla?",
    respuesta: "Ninguno, porque es un problema de hardware.",
  },
];

// Referencia al elemento <div> que contiene el listado
let contenedor = document.getElementById("contenedor");

// Iteramos sobre el arreglo de chistes
chistes.forEach((chiste) => {
  // Creamos un <div> para cada chiste
  let divChiste = document.createElement("div");

  // Creamos un botón para cada chiste
  let boton = document.createElement("button");
  boton.textContent = "Ver respuesta";

  // Creamos un objeto manejador de eventos para el botón
  let manejadorBoton = new ManejadorRespuesta(divChiste);
  manejadorBoton.objetoChiste = chiste;

  // Añadimos el objeto manejador de eventos al botón al hacer click
  boton.addEventListener("click", manejadorBoton);

  // Añadimos el enunciado al <div>
  divChiste.append(chiste.enunciado, boton);

  // Añadimos el <div> al contenedor
  contenedor.append(divChiste);

  // Creamos un botón para editar el chiste
  let botonEditar = document.createElement("button");
  botonEditar.textContent = "Editar";
  divChiste.append(botonEditar);

  // Creamos un objeto manejador de eventos para el botón de editar
  let manejadorBotonEditar = new ManejadorEditar(divChiste, chiste);
  botonEditar.addEventListener("click", manejadorBotonEditar);
});

// Función constructora para crear los manejadores de eventos de ver respuesta de los botones
function ManejadorRespuesta(divChiste) {
  this.handleEvent = function (evento) {
    alert(this.objetoChiste.respuesta);
  };
}

// Función constructora para crear los manejadores de eventos de los botones de editar
function ManejadorEditar(divChiste, chiste) {
  this.handleEvent = function (evento) {
    chiste.enunciado = prompt("Editar enunciado:", chiste.enunciado);
    chiste.respuesta = prompt("Editar respuesta:", chiste.respuesta);

    // Actualizamos el contenido del divChiste
    // divChiste.childNodes[0] es el primer nodo hijo del <div>,
    // que es un nodo de texto que contiene el enunciado del chiste.
    // nodeValue es la propiedad que devuelve el contenido de texto
    // de un nodo de texto. Al asignarle un nuevo valor, estamos
    // actualizando el contenido del nodo de texto, que se verá reflejado
    // en la interfaz de usuario.
    divChiste.childNodes[0].nodeValue = chiste.enunciado;
    alert("Chiste editado");
  };
}


