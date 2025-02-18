// productInteractions.js
document.addEventListener("DOMContentLoaded", initializeProductInteractions);
window.addEventListener("contentUpdated", initializeProductInteractions);

function initializeProductInteractions() {
    initializeModals();
    setupEventDelegation();
}

function initializeModals() {
    // Inicializar modales existentes
    document.querySelectorAll('.product-modal').forEach(modalEl => {
        new bootstrap.Modal(modalEl, { keyboard: false });
    });
}

function setupEventDelegation() {
    // Delegación de eventos para elementos dinámicos
    document.body.addEventListener('click', (e) => {
        const target = e.target;

        // Manejar tamaño seleccionado
        if (target.closest('.size-btn:not(.disabled)')) {
            handleSizeSelection(target);
        }

        // Manejar cantidad
        if (target.closest('.qty-btn')) {
            handleQuantityChange(target);
        }
    });

    // Validación de input manual
    document.body.addEventListener('input', (e) => {
        if (e.target.closest('.quantity-input')) {
            validateQuantityInput(e.target);
        }
    });
}

function handleSizeSelection(button) {
    const modal = button.closest('.product-modal');
    const productId = modal.dataset.productId;

    modal.querySelectorAll('.size-btn').forEach(btn => {
        btn.classList.remove('active', 'selected');
    });

    button.classList.add('active', 'selected');
    modal.querySelector(`#selectedSize-${productId}`).value = button.dataset.sizeId;

    // Actualizar cantidad máxima
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

    let newValue = parseInt(quantityInput.value) + delta;
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
    value = Math.max(1, Math.min(value, maxStock));

    input.value = value;
    input.classList.toggle('is-invalid', value > maxStock);

    const addToCartBtn = modal.querySelector('.btn-add-cart');
    if (addToCartBtn) {
        addToCartBtn.disabled = value < 1 || value > maxStock;
    }
}