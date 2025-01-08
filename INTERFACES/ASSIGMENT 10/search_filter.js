document.addEventListener("DOMContentLoaded", function () {
    const tabla = document.getElementById("tablaUsuarios");
    const tbody = tabla.querySelector("tbody");
    const inputBusqueda = document.getElementById("inputBusqueda");
    const selectorRegistros = document.getElementById("selectorRegistros");
    const filas = Array.from(tbody.querySelectorAll("tr"));
    let registrosPorPagina = parseInt(selectorRegistros.value);
    let paginaActual = 1;
    let sortDirection = 1; // 1 ascendente, -1 descendente

    // Filtrado en tiempo real
    inputBusqueda.addEventListener("input", () => {
        paginaActual = 1;
        actualizarTabla();
    });

    // Cambio en registros por página
    selectorRegistros.addEventListener("change", () => {
        registrosPorPagina = parseInt(selectorRegistros.value);
        paginaActual = 1;
        actualizarTabla();
    });

    // Ordenar columnas al hacer clic
    document.querySelectorAll("th").forEach((th, index) => {
        if (th.textContent.trim() !== "Acción") {
            th.addEventListener("click", () => {
                ordenarTabla(index);
            });
        }
    });

    // Función para ordenar la tabla
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

    // Actualizar tabla con paginación y filtrado
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

    // Actualizar los controles de paginación
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
                paginaActual = i;
                actualizarTabla();
            });
        }

        crearItemPaginacion("Siguiente", paginaActual < totalPaginas, () => {
            paginaActual++;
            actualizarTabla();
        });
    }

    // Inicializar tabla al cargar
    actualizarTabla();
});
