using Npgsql;
using Microsoft.Extensions.Configuration;

namespace facturacion2.Models
{
    public class DbConnectionModelo
    {
        private readonly IConfiguration _configuration;

        public DbConnectionModelo(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public NpgsqlConnection GetConnection(string usuario, string contrasena)
        {
            var host = _configuration["DatabaseSettings:Host"];
            var database = _configuration["DatabaseSettings:Database"];
            var port = _configuration["DatabaseSettings:Port"];

            var connectionString =
                $"Server={host};Port={port};Database={database};User Id={usuario};Password={contrasena}";

            return new NpgsqlConnection(connectionString);
        }
    }
}
