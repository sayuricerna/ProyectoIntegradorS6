using System;
using Microsoft.AspNetCore.Mvc;
using ProyectoIntegradorS6G7.Models;

namespace ProyectoIntegradorS6G7.Controllers
{
    public class CreditosController : Controller
    {
        private readonly AppDbContext _contexto;

        public CreditosController(AppDbContext contexto)
        {
            _contexto = contexto;
        }

        // Muestra el formulario del Stepper
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GuardarCredito(Credito nuevoCredito)
        {
            // AQUÍ LLAMAREMOS A LA IA DE PYTHON PRÓXIMAMENTE
            nuevoCredito.nivelRiesgoIA = "Procesando...";
            nuevoCredito.recomendacionIA = "Esperando respuesta de Python";

            _contexto.Creditos.Add(nuevoCredito);
            _contexto.SaveChanges();

            return RedirectToAction("Index", "Home");
        }
    }
}