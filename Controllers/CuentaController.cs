using System;
using System.Linq;
using Microsoft.AspNetCore.Http; 
using Microsoft.AspNetCore.Mvc;
using ProyectoIntegradorS6G7.Models;
namespace ProyectoIntegradorS6G7.Controllers
{
    public class CuentaController : Controller
    {
        private readonly AppDbContext _contexto;

        public CuentaController(AppDbContext contexto)
        {
            _contexto = contexto;
        }

        // Este muestra la pantalla de Login
        public IActionResult Ingresar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Ingresar(string correo, string clave)
        {
            var usuario = _contexto.Usuarios.FirstOrDefault(u => u.email == correo && u.password == clave);

            if (usuario != null)
            {
                HttpContext.Session.Clear();
                HttpContext.Session.SetString("UsuarioRol", usuario.rol); // Aquí guardará "Administrador"
                HttpContext.Session.SetString("UsuarioEmail", usuario.email);

                // CAMBIO AQUÍ: Usamos "Administrador" tal cual está en tu imagen
                if (usuario.rol.Equals("Administrador", StringComparison.OrdinalIgnoreCase))
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    HttpContext.Session.SetString("UsuarioRuc", usuario.rucAsociado ?? "");
                    return RedirectToAction("Portal", "Clientes");
                }
            }

            ViewBag.Error = "Credenciales incorrectas";
            return View();
        }


        // Asegúrate de tener este método también para cerrar sesión
        public IActionResult Salir()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Ingresar");
        }
    }
}
