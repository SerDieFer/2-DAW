"use strict";

let listaOrdenadores = [];

function CrearOrdernador(marca, modelo, ram, disco, pulgadas, fecha, ...accesorios) {

  if (isNaN(marca)) this.marca = marca;
  else this.marca = undefined;

  if (isNaN(modelo)) this.modelo = modelo;
  else this.modelo = undefined;

  if (!isNaN(ram)) this.ram = ram;
  else this.ram = 16;

  if (!isNaN(disco)) this.disco = disco;
  else this.disco = 256;

  if (!isNaN(pulgadas)) this.pulgadas = pulgadas;
  else this.pulgadas = 15.6;

  if (isNaN(Date.parse(fecha))) {
    this.fecha = new Date().toISOString().split("T")[0];
  } else {
    this.fecha = new Date(fecha).toISOString().split("T")[0];
  }

  if (accesorios.length > 0) this.accesorios = accesorios;
  else this.accesorios = [];

  this.mostrarOrdenador = function () {
    let listaAccesorios = this.accesorios.forEach((accesorios) => {
      listaAccesorios += `- ${accesorios}\n`;
    });

    return `ORDENADOR: ${this.marca + this.modelo}
            RAM: ${this.ram} GB
            DISCO: ${this.disco} GB
            PULGADAS: ${this.pulgadas}"
            ALTA: ${this.fecha}
            ACCESORIOS: \n${listaAccesorios}`;
  };

  this.actualizarMarcaModelo = function (nuevaMarca, nuevoModelo) {
    if (isNaN(nuevaMarca)) this.marca = nuevaMarca;
    if (isNaN(nuevoModelo)) this.modelo = nuevoModelo;
  };

  this.actualizarRamDiscoPulgadas = function (
    nuevaRam,
    nuevoDisco,
    nuevasPulgadas
  ) {
    if (!isNaN(nuevaRam)) this.ram = nuevaRam;
    if (!isNaN(nuevoDisco)) this.disco = nuevoDisco;
    if (!isNaN(nuevasPulgadas)) this.pulgadas = nuevasPulgadas;
  };

  this.actualizarFecha = function (nuevaFecha) {
    let fechaNuevaFormateada = Date.parse(nuevaFecha);
    if (!isNaN(fechaNuevaFormateada)) this.fecha = fechaNuevaFormateada;
  };

  this.anyadirAccesorios = function (...nuevosAccesorios) {
    nuevosAccesorios.forEach((accesorio) => {
      if (!this.accesorios.includes(accesorio)) this.accesorios.push(accesorio);
    });
  };

  this.borrarAccesorios = function (...accesoriosABorrar) {
    this.accesorios = this.accesorios.filter(
      (accesorios) => !accesoriosABorrar.includes(accesorios)
    );
  };
}

let ordenador1 = new CrearOrdernador(
  "lenovo",
  "legion",
  32,
  256,
  15.6,
  "2022-11-09",
  "ratón",
  "teclado"
);

let ordenador2 = new CrearOrdernador(
  "hp",
  "omen",
  32,
  256,
  15.6,
  "2022-11-09",
  "ratón",
  "teclado"
);

let ordenador3 = new CrearOrdernador(
  "acer",
  "ferrari",
  32,
  256,
  15.6,
  "2022-09-09",
  "ratón",
  "teclado"
);

let ordenador4 = new CrearOrdernador(
  "msi",
  "modern",
  32,
  256,
  15.6,
  "2022-08-09",
  "ratón",
  "teclado"
);

function anyadirOrdenadores(...objetoOrdenador) {
  objetoOrdenador.forEach((ordenador) => listaOrdenadores.push(ordenador));
}

anyadirOrdenadores(ordenador1, ordenador2, ordenador3, ordenador4);

function mostrarOrdenadores() {
  return listaOrdenadores;
}

console.log(mostrarOrdenadores(listaOrdenadores));

function agruparOrdenadores(listaQueAgrupar) {
  return listaQueAgrupar.reduce((accumulator, ordenador) => {
    let fecha = ordenador.fecha;
    // Si la fecha ya existe en el acumulador, incrementamos su valor
    if (accumulator[fecha]) {
      accumulator[fecha]++;
    } else {
      // Si no existe, la inicializamos con el valor 1
      accumulator[fecha] = 1;
    }
    return accumulator;
  }, {});
}

let ordenadoresAgrupadosPorFecha5 = console.log(agruparOrdenadores(listaOrdenadores));
