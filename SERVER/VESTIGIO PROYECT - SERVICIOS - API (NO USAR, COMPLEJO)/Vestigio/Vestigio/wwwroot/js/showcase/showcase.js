// ================== GLOBAL STATE ==================
const state = {
    activeTab: "challenges", // Valor por defecto
    filters: {
        challenges: {},
        products: {}
    }
};

// ================== UTILIDADES ==================
function showLoader(show) {
    const loader = document.getElementById("loadingOverlay");
    if (loader) loader.style.display = show ? "flex" : "none";
}

function showErrorModal(message) {
    const errorModal = document.createElement("div");
    errorModal.innerHTML = `
        <div class="error-modal bg-danger text-white p-3 rounded">
            <p class="mb-2">${message}</p>
            <button class="btn btn-sm btn-light" onclick="this.parentElement.parentElement.remove()">Cerrar</button>
        </div>
    `;
    document.body.appendChild(errorModal);
}

function highlightActiveTab() {
    document.querySelectorAll("[data-bs-toggle='pill']").forEach(tab => {
        const tabName = tab.getAttribute("data-tab");
        tab.classList.toggle("active", tabName === state.activeTab);
    });
}

// ================== FUNCIONES PRINCIPALES ==================
function getFormFilters() {
    const formData = new FormData(document.getElementById("filterForm"));
    return Object.fromEntries(formData.entries());
}

async function fetchFilteredResults() {
    showLoader(true);

    // Limpiar valores y manejar campos específicos
    const cleanFilters = Object.fromEntries(
        Object.entries(state.filters[state.activeTab]).map(([key, value]) => {
            // Convertir campos numéricos vacíos a null
            if (["minLevel", "maxLevel", "minXP", "maxXP", "minCoins", "maxCoins", "minPrice", "maxPrice"].includes(key)) {
                return [key, value === "" ? null : Number(value)];
            }
            // Forzar solutionType a cadena vacía si es null/undefined
            if (key === "solutionType") {
                return [key, value || ""];
            }
            return [key, value];
        })
    );

    // Construir payload
    const payload = {
        activeTab: state.activeTab,
        UserLevel: state.filters[state.activeTab].UserLevel || 1, // Default 1
        ShowSolved: state.filters[state.activeTab].ShowSolved ?? false,
        ...cleanFilters,
        SolvedChallenges: [],
        UnlockedProductIds: window.unlockedProductIds || [],
        UnlockedProductLevels: window.unlockedProductLevels || []
    };

    try {

        const response = await fetch("/api/apishowcase/filter", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload)
        });

        if (!response.ok) throw new Error(`Error HTTP: ${response.status}`);

        const { html } = await response.json();
        document.getElementById("tabContent").innerHTML = html;
    } catch (error) {
        console.error("Error:", error);
        showErrorModal("Error al cargar datos. Intenta recargar.");
    } finally {
        showLoader(false);
    }
}

// ================== EVENT HANDLERS ==================
function handleTabClick(e) {
    e.preventDefault();
    const newTab = e.currentTarget.getAttribute("data-tab");
    state.activeTab = newTab; // Actualizar estado
    document.getElementById("activeTab").value = newTab; // Actualizar input hidden
    fetchFilteredResults(); // Recargar datos
}
function initializeEventListeners() {
    // Tabs
    document.querySelectorAll("[data-bs-toggle='pill'][data-tab]").forEach(tab => {
        tab.addEventListener("click", handleTabClick);
    });

    // Formulario
    const filterForm = document.getElementById("filterForm");
    if (filterForm) {
        filterForm.addEventListener("submit", (e) => {
            e.preventDefault();
            state.filters[state.activeTab] = getFormFilters();
            fetchFilteredResults();
        });
    }

    // Botón reset
    const resetBtn = document.getElementById("resetFilters");
    if (resetBtn) {
        resetBtn.addEventListener("click", () => {
            filterForm?.reset();
            state.filters[state.activeTab] = {};
            fetchFilteredResults();
        });
    }
}

// ================== INICIALIZACIÓN ==================
function initialize() {
    // Cargar estado inicial desde HTML
    const activeTabElement = document.getElementById("activeTab");
    if (activeTabElement) state.activeTab = activeTabElement.value;

    // Configurar UI inicial
    highlightActiveTab();
    initializeEventListeners();
    fetchFilteredResults();
}

// Iniciar cuando el DOM esté listo
document.addEventListener("DOMContentLoaded", initialize);