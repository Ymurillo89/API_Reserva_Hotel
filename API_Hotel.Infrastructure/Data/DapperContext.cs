using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Hotel.Infrastructure.Data
{
    public class DapperContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;

            // Busca en appsettings.json la clave "DefaultConnection". 
            // Si no la encuentra, usa la de localhost por defecto:
            _connectionString = _configuration.GetConnectionString("DefaultConnection")
                ?? "Server=localhost,1433;Database=API_HotelDB;User Id=sa;Password=SuperSecurePassword123!;TrustServerCertificate=True;";


        }

        public IDbConnection CreateConnection()=> new SqlConnection(_connectionString);

    }
}
