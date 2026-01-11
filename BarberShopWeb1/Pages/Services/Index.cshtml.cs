using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BarberShopWeb1.Data;
using BarberShopWeb1.Models;

namespace BarberShopWeb1.Pages.Services
{
    public class IndexModel : PageModel
    {
        private readonly BarberShopWeb1.Data.BarberShopContext _context;

        public IndexModel(BarberShopWeb1.Data.BarberShopContext context)
        {
            _context = context;
        }

        public IList<Service> Service { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Service = await _context.Service.ToListAsync();
        }
    }
}
