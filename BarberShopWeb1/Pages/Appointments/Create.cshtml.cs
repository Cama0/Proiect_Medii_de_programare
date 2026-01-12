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
    public class CreateModel : PageModel
    {
        private readonly BarberShopWeb1.Data.BarberShopContext _context;

        public CreateModel(BarberShopWeb1.Data.BarberShopContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Appointment Appointment { get; set; } = default!;

        [BindProperty]
        public Member InputMember { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            ViewData["StylistID"] = new SelectList(_context.Stylist, "ID", "Name");
            ViewData["ServiceID"] = new SelectList(_context.Service, "ID", "Name");

            if (User.IsInRole("Admin"))
            {
                
                ViewData["MemberID"] = new SelectList(_context.Member.Select(m => new { 
                    ID = m.ID, 
                    FullName = m.FirstName + " " + m.LastName 
                }), "ID", "FullName");
            }
            else
            {
                var userEmail = User.Identity.Name;
                var existingMember = await _context.Member.FirstOrDefaultAsync(m => m.Email == userEmail);
                if (existingMember != null) InputMember = existingMember;
                else InputMember = new Member();
                ViewData["MemberID"] = null;
            }
            return Page();
        }

        public JsonResult OnGetTimeSlots(string date, int stylistId)
        {
            var selectedDate = DateTime.Parse(date);
            var startTime = new TimeSpan(9, 0, 0);
            var endTime = new TimeSpan(17, 0, 0);
            var slotDuration = TimeSpan.FromMinutes(30);

            var existingAppointments = _context.Appointment
                .Where(a => a.StylistID == stylistId)
                .Where(a => a.Date.Date == selectedDate.Date)
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
    
    if (User.IsInRole("Admin"))
    {
        
        ModelState.Clear();
        
        if (Appointment.Date == DateTime.MinValue)
        {
            ModelState.AddModelError("Appointment.Date", "Eroare: Data nu a fost selectată corect.");
        }
        else if (Appointment.Date.Hour < 9 || Appointment.Date.Hour >= 17)
        {
            ModelState.AddModelError("Appointment.Date", "Programările se fac doar 09:00 - 17:00.");
        }
        
        if (Appointment.MemberID == 0)
        {
            ModelState.AddModelError("Appointment.MemberID", "Te rog selectează un client din listă.");
        }
        
        if (ModelState.IsValid)
        {
            int duration = 30;
            var newStart = Appointment.Date;
            var newEnd = newStart.AddMinutes(duration);

            var conflict = await _context.Appointment
                .Where(a => a.StylistID == Appointment.StylistID)
                .Where(a => a.Date < newEnd && a.Date.AddMinutes(duration) > newStart)
                .FirstOrDefaultAsync();

            if (conflict != null)
            {
                ModelState.AddModelError("Appointment.Date", $"Interval ocupat! Conflict la ora {conflict.Date.ToShortTimeString()}.");
            }
        }
        
        if (!ModelState.IsValid)
        {
            ViewData["StylistID"] = new SelectList(_context.Stylist, "ID", "Name");
            ViewData["ServiceID"] = new SelectList(_context.Service, "ID", "Name");
            ViewData["MemberID"] = new SelectList(_context.Member.Select(m => new { ID = m.ID, FullName = m.FirstName + " " + m.LastName }), "ID", "FullName");
            return Page();
        }
        
        _context.Appointment.Add(Appointment);
        await _context.SaveChangesAsync();
        return RedirectToPage("./Index");
    }
    
    else 
    {
        ModelState.Remove("Appointment.Member");
        ModelState.Remove("Appointment.MemberID");
        ModelState.Remove("InputMember.Email");
        
        var hour = Appointment.Date.Hour;
        if (hour < 9 || hour >= 17)
        {
            ModelState.AddModelError("Appointment.Date", "Programările se fac doar între orele 09:00 și 17:00.");
        }
        
        if (ModelState.IsValid)
        {
            int duration = 30;
            var newStart = Appointment.Date;
            var newEnd = newStart.AddMinutes(duration);
            var conflict = await _context.Appointment
                .Where(a => a.StylistID == Appointment.StylistID)
                .Where(a => a.Date < newEnd && a.Date.AddMinutes(duration) > newStart)
                .FirstOrDefaultAsync();

            if (conflict != null) ModelState.AddModelError("Appointment.Date", $"Interval ocupat! Conflict cu ora {conflict.Date.ToShortTimeString()}.");
        }
        
        var userEmail = User.Identity.Name;
        var member = await _context.Member.FirstOrDefaultAsync(m => m.Email == userEmail);
        if (member == null)
        {
            member = new Member { FirstName = InputMember.FirstName, LastName = InputMember.LastName, Phone = InputMember.Phone, Email = userEmail };
            _context.Member.Add(member);
        }
        else
        {
            member.FirstName = InputMember.FirstName; member.LastName = InputMember.LastName; member.Phone = InputMember.Phone;
            _context.Member.Update(member);
        }
        await _context.SaveChangesAsync();
        Appointment.MemberID = member.ID;

        if (!ModelState.IsValid)
        {
            ViewData["StylistID"] = new SelectList(_context.Stylist, "ID", "Name");
            ViewData["ServiceID"] = new SelectList(_context.Service, "ID", "Name");
            ViewData["MemberID"] = null;
            return Page();
        }

        _context.Appointment.Add(Appointment);
        await _context.SaveChangesAsync();
        return RedirectToPage("./Index");
    }
}
    }
}