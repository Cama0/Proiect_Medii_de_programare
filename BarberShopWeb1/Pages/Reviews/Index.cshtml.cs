using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BarberShopWeb1.Models;

namespace BarberShopWeb1.Pages.Reviews
{
    public class IndexModel : PageModel
    {
        private readonly BarberShopWeb1.Data.BarberShopContext _context;

        public IndexModel(BarberShopWeb1.Data.BarberShopContext context)
        {
            _context = context;
        }

        public IList<Review> Review { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Review = await _context.Review
                .Include(r => r.Stylist)
                .Include(r => r.Member)
                .ToListAsync();
        }
    }
}