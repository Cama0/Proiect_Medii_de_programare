using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BarberShopWeb1.Models;

namespace BarberShopWeb1.Pages.Reviews
{
    public class EditModel : PageModel
    {
        private readonly BarberShopWeb1.Data.BarberShopContext _context;

        public EditModel(BarberShopWeb1.Data.BarberShopContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Review Review { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var review = await _context.Review
                .Include(r => r.Stylist) 
                .FirstOrDefaultAsync(m => m.ID == id);

            if (review == null) return NotFound();

            Review = review;
            ViewData["StylistName"] = review.Stylist?.Name;
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("Review.Member");
            ModelState.Remove("Review.Stylist");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Review).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Review.Any(e => e.ID == Review.ID)) return NotFound();
                else throw;
            }

            return RedirectToPage("./Index");
        }
    }
}