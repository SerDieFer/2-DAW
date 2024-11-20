"use strict"

let presupuesto = 0;

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

function CrearGasto(descripcionGasto, valorGasto) {
    this.valor = valorGasto < 0 || isNaN(valorGasto) ? 0 : valorGasto;
    this.descripcion = descripcionGasto;

    this.mostrarGasto() = function () {
        return `El gasto ${this.descripcion} tiene un valor de ${this.valor}€.`;
    }

    this.actualizarValor() = function (nuevoValorParaActualizar) {
        this.valor = nuevoValorParaActualizar < 0 || isNaN(nuevoValorParaActualizar) ? this.valor : nuevoValorParaActualizar;
    }


}


export {
    mostrarPresupuesto,
    actualizarPresupuesto,
    CrearGasto,
}

