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
    public class IndexModel : PageModel
    {
        private readonly BarberShopWeb1.Data.BarberShopContext _context;

        public IndexModel(BarberShopWeb1.Data.BarberShopContext context)
        {
            _context = context;
        }

        public IList<Appointment> Appointment { get;set; } = default!;

        public async Task OnGetAsync()
        {
            var query = _context.Appointment
                .Include(a => a.Stylist)
                .Include(a => a.Service)
                .Include(a => a.Member) 
                .OrderByDescending(a => a.Date) 
                .AsQueryable();
            
            if (!User.IsInRole("Admin"))
            {
                
                var userEmail = User.Identity.Name;
                
                query = query.Where(a => a.Member.Email == userEmail);
            }
            
            Appointment = await query.ToListAsync();
        }
    }
}