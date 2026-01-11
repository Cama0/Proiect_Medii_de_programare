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

        // Primim ID-ul stilistului din URL (când dăm click pe butonul de la stiliști)
        public IActionResult OnGet(int? stylistId)
        {
            // Dacă nu ești logat, te trimitem la login
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            // Încărcăm lista de stiliști
            // Dacă am primit un stylistId, îl selectăm pe acela implicit
            ViewData["StylistID"] = new SelectList(_context.Stylist, "ID", "Name", stylistId);

            // Pentru Admin, încărcăm lista de membri. Pentru Client, va fi null (ascuns).
            if (User.IsInRole("Admin"))
            {
                ViewData["MemberID"] = new SelectList(_context.Member, "ID", "LastName");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Ignorăm validarea membrului (îl completăm noi automat)
            ModelState.Remove("Review.Member");
            ModelState.Remove("Review.MemberID");
            ModelState.Remove("Review.Stylist");

            // --- LOGICA PENTRU CLIENT ---
            if (!User.IsInRole("Admin"))
            {
                var userEmail = User.Identity.Name;
                
                // Căutăm membrul asociat contului
                var member = await _context.Member.FirstOrDefaultAsync(m => m.Email == userEmail);

                // Dacă membrul nu există (e.g. e un cont nou care nu a făcut programări încă), îl creăm
                if (member == null)
                {
                    member = new Member
                    {
                        FirstName = "Utilizator", // Generic
                        LastName = "Recenzie",
                        Email = userEmail,
                        Phone = ""
                    };
                    _context.Member.Add(member);
                    await _context.SaveChangesAsync();
                }

                // Asociem review-ul cu acest membru
                Review.MemberID = member.ID;
            }

            if (!ModelState.IsValid)
            {
                ViewData["StylistID"] = new SelectList(_context.Stylist, "ID", "Name");
                return Page();
            }

            _context.Review.Add(Review);
            await _context.SaveChangesAsync();

            // După ce lasă review, îl trimitem înapoi la lista de stiliști
            return RedirectToPage("/Stylists/Index");
        }
    }
}