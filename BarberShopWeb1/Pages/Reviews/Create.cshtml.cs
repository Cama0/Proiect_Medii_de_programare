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

namespace BarberShopWeb1.Pages.Reviews
{
    public class CreateModel : PageModel
    {
        private readonly BarberShopWeb1.Data.BarberShopContext _context;

        public CreateModel(BarberShopWeb1.Data.BarberShopContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Review Review { get; set; } = default!;
        
        public IActionResult OnGet(int? stylistId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            
            ViewData["StylistID"] = new SelectList(_context.Stylist, "ID", "Name", stylistId);
            
            if (User.IsInRole("Admin"))
            {
                ViewData["MemberID"] = new SelectList(_context.Member, "ID", "LastName");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("Review.Member");
            ModelState.Remove("Review.MemberID");
            ModelState.Remove("Review.Stylist");
            
            if (!User.IsInRole("Admin"))
            {
                var userEmail = User.Identity.Name;
                
                var member = await _context.Member.FirstOrDefaultAsync(m => m.Email == userEmail);
                
                if (member == null)
                {
                    member = new Member
                    {
                        FirstName = "Utilizator", 
                        LastName = "Recenzie",
                        Email = userEmail,
                        Phone = ""
                    };
                    _context.Member.Add(member);
                    await _context.SaveChangesAsync();
                }
                
                Review.MemberID = member.ID;
            }

            if (!ModelState.IsValid)
            {
                ViewData["StylistID"] = new SelectList(_context.Stylist, "ID", "Name");
                return Page();
            }

            _context.Review.Add(Review);
            await _context.SaveChangesAsync();
            
            return RedirectToPage("/Stylists/Index");
        }
    }
}