function renderItems(items) {
    const container = document.getElementById('ItemArea');
    container.innerHTML = '';

    if (!items || items.length === 0) {
        container.innerHTML = `
                    <div class="col-12 text-center mt-5">
                        <h5>No products found</h5>
                    </div>`;
        return;
    }

    items.forEach(item => {
        container.innerHTML += `
                <div class="col-xl-3 col-6 col-grid-box">
                    <div class="product-box">
                        <div class="img-wrapper" style="position:relative;">

                            
                            <a href="#" class="btn-wishlist"
                               data-itemid="${item.itemId}"
                               data-inwishlist="false"
                               style="position:absolute;top:10px;right:10px;z-index:5;">
                                <i class="fa fa-heart"></i>
                            </a>

                            <div class="front">
                                <a href="/Items/ItemDetails/${item.itemId}">
                                    <img data-src="/Uploads/Items/${item.imageName ?? 'no-image.png'}"
                                         class="img-fluid blur-up lazyload bg-img">
                                </a>
                            </div>

                            <div class="back">
                                <a href="/Items/ItemDetails/${item.itemId}">
                                    <img data-src="/Uploads/Items/${item.imageName ?? 'no-image.png'}"
                                         class="img-fluid blur-up lazyload bg-img">
                                </a>
                            </div>
                        </div>

                        <div class="product-detail">
                            <a href="/Items/ItemDetails/${item.itemId}">
                                <h6>${item.itemName}</h6>
                            </a>

                            <h4>JD ${item.salesPrice}</h4>

                            <div class="mt-1">
                                <small><b>Score:</b> ${item.score}</small>
                            </div>

                            <div class="mt-2 d-flex gap-1 flex-wrap">
                                <button class="btn btn-sm btn-outline-primary"
                                        onclick="showExternalLinks(${item.itemId})">
                                    View External Links
                                </button>

                                <button class="btn btn-sm btn-outline-dark"
                                        onclick="addToCompare(${item.itemId})">
                                    Add to Compare
                                </button>

                                <button class="btn btn-sm btn-solid"
                                        onclick="addToCart(${item.itemId})">
                                    Add to Cart
                                </button>
                            </div>
                        </div>
                    </div>
                </div>`;
    });
}

// =========================
// Pagination
// =========================
function renderPagination(totalCount, currentPage) {
    if (!$('#ItemPagination').pagination) return; // لو البلجن مش محمّل

    $('#ItemPagination').pagination({
        dataSource: function (done) {
            done(new Array(totalCount));
        },
        pageSize: 12,
        pageNumber: currentPage,
        showGoInput: true,
        showGoButton: true,
        callback: function (data, pagination) {
            if (pagination.pageNumber !== currentPage) {
                loadRecommendations(pagination.pageNumber);
            }
        }
    });
}

// =========================
// Load Recommendations
// =========================
function getRequest(page = 1) {
    return {
        purpose: document.getElementById('recPurpose').value,
        budget: document.getElementById('recBudget').value,
        priority: document.getElementById('recPriority').value,
        screenSize: document.getElementById('recScreen').value,
        page: page,
        pageSize: 12
    };
}

function loadRecommendations(page = 1) {
    const req = getRequest(page);

    // 🔹 احفظ الحالة
    sessionStorage.setItem("rec_request", JSON.stringify(req));

    fetch('/Recommendation/GetRecommendations', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(req)
    })
        .then(res => res.json())
        .then(result => {
            renderItems(result.data);
            renderPagination(result.totalCount, page);
        });
}


document.getElementById('btnGetRec').addEventListener('click', () => loadRecommendations(1));

