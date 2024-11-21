"use strict";

function mostrarDatoEnId(idElemento, valor) {
    let elemento = document.getElementById(idElemento);

    if (elemento) {
        elemento.innerHTML = valor;
    } else {
        alert("El elemento seleccionado no existe");
    }
}

function mostrarGastoWeb(idElemento, gasto)  {
    let elemento = document.getElementById(idElemento);
    let esGastoFiltrado = idElemento.includes("listado-gastos-filtrado-");

    if (elemento) {
        let divGasto = document.createElemento("div");
        divGasto.className = "gasto";

        let divDescripcion = document.createElemento("div");
        divDescripcion.className = "gasto-descripcion";
        divDescripcion.innerHTML = gasto.descripcion;

        let divFecha = document.createElemento("div");
        divFecha.className = "gasto-fecha";
        divFecha.innerHTML = new Date(gasto.fecha).toLocaleString();

        let divValor = document.createElemento("div");
        divValor.className = "gasto-valor";
        divValor.innerHTML = gasto.valor;

        let divEtiquetas = document.createElemento("div");
        divEtiquetas.className = "gasto-etiquetas";
        gasto.etiquetas.forEach(etiqueta => { 
            if (!esGastoFiltrado) {
                let spanEtiqueta = document.createElemento("span");
                spanEtiqueta.className = "gasto-etiquetas-etiqueta";
                spanEtiqueta.innerHTML = etiqueta;

                    let manejadorBorrarEtiqueta = new BorrarEtiquetasHandle();
                    manejadorBorrarEtiqueta.gasto = gasto;
                    manejadorBorrarEtiqueta.etiqueta = etiqueta;
                    spanEtiqueta.addEventListener("click", manejadorBorrarEtiqueta);
            }
            else {
                spanEtiqueta.style.pointerEvents = "none"; 
            }
            divEtiquetas.appendChild(spanEtiqueta);
        });

        let divGastoOpciones = document.createElement("div");
        divGastoOpciones.classList.add("gasto-opciones-div");
        
            let botonEditar = document.createElement("button");
            botonEditar.classList.add("gasto-editar");
            botonEditar.type = "button"; 
            botonEditar.innerHTML = "Editar Gasto";

                if (!esGastoFiltrado) {
                    let manejadorEditar = new EditarGastoHandle();
                    manejadorEditar.gasto = gasto;
                    botonEditar.addEventListener("click", manejadorEditar);
                }

            divGastoOpciones.append(botonEditar);

            let botonEliminar = document.createElement("button");
            botonEliminar.classList.add("gasto-eliminar");
            botonEliminar.type = "button"; 
            botonEliminar.innerHTML = "Eliminar Gasto";

                if (!esGastoFiltrado) {
                    let manejadorEliminar = new BorrarGastoHandle();
                    manejadorEliminar.gasto = gasto;
                    botonEliminar.addEventListener("click", manejadorEliminar);
                }

            divGastoOpciones.append(botonEliminar);
        
        divGasto.appendChild(divDescripcion);
        divGasto.appendChild(divFecha);
        divGasto.appendChild(divValor);
        divGasto.appendChild(divEtiquetas);

        if (!esGastoFiltrado) {
            divGasto.appendChild(divGastoOpciones);
        }

        elemento.appendChild(divGasto);
    }
    else {
        alert("El elemento seleccionado no existe");
    }
}

function mostrarGastosAgrupadosWeb(idElemento, agroup, periodo) {
    let elemento = document.getElementById(idElemento);
    if (elemento) {
        let divAgrupacion = document.createElemento("div");
        divAgrupacion.className = "agrupacion";

        let tituloH1 = document.createElemento("h1");
        tituloH1.innerHTML = periodo === "anyo" ? "Gastos agrupados por año" 
            : periodo === "mes" ? "Gastos agrupados por mes" 
            : periodo === "dia" ? "Gastos agrupados por dia" 
            : "ERROR - Período no especificado";

        divAgrupacion.appendChild(tituloH1);

        //Repasar
        Object.entries(agroup).forEach(([clave, valor]) => {
                
            let divAgrupacionDato = document.createElemento("div");
            divAgrupacionDato.className = "agrupacion-dato";

            let spanDatoClave = document.createElemento("span");
            spanDatoClave.className = "agrupacion-dato-clave";
            spanDatoClave.innerHTML = clave;

            let spanDatoValor = document.createElemento("span");
            spanDatoValor.className = "agrupacion-dato-valor";
            spanDatoValor.innerHTML = valor;

            divAgrupacionDato.appendChild(spanDatoClave);
            divAgrupacionDato.appendChild(spanDatoValor);

            divAgrupacion.appendChild(divAgrupacionDato);
        })
    
        elemento.appendChild(divAgrupacion);

    }
    else {
        alert("El elemento seleccionado no existe");
    }
}

