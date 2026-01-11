using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BarberShopWeb1.Data;
using BarberShopWeb1.Models;

namespace BarberShopWeb1.Pages.Stylists
{
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        private readonly BarberShopWeb1.Data.BarberShopContext _context;

        public DeleteModel(BarberShopWeb1.Data.BarberShopContext context)
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // 1. Găsim Stilistul
            var stylist = await _context.Stylist.FindAsync(id);

            if (stylist != null)
            {
                // 2. Găsim TOATE programările acestui stilist
                var appointments = _context.Appointment
                    .Where(a => a.StylistID == id)
                    .ToList();

                // 3. Le ștergem mai întâi pe ele (Ca să nu mai avem eroarea Foreign Key)
                if (appointments.Any())
                {
                    _context.Appointment.RemoveRange(appointments);
                }

                // 4. Acum putem șterge liniștit stilistul
                Stylist = stylist;
                _context.Stylist.Remove(Stylist);
        
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
