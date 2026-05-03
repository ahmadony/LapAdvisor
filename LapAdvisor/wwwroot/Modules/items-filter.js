const urlParams = new URLSearchParams(window.location.search);
// This improves usability by pre-applying filters based on navigation source
const preSelectedBrand = urlParams.get('brand');

/* =========================
   Render Items
========================= */
function renderItems(items) {
    const container = document.getElementById('ItemArea');
    container.innerHTML = '';
    // if no reslut
    if (!items || items.length === 0) {
        container.innerHTML = `
            <div class="col-12 text-center mt-5">
                <h5>No products found</h5>
            </div>`;
        return;
    }
    // for each item what will be shown
    items.forEach(item => {
        container.innerHTML += `
<div class="col-xl-3 col-6 col-grid-box">
    <div class="product-box">
        <div class="img-wrapper" style="position:relative;">
            <!-- ❤️ Wishlist -->
            <a href="javascript:void(0)"
               class="btn-wishlist ${item.inWishlist ? 'active' : ''}"
               data-itemid="${item.itemId}"
               data-inwishlist="${item.inWishlist ? 'true' : 'false'}"
               style="position:absolute;top:10px;right:10px;z-index:5;">
                <i class="fa fa-heart"></i>
            </a>

            <div class="front">
                <a href="/Items/ItemDetails/${item.itemId}">
                    <img src="/Uploads/Items/${item.imageName ?? 'no-image.png'}"
                         class="img-fluid blur-up lazyload bg-img">
                </a>
            </div>

             <!-- Back Image (مهم جدًا للترانزيشن) -->
            <div class="back">
                <a href="/Items/ItemDetails/${item.itemId}">
                    <img src="/Uploads/Items/${item.imageName ?? 'no-image.png'}"
                         class="img-fluid blur-up lazyload bg-img">
                </a>
            </div>

        </div>

        <div class="product-detail">
            <a href="/Items/ItemDetails/${item.itemId}">
                <h6>${item.itemName}</h6>
            </a>

            <h4>JD ${item.salesPrice}</h4>

            <!-- 🛒 Add To Cart -->
           <div class="mt-2">
            <a href="/Order/AddToCart?itemId=${item.itemId}"
                class="btn btn-solid btn-sm">
                ADD TO CART
            </a>
          </div>
        </div>
    </div>
</div>`;
    });
}


/* =========================
   Pagination
========================= */
function renderPagination(totalCount, currentPage) {

    $('#ItemPagination').pagination({
        dataSource: function (done) {
            done(new Array(totalCount));
        },
        pageSize: 12,
        pageNumber: currentPage,
        showGoInput: true,
        showGoButton: true,
        callback: function (data, pagination) {
            // when the user go into another page the filter stay on
            if (pagination.pageNumber !== currentPage) {
                applyFilters(pagination.pageNumber); // 🔥 نفس الفلترة
            }
        }
    });
}



/* =========================
   Helper
========================= */
function getCheckedValues(selector) {
    return Array.from(document.querySelectorAll(selector + ':checked'))
        .map(cb => cb.dataset.value);
}

/* =========================
   Apply Filters
========================= */
//let filterTimeout;

function applyFilters(page = 1) {
    // We use a filter DTO to send structured data to the backend
    const filter = {
        Brands: getCheckedValues('.filter-brand'),
        LaptopTypes: getCheckedValues('.filter-type'),
        OS: getCheckedValues('.filter-os'),
        Processors: getCheckedValues('.filter-cpu'),
        Rams: getCheckedValues('.filter-ram').map(Number),
        MinPrice: document.getElementById('minPrice')?.value || null,
        MaxPrice: document.getElementById('maxPrice')?.value || null,
        Page: page,
        PageSize: 12
    };
    // AJAX request
    fetch('/Items/FilterItems', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(filter)
    })
        // Receiving the result
        .then(res => res.json())
        .then(result => {
            renderItems(result.data);
            renderPagination(result.totalCount, page);
        });
}

/* =========================
   Events
========================= */
document.querySelectorAll(
    '.filter-brand, .filter-type, .filter-os, .filter-cpu, .filter-ram'
).forEach(cb => {
    // when the user change the filter the system will back to the first page and apply filter
    cb.addEventListener('change', () => applyFilters(1));
});

document.getElementById('minPrice')?.addEventListener('change', () => applyFilters(1));
document.getElementById('maxPrice')?.addEventListener('change', () => applyFilters(1));

document.addEventListener('DOMContentLoaded', () => {

    if (preSelectedBrand) {
        const cb = document.querySelector(
            `.filter-brand[data-value="${preSelectedBrand}"]`
        );
        if (cb) cb.checked = true;
    }

    applyFilters(1);
});