"use strict";

/* FUNCIÓN CONSTRUCTORA PARA CREAR UN OBJETO DE TIPO CREARPERSONA
   QUE TIENE LOS ATRIBUTOS NOMBRE, APELLIDOS, NIF, EDAD, PUESTO, SALARIO Y ANTIGUEDAD */
function CrearPersona(nombre, apellidos, nif, edad, puesto, salario, antiguedad) {
    // SE ASIGNAN LOS ATRIBUTOS AL OBJETO
    this.nombre = nombre;
    this.apellidos = apellidos;
    this.nif = nif;
    this.edad = edad;
    this.puesto = puesto;
    this.salario = salario;
    this.antiguedad = antiguedad;
}

/* FUNCIÓN QUE RECIBE UN OBJETO DE TIPO CREARPERSONA Y LO AGREGA AL 
   ARRAY EMPLEADOS Y ASIGNA UN ID UNICO A CADA OBJETO */
function anyadirEmpleado(empleado) {
    // SE ASIGNA UN ID UNICO AL OBJETO
    empleado.id = contadorId++;
    // SE AGREGA EL OBJETO AL ARRAY EMPLEADOS
    empleados.push(empleado);
}

/* FUNCIÓN QUE RECIBE UN OBJETO DE TIPO CREARPERSONA Y DEVUELVE UN ELEMENTO <LI>
   QUE CONTIENE LOS DATOS DEL EMPLEADO */
function mostrarEmpleado(empleado) {
    // SE CREA EL ELEMENTO <LI> QUE CONTENDRA LOS DATOS DEL EMPLEADO CONCRETO
    let li = document.createElement("li");
    li.className = "rounded-list";
    li.id = "li" + empleado.id;

    // SE CREA EL ELEMENTO <DIV> QUE CONTENDRA LOS DATOS DEL EMPLEADO
    let divEmpleado = document.createElement("div");
    divEmpleado.className = "empleado";
    divEmpleado.id = empleado.id;
    li.appendChild(divEmpleado);

    // SE CREA EL PARRAFO QUE CONTENDRA EL NOMBRE Y APELLIDOS DEL EMPLEADO
    let pNombreApellidos = document.createElement("p");
    pNombreApellidos.textContent = `${empleado.nombre} ${empleado.apellidos}`;
    divEmpleado.appendChild(pNombreApellidos);

    // SE CREA EL PARRAFO QUE CONTENDRA EL NIF DEL EMPLEADO
    let pNIF = document.createElement("p");
    pNIF.textContent = `NIF: ${empleado.nif}`;
    divEmpleado.appendChild(pNIF);

    // SE CREA EL PARRAFO QUE CONTENDRA LA EDAD DEL EMPLEADO
    let pEdad = document.createElement("p");
    pEdad.textContent = `Edad: ${empleado.edad}`;
    divEmpleado.appendChild(pEdad);

    // SE CREA EL PARRAFO QUE CONTENDRA EL PUESTO DEL EMPLEADO
    let pPuesto = document.createElement("p");
    pPuesto.textContent = `Puesto: ${empleado.puesto}`;
    divEmpleado.appendChild(pPuesto);

    // SE CREA EL PARRAFO QUE CONTENDRA EL SALARIO DEL EMPLEADO
    let pSalario = document.createElement("p");
    pSalario.textContent = `Salario: ${empleado.salario}`;
    divEmpleado.appendChild(pSalario);

    // SE CREA EL PARRAFO QUE CONTENDRA LA ANTIGUEDAD DEL EMPLEADO
    let pAntiguedad = document.createElement("p");
    pAntiguedad.textContent = `Antigüedad: ${empleado.antiguedad}`;
    divEmpleado.appendChild(pAntiguedad);

    // SE CREA EL BOTON DE EDITAR Y SU MANEJADOR, POSTERIORMENTE SE AÑADE AL DIVEMPLEADO
    let botonEditar = document.createElement("button");
    botonEditar.id = "bEdit" + empleado.id;
    botonEditar.textContent = "Editar";
    let manejadorBotonEditar = new ManejadorBotonEditarEmpleado();
    manejadorBotonEditar.empleado = empleado;
    botonEditar.addEventListener("click", manejadorBotonEditar)
    divEmpleado.appendChild(botonEditar);

    // SE CREA EL BOTON DE BORRAR Y SU MANEJADOR, POSTERIORMENTE SE AÑADE AL DIVEMPLEADO
    let botonBorrar = document.createElement("button");
    botonBorrar.id = "bDelete" + empleado.id;
    botonBorrar.textContent = "Borrar";
    let manejadorBotonBorrar = new ManejadorBotonBorrarEmpleado();
    manejadorBotonBorrar.empleado = empleado;
    botonBorrar.addEventListener("click", manejadorBotonBorrar)
    divEmpleado.appendChild(botonBorrar);

    return li;
}

