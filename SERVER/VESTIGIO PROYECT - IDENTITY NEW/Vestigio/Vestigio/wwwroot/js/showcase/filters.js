document.addEventListener("DOMContentLoaded", initializeFilters);

function initializeFilters() {
    console.log("Inicializando filtros...");

    const filterForm = document.getElementById("filterForm");
    const activeTabInput = document.getElementById("activeTab");
    const resetButton = document.getElementById("resetFilters");
    const tabContent = document.getElementById("tabContent");
    const tabLinks = document.querySelectorAll('[data-bs-toggle="pill"]');
    let offcanvas = null;

    if (!filterForm || !activeTabInput || !resetButton || !tabContent) {
        console.error("Faltan elementos esenciales en el DOM");
        return;
    }

    offcanvas = new bootstrap.Offcanvas('#filterOffcanvas');

    // Capturamos el estado inicial de cada control en cada sección usando un Map
    const originalStates = {
        challenges: captureInitialState("challenges"),
        products: captureInitialState("products")
    };

    let currentTab = activeTabInput.value;
    console.log("Pestaña activa inicial:", currentTab);

    setupEventListeners();

    // Captura los controles y guarda su estado inicial en un Map
    function captureInitialState(tabName) {
        const section = document.getElementById(`${tabName}Filters`);
        if (!section) {
            console.warn(`No se encontró la sección: ${tabName}Filters`);
            return new Map();
        }
        const controls = section.querySelectorAll("input, select, textarea");
        const map = new Map();
        Array.from(controls).forEach(el => {
            map.set(el, { value: el.value, checked: el.checked, type: el.type });
        });
        return map;
    }

    function setupEventListeners() {
        filterForm.addEventListener('input', (e) => {
            if (e.target.type === 'number') handleNumberInput(e.target);
        });

        filterForm.addEventListener('blur', (e) => {
            if (e.target.type === 'number') validateNumberInput(e.target);
        }, true);

        tabLinks.forEach(tab => tab.addEventListener("click", handleTabChange));

        resetButton.addEventListener("click", handleReset);
        filterForm.addEventListener("submit", handleFormSubmit);

        const debouncedFetch = debounce(fetchTabContent, 300);
        filterForm.addEventListener('input', debouncedFetch);
        filterForm.addEventListener('change', debouncedFetch);
    }

    function handleNumberInput(input) {
        const min = input.hasAttribute('min') ? parseInt(input.min) : 0;
        const max = input.hasAttribute('max') ? parseInt(input.max) : Infinity;
        let value = parseInt(input.value) || min;

        // Para filtros de challenges, asegurar que el nivel esté entre 1 y 10
        if (input.closest('#challengesFilters') && (input.name === 'minLevel' || input.name === 'maxLevel')) {
            value = Math.max(1, Math.min(10, value));
        } else {
            value = Math.max(min, value);
        }
        input.value = value;
    }

    function validateNumberInput(input) {
        if (input.type === 'number') {
            const min = input.hasAttribute('min') ? parseInt(input.min) : 0;
            const max = input.hasAttribute('max') ? parseInt(input.max) : Infinity;
            let value = parseInt(input.value) || min;
            value = Math.max(min, Math.min(max, value));
            input.value = value;
        }
    }

    async function handleTabChange(e) {
        e.preventDefault();
        console.log("Cambio de pestaña:", e.currentTarget);

        tabLinks.forEach(tab => tab.classList.remove("active"));
        e.currentTarget.classList.add("active");

        const newTab = e.currentTarget.getAttribute("data-bs-target").replace("#", "");
        console.log("Nueva pestaña:", newTab);

        // Restablecemos los filtros de la pestaña saliente y la entrante a sus valores por defecto
        resetTabFilters(currentTab);
        currentTab = newTab;
        activeTabInput.value = newTab;
        resetTabFilters(newTab);

        await fetchTabContent();
        updateFilterVisibility();
    }

    // Restaura cada control:
    // - Si es un SELECT, se pone al primer option (selectedIndex = 0)
    // - En caso contrario, se restaura a lo capturado inicialmente
    function resetTabFilters(tabName) {
        console.log(`Reseteando filtros para ${tabName}`);
        const section = document.getElementById(`${tabName}Filters`);
        if (!section || !originalStates[tabName]) return;
        const controls = section.querySelectorAll("input, select, textarea");
        Array.from(controls).forEach(el => {
            if (!el.name) return;
            if (el.tagName === "SELECT") {
                el.selectedIndex = 0;
            } else if (el.type === 'checkbox' || el.type === 'radio') {
                const original = originalStates[tabName].get(el);
                if (original) {
                    el.checked = original.checked;
                }
            } else {
                const original = originalStates[tabName].get(el);
                if (original) {
                    el.value = original.value;
                }
            }
        });
    }

    async function handleReset() {
        console.log("Reseteando filtros manualmente...");
        resetTabFilters(currentTab);
        await fetchTabContent();
        offcanvas.show();
    }

    function handleFormSubmit(e) {
        e.preventDefault();
        console.log("Formulario enviado, aplicando filtros...");
        fetchTabContent();
        return false;
    }

    async function fetchTabContent() {
        console.log(`Cargando contenido para la pestaña: ${currentTab}`);
        try {
            const params = new URLSearchParams();
            const section = document.getElementById(`${currentTab}Filters`);
            if (!section) {
                console.warn(`No se encontró la sección de filtros para ${currentTab}`);
                return;
            }

            const controls = section.querySelectorAll("input, select, textarea");
            console.log("Controles encontrados:", controls);
            const selectedCategories = [];

            // Limpiar las categorías previas antes de agregar nuevas
            params.delete('categories');  // Eliminar cualquier categoría que esté ya en los parámetros

            Array.from(controls).forEach(el => {
                if (!el.name) return;

                // Si es un checkbox y está marcado, lo añadimos
                if (el.type === 'checkbox') {
                    // Si está marcado, añadimos a los parámetros
                    if (el.checked) {
                        console.log(`Categoría seleccionada: ${el.value}`);
                        params.append("categories", el.value); // Usamos 'categories' como nombre de parámetro
                        selectedCategories.push(el.value); // Guardamos la categoría seleccionada
                    } else {
                        // Si no está marcado, no hacemos nada (no la añadimos a la URL)
                        console.log(`Categoría desmarcada: ${el.value}`);
                    }
                }
                else if (el.tagName === 'SELECT' && el.value) {
                    params.append(el.name, el.value);
                }
                else if (el.value && el.value.trim() !== '') {
                    params.append(el.name, el.value);
                }
            });

            console.log("Categorías seleccionadas:", selectedCategories); // Log de categorías seleccionadas

            params.append("activeTab", currentTab);
            console.log("Parámetros enviados:", params.toString());

            const response = await fetch(`/Showcase/Index?${params.toString()}`, {
                headers: { "X-Requested-With": "XMLHttpRequest" }
            });
            console.log("Respuesta HTTP:", response.status);
            if (!response.ok) throw new Error(`Error HTTP: ${response.status}`);
            const content = await response.text();
            console.log("Contenido recibido:", content);
            document.getElementById("tabContent").innerHTML = content;
            history.replaceState({}, "", `?${params.toString()}`);
            window.dispatchEvent(new CustomEvent("contentUpdated", { detail: { tab: currentTab } }));
        } catch (error) {
            console.error("Error al cargar contenido:", error);
        }
    }

    function updateFilterVisibility() {
        console.log("Actualizando visibilidad de filtros...");
        document.querySelectorAll(".filter-section").forEach(section => {
            section.style.display = (section.id === `${currentTab}Filters`) ? "block" : "none";
        });
    }

    function debounce(func, timeout = 300) {
        let timer;
        return (...args) => {
            clearTimeout(timer);
            timer = setTimeout(() => func.apply(this, args), timeout);
        };
    }
}
