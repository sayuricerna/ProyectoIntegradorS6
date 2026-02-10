using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ProyectoIntegradorS6G7.Models;
using ProyectoIntegradorS6G7.Services;

namespace ProyectoIntegradorS6G7.Controllers
{
    public class CreditosController : Controller
    {
        private readonly AppDbContext _contexto;

        private readonly IAService _iaService;

        public CreditosController(AppDbContext contexto, IAService iaService)
        {
            _contexto = contexto;
            _iaService = iaService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var listaCreditos = _contexto.Creditos.OrderByDescending(c => c.fechaRegistro).ToList();
            return View(listaCreditos);
        }
        [HttpGet]
        public IActionResult Crear()
        {
            var config = _contexto.ConfiguracionIA.FirstOrDefault();

            if (config == null)
            {
                config = new ConfiguracionIA();
            }

            ViewBag.Config = config;

            return View();
        }
        // IA CON HISTORIAL CREDITICIO 
        [HttpGet]
        public IActionResult ObtenerRecomendacionIA(string ruc)
        {
            try
            {
                var recomendacion = _iaService.ObtenerRecomendacion(ruc);
                var config = _contexto.ConfiguracionIA.FirstOrDefault() ?? new ConfiguracionIA();

                return Json(new
                {
                    success = true,
                    nivelRiesgo = recomendacion.NivelRiesgo,
                    interesRecomendado = recomendacion.InteresRecomendado,
                    cuotasRecomendadas = recomendacion.CuotasRecomendadas,
                    razonamiento = recomendacion.Razonamiento,
                    usaModelo = recomendacion.UsaModelo,
                    config = new
                    {
                        interesMinimo = config.interesMinimo,
                        interesMaximo = config.interesMaximo,
                        cuotasMinimas = config.cuotasMinimasPorDefecto,
                        cuotasMaximas = config.cuotasMaximasPorDefecto
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpPost]
        public IActionResult GuardarCredito(Credito c)
        {
            try
            {
                ModelState.Remove("idObligacion");
                ModelState.Remove("fechaRegistro");
                ModelState.Remove("estado");

                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .Select(x => new {
                            Field = x.Key,
                            Errors = x.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                        })
                        .ToList();

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


        [HttpGet]
        public IActionResult BuscarClientePorRUC(string ruc)
        {
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