// =========================
// External Links Popup
// =========================
function showExternalLinks(itemId) {
    fetch('/Recommendation/ExternalLinksPopup?id=' + itemId)
        .then(res => res.text())
        .then(html => {
            document.getElementById('externalLinksContainer').innerHTML = html;
            const overlay = document.querySelector('.ext-overlay');
            const box = document.querySelector('.ext-popup');
            if (overlay) overlay.style.display = 'block';
            if (box) box.style.display = 'block';
        });

    document.addEventListener('click', function (e) {

        // زر X
        if (e.target.classList.contains('ext-close')) {
            closeExternalPopup();
        }

        // الضغط على الخلفية
        if (e.target.classList.contains('ext-overlay')) {
            closeExternalPopup();
        }
    });

    function closeExternalPopup() {
        const container = document.getElementById('externalLinksContainer');
        if (container) {
            container.innerHTML = '';
        }
    }

}

// =========================
// Compare
// =========================
async function addToCompare(itemId) {
    try {
        const r = await fetch('/Compare/AddAjax', {
            method: 'POST',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            body: new URLSearchParams({ itemId })
        });

        const res = await r.json();

        if (res.status === "FULL") {
            showToast(`Compare list is full (max ${res.max}). Remove one item first.`);
            return;
        }

        if (res.status === "ADDED") {
            showToast("Added to compare ✅");
            updateCompareBadge(res.count);
        }
    } catch (e) {
        console.error(e);
        showToast("Unexpected error");
    }
}

// =========================
// Action Cart
// =========================
function addToCart(itemId) {

    // 🔹 خزّن حالة Recommendation قبل ما تطلع
    const state = {
        purpose: document.getElementById('recPurpose').value,
        budget: document.getElementById('recBudget').value,
        priority: document.getElementById('recPriority').value,
        screenSize: document.getElementById('recScreen').value,
        page: 1   // لاحقًا بنخليها الصفحة الحالية لو بدك
    };

    sessionStorage.setItem('recommendation_state', JSON.stringify(state));

    // 🔹 روح على صفحة Cart (نفس السلوك الحالي)
    window.location.href = '/Order/AddToCart?itemId=' + itemId;
}
document.addEventListener('DOMContentLoaded', function () {

    const saved = sessionStorage.getItem('recommendation_state');
    if (!saved) return;

    const state = JSON.parse(saved);

    // رجّع القيم للـ selects
    document.getElementById('recPurpose').value = state.purpose || '';
    document.getElementById('recBudget').value = state.budget || '';
    document.getElementById('recPriority').value = state.priority || '';
    document.getElementById('recScreen').value = state.screenSize || '';

    // أعد تحميل النتائج تلقائيًا
    loadRecommendations(state.page || 1);
    sessionStorage.removeItem('recommendation_state');
});

function showToast(msg) {
    let t = document.getElementById('simpleToast');
    if (!t) {
        t = document.createElement('div');
        t.id = "simpleToast";
        t.style.position = "fixed";
        t.style.bottom = "25px";
        t.style.right = "25px";
        t.style.zIndex = "99999";
        t.style.padding = "12px 16px";
        t.style.borderRadius = "10px";
        t.style.background = "#222";
        t.style.color = "#fff";
        t.style.boxShadow = "0 10px 30px rgba(0,0,0,.25)";
        t.style.opacity = "0";
        t.style.transition = "opacity .2s ease";
        document.body.appendChild(t);
    }
    t.textContent = msg;
    t.style.opacity = "1";
    setTimeout(() => t.style.opacity = "0", 1600);
}

function updateCompareBadge(count) {
    const el = document.getElementById('compareCount');
    if (el) el.textContent = count;
}

document.addEventListener("DOMContentLoaded", function () {

    const saved = sessionStorage.getItem("rec_request");
    if (!saved) return;

    const req = JSON.parse(saved);

    // رجّع القيم للـ inputs
    document.getElementById('recPurpose').value = req.purpose ?? "";
    document.getElementById('recBudget').value = req.budget ?? "";
    document.getElementById('recPriority').value = req.priority ?? "";
    document.getElementById('recScreen').value = req.screenSize ?? "";

    // حمّل النتائج
    loadRecommendations(req.page || 1);
});