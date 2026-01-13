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
            
            DateTime start = request.Date;
            DateTime end = start.AddMinutes(30);

            var conflict = await _context.Appointment
                .Where(a => a.StylistID == request.StylistID)
                .Where(a => a.Date < end && a.Date.AddMinutes(30) > start) 
                .FirstOrDefaultAsync();

            if (conflict != null)
            {
                return BadRequest("Acest interval orar este deja ocupat.");
            }
            
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
        
        [HttpGet("my-appointments")]
        public async Task<ActionResult<IEnumerable<object>>> GetMyAppointments([FromQuery] string phone)
        {
            var member = await _context.Member.FirstOrDefaultAsync(m => m.Phone == phone);
            if (member == null) return NotFound(); 

            var appointments = await _context.Appointment
                .Include(a => a.Stylist)
                .Include(a => a.Service)
                .Where(a => a.MemberID == member.ID)
                .OrderByDescending(a => a.Date)
                .Select(a => new 
                {
                    ID = a.ID, 
                    Date = a.Date,
                    StylistName = a.Stylist.Name,
                    ServiceName = a.Service.Name
                })
                .ToListAsync();

            return Ok(appointments);
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var appointment = await _context.Appointment.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            _context.Appointment.Remove(appointment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    
    public class AppointmentRequest
    {
        public int StylistID { get; set; }
        public string ClientName { get; set; }
        public string Phone { get; set; }
        public DateTime Date { get; set; }
        public int ServiceID { get; set; }
    }
}