/* FUNCIÓN QUE MUESTRA / REPINTA LOS EMPLEADOS EN EL HTML */
function muestraWeb() {
    // SE LIMPIA EL CONTENIDO DEL DIV
    let divEmpleado = document.getElementById("divEmple");
    divEmpleado.innerHTML = '';

    // SE CREA EL TITULO DEL LISTADO
    let h1 = document.createElement("h1");
    h1.id = "tituloH1";
    h1.textContent = "Listado de Empleados";
    divEmpleado.appendChild(h1);

    // SE CREA LA LISTA DE LOS EMPLEADOS
    let ol = document.createElement("ol");
    ol.className = "rounded-list";
    ol.id = "listaOrd";
    divEmpleado.appendChild(ol);

    // SE RECORRE EL ARRAY EMPLEADOS Y SE AGREGA CADA EMPLEADO A LA LISTA
    empleados.forEach(empleado => {
        let li = mostrarEmpleado(empleado);
        ol.appendChild(li);
    });
}

/* FUNCIÓN QUE PIDE DATOS PARA UN NUEVO EMPLEADO Y LO AGREGA AL ARRAY EMPLEADOS */
function datosNuevoEmpleado() {
    // SE PIDEN LOS DATOS DEL NUEVO EMPLEADO
    let nombre = prompt("Introduce el nombre del nuevo empleado:");
    let apellidos = prompt("Introduce los apellidos del nuevo empleado:");
    let nif = prompt("Introduce el nif del nuevo empleado:");
    let edad = prompt("Introduce la edad del nuevo empleado:");
    let puesto = prompt("Introduce el puesto del nuevo empleado:");
    let salario = prompt("Introduce el salario del nuevo empleado:");
    let antiguedad = prompt("Introduce la antigüedad del nuevo empleado:");
    let empleado = new CrearPersona(nombre, apellidos, nif, edad, puesto, salario, antiguedad);

    // SE AGREGA EL NUEVO EMPLEADO AL ARRAY EMPLEADOS
    anyadirEmpleado(empleado);
    alert("Empleado creado correctamente");
}

/* FUNCIÓN CONSTRUCTORA PARA CREAR UN OBJETO MANEJADOR DE EVENTOS 
   QUE MANEJA EL EVENTO DE CLICK EN EL BOTÓN DE NUEVO EMPLEADO */
function ManejadorBotonNuevoEmpleado() {
    this.handleEvent = function () {
        // SE LLAMA A LA FUNCIÓN QUE PEDIRÁ LOS DATOS DEL NUEVO EMPLEADO
        datosNuevoEmpleado();
        // SE MUESTRA EL LISTADO DE EMPLEADOS
        muestraWeb();
    };
}

/* FUNCIÓN CONSTRUCTORA PARA CREAR UN OBJETO MANEJADOR DE EVENTOS
   QUE MANEJA EL EVENTO DE CLICK EN EL BOTÓN DE EDITAR EMPLEADO */
function ManejadorBotonEditarEmpleado() {
    this.handleEvent = function () {
        // SE LLAMA A LA FUNCIÓN QUE EDITA EL EMPLEADO
        editarEmpleado(this.empleado);
        // SE MUESTRA EL LISTADO DE EMPLEADOS
        muestraWeb();
    };
}

/* FUNCIÓN QUE RECIBE UN OBJETO DE TIPO CREARPERSONA Y LO EDITA */
function editarEmpleado(empleado) {
    // SE PIDEN LOS DATOS NUEVOS DEL EMPLEADO A EDITAR
    empleado.nombre = prompt("Introduce el nombre del empleado:");
    empleado.apellidos = prompt("Introduce los apellidos del empleado:");
    empleado.nif = prompt("Introduce el NIF del empleado:");
    empleado.edad = prompt("Introduce la edad del empleado:");
    empleado.puesto = prompt("Introduce el puesto del empleado:");
    empleado.salario = prompt("Introduce el salario del empleado:");
    empleado.antiguedad = prompt("Introduce la antigüedad del empleado:");
}

/* FUNCIÓN CONSTRUCTORA PARA CREAR UN OBJETO MANEJADOR DE EVENTOS 
   QUE MANEJA EL EVENTO DE CLICK EN EL BOTÓN DE BORRAR EMPLEADO */
function ManejadorBotonBorrarEmpleado() {
    this.handleEvent = function () {
        // SE LLAMA A LA FUNCIÓN QUE BORRA EL EMPLEADO
        borrarEmpleado(this.empleado);
        // SE MUESTRA EL LISTADO DE EMPLEADOS
        muestraWeb();
    };
}

/* FUNCIÓN QUE RECIBE UN OBJETO DE TIPO CREARPERSONA Y LO BORRA DEL ARRAY EMPLEADOS */
function borrarEmpleado(empleado) {
    // SE ENCUENTRA EL INDICE DEL EMPLEADO EN EL ARRAY EMPLEADOS
    let indice = empleados.indexOf(empleado);
    if (indice >= 0) {
        // SE BORRA EL EMPLEADO DEL ARRAY EMPLEADOS
        empleados.splice(indice, 1);
    }
}



