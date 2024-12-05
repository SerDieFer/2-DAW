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
        const home = await fetchComponent("./pages/home/index.html");
        if (home) {
            // REEMPLAZAR TODO EL DOCUMENTO
            document.open();
            document.write(home);
            document.close();

            // ASEGURARSE DE QUE EL SCRIPT ADICIONAL (REDIRIGIR) SE CARGUE
            const script = document.createElement("script");
            script.type = "module";
            script.src = "./pages/home/load_components.js";
            document.body.appendChild(script);
        }
    } catch (error) {
        console.error("Error cargando los componentes:", error);
    }
});