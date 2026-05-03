using LapAdvisor.Bl;
using LapAdvisor.Model;
using LapAdvisor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace LapAdvisor.Controllers
{
    public class OrderController : Controller
    {
        IItems itemService;
        UserManager<ApplicationUser> _userManager;
        ISalesInvoice salesInvoiceService;
        ISalesInvoiceItems salesInvoiceItemsService;
        public OrderController(IItems itemservice, UserManager<ApplicationUser> userManager , ISalesInvoice salesInvoice, ISalesInvoiceItems salesInvoiceItems)
        {
            itemService = itemservice;
            _userManager = userManager;
            salesInvoiceService = salesInvoice;
            salesInvoiceItemsService = salesInvoiceItems;
        }
        public IActionResult Cart()
        {
            ShoppingCart cart;

            if (HttpContext.Request.Cookies["Cart"] != null)
            {
                cart = JsonConvert.DeserializeObject<ShoppingCart>(
                    HttpContext.Request.Cookies["Cart"]
                );

                if (cart == null)
                    cart = new ShoppingCart();
            }
            else
            {
                cart = new ShoppingCart();
            }

            return View(cart);
        }

        [Authorize]
        public async Task<IActionResult> OrderSuccess(int id)
        {
            ViewBag.InvoiceId = id;
            return View();
        }
        public IActionResult AddToCart(int itemId, int qty)
        {
            if (qty < 1) qty = 1;

            ShoppingCart cart;

            if (HttpContext.Request.Cookies["Cart"] != null)
            {
                cart = JsonConvert.DeserializeObject<ShoppingCart>(
                    HttpContext.Request.Cookies["Cart"]
                );

                if (cart.lstItems == null)
                    cart.lstItems = new List<ShoppingCartItem>();
            }
            else
            {
                cart = new ShoppingCart();
                cart.lstItems = new List<ShoppingCartItem>();
            }

            var item = itemService.GetById(itemId);
            if (item == null)
                return RedirectToAction("Cart");

            var itemInList = cart.lstItems.FirstOrDefault(a => a.ItemId == itemId);

            if (itemInList != null)
            {
                itemInList.Qty += qty;                  
                itemInList.Total = itemInList.Qty * itemInList.Price;
            }
            else
            {
                cart.lstItems.Add(new ShoppingCartItem
                {
                    ItemId = item.ItemId,
                    ItemName = item.ItemName,
                    Price = item.SalesPrice,
                    Qty = qty,
                    Total = item.SalesPrice * qty,
                    ImageName = item.ImageName
                });
            }

            cart.Total = cart.lstItems.Sum(a => a.Total);

            HttpContext.Response.Cookies.Append(
                "Cart",
                JsonConvert.SerializeObject(cart)
            );

            return RedirectToAction("Cart");
        }

        [HttpPost]
        public IActionResult UpdateQty(int itemId, int qty)
        {
            if (qty < 1) qty = 1;

            ShoppingCart cart;

            if (HttpContext.Request.Cookies["Cart"] != null)
            {
                cart = JsonConvert.DeserializeObject<ShoppingCart>(HttpContext.Request.Cookies["Cart"]);
                if (cart == null) cart = new ShoppingCart();
                if (cart.lstItems == null) cart.lstItems = new List<ShoppingCartItem>();
            }
            else
            {
                cart = new ShoppingCart();
                cart.lstItems = new List<ShoppingCartItem>();
            }

            var itemInCart = cart.lstItems.FirstOrDefault(x => x.ItemId == itemId);
            if (itemInCart == null)
                return Json(new { ok = false });

            itemInCart.Qty = qty;
            itemInCart.Total = itemInCart.Price * itemInCart.Qty;

            cart.Total = cart.lstItems.Sum(x => x.Total);

            HttpContext.Response.Cookies.Append("Cart", JsonConvert.SerializeObject(cart));

            return Json(new
            {
                ok = true,
                itemId = itemId,
                rowTotal = itemInCart.Total,
                cartTotal = cart.Total
            });
        }

        [HttpPost]
        public IActionResult RemoveItem(int itemId)
        {
            ShoppingCart cart;

            if (HttpContext.Request.Cookies["Cart"] != null)
            {
                cart = JsonConvert.DeserializeObject<ShoppingCart>(HttpContext.Request.Cookies["Cart"]);
                if (cart == null) cart = new ShoppingCart();
                if (cart.lstItems == null) cart.lstItems = new List<ShoppingCartItem>();
            }
            else
            {
                cart = new ShoppingCart();
                cart.lstItems = new List<ShoppingCartItem>();
            }

            cart.lstItems.RemoveAll(x => x.ItemId == itemId);
            cart.Total = cart.lstItems.Sum(x => x.Total);

            HttpContext.Response.Cookies.Append("Cart", JsonConvert.SerializeObject(cart));

            return Json(new { ok = true, cartTotal = cart.Total });
        }


        async Task<int> SaveOrder(ShoppingCart oShopingCart)
        {

            List<TbSalesInvoiceItem> lstInvoiceItems = new();

            foreach (var item in oShopingCart.lstItems)
            {
                lstInvoiceItems.Add(new TbSalesInvoiceItem()
                {
                    ItemId = item.ItemId,
                    Qty = item.Qty,
                    InvoicePrice = item.Price
                });
            }

            var user = await _userManager.GetUserAsync(User);

            TbSalesInvoice oSalesInvoice = new TbSalesInvoice()
            {
                InvoiceDate = DateTime.Now,
                CustomerId = Guid.Parse(user.Id),
                DelivryDate = DateTime.Now.AddDays(5),
                CreatedBy = user.Id,
                CreatedDate = DateTime.Now,
                OrderStatus = "Pending" // 👈 ممتاز
            };

            salesInvoiceService.Save(oSalesInvoice, lstInvoiceItems, true);

            return oSalesInvoice.InvoiceId;
        }
        [Authorize]

        //This is from itemDetails view
        [Authorize]
        public IActionResult BuyNow(int itemId, int qty)
        {
            // نضيف المنتج بالكميات
            AddToCart(itemId, qty);

            // بعدها نعمل checkout
            return RedirectToAction("Checkout");
        }

        //This is from Cart view
        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            string sessionCart = HttpContext.Request.Cookies["Cart"];

            if (string.IsNullOrEmpty(sessionCart))
                return RedirectToAction("Cart");

            var cart = JsonConvert.DeserializeObject<ShoppingCart>(sessionCart);

            if (cart == null || cart.lstItems == null || cart.lstItems.Count == 0)
                return RedirectToAction("Cart");

            int invoiceId = await SaveOrder(cart);

            HttpContext.Response.Cookies.Delete("Cart");

            return RedirectToAction("OrderSuccess", new { id = invoiceId });
        }

        [Authorize]
        public async Task<IActionResult> MyOrders()
        {
            var user = await _userManager.GetUserAsync(User);

            var orders = salesInvoiceService
                .GetByCustomer(Guid.Parse(user.Id));

            return View(orders);
        }

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            // 1) جيب المستخدم الحالي
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            // 2) جيب الفاتورة
            var invoice = salesInvoiceService.GetById(id);
            if (invoice == null || invoice.InvoiceId == 0)
                return NotFound();

            // 3) حماية: هذا الأوردر لازم يكون لنفس المستخدم
            if (invoice.CustomerId != Guid.Parse(user.Id))
                return Forbid(); // أو NotFound() إذا بدك تخفي وجوده

            // 4) جيب عناصر الفاتورة
            var dbItems = salesInvoiceItemsService.GetSalesInvoiceId(id);

            // 5) جهّز ViewModel
            var vm = new VmOrderDetails
            {
                Invoice = invoice
            };

            foreach (var it in dbItems)
            {
                var product = itemService.GetById(it.ItemId);

                vm.Items.Add(new VmOrderDetailsItem
                {
                    ItemId = it.ItemId,
                    ItemName = product?.ItemName ?? ("Item #" + it.ItemId),
                    ImageName = product?.ImageName,
                    Qty = (int)it.Qty,
                    Price = it.InvoicePrice
                });
            }

            return View(vm);
        }

    }
}
