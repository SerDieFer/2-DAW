document.addEventListener("DOMContentLoaded", initialize);

// ============ GLOBAL STATE ============
const state = {
    activeTab: "challenges",
    filters: {
        challenges: {},
        products: {}
    }
};

// ============ INITIALIZATION ============
function initialize() {º
    state.activeTab = getActiveTabFromURL() || "challenges";
    highlightActiveTab();
    setupEventListeners();
    fetchFilteredResults();
}

// ============ EVENT LISTENERS ============
function setupEventListeners() {
    // Cambiar de pestaña
    document.querySelectorAll("[data-bs-toggle='pill']").forEach(tab => {
        tab.addEventListener("click", handleTabClick);
    });

    // Aplicar filtros dinámicamente (con debounce para mejorar rendimiento)
    document.getElementById("filterForm").addEventListener("input", debounce(updateFilters, 300));

    // Botón de aplicar filtros manualmente
    document.getElementById("applyFilters").addEventListener("click", (e) => {
        e.preventDefault();
        updateFilters();
    });

    // Resetear filtros
    document.getElementById("resetFilters").addEventListener("click", resetFilters);
}

// ============ TAB MANAGEMENT ============
function handleTabClick(e) {
    e.preventDefault();

    const newTab = e.target.dataset.bsTarget.replace("#", "");
    if (state.activeTab === newTab) return;

    state.activeTab = newTab;
    highlightActiveTab();
    restoreFilters();
    fetchFilteredResults();
}

function highlightActiveTab() {
    document.querySelectorAll("[data-bs-toggle='pill']").forEach(tab => {
        tab.classList.toggle("active", tab.dataset.bsTarget === `#${state.activeTab}`);
    });
    window.history.replaceState({}, "", `?activeTab=${state.activeTab}`);
}

// ============ FILTER MANAGEMENT ============
function updateFilters() {
    const formData = new FormData(document.getElementById("filterForm"));
    state.filters[state.activeTab] = Object.fromEntries(formData.entries());
    fetchFilteredResults();
}

function restoreFilters() {
    const form = document.getElementById("filterForm");
    const filters = state.filters[state.activeTab];

    for (const key in filters) {
        if (form.elements[key]) {
            form.elements[key].value = filters[key];
        }
    }
}

function resetFilters() {
    document.getElementById("filterForm").reset();
    state.filters[state.activeTab] = {};
    fetchFilteredResults();
}

// ============ AJAX REQUEST ============
async function fetchFilteredResults() {
    showLoader(true);

    const response = await fetch("/api/apishowcase/filter", {
        method: "POST",
        headers: { "Content-Type": "application/json", "X-Requested-With": "XMLHttpRequest" },
        body: JSON.stringify({
            activeTab: state.activeTab,
            ...state.filters[state.activeTab]
        })
    });

    if (response.ok) {
        const { html } = await response.json();
        document.getElementById("tabContent").innerHTML = html;
    } else {
        console.error("Error al obtener datos filtrados");
    }

    showLoader(false);
}

// ============ UTILITIES ============
function debounce(func, wait) {
    let timeout;
    return (...args) => {
        clearTimeout(timeout);
        timeout = setTimeout(() => func.apply(this, args), wait);
    };
}

function showLoader(show) {
    const loader = document.getElementById("loadingOverlay");
    if (loader) loader.style.display = show ? "flex" : "none";
}

function getActiveTabFromURL() {
    return new URLSearchParams(window.location.search).get("activeTab");
}
