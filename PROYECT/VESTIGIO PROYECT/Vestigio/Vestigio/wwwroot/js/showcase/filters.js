document.addEventListener("DOMContentLoaded", initializeFilters);

function initializeFilters() {
    /*console.log("Inicializando filtros...");*/

    const filterForm = document.getElementById("filterForm");
    const activeTabInput = document.getElementById("activeTab");
    const resetButton = document.getElementById("resetFilters");
    const tabContent = document.getElementById("tabContent");
    const tabLinks = document.querySelectorAll('[data-bs-toggle="pill"]');
    let offcanvas = null;

    if (!filterForm || !activeTabInput || !resetButton || !tabContent) {
        /*console.error("Faltan elementos esenciales en el DOM");*/
        return;
    }

    offcanvas = new bootstrap.Offcanvas('#filterOffcanvas');

    // Capturamos el estado inicial de cada control en cada sección usando un Map
    const originalStates = {
        challenges: captureInitialState("challenges"),
        products: captureInitialState("products")
    };

    let currentTab = activeTabInput.value;

    // Inputs reseteados de inicio
    resetTabFilters(currentTab);
    /*console.log("Pestaña activa inicial:", currentTab);*/

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

        const debouncedFetch = debounce(fetchTabContent, 10);
        filterForm.addEventListener('input', debouncedFetch);
        filterForm.addEventListener('change', debouncedFetch);
    }

    function handleNumberInput(input) {
        const min = input.hasAttribute('min') ? parseInt(input.min) : 0;
        const max = input.hasAttribute('max') ? parseInt(input.max) : Infinity;
        let value = parseInt(input.value) || min;

        if (input.name === 'minLevel' || input.name === 'maxLevel') {
            value = Math.max(1, Math.min(10, value));
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
        updateItemCounts(currentTab);
        /*console.log("Cambio de pestaña:", e.currentTarget);*/

        tabLinks.forEach(tab => tab.classList.remove("active"));
        e.currentTarget.classList.add("active");

        const newTab = e.currentTarget.getAttribute("data-bs-target").replace("#", "");
        /*console.log("Nueva pestaña:", newTab);*/

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
/*        console.log(`Reseteando filtros para ${tabName}`);*/
        const section = document.getElementById(`${tabName}Filters`);
        if (!section || !originalStates[tabName]) return;

        const controls = section.querySelectorAll("input, select, textarea");
        Array.from(controls).forEach(el => {
            if (!el.name) return;

            if (el.tagName === "SELECT")
            {
                el.selectedIndex = 0;
            }
            else if (el.type === 'checkbox' || el.type === 'radio')
            {
                const original = originalStates[tabName].get(el);

                if (original)
                {
                    el.checked = original.checked;
                }
            }
            else
            {
                const original = originalStates[tabName].get(el);
                if (original)
                {
                    el.value = original.value;
                }
            }
        });
    }

    async function handleReset() {
        resetTabFilters(currentTab);
        await fetchTabContent();
        offcanvas.show();
    }

    function handleFormSubmit(e) {
        e.preventDefault();
        fetchTabContent();
        return false;
    }

    async function fetchTabContent() {
        //console.log(`Cargando contenido para la pestaña: ${currentTab}`);
        try {
            const params = new URLSearchParams();
            const section = document.getElementById(`${currentTab}Filters`);
            if (!section) return;

            // Limpiar TODOS los parámetros previos excepto activeTab
            const currentParams = new URLSearchParams(window.location.search);
            currentParams.forEach((value, key) => {
                if (key !== "activeTab") params.delete(key);
            });

            // Capturar SOLO los filtros de la pestaña actual
            const controls = section.querySelectorAll("input, select, textarea");
            const activeFilters = {
                challenges: ["minLevel", "maxLevel", "solutionType", "minXP", "maxXP", "minCoins", "maxCoins", "showSolved", "challengeSort"],
                products: ["minPrice", "maxPrice", "categories", "sizes", "productSort"]
            };

            Array.from(controls).forEach(el => {
                if (!el.name || !activeFilters[currentTab].includes(el.name.replace("[]", ""))) return;

                // Manejar checkboxes (categorías y tallas)
                if (el.type === "checkbox") {
                    if (el.checked) params.append(el.name, el.value);
                }
                // Manejar selects e inputs
                else if ((el.tagName === "SELECT" || el.type === "number") && el.value) {
                    params.set(el.name, el.value);
                }
            });

            // Forzar actualización de activeTab
            params.set("activeTab", currentTab);
            //console.log("Parámetros enviados:", params.toString());

            const response = await fetch(`/Showcase/Index?${params.toString()}`, {
                headers: { "X-Requested-With": "XMLHttpRequest" }
            });
            //console.log("Respuesta HTTP:", response.status);

            if (!response.ok) throw new Error(`Error HTTP: ${response.status}`);
            const content = await response.text();
            //console.log("Contenido recibido:", content);

            // Verificar si el contenido devuelto es vacío o solo contiene el mensaje "no hay productos"
            if (content.trim() === '' || content.includes('No se encontraron productos')) {
                tabContent.innerHTML = '<div id="noProductsFilter" class="alert alert-warning">No se encontraron productos con los filtros seleccionados.</div>';
            } else {
                tabContent.innerHTML = content;
            }

            document.getElementById("tabContent").innerHTML = content;
            history.replaceState({}, "", `?${params.toString()}`);
            window.dispatchEvent(new CustomEvent("contentUpdated", { detail: { tab: currentTab } }));
            updateItemCounts(currentTab);
        } catch (error) {
            console.error("Error al cargar contenido:", error);
        }
    }

    function updateItemCounts(tab) {
        // Contar elementos directamente en el DOM actual, no en el HTML recibido
        const count = tab === 'challenges'
            ? document.querySelectorAll('.challenge-card').length
            : document.querySelectorAll('.product-card').length;

        // Actualizar badge
        const badge = document.querySelector(`#${tab}-tab .badge`);
        if (badge) badge.textContent = count;
    }

    function updateFilterVisibility() {
        /*console.log("Actualizando visibilidad de filtros...");*/
        document.querySelectorAll(".filter-section").forEach(section => {
            section.style.display = (section.id === `${currentTab}Filters`) ? "block" : "none";
        });
    }

    function debounce(func, timeout = 10) {
        let timer;
        return (...args) => {
            clearTimeout(timer);
            timer = setTimeout(() => func.apply(this, args), timeout);
        };
    }
}
