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

namespace BarberShopWeb1.Pages.Appointments
{
    public class EditModel : PageModel
    {
        private readonly BarberShopWeb1.Data.BarberShopContext _context;

        public EditModel(BarberShopWeb1.Data.BarberShopContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Appointment Appointment { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var appointment = await _context.Appointment
                .Include(a => a.Member) // Aducem membrul
                .FirstOrDefaultAsync(m => m.ID == id);

            if (appointment == null) return NotFound();

            // --- SECURITATE: Verificăm dacă ești proprietarul sau Admin ---
            if (!User.IsInRole("Admin"))
            {
                if (appointment.Member.Email != User.Identity.Name)
                {
                    return Forbid();
                }
            }

            Appointment = appointment;

            // Încărcăm listele
            ViewData["StylistID"] = new SelectList(_context.Stylist, "ID", "Name");
            ViewData["ServiceID"] = new SelectList(_context.Service, "ID", "Name");

            // --- FIX PENTRU EROAREA NULL REFERENCE ---
            if (User.IsInRole("Admin"))
            {
                ViewData["MemberID"] = new SelectList(_context.Member.Select(m => new { 
                    ID = m.ID, 
                    FullName = m.FirstName + " " + m.LastName 
                }), "ID", "FullName");
            }
            else
            {
                // Dacă ești client, NU încărcăm lista, ca să nu crape HTML-ul
                ViewData["MemberID"] = null; 
            }
            
            return Page();
        }

        // --- AJAX: Metoda pentru ore disponibile (La fel ca la Create) ---
        public JsonResult OnGetTimeSlots(string date, int stylistId)
        {
            var selectedDate = DateTime.Parse(date);
            var startTime = new TimeSpan(9, 0, 0);
            var endTime = new TimeSpan(17, 0, 0);
            var slotDuration = TimeSpan.FromMinutes(30);

            // Luăm programările, DAR o excludem pe CURENTA (ca să nu se blocheze singur pe propria oră)
            var currentId = Appointment?.ID ?? 0;

            var existingAppointments = _context.Appointment
                .Where(a => a.StylistID == stylistId)
                .Where(a => a.Date.Date == selectedDate.Date)
                .Where(a => a.ID != currentId) // <--- IMPORTANT: Ignoră programarea curentă la verificare
                .Select(a => a.Date.TimeOfDay)
                .ToList();

            var availableSlots = new List<string>();
            for (var time = startTime; time < endTime; time = time.Add(slotDuration))
            {
                if (!existingAppointments.Any(taken => taken == time))
                {
                    availableSlots.Add(time.ToString(@"hh\:mm"));
                }
            }

            return new JsonResult(availableSlots);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Ignorăm validarea membrului
            ModelState.Remove("Appointment.Member");
            ModelState.Remove("Appointment.MemberID");

            // --- VALIDARE ORAR ---
            var hour = Appointment.Date.Hour;
            if (hour < 9 || hour >= 17)
            {
                ModelState.AddModelError("Appointment.Date", "Programările se fac doar între orele 09:00 și 17:00.");
            }

            // --- VALIDARE SUPRAPUNERE ---
            if (ModelState.IsValid)
            {
                int duration = 30;
                var newStart = Appointment.Date;
                var newEnd = newStart.AddMinutes(duration);

                var conflict = await _context.Appointment
                    .Where(a => a.StylistID == Appointment.StylistID)
                    .Where(a => a.Date < newEnd && a.Date.AddMinutes(duration) > newStart)
                    .Where(a => a.ID != Appointment.ID) // Ignorăm programarea curentă (Editare)
                    .FirstOrDefaultAsync();

                if (conflict != null)
                {
                    ModelState.AddModelError("Appointment.Date", $"Interval indisponibil! Conflict cu ora {conflict.Date.ToShortTimeString()}.");
                }
            }

            // Dacă sunt erori, reîncărcăm listele
            if (!ModelState.IsValid)
            {
                ViewData["StylistID"] = new SelectList(_context.Stylist, "ID", "Name");
                ViewData["ServiceID"] = new SelectList(_context.Service, "ID", "Name");
                if (User.IsInRole("Admin"))
                {
                    ViewData["MemberID"] = new SelectList(_context.Member.Select(m => new { ID = m.ID, FullName = m.FirstName + " " + m.LastName }), "ID", "FullName");
                }
                return Page();
            }

            _context.Attach(Appointment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppointmentExists(Appointment.ID)) return NotFound();
                else throw;
            }

            return RedirectToPage("./Index");
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointment.Any(e => e.ID == id);
        }
    }
}