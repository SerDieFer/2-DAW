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

    this.obtenerPeriodoAgrupacion = function (periodo) {
        let fechaAgrupada = new Date(this.fecha);
        let anyo = fechaAgrupada.getFullYear().toString();
        let mes = (fechaAgrupada.getMonth() + 1).toString().padStart(2, "0");
        let dia = fechaAgrupada.getDate().toString().padStart(2, "0");

        switch (periodo) {
            case "anyo":
                return anyo;
            case "mes":
                return `${anyo}-${mes}`;
            case "dia":
                return `${anyo}-${mes}-${dia}`;
        }   
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

function filtrarGastos({fechaDesde, fechaHasta, valorMinimo, valorMaximo, descripcionContiene, etiquetasTiene}) {
    let filtrado = [...gastos];

    if(fechaDesde !== undefined) {
        filtrado = filtrado.filter((gasto) => Date.parse(fechaDesde) <= gasto.fecha);
    }

    if(fechaHasta !== undefined) {
        filtrado = filtrado.filter((gasto) => Date.parse(fechaHasta) >= gasto.fecha);
    }

    if(valorMinimo !== undefined) {
        filtrado = filtrado.filter((gasto) => gasto.valor >= valorMinimo);
    }

    if(valorMaximo !== undefined) {
        filtrado = filtrado.filter((gasto) => gasto.valor <= ValorMaximo);
    }

    if(descripcionContiene !== undefined) {
        filtrado = filtrado.filter((gasto) => gasto.descripcion.toLowerCase().includes(descripcionContiene.toLowerCase()));
    }

    if(etiquetasTiene !== undefined && etiquetasTiene.length > 0) {
        filtrado = filtrado.filter((gasto) => gasto.etiquetas.includes(etiquetasTiene));
    }

    return filtrado;
}

function agruparGastos (periodo = "mes", etiquetas = [], fechaDesde = undefined, fechaHasta = undefined) {
    
    let gastosFiltrados = filtrarGastos({fechaDesde: fechaDesde, fechaHasta: fechaHasta, etiquetasTiene: etiquetas});
    gastosFiltrados = gastosFiltrados.reduce((acc, gasto) => {
        let periodoAgrupacion= gasto.obtenerPeriodoAgrupacion(periodo);
        if(acc[periodoAgrupacion] == undefined) {
            acc[periodoAgrupacion] = 0;
        }
        else {
            acc[periodoAgrupacion] += gasto.valor;
        }
        return acc;
    }, {});
}



export {
    mostrarPresupuesto,
    actualizarPresupuesto,
    CrearGasto,
    listarGastos,
    anyadirGasto,
    borrarGasto,
    calcularTotalGastos,
    calcularBalance,
    filtrarGastos,
    agruparGastos
}

