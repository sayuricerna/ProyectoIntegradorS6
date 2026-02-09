using Microsoft.AspNetCore.Mvc;
using ProyectoIntegradorS6G7.Models;
using System;
using System.Linq;

namespace ProyectoIntegradorS6G7.Controllers
{
    public class CreditosController : Controller
    {
        private readonly AppDbContext _contexto;

        public CreditosController(AppDbContext contexto)
        {
            _contexto = contexto;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // Obtenemos la lista de créditos desde la base de datos
            var listaCreditos = _contexto.Creditos.OrderByDescending(c => c.fechaRegistro).ToList();
            return View(listaCreditos);
        }
        [HttpGet] // Cambiado a GET para que cargue la vista inicialmente
        public IActionResult Crear()
        {
            return View();
        }
        
        
        [HttpPost]
        public IActionResult GuardarCredito(Credito c)
        {
            try
            {
                // Limpiamos errores de validación para campos que generamos nosotros
                ModelState.Remove("idObligacion");
                ModelState.Remove("fechaRegistro");
                ModelState.Remove("estado");

                // 🔍 DEBUG: Ver errores de validación
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .Select(x => new {
                            Field = x.Key,
                            Errors = x.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                        })
                        .ToList();

                    // Esto mostrará en la consola del navegador qué campos faltan
                    return Json(new
                    {
                        success = false,
                        message = "Error de validación",
                        errors = errors,
                        receivedData = new
                        {
                            ruc = c.rucCliente,
                            monto = c.montoTotal,
                            cuotas = c.cuotas,
                            interes = c.tasaInteres
                        }
                    });
                }

                c.idObligacion = "FACT-" + Guid.NewGuid().ToString().Substring(0, 5).ToUpper() + "-2026";
                c.fechaRegistro = DateTime.Now;
                c.estado = "Pendiente";

                _contexto.Creditos.Add(c);
                _contexto.SaveChanges();

                return Json(new { success = true, message = "¡Venta a crédito confirmada exitosamente!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }
        //public IActionResult GuardarCredito(Credito c)
        //{
        //    // Limpiamos errores de validación para campos que generamos nosotros
        //    ModelState.Remove("idObligacion");
        //    ModelState.Remove("fechaRegistro");

        //    if (ModelState.IsValid)
        //    {
        //        c.idObligacion = "FACT-" + Guid.NewGuid().ToString().Substring(0, 5).ToUpper() + "-2026";
        //        c.fechaRegistro = DateTime.Now;
        //        c.estado = "Pendiente"; // Estado inicial para Cobranzas

        //        _contexto.Creditos.Add(c);
        //        _contexto.SaveChanges();

        //        TempData["Mensaje"] = "¡Venta a crédito confirmada exitosamente!";
        //        return RedirectToAction("Index");
        //    }


        //    // Si llegas aquí, algo falló en la validación
        //    return View("Crear", c);
        //}


        [HttpGet]
        public IActionResult BuscarClientePorRUC(string ruc)
        {
            // Buscamos en la tabla Clientes
            var cliente = _contexto.Clientes.FirstOrDefault(c => c.ruc == ruc);

            if (cliente != null)
            {
                return Json(new
                {
                    encontrado = true,
                    razonSocial = cliente.razonSocial,
                    contacto = cliente.contactoPrincipal 
                });
            }

            return Json(new { encontrado = false });
        }


    }
}