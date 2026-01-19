using facturacion2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Data.Common;

namespace facturacion2.Controllers
{
    public class AuthController : Controller
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Muestra la vista de login
        public IActionResult login()
        {
            return View();
        }

        // Procesa el login
        [HttpPost]
        public IActionResult Login(string usuario, string contrasena)
        {
            try
            {
                Console.WriteLine("Intentando conectar con usuario: " + usuario);
                Console.WriteLine("Contraseña: " + contrasena);
                // 1. Crear instancia del Model de conexión
                DbConnectionModelo db = new DbConnectionModelo(_configuration);

                // 2. Obtener conexión usando credenciales ingresadas
                using var conn = db.GetConnection(usuario, contrasena);
                {
                    conn.Open();
                    Console.WriteLine("Conexión abierta correctamente");
                } // aquí PostgreSQL valida usuario y password

                // 3. Obtener el rol del usuario conectado
                var cmd = new NpgsqlCommand(@"
                                            SELECT rolname
                                            FROM pg_roles
                                            WHERE pg_has_role(current_user, oid, 'member')
                                        ", conn);

                var role = cmd.ExecuteScalar()?.ToString();
                Console.WriteLine("Rol del usuario: " + role);

                // 4. Guardar datos en sesión
                HttpContext.Session.SetString("db_user", usuario);
                Console.WriteLine("Guardado en sesión: db_user = " + usuario);
                HttpContext.Session.SetString("db_password", contrasena);
                Console.WriteLine("Guardado en sesión: db_password = " + contrasena);
                HttpContext.Session.SetString("db_role", role ?? "sin_rol");
                Console.WriteLine("Guardado en sesión: db_role = " + (role ?? "sin_rol"));

                // 5. Redirigir a pantalla principal
                return RedirectToAction("Index", "Pruebas");
            }
            catch (Exception)
            {
                ViewBag.Error = "Usuario o contraseña incorrectos";
                return View();
            }
        }

        // Cerrar sesión
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}