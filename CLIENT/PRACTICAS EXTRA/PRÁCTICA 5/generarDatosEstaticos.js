import *  as gestionWeb from "./gestionPresupuestoWeb.js"
import * as gestion from "./gestionPresupuesto.js";


gestion.actualizarPresupuesto(1500);

gestionWeb.mostrarDatoEnId("presupuesto", gestion.mostrarPresupuesto());

let nuevosGastos = [];
nuevosGastos.push(
    new gestion.CrearGasto("Compra carne", 23.44, "2021-10-06", "casa", "comida"),
    new gestion.CrearGasto("Compra fruta y verdura", 14.25, "2021-09-06", "supermercado", "comida"),
    new gestion.CrearGasto("Bonobús", 18.6, "2020-05-26", "transporte"),
    new gestion.CrearGasto("Gasolina", 60.42, "2021-10-08", "transporte", "gasolina"),
    new gestion.CrearGasto("Seguro hogar", 206.45, "2021-09-26", "casa", "seguros"),
    new gestion.CrearGasto("Seguro coche", 195.78, "2021-10-06", "transporte", "seguros")
);

nuevosGastos.forEach((gasto) => {
    gestion.anyadirGasto(gasto);
});

gestionWeb.mostrarDatoEnId("gastos-totales", gestion.calcularTotalGastos());
gestionWeb.mostrarDatoEnId("balance-total", gestion.calcularBalance());

let gastosActuales = gestion.listarGastos();
gastos.forEach((gasto) => {
    gestionWeb.mostrarGastoWeb("listado-gastos-completo", gasto);
});

let gastosFiltrados1 = gestion.filtrarGastos({
    fechaDesde: "2021/09/01",
    fechaHasta: "2021/09/30",
});

gastosFiltrados1.forEach((gasto) => {
    gestionWeb.mostrarGastoWeb("listado-gastos-filtrado-1", gasto);
});

let gastosFiltrado2 = gestion.filtrarGastos({
    valorMinimo: 50.0,
});

gastosFiltrado2.forEach((gasto) => {
    gestionWeb.mostrarGastoWeb("listado-gastos-filtrado-2", gasto);
});

let gastosFiltrado3 = gestion.filtrarGastos({
    valorMinimo: 200.0,
    etiquetasTiene: ["seguros"],
});

gastosFiltrado3.forEach((gasto) => {
    gestionWeb.mostrarGastoWeb("listado-gastos-filtrado-3", gasto);
});

let gastosFiltrados4 = gestion.filtrarGastos({
    etiquetasTiene: ["gasolina"],
    etiquetasNoTiene: ["seguros"],
});

gastosFiltrados4.forEach((gasto) => {
    gestionWeb.mostrarGastoWeb("listado-gastos-filtrado-4", gasto);
});

let gastosAgrupados1 = gestion.agruparGastos("dia");
gestionWeb.mostrarGastosAgrupadosWeb("agrupacion-dia", gastosAgrupados1, "dia");

let gastosAgrupados2 = gestion.agruparGastos("mes");
gestionWeb.mostrarGastosAgrupadosWeb("agrupacion-mes", gastosAgrupados2, "mes");

let gastosAgrupados3 = gestion.agruparGastos("anyo");
gestionWeb.mostrarGastosAgrupadosWeb("agrupacion-anyo", gastosAgrupados3, "anyo");


