using System;
using System.Linq;
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

        // Este procesa el formulario de Login
        [HttpPost]
        public IActionResult Ingresar(string correo, string clave)
        {
            // Buscamos si existe el usuario en la DB
            var usuario = _contexto.Usuarios
                .FirstOrDefault(u => u.email == correo && u.password == clave);

            if (usuario != null)
            {
                // Si es ADMIN, va al Dashboard global
                if (usuario.rol == "Administrador")
                {
                    return RedirectToAction("Index", "Home");
                }
                // Si es CLIENTE, va a su portal personal
                else
                {
                    return RedirectToAction("MiPortal", "Clientes");
                }
            }

            // Si falla, regresa a la vista con un error
            ViewBag.Error = "Credenciales incorrectas";
            return View();
        }
    }
}
