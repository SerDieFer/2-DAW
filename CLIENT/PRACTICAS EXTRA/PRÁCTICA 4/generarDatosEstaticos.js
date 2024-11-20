import *  as gestionWeb from "./gestionPresupuestoWeb.js"
import * as gestion from "./gestionPresupuesto.js"


gestion.actualizarPresupuesto(1500);

gestionWeb.mostrarDatoEnId("presupuesto", gestion.mostrarPresupuesto());

let gastos = [];




