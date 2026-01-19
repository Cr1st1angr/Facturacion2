using Microsoft.AspNetCore.Mvc;
using Npgsql;
using facturacion2.Models;

namespace facturacion2.Controllers
{
    public class PruebasController : Controller
    {
        private readonly DbConnectionModelo _db;

        public PruebasController(DbConnectionModelo db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        // 1️⃣ SELECT (permitido para todos)
        public IActionResult VerFacturas()
        {
            try
            {
                var user = HttpContext.Session.GetString("db_user");
                var pass = HttpContext.Session.GetString("db_password");

                using var conn = _db.GetConnection(user, pass);
                conn.Open();

                var cmd = new NpgsqlCommand("SELECT * FROM facturas", conn);
                var reader = cmd.ExecuteReader();

                return Content("SELECT ejecutado correctamente");
            }
            catch (Exception ex)
            {
                return Content("ERROR: " + ex.Message);
            }
        }

        // 2️⃣ INSERT (permitido solo para cajero y admin)
        public IActionResult InsertarFactura()
        {
            try
            {
                var user = HttpContext.Session.GetString("db_user");
                var pass = HttpContext.Session.GetString("db_password");

                using var conn = _db.GetConnection(user, pass);
                conn.Open();

                var cmd = new NpgsqlCommand(@"
                    INSERT INTO facturas (fecha, total)
                    VALUES (CURRENT_DATE, 100)
                ", conn);

                cmd.ExecuteNonQuery();

                return Content("INSERT ejecutado correctamente");
            }
            catch (Exception ex)
            {
                return Content("ERROR: " + ex.Message);
            }
        }

        // 3️⃣ DELETE (solo admin)
        public IActionResult EliminarFactura()
        {
            try
            {
                var user = HttpContext.Session.GetString("db_user");
                var pass = HttpContext.Session.GetString("db_password");

                using var conn = _db.GetConnection(user, pass);
                conn.Open();

                var cmd = new NpgsqlCommand("DELETE FROM facturas", conn);
                cmd.ExecuteNonQuery();

                return Content("DELETE ejecutado correctamente");
            }
            catch (Exception ex)
            {
                return Content("ERROR: " + ex.Message);
            }
        }
    }
}