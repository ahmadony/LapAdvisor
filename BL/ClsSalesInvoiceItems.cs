using LapAdvisor.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapAdvisor.Bl
{
    public interface ISalesInvoiceItems
    {
        public List<TbSalesInvoiceItem> GetSalesInvoiceId(int id);

        public bool Save(IList<TbSalesInvoiceItem> Items, int salesInvoiceId, bool isNew);
    }

    public class ClsSalesInvoiceItems : ISalesInvoiceItems
    {
        LapAdvisorDbContext ctx;
        public ClsSalesInvoiceItems(LapAdvisorDbContext context)
        {
            ctx = context;
        }

        public List<TbSalesInvoiceItem> GetSalesInvoiceId(int id)
        {
            try
            {
                var Items = ctx.TbSalesInvoiceItems.Where(a => a.InvoiceId == id).ToList();
                if (Items == null)
                    return new List<TbSalesInvoiceItem>();
                else
                    return Items;
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }

        public bool Save(IList<TbSalesInvoiceItem> Items, int salesInvoiceId, bool isNew)
        {
            // ✅ إذا ما في عناصر، لا تعمل شي (حماية)
            if (Items == null || Items.Count == 0)
                return true;

            List<TbSalesInvoiceItem> dbInvoiceItems =
                GetSalesInvoiceId(salesInvoiceId);

            foreach (var interfaceItem in Items)
            {
                var dbObject = dbInvoiceItems
                    .FirstOrDefault(a => a.InvoiceItemId == interfaceItem.InvoiceItemId);

                if (dbObject != null)
                {
                    // Update
                    dbObject.ItemId = interfaceItem.ItemId;
                    dbObject.Qty = interfaceItem.Qty;
                    dbObject.InvoicePrice = interfaceItem.InvoicePrice;

                    ctx.Entry(dbObject).State = EntityState.Modified;
                }
                else
                {
                    // Insert
                    interfaceItem.InvoiceId = salesInvoiceId;
                    ctx.TbSalesInvoiceItems.Add(interfaceItem);
                }
            }

            // Delete removed items
            foreach (var item in dbInvoiceItems)
            {
                var exists = Items.Any(a => a.InvoiceItemId == item.InvoiceItemId);
                if (!exists)
                    ctx.TbSalesInvoiceItems.Remove(item);
            }

            ctx.SaveChanges();
            return true;
        }

    }
}
