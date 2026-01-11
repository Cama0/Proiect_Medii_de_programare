using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BarberShopWeb1.Data;
using BarberShopWeb1.Models;

namespace BarberShopWeb1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly BarberShopContext _context;

        public AppointmentsController(BarberShopContext context)
        {
            _context = context;
        }

        // POST: api/Appointments[HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody] AppointmentRequest request)
        {
            if (request == null) return BadRequest("Date invalide.");

            // --- VERIFICARE CONFLICT (NOU) ---
            // Presupunem că o tunsoare durează 30 minute
            DateTime start = request.Date;
            DateTime end = start.AddMinutes(30);

            var conflict = await _context.Appointment
                .Where(a => a.StylistID == request.StylistID)
                .Where(a => a.Date < end && a.Date.AddMinutes(30) > start) // Logica de suprapunere
                .FirstOrDefaultAsync();

            if (conflict != null)
            {
                return BadRequest("Acest interval orar este deja ocupat.");
            }
            // ---------------------------------

            // Căutăm sau creăm clientul (Codul vechi...)
            var member = await _context.Member.FirstOrDefaultAsync(m => m.Phone == request.Phone);

            if (member == null)
            {
                member = new Member
                {
                    FirstName = request.ClientName,
                    LastName = "",
                    Phone = request.Phone,
                    Email = "mobile_app@barber.com"
                };
                _context.Member.Add(member);
                await _context.SaveChangesAsync();
            }

            // Creăm programarea (Codul vechi...)
            var appointment = new Appointment
            {
                StylistID = request.StylistID,
                MemberID = member.ID,
                Date = request.Date,
                ServiceID = request.ServiceID
            };

            _context.Appointment.Add(appointment);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Programare reușită!" });
        }
    }

    // Aceasta este o clasă "plic" (DTO) doar pentru transferul de date de la mobil
    public class AppointmentRequest
    {
        public int StylistID { get; set; }
        public string ClientName { get; set; }
        public string Phone { get; set; }
        public DateTime Date { get; set; }
        public int ServiceID { get; set; }
    }
}