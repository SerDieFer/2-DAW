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
            let spanEtiqueta = document.createElemento("span");
            spanEtiqueta.className = "gasto-etiquetas-etiqueta";
            spanEtiqueta.innerHTML = etiqueta;

            divEtiquetas.appendChild(spanEtiqueta);
        });

        divGasto.appendChild(divDescripcion);
        divGasto.appendChild(divFecha);
        divGasto.appendChild(divValor);
        divGasto.appendChild(divEtiquetas);

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

export {
    mostrarDatoEnId,
    mostrarGastoWeb,
    mostrarGastosAgrupadosWeb
}