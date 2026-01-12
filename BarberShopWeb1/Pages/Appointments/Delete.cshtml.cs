using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BarberShopWeb1.Data;
using BarberShopWeb1.Models;

namespace BarberShopWeb1.Pages.Appointments
{
    public class DeleteModel : PageModel
    {
        private readonly BarberShopWeb1.Data.BarberShopContext _context;

        public DeleteModel(BarberShopWeb1.Data.BarberShopContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Appointment Appointment { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();
            
            var appointment = await _context.Appointment
                .Include(a => a.Member) 
                .Include(a => a.Service)
                .Include(a => a.Stylist)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (appointment == null) return NotFound();
            
            if (!User.IsInRole("Admin"))
            {
                if (appointment.Member.Email != User.Identity.Name)
                {
                    return Forbid();
                }
            }

            Appointment = appointment;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointment.FindAsync(id);
            if (appointment != null)
            {
                Appointment = appointment;
                _context.Appointment.Remove(Appointment);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
