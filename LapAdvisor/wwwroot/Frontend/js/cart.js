function ChangeQty(input) {
    let itemId = $(input).data("id");
    let qty = $(input).val();

    $.ajax({
        url: '/Order/UpdateQty',
        type: 'POST',
        data: { itemId: itemId, qty: qty },
        success: function (res) {
            if (!res.ok) return;

            // تحديث Total للسطر
            $("#rowTotal_" + res.itemId).text(res.rowTotal);

            // تحديث Total Price
            $("#OverAllTotal").text("JD " + res.cartTotal);
        }
    });
}

function RemoveItem(itemId) {
    $.ajax({
        url: '/Order/RemoveItem',
        type: 'POST',
        data: { itemId: itemId },
        success: function (res) {
            if (!res.ok) return;

            // احذف السطر من الجدول
            $("#rowTotal_" + itemId).closest("tr").remove();

            // حدث الإجمالي
            $("#OverAllTotal").text("JD " + res.cartTotal);
        }
    });
}