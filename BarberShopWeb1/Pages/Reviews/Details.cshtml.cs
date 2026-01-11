using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BarberShopWeb1.Models;

namespace BarberShopWeb1.Pages.Reviews
{
    public class DetailsModel : PageModel
    {
        private readonly BarberShopWeb1.Data.BarberShopContext _context;

        public DetailsModel(BarberShopWeb1.Data.BarberShopContext context)
        {
            _context = context;
        }

        public Review Review { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Aducem Review-ul împreună cu Clientul și Stilistul
            Review = await _context.Review
                .Include(r => r.Stylist)
                .Include(r => r.Member)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (Review == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}