"use strict"

let presupuesto = 0;
let gastos = [];
let idGasto = 0;

function actualizarPresupuesto(nuevoPresupuesto) {
    if (nuevoPresupuesto >= 0 || isNaN(nuevoPresupuesto)) {
        presupuesto = nuevoPresupuesto;
        return presupuesto;
    }
    else {
        console.log("El presupuesto tiene que ser mayor a 0");
        return -1;
    }
}

function mostrarPresupuesto() {
    return `Tu presupuesto actual es de ${presupuesto} €`;
}

function CrearGasto(descripcionGasto, valorGasto, fechaGasto, ...etiquetasGasto) {

    this.valor = valorGasto < 0 || isNaN(valorGasto) ? 0 : valorGasto;
    this.descripcion = descripcionGasto;
    this.fecha = isNan(Date.parse(fechaGasto)) ? Date.now() : Date.parse(fechaGasto);
    this.etiqueta = etiquetasGasto.length > 0 ? etiquetasGasto : [];

    this.mostrarGasto() = function () {
        return `El gasto ${this.descripcion} tiene un valor de ${this.valor}€.`;
    }

    this.mostrarGastoCompleto() = function () {
        let fechaLocalizada = new Date(this.fecha).toLocaleString();
        let etiquetasListadas = "";
        this.etiquetas.forEach(etiqueta => {
            etiquetasListadas += `- ${etiqueta}\n`;
        });

        return (
            `Gasto correspondiente a ${this.descripcion} con valor ${this.valor} €.` +
            `\nFecha: ${fechaLocalizada}` +
            `\nEtiquetas:\n${etiquetasListadas}`
        );
    }

    this.actualizarValor() = function (nuevoValorParaActualizar) {
        this.valor = nuevoValorParaActualizar < 0 || isNaN(nuevoValorParaActualizar) ? this.valor : nuevoValorParaActualizar;
    }

    this.actualizarFecha() = function (nuevaFechaParaActualizar) {
        let nuevaFechaParaActualizar = Date.parse(nuevaFechaParaActualizar);
        this.fecha = !isNan(nuevaFechaParaActualizar) ? nuevaFechaParaActualizar : this.fecha;
    }

    this.anyadirEtiquetas() = function (...nuevasEtiquetasParaAnyadir) {
        nuevasEtiquetasParaAnyadir.forEach((etiqueta => {
            this.etiqueta.includes(etiqueta) ? null : this.etiqueta.push(etiqueta);
        }));
    }

    this.borrarEtiquetas = function (...etiquetasSeleccionadasParaBorrar) {
        this.etiquetas = this.etiquetas.filter(etiqueta => !etiquetasSeleccionadasParaBorrar.includes(etiqueta));
    }
}

function listarGastos() {
    return gastos;
}

function anyadirGasto(gasto) {
    gasto.id = idGasto;
    idGasto++;
    gastos.push(gasto);
}

function borrarGasto(id) {
    gastos = gastos.filter(gasto => gasto.id !== id)
}

function calcularTotalGastos() {
    let totalGastos = 0;
    gastos.forEach(gasto => totalGastos + gasto.valor);
    return totalGastos;
}

function calcularBalance() {
    return presupuesto - calcularTotalGastos();
}

export {
    mostrarPresupuesto,
    actualizarPresupuesto,
    CrearGasto,
    listarGastos,
    anyadirGasto,
    borrarGasto,
    calcularTotalGastos,
    calcularBalance
}

