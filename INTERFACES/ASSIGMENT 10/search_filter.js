document.addEventListener("DOMContentLoaded", function () {
    const tabla = document.getElementById("tablaUsuarios");
    const tbody = tabla.querySelector("tbody");
    const inputBusqueda = document.getElementById("inputBusqueda");
    const selectorRegistros = document.getElementById("selectorRegistros");
    const filas = Array.from(tbody.querySelectorAll("tr"));
    let registrosPorPagina = parseInt(selectorRegistros.value);
    let paginaActual = 1;
    let sortDirection = 1; // 1 ascendente, -1 descendente

    // FILTRADO EN TIEMPO REAL
    inputBusqueda.addEventListener("input", () => {
        paginaActual = 1;
        actualizarTabla();
    });

    // CAMBIO EN REGISTROS POR PÁGINA
    selectorRegistros.addEventListener("change", () => {
        registrosPorPagina = parseInt(selectorRegistros.value);
        paginaActual = 1;
        actualizarTabla();
    });

    // ORDENAR COLUMNAS AL HACER CLIC
    document.querySelectorAll("th").forEach((th, index) => {
        if (th.textContent.trim() !== "Acción") {
            th.addEventListener("click", () => {
                ordenarTabla(index);
            });
            th.addEventListener("mouseover", () => {
                th.style.cursor = "pointer";
                th.style.backgroundColor = "#0d6efd";
                th.style.color = "white";
            });
            th.addEventListener("mouseout", () => {
                th.style.cursor = "default";
                th.style.backgroundColor = "";
                th.style.color = "";
            });
        }
    });

    // FUNCIÓN PARA ORDENAR LA TABLA
    function ordenarTabla(index) {
        filas.sort((a, b) => {
            const celdaA = a.children[index].textContent.trim();
            const celdaB = b.children[index].textContent.trim();

            const esNumero = !isNaN(celdaA) && !isNaN(celdaB);
            const valorA = esNumero ? parseFloat(celdaA) : celdaA;
            const valorB = esNumero ? parseFloat(celdaB) : celdaB;

            return (valorA > valorB ? sortDirection : -sortDirection);
        });

        sortDirection *= -1;
        actualizarTabla();
    }

    // ACTUALIZAR TABLA CON PAGINACIÓN Y FILTRADO
    function actualizarTabla() {
        const textoBusqueda = inputBusqueda.value.toLowerCase();
        const filasFiltradas = filas.filter(fila => 
            fila.textContent.toLowerCase().includes(textoBusqueda)
        );

        tbody.innerHTML = "";

        const inicio = (paginaActual - 1) * registrosPorPagina;
        const filasPagina = filasFiltradas.slice(inicio, inicio + registrosPorPagina);

        filasPagina.forEach(fila => tbody.appendChild(fila));

        actualizarPaginacion(filasFiltradas.length);
    }

    // ACTUALIZAR LOS CONTROLES DE PAGINACIÓN
    function actualizarPaginacion(totalFilas) {
        const textoRegistros = document.getElementById("textoRegistros");
        const totalPaginas = Math.ceil(totalFilas / registrosPorPagina);
        textoRegistros.textContent = `Mostrando del ${(paginaActual - 1) * registrosPorPagina + 1} al ${Math.min(paginaActual * registrosPorPagina, totalFilas)} de un total de ${totalFilas} registros`;

        const paginacion = document.querySelector(".pagination");
        paginacion.innerHTML = "";

        const crearItemPaginacion = (texto, habilitado, callback) => {
            const li = document.createElement("li");
            li.classList.add("page-item");
            if (!habilitado) li.classList.add("disabled");
            if (texto === paginaActual) li.classList.add("active");
            
            const link = document.createElement("a");
            link.classList.add("page-link");
            link.textContent = texto;
            if (habilitado) link.addEventListener("click", callback);
            li.appendChild(link);
            paginacion.appendChild(li);
        };

        crearItemPaginacion("Anterior", paginaActual > 1, () => {
            paginaActual--;
            actualizarTabla();
        });

        for (let i = 1; i <= totalPaginas; i++) {
            crearItemPaginacion(i, true, () => {
                const item = paginacion.querySelector(".page-item.active");
                if (item) item.classList.remove("active");
                paginaActual = i;
                actualizarTabla();
            });
        }

        crearItemPaginacion("Siguiente", paginaActual < totalPaginas, () => {
            paginaActual++;
            actualizarTabla();
        });
    }

    // INICIALIZAR TABLA AL CARGAR
    actualizarTabla();
});
