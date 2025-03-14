// FUNCIÓN PARA CARGAR UN COMPONENTE
function fetchComponent(file) {
    return fetch(file)
        .then((response) => {
            if (!response.ok) {
                throw new Error(`Error al cargar ${file}: ${response.statusText}`);
            }
            return response.text();
        })
        .catch((error) => {
            console.error(error);
            // DEVUELVE CONTENIDO VACÍO EN CASO DE ERROR
            return ""; 
        });
}

// CARGAR LOS COMPONENTES Y AGREGARLOS AL BODY
document.addEventListener("DOMContentLoaded", async () => {
    try {
        // FETCH DE CADA COMPONENTE
        const header = await fetchComponent("../../components/header/header.html");
        const nav = await fetchComponent("../../components/nav/nav.html");
        const footer = await fetchComponent("../../components/footer/footer.html");

        // DETERMINAR EL COMPONENTE MAIN BASADO EN LA PÁGINA
        let mainComponent = "";
        if (window.location.pathname.includes("home")) {
            mainComponent = await fetchComponent("../../components/main/main_home/main_home.html");
        } else if (window.location.pathname.includes("productos")) {
            mainComponent = await fetchComponent("../../components/main/main_productos/main_productos.html");
        }

        // INSERTA LOS COMPONENTES EN EL BODY
        document.body.innerHTML = `
            ${header}
            ${nav}
            ${mainComponent}
            ${footer}
        `;
    } catch (error) {
        console.error("Error cargando los componentes:", error);
    }
});
