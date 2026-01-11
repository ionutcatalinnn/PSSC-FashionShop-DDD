using FashionShop.Domain.Repositories;
using FashionShop_Hub.Data;
using FashionShop_Hub.Data.Repositories;
using Microsoft.EntityFrameworkCore;

// --- USING-URI NOI PENTRU AZURE & WORKERS ---
using FashionShop.Domain;                  // Pentru interfața IAsyncEventBus
using FashionShop.Events.ServiceBus;       // Pentru implementarea AzureServiceBusEventBus
using FashionShop_Hub.BackgroundServices;  // Pentru PaymentWorker și ShippingWorker
// ---------------------------------------------

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- CONFIGURARE DB ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<FashionDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<PaymentRepository>();
builder.Services.AddScoped<ShippingRepository>();

// --- CONFIGURARE AZURE SERVICE BUS (NOU) ---
// 1. Înregistrăm Event Bus-ul conectat la Azure
// Singleton = o singură conexiune pentru toată aplicația
builder.Services.AddSingleton<IAsyncEventBus, AzureServiceBusEventBus>();

// 2. Pornim Workerii care ascultă cozile din Azure
// Ei vor rula în fundal imediat ce pornește aplicația
builder.Services.AddHostedService<PaymentWorker>();
builder.Services.AddHostedService<ShippingWorker>();
// -------------------------------------------

var app = builder.Build();

// --- AUTO-MIGRARE ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FashionDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();