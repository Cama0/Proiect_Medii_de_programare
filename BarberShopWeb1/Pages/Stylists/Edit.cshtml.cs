using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BarberShopWeb1.Data;
using BarberShopWeb1.Models;

namespace BarberShopWeb1.Pages.Stylists
{
    public class EditModel : PageModel
    {
        private readonly BarberShopWeb1.Data.BarberShopContext _context;

        public EditModel(BarberShopWeb1.Data.BarberShopContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Stylist Stylist { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stylist =  await _context.Stylist.FirstOrDefaultAsync(m => m.ID == id);
            if (stylist == null)
            {
                return NotFound();
            }
            Stylist = stylist;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Stylist).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StylistExists(Stylist.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool StylistExists(int id)
        {
            return _context.Stylist.Any(e => e.ID == id);
        }
    }
}
