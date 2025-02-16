// filters.js
document.addEventListener("DOMContentLoaded", initializeFilters);

function initializeFilters() {
    const filterForm = document.getElementById("filterForm");
    const activeTabInput = document.getElementById("activeTab");
    const resetButton = document.getElementById("resetFilters");
    const tabContent = document.getElementById("tabContent");
    const tabLinks = document.querySelectorAll('[data-bs-toggle="pill"]');
    let offcanvas = null;

    if (!filterForm || !activeTabInput || !resetButton || !tabContent) return;

    // Inicializar Offcanvas de Bootstrap
    offcanvas = new bootstrap.Offcanvas('#filterOffcanvas');

    let currentTab = activeTabInput.value;
    let initialStates = {
        challenges: captureTabState("challenges"),
        products: captureTabState("products")
    };

    setupEventListeners();

    function captureTabState(tabName) {
        return {
            filters: Array.from(filterForm.elements)
                .filter(el => el.closest(`#${tabName}Filters`))
                .reduce((acc, el) => {
                    if (el.name) acc[el.name] = el.type === 'checkbox' ? el.checked : el.value;
                    return acc;
                }, {}),
            url: new URL(window.location.href)
        };
    }

    function setupEventListeners() {

        filterForm.addEventListener("change", validateFilters);

        function validateFilters() {
            const activeTab = activeTabInput.value;

            // Validar nivel entre 1-10
            if (activeTab === 'challenges') {
                const minLevel = filterForm.querySelector('[name="minLevel"]');
                const maxLevel = filterForm.querySelector('[name="maxLevel"]');

                if (minLevel) minLevel.value = Math.max(1, Math.min(10, minLevel.value || 1));
                if (maxLevel) maxLevel.value = Math.max(1, Math.min(10, maxLevel.value || 10));
            }

            // Validar valores no negativos
            document.querySelectorAll('input[type="number"]').forEach(input => {
                if (!input.hasAttribute('min')) {
                    input.value = Math.max(0, input.value);
                }
            });
        }

        // Eventos para tabs
        tabLinks.forEach(tab => {
            tab.addEventListener("click", handleTabChange);
        });

        // Eventos para filtros
        filterForm.addEventListener("input", debounce(handleFilterInput, 300));
        filterForm.addEventListener("submit", handleFormSubmit);
        resetButton.addEventListener("click", handleReset);
    }

    async function handleTabChange(e) {
        e.preventDefault();
        const newTab = e.target.getAttribute("data-bs-target").replace("#", "");

        // Guardar estado actual
        initialStates[currentTab] = captureTabState(currentTab);

        // Actualizar tab sin abrir offcanvas
        currentTab = newTab;
        activeTabInput.value = newTab;

        restoreTabState(newTab);
        await fetchTabContent();
        updateFilterVisibility();
    }

    async function handleFilterInput() {
        await fetchTabContent();
    }

    function handleFormSubmit(e) {
        e.preventDefault();
        handleFilterInput();
        return false;
    }

    async function handleReset() {
        restoreTabState(currentTab, true);
        await fetchTabContent();
        offcanvas.show(); // Forzar reapertura
    }

async function fetchTabContent() {
    try {
        const params = new URLSearchParams();
        Array.from(filterForm.elements)
            .filter(el => el.closest(`#${currentTab}Filters`))
            .forEach(el => {
                if (el.name) {
                    if (el.type === 'checkbox') {
                        if (el.checked) {
                            params.append(el.name, el.value);
                        }
                    } else if (el.value) {
                        params.append(el.name, el.value);
                    }
                }
            });

        params.append("activeTab", currentTab);

        const response = await fetch(`?${params.toString()}`, {
            headers: { "X-Requested-With": "XMLHttpRequest" }
        });

        if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);

        tabContent.innerHTML = await response.text();
        history.replaceState({}, "", `?${params.toString()}`);

        window.dispatchEvent(new CustomEvent("contentUpdated", {
            detail: { tab: currentTab }
        }));

    } catch (error) {
        console.error("Error loading content:", error);
    }
}

    function updateFilterVisibility() {
        document.querySelectorAll(".filter-section").forEach(section => {
            section.style.display = section.id.includes(currentTab) ? "block" : "none";
        });
    }

    function restoreTabState(tabName, resetToInitial = false) {
        const state = resetToInitial ? initialStates[tabName].filters : captureTabState(tabName).filters;
        Array.from(filterForm.elements)
            .filter(el => el.closest(`#${tabName}Filters`))
            .forEach(el => {
                if (el.name && state[el.name] !== undefined) {
                    if (el.type === 'checkbox') {
                        el.checked = state[el.name];
                    } else {
                        el.value = state[el.name];
                    }
                }
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