using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoIntegradorS6G7.Models;

namespace ProyectoIntegradorS6G7.Controllers
{
    public class CobranzasController : Controller
    {
        private readonly AppDbContext _contexto;
        private readonly IWebHostEnvironment _env;

        public CobranzasController(AppDbContext contexto, IWebHostEnvironment env)
        {
            _contexto = contexto;
            _env = env;
        }

        [HttpGet]
        public IActionResult Index(string buscar, DateTime? fecha, string estado)
        {
            var query = _contexto.Creditos.Where(c => c.estado != "Pagado").AsQueryable();

            if (!string.IsNullOrEmpty(buscar))
                query = query.Where(c => c.rucCliente.Contains(buscar));

            if (fecha.HasValue)
                query = query.Where(c => c.fechaRegistro.Date == fecha.Value.Date);

            if (!string.IsNullOrEmpty(estado) && estado != "Estado")
                query = query.Where(c => c.estado == estado);

            var creditos = query.OrderByDescending(c => c.fechaRegistro).ToList();

            foreach (var credito in creditos)
            {
                var pagosTotales = _contexto.Pagos
                    .Where(p => p.idObligacion == credito.idObligacion)
                    .Sum(p => (decimal?)p.montoPagado) ?? 0;

                var montoConInteres = credito.montoTotal + (credito.montoTotal * (credito.tasaInteres / 100));
                credito.montoTotal = montoConInteres - pagosTotales;
            }

            return View(creditos);
        }

        [HttpGet]
        public IActionResult ObtenerDetalleCuotas(string idObligacion)
        {
            var credito = _contexto.Creditos.FirstOrDefault(c => c.idObligacion == idObligacion);
            if (credito == null)
                return Json(new { success = false, message = "Crédito no encontrado" });

            var cliente = _contexto.Clientes.FirstOrDefault(c => c.ruc == credito.rucCliente);
            var montoConInteres = credito.montoTotal + (credito.montoTotal * (credito.tasaInteres / 100));
            var montoPorCuota = montoConInteres / credito.cuotas;

            var cuotasPagadas = _contexto.Pagos
                .Where(p => p.idObligacion == idObligacion)
                .Select(p => p.numeroCuota)
                .ToList();

            var cuotas = new List<object>();
            for (int i = 1; i <= credito.cuotas; i++)
            {
                var pagado = cuotasPagadas.Contains(i);
                cuotas.Add(new
                {
                    numero = i,
                    monto = montoPorCuota,
                    pagado = pagado,
                    estado = pagado ? "Pagado" : "Pendiente"
                });
            }

            return Json(new
            {
                success = true,
                credito = new
                {
                    idObligacion = credito.idObligacion,
                    rucCliente = credito.rucCliente,
                    razonSocial = cliente?.razonSocial ?? "N/A",
                    montoTotal = montoConInteres,
                    cuotasTotal = credito.cuotas,
                    tasaInteres = credito.tasaInteres,
                    montoPorCuota = montoPorCuota
                },
                cuotas = cuotas
            });
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarPago(
            string idObligacion,
            int numeroCuota,
            decimal monto,
            string metodoPago,
            string observaciones,
            IFormFile comprobante,
            string firmaBase64)
        {
            try
            {
                var yaExiste = _contexto.Pagos.Any(p => p.idObligacion == idObligacion && p.numeroCuota == numeroCuota);
                if (yaExiste)
                    return Json(new { success = false, message = "Esta cuota ya fue pagada" });

                string rutaComprobante = null;
                if (comprobante != null && comprobante.Length > 0)
                {
                    var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
                    var extension = Path.GetExtension(comprobante.FileName).ToLower();

                    if (!extensionesPermitidas.Contains(extension))
                        return Json(new { success = false, message = "Solo JPG, PNG o PDF" });

                    var nombreArchivo = $"{idObligacion}_cuota{numeroCuota}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
                    var carpetaDestino = Path.Combine(_env.WebRootPath, "uploads", "comprobantes");

                    if (!Directory.Exists(carpetaDestino))
                        Directory.CreateDirectory(carpetaDestino);

                    var rutaCompleta = Path.Combine(carpetaDestino, nombreArchivo);

                    using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                    {
                        await comprobante.CopyToAsync(stream);
                    }

                    rutaComprobante = $"/uploads/comprobantes/{nombreArchivo}";
                }

                // ✍️ VALIDAR FIRMA PARA TRANSFERENCIAS
                if (metodoPago == "Transferencia" && string.IsNullOrEmpty(firmaBase64))
                    return Json(new { success = false, message = "La firma es requerida para transferencias" });

                var usuarioEmail = HttpContext.Session.GetString("UsuarioEmail") ?? "Sistema";

                var pago = new Pago
                {
                    idObligacion = idObligacion,
                    numeroCuota = numeroCuota,
                    montoPagado = monto,
                    fechaPago = DateTime.Now,
                    metodoPago = metodoPago,
                    observaciones = observaciones,
                    reciboNumero = "REC-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                    comprobanteRuta = rutaComprobante,
                    firmaBase64 = firmaBase64,
                    registradoPor = usuarioEmail
                };

                _contexto.Pagos.Add(pago);

                var credito = _contexto.Creditos.FirstOrDefault(c => c.idObligacion == idObligacion);
                var totalPagos = _contexto.Pagos.Count(p => p.idObligacion == idObligacion) + 1;

                if (totalPagos >= credito.cuotas)
                {
                    credito.estado = "Pagado";
                    var cliente = _contexto.Clientes.FirstOrDefault(c => c.ruc == credito.rucCliente);
                    if (cliente != null)
                    {
                        cliente.saldoPendiente = 0;
                        cliente.estado = "Activo";
                    }
                }

                _contexto.SaveChanges();

                return Json(new
                {
                    success = true,
                    message = "Pago registrado exitosamente",
                    recibo = pago.reciboNumero,
                    creditoCompleto = totalPagos >= credito.cuotas,
                    comprobanteGuardado = rutaComprobante != null,
                    firmaGuardada = !string.IsNullOrEmpty(firmaBase64)
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpGet]
        public IActionResult HistorialPagos(string idObligacion)
        {
            var pagos = _contexto.Pagos
                .Where(p => p.idObligacion == idObligacion)
                .OrderBy(p => p.numeroCuota)
                .ToList();

            var credito = _contexto.Creditos.FirstOrDefault(c => c.idObligacion == idObligacion);
            ViewBag.Credito = credito;

            return View(pagos);
        }
    }
}