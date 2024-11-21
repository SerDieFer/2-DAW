"use strict"

let coches = [];

function CrearCoche (marca, modelo, puertas = 5, potencia = 120, plazas = 5, fecha, ...accesorios) {
    this.marca = isNaN(marca) ? marca : undefined;
    this.modelo = isNaN(modelo) ? modelo : undefined;
    this.puertas = !isNaN(puertas) ? puertas : 3;
    this.potencia = !isNaN(potencia) ? potencia : 120;
    this.plazas = !isNaN(plazas) ? plazas : 5;
    this.fecha = isNaN(Date.parse(fecha)) ? new Date().toISOString().split("T")[0] : new Date(fecha).toISOString().split("T")[0];

    this.accesorios = [];
    accesorios.forEach(accesorio => {
        let accesorioLower = accesorio.toLowerCase();
        if(!this.accesorios.includes(accesorioLower)) this.accesorios.push(accesorioLower);
    });


    this.mostrarCoche = function () {
        let fechaLocal = new Date(this.fecha).toLocaleString();
        let accesoriosMostrar = '';
        this.accesorios.forEach(accesorio => {
            accesoriosMostrar += `- ${accesorio}\n`;
        });

        return `COCHE: ${this.marca} ${this.modelo}
                PUERTAS: ${this.puertas}
                POTENCIA: ${this.potencia}
                PLAZAS: ${this.plazas}
                F.VENTA: ${fechaLocal}
                ACCESORIOS: ${accesoriosMostrar}`;
    }

    this.actualizarMarcaModelo = function(nuevaMarca,nuevoModelo) {
        this.marca = isNaN(nuevaMarca) ? nuevaMarca : this.marca;
        this.modelo = isNaN(nuevoModelo) ? nuevoModelo : this.modelo;
    }

    this.actualizarPuertasPotenciaPlazas = function(nuevasPuertas,nuevaPotencia,nuevasPlazas) {
        this.puertas = !isNaN(nuevasPuertas) ? nuevasPuertas : this.puertas;
        this.potencia = !isNaN(nuevaPotencia) ? nuevaPotencia : this.potencia;
        this.plazas = !isNaN(nuevasPlazas) ? nuevasPlazas : this.plazas;
    }
    
    this.actualizarFecha = function(nuevaFecha) {
        let fechaFormateada =  Date.parse(nuevaFecha);
        this.fecha = !isNaN(fechaFormateada) ? fechaFormateada : this.fecha;
    }

    this.anyadirAccesorios = function(...nuevosAccesorios) {
        nuevosAccesorios.forEach((nuevoAccesorio) => {    
            let nuevoAccesorioLower = nuevoAccesorio.toLowerCase();
            if(!this.accesorios.includes(nuevoAccesorioLower)) 
                this.accesorios.push(nuevoAccesorioLower);
        });
    }

    this.borrarAccesorios = function(...accesoriosQueBorrar) {
        let accesoriosQueBorrarLower = accesoriosQueBorrar.map(accesorio => accesorio.toLowerCase());
        this.accesorios = this.accesorios.filter((accesorio) => !accesoriosQueBorrarLower.includes(accesorio));
    }

    this.ordenarArrayAccesorios = function() {
        this.accesorios.sort();
    }
}


function anyadirCoches(...cochesQueAnyadir) {
    coches.push(...cochesQueAnyadir);
}

let cochesQueAnyadir = anyadirCoches(
    new CrearCoche('citroën', 'c4', 4, 120, 5, '2022-11-09', 'elevalunas', 'cierre', 'climatizador'),
    new CrearCoche('seat', 'ibiza', 5, 110, 5, '2022-11-09', 'elevalunas', 'cierre'),
    new CrearCoche('audi', 'a4', 5, 180, 5, '2022-09-09', 'elevalunas', 'cierre'),
    new CrearCoche('renault', 'vel satis', 3, 200, 4, '2022-08-09', 'elevalunas', 'cierre')
)

function mostrarCoches() {
    coches.forEach(coche => {
        console.log(coche.mostrarCoche());
    });
}

function cochesAgrupados() {
    return coches.reduce((acumulador, coche) => { 
        let fechaVenta = coche.fecha;
        let fechaVentaFormateada = new Date(fechaVenta).toLocaleDateString();
        if(acumulador[fechaVentaFormateada]) {
            acumulador[fechaVentaFormateada] ++;
        } else {
            acumulador[fechaVentaFormateada] = 1;
        }

        return acumulador;
    }, {});
}

console.log(cochesAgrupados());

function filtrarCoches({mark, model, npuertas, potDesde, nplazas, acces}) {
    let cochesFiltrados = [...coches]

    if(mark !== undefined) cochesFiltrados = cochesFiltrados.filter(coche => coche.marca === mark);
    if(model !== undefined) cochesFiltrados = cochesFiltrados.filter(coche => coche.modelo === model);
    if(npuertas !== undefined) cochesFiltrados = cochesFiltrados.filter(coche => coche.puertas === npuertas);
    if(potDesde !== undefined) cochesFiltrados = cochesFiltrados.filter(coche => coche.potencia >= potDesde);
    if(nplazas !== undefined) cochesFiltrados = cochesFiltrados.filter(coche => coche.plazas === nplazas);
    if(acces !== undefined) cochesFiltrados = cochesFiltrados.filter(coche => coche.accesorios.includes(acces.toLowerCase()));

    return cochesFiltrados;

}

console.log(filtrarCoches({acces: 'climatizador'}));