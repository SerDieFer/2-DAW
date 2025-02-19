// productInteractions.js
document.addEventListener("DOMContentLoaded", initializeProductInteractions);
window.addEventListener("contentUpdated", initializeProductInteractions);

// Funciones nombradas para manejar eventos
function handleBodyClick(e) {
    const target = e.target;

    // Manejar tamaño seleccionado
    const sizeBtn = target.closest('.size-btn:not(.disabled)');
    if (sizeBtn) {
        handleSizeSelection(sizeBtn);
        return;
    }

    // Manejar cantidad
    const qtyBtn = target.closest('.qty-btn');
    if (qtyBtn) {
        handleQuantityChange(qtyBtn);
    }
}

function handleBodyInput(e) {
    const quantityInput = e.target.closest('.quantity-input');
    if (quantityInput) {
        validateQuantityInput(quantityInput);
    }
}

function initializeProductInteractions() {
    initializeModals();
    setupEventDelegation();
}

function initializeModals() {
    document.querySelectorAll('.product-modal').forEach(modalEl => {
        new bootstrap.Modal(modalEl, { keyboard: false });
    });
}

function setupEventDelegation() {
    // Eliminar listeners previos y agregar nuevos
    document.body.removeEventListener('click', handleBodyClick);
    document.body.removeEventListener('input', handleBodyInput);
    document.body.addEventListener('click', handleBodyClick);
    document.body.addEventListener('input', handleBodyInput);
}

// Resto del código permanece igual...
function handleSizeSelection(button) {
    const modal = button.closest('.product-modal');
    const productId = modal.dataset.productId;

    modal.querySelectorAll('.size-btn').forEach(btn => {
        btn.classList.remove('active', 'selected');
    });

    button.classList.add('active', 'selected');
    modal.querySelector(`#selectedSize-${productId}`).value = button.dataset.sizeId;

    const quantityInput = modal.querySelector(`#quantity-${productId}`);
    quantityInput.max = button.dataset.stock;
    validateQuantityInput(quantityInput);
}

function handleQuantityChange(button) {
    const modal = button.closest('.product-modal');
    const productId = modal.dataset.productId;
    const quantityInput = modal.querySelector(`#quantity-${productId}`);
    const delta = button.classList.contains('qty-decrease') ? -1 : 1;

    const selectedSize = modal.querySelector('.size-btn.selected');
    const maxStock = selectedSize ? parseInt(selectedSize.dataset.stock) : 0;

    let currentValue = parseInt(quantityInput.value) || 0;
    let newValue = currentValue + delta;
    newValue = Math.max(1, Math.min(newValue, maxStock));

    quantityInput.value = newValue;
    modal.querySelector(`#selectedQuantity-${productId}`).value = newValue;
}

function validateQuantityInput(input) {
    const productId = input.dataset.productId;
    const modal = input.closest('.product-modal');
    const selectedSize = modal.querySelector('.size-btn.selected');
    const maxStock = selectedSize ? parseInt(selectedSize.dataset.stock) : 0;

    let value = parseInt(input.value) || 0;
    value = Math.max(0, Math.min(value, maxStock)); // Permite 0 si no hay stock

    // Asegurar mínimo 1 solo si hay stock disponible
    if (maxStock > 0) {
        value = Math.max(1, value);
    } else {
        value = 0;
    }

    input.value = value;
    input.classList.toggle('is-invalid', value > maxStock);

    const addToCartBtn = modal.querySelector('.btn-add-cart');
    if (addToCartBtn) {
        addToCartBtn.disabled = value < 1 || value > maxStock;
    }
}