using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;

namespace FashionShop_Hub.Data.Repositories
{
    public class PaymentRepository
    {
        private readonly string _connectionString;

        public PaymentRepository(IConfiguration configuration)
        {
            // Preluăm connection string-ul din appsettings.json
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // --- ACEASTA ESTE METODA CARE LIPSEA ---
        public void SavePayment(Guid orderId, decimal amount)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
        
                // Asigură-te că numele de aici (PaymentDate) coincide cu cel din SQL
                var query = @"INSERT INTO Payments (PaymentId, OrderId, Amount, PaymentDate) 
                      VALUES (@PaymentId, @OrderId, @Amount, @PaymentDate)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PaymentId", Guid.NewGuid());
                    command.Parameters.AddWithValue("@OrderId", orderId);
                    command.Parameters.AddWithValue("@Amount", amount);
                    command.Parameters.AddWithValue("@PaymentDate", DateTime.UtcNow);

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}