import * as gestion from "./gestionPresupuesto.js";

function repintar() {
    mostrarDatoEnId("presupuesto", gestion.mostrarPresupuesto());
    mostrarDatoEnId("gastos-totales", gestion.calcularTotalGastos());
    mostrarDatoEnId("balance-total", gestion.calcularBalance());

    let gastos = gestion.listarGastos();
    gastos.forEach((gasto) => {
        mostrarGastoWeb("listado-gastos-completo", gasto);
    });
    
}

function ActualizarPresupuestoHandle() {
    this.handleEvent = function () {
        let presupuestoActualizado = Number(prompt("Introduce un presupuesto: "));
        gestion.actualizarPresupuesto(presupuestoActualizado);
        repintar();
    }
}

let botonActualizar = document.getElementById("boton-actualizar");
let manejadorActualizar = new ActualizarPresupuestoHandle();
botonActualizar.addEventListener("click", manejadorActualizar);

function NuevoGastoHandle() {
    this.handleEvent = function () {
        let descripcionGastoNuevo = prompt("Introduce una descripción para este gasto:");
        let valorGastoNuevo = Number(prompt("Introduce un valor para este gasto:"));
        let fechaGastoNuevo = prompt("Introduce una fecha para este nuevo gasto (en formato yyyy-mm-dd):");  
        let etiquetasGastoNuevo = prompt("Introduce una/s etiquetas para este nuevo gasto (separadas por comas):");
        let etiquetasSeparadas = etiquetasGastoNuevo.split(",");
        let nuevoGasto = new gestion.CrearGasto(descripcionGastoNuevo, valorGastoNuevo, fechaGastoNuevo, ...etiquetasSeparadas);
        repintar();
    }
}

let botonNuevoGasto = document.getElementById("boton-nuevo-gasto");
let manejadorNuevoGasto = new NuevoGastoHandle();
botonNuevoGasto.addEventListener("click", manejadorNuevoGasto);


function EditarGastoHandle() {
    this.handleEvent = function () {
        let descripcionEditada = prompt("Introduce una descripción para este gasto:", this.gasto.descripcion);
        let valorEditado =  Number(prompt("Introduce un valor para este gasto:", this.gasto.valor));
        let fechaEditada = prompt("Introduce una fecha para este nuevo gasto (en formato yyyy-mm-dd):", this.gasto.fecha);  
        let etiquetasEditadas = (prompt("Introduce una/s etiquetas para este nuevo gasto (separadas por comas):", this.gasto.etiquetas)).trim();
        let etiquetasEditadasSeparadas = etiquetasEditadas ? etiquetasEditadas.split(",") : [];
        this.gasto.actualizarDescripcion(descripcionEditada);
        this.gasto.actualizarValor(valorEditado);
        this.gasto.actualizarFecha(fechaEditada);
        this.gasto.anyadirEtiquetas(...etiquetasEditadasSeparadas);
        repintar();
    }
}

function BorrarGastoHandle() {
    this.handleEvent = function () {
        let gastoABorrar = this.gasto;
        gestion.borrarGasto(gastoABorrar);
        repintar();
    }
}

function BorrarEtiquetasHandle() {
    this.handleEvent = function () {
        this.gasto.borrarEtiquetas(this.etiqueta);
        repintar();
    }
}

export {
    mostrarDatoEnId,
    mostrarGastoWeb,
    mostrarGastosAgrupadosWeb,
    repintar,
    actualizarPresupuestoWeb,
    nuevoGasto
}