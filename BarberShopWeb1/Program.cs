using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BarberShopWeb1.Data;
using BarberShopWeb1.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<BarberShopContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("BarberShopConnection")));
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Asta previne erorile "A cycle was detected" când un obiect face referire la altul
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddDefaultIdentity<IdentityUser>(options => 
    {
        options.SignIn.RequireConfirmedAccount = false; // Nu cerem confirmare email pt laborator
        options.Password.RequireDigit = true;
        options.Password.RequireNonAlphanumeric = false; // Simplificăm parola (ex: Admin123!)
        options.Password.RequireUppercase = true;
        options.Password.RequiredLength = 6;
    })
    .AddRoles<IdentityRole>() // <--- LINIE CRITICĂ PENTRU ROLURI
    .AddEntityFrameworkStores<BarberShopContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

// --- SEEDING PENTRU ROLURI ȘI ADMIN ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

    // 1. Verificăm dacă există rolul Admin
    var roleExist = await roleManager.RoleExistsAsync("Admin");
    if (!roleExist)
    {
        // Creăm rolurile
        await roleManager.CreateAsync(new IdentityRole("Admin"));
        await roleManager.CreateAsync(new IdentityRole("User"));
    }

    // 2. Creăm userul Admin implicit (dacă nu există)
    var adminEmail = "admin@barber.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        var newAdmin = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
        var createAdmin = await userManager.CreateAsync(newAdmin, "Admin123!"); // Parola Adminului
        if (createAdmin.Succeeded)
        {
            await userManager.AddToRoleAsync(newAdmin, "Admin");
        }
    }
}
// --- FINAL SEEDING ---

app.MapRazorPages();

app.MapControllers();

app.Run();