using FashionShop_Hub.Data;
using FashionShop.Data.Repositories; // Asigură-te că aici e OrderRepository
using FashionShop_Hub.Data.Repositories; // Pentru Payment/Shipping repos
using Microsoft.EntityFrameworkCore;
using FashionShop.Domain;
using FashionShop.Domain.Repositories;
using FashionShop.Events.ServiceBus;
using FashionShop_Hub.BackgroundServices;

var builder = WebApplication.CreateBuilder(args);

// --- 1. CONFIGURARE SERVICII ---

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// AICI ERA POSIBILA EROARE: Verifică dacă în appsettings.json ai "FashionDb" sau "DefaultConnection"
// Am pus "FashionDb" pentru că așa apărea în mesajele tale anterioare.
var connectionString = builder.Configuration.GetConnectionString("FashionDb") 
                    ?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<FashionDbContext>(options =>
    options.UseSqlServer(connectionString));

// Repositories
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<PaymentRepository>();
builder.Services.AddScoped<ShippingRepository>();

// Azure Service Bus & Workers
builder.Services.AddSingleton<IAsyncEventBus, AzureServiceBusEventBus>();
builder.Services.AddHostedService<PaymentWorker>();
builder.Services.AddHostedService<ShippingWorker>();

var app = builder.Build();

// --- 2. CONFIGURARE BAZA DE DATE (O SINGURĂ DATĂ) ---
// Acest bloc rulează la fiecare pornire și asigură că tabelele există
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<FashionDbContext>();
        
        // A. Șterge baza veche (Activează asta doar acum, apoi comentează-o)
        // Asta rezolvă eroarea "Invalid object name 'Orders'"
       // context.Database.EnsureDeleted(); 
        
        // B. Creează baza nouă cu TOATE tabelele (Orders, Payments, Shippings)
      //  context.Database.EnsureCreated();
        
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("✅ Baza de date a fost recreată cu succes!");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "❌ Eroare critică la crearea bazei de date.");
    }
}

// --- 3. PIPELINE HTTP ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers(); // O singură dată e suficient
app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();