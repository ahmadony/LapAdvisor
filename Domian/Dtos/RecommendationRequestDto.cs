using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domains.Dtos
{
    public class RecommendationRequestDto
    {
        public string? Purpose { get; set; }      // gaming / programming / university / business / design
        public string? Budget { get; set; }       // 0-400 / 400-600 / 600-800 / 800+
        public string? Priority { get; set; }     // performance / price / display
        public string? ScreenSize { get; set; }   // 14 / 15.6 / 16+
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
    }
}
