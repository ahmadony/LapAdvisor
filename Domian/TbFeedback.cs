using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domains
{
    public class TbFeedback
    {
        public int TbFeedbackId { get; set; }   // ✅ Primary Key

        public int ItemId { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }

        public int Rating { get; set; }
        public string Title { get; set; }
        public string Comment { get; set; }

        public DateTime CreatedDate { get; set; }
        public int CurrentState { get; set; }
    }
}
