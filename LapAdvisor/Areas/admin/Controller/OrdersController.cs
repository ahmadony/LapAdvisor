using LapAdvisor.Bl;
using LapAdvisor.Model;
using LapAdvisor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LapAdvisor.Areas.admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("admin")]
    public class OrdersController : Controller
    {
        private readonly ISalesInvoice _salesInvoice;
        private readonly ISalesInvoiceItems _invoiceItems;
        private readonly IItems _items;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrdersController(
            ISalesInvoice salesInvoice,
            ISalesInvoiceItems invoiceItems,
            IItems items,
            UserManager<ApplicationUser> userManager)
        {
            _salesInvoice = salesInvoice;
            _invoiceItems = invoiceItems;
            _items = items;
            _userManager = userManager;
        }

        public IActionResult List()
        {
            // عندك شغال على VwSalesInvoice (FirstName/LastName)
            var orders = _salesInvoice.GetAll();
            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            // 1) Header من TbSalesInvoice (عشان OrderStatus موجود هناك)
            var inv = _salesInvoice.GetById(id);
            if (inv == null || inv.InvoiceId == 0)
                return RedirectToAction("List");

            // 2) اسم العميل من AspNetUsers (CustomerId = Guid)
            var user = await _userManager.FindByIdAsync(inv.CustomerId.ToString());
            string customerName = user != null ? $"{user.FirstName} {user.LastName}" : inv.CustomerId.ToString();

            // 3) Items من TbSalesInvoiceItems
            var dbItems = _invoiceItems.GetSalesInvoiceId(id);

            // 4) Build VM
            var vm = new VmAdminOrderDetails
            {
                InvoiceId = inv.InvoiceId,
                CustomerName = customerName,
                InvoiceDate = inv.InvoiceDate,
                DelivryDate = inv.DelivryDate,
                OrderStatus = inv.OrderStatus
            };

            foreach (var it in dbItems)
            {
                var product = _items.GetById(it.ItemId);

                vm.Items.Add(new VmAdminOrderItem
                {
                    ItemId = it.ItemId,
                    ItemName = product?.ItemName ?? $"Item #{it.ItemId}",
                    ImageName = product?.ImageName,
                    // ✅ مهم: عندك Qty في الداتابيس غالبًا double
                    Qty = (int)it.Qty,
                    Price = Convert.ToDecimal(it.InvoicePrice)
                });
            }

            vm.Total = vm.Items.Sum(x => x.LineTotal);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateStatus(int invoiceId, string orderStatus)
        {
            var inv = _salesInvoice.GetById(invoiceId);
            if (inv == null || inv.InvoiceId == 0)
                return RedirectToAction("List");

            inv.OrderStatus = orderStatus;

            // ✅ نحدّث الهيدر فقط بدون items
            _salesInvoice.Save(inv, new List<TbSalesInvoiceItem>(), false);

            return RedirectToAction("Details", new { id = invoiceId });
        }
    }
}
