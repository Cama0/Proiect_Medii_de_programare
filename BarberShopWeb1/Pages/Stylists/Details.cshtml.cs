using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BarberShopWeb1.Data;
using BarberShopWeb1.Models;

namespace BarberShopWeb1.Pages.Stylists
{
    public class DetailsModel : PageModel
    {
        private readonly BarberShopWeb1.Data.BarberShopContext _context;

        public DetailsModel(BarberShopWeb1.Data.BarberShopContext context)
        {
            _context = context;
        }

        public Stylist Stylist { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stylist = await _context.Stylist.FirstOrDefaultAsync(m => m.ID == id);
            if (stylist == null)
            {
                return NotFound();
            }
            else
            {
                Stylist = stylist;
            }
            return Page();
        }
    }
}
