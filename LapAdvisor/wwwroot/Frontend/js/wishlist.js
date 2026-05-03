document.addEventListener('click', async function (e) {
    const btn = e.target.closest('.btn-wishlist');
    if (!btn) return;

    e.preventDefault();

    // منع الضغط المتكرر (يعمل مشاكل مرات يضيف مرات لا)
    if (btn.dataset.loading === "true") return;
    btn.dataset.loading = "true";

    const itemId = btn.dataset.itemid;
    const inWishlist = btn.dataset.inwishlist === "true";
    const url = inWishlist ? '/Wishlist/RemoveAjax' : '/Wishlist/AddAjax';

    try {
        const r = await fetch(url, {
            method: 'POST',
            credentials: 'include',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            body: new URLSearchParams({ itemId })
        });

        const res = await r.json();

        if (res.status === "NOT_LOGGED_IN") {
            alert("Please login first");
            return;
        }

        if (res.status === "ADDED") {
            btn.classList.add('active');
            btn.dataset.inwishlist = "true";
            alert("Added to Wishlist ❤️");
        }
        else if (res.status === "REMOVED") {
            btn.classList.remove('active');
            btn.dataset.inwishlist = "false";
            alert("Removed from Wishlist 💔");
        }
        else if (res.status === "EXISTS") {
            // للتأكد: خلّيها active لأن العنصر موجود فعلاً
            btn.classList.add('active');
            btn.dataset.inwishlist = "true";
            alert("Already in Wishlist");
        }
    } catch (err) {
        console.error(err);
        alert("Unexpected error");
    } finally {
        btn.dataset.loading = "false";
    }
});

document.addEventListener('click', async function (e) {
    const btn = e.target.closest('.js-remove-wishlist');
    if (!btn) return;

    e.preventDefault();

    const itemId = btn.dataset.itemid;

    try {
        const r = await fetch('/Wishlist/RemoveAjax', {
            method: 'POST',
            credentials: 'include',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            body: new URLSearchParams({ itemId })
        });

        const res = await r.json();

        if (res.status === "REMOVED") {
            alert("Removed from Wishlist 💔");

            // احذف الكرت من الصفحة
            const card = btn.closest('.col-xl-3') || btn.closest('.product-box');
            if (card) card.remove();
        }
    } catch (err) {
        console.error(err);
        alert("Unexpected error");
    }
});