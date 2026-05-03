document.addEventListener("DOMContentLoaded", function () {

    const qtyInput = document.getElementById("qty");
    const qtyAdd = document.getElementById("qtyAdd");
    const qtyBuy = document.getElementById("qtyBuy");

    if (!qtyInput) return;

    function syncQty() {
        if (qtyAdd) qtyAdd.value = qtyInput.value;
        if (qtyBuy) qtyBuy.value = qtyInput.value;
    }

    document.querySelector(".quantity-right-plus")?.addEventListener("click", function () {
        qtyInput.value = parseInt(qtyInput.value || "1") + 1;
        syncQty();
    });

    document.querySelector(".quantity-left-minus")?.addEventListener("click", function () {
        let current = parseInt(qtyInput.value || "1");
        if (current > 1) qtyInput.value = current - 1;
        syncQty();
    });

    qtyInput.addEventListener("input", syncQty);

    syncQty(); // أول تحميل
});