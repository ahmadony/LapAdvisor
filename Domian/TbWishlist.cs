using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapAdvisor.Models
{
    public class TbWishlist
    {
        public int WishlistId { get; set; }

        public string UserId { get; set; }

        public int ItemId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
