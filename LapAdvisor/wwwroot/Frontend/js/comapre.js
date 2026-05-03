function updateCompareBadge(count) {
    const el = document.getElementById('compareCount');
    if (el) el.textContent = count;
}

document.addEventListener("DOMContentLoaded", function () {
    fetch('/Compare/Count')
        .then(r => r.json())
        .then(x => updateCompareBadge(x.count))
        .catch(() => { });
});

async function removeCompare(itemId) {
    const r = await fetch('/Compare/RemoveAjax', {
        method: 'POST',
        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
        body: new URLSearchParams({ itemId })
    });

    const res = await r.json();
    if (res.status === "REMOVED") location.reload();
}

async function clearCompare() {
    const r = await fetch('/Compare/ClearAjax', { method: 'POST' });
    const res = await r.json();
    if (res.status === "CLEARED") location.reload();
}