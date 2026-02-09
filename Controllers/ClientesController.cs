using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using ProyectoIntegradorS6G7.Models;

namespace ProyectoIntegradorS6G7.Controllers
{
    public class ClientesController : Controller
    {
        private readonly AppDbContext _contexto;

        public ClientesController(AppDbContext contexto)
        {
            _contexto = contexto;
        }

        // 1. Listado principal
        //public IActionResult Index()
        //{
        //    var clientes = _contexto.Clientes.ToList();
        //    return View(clientes);
        //}

        // 2. Vista Crear Cliente
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        
        public IActionResult Crear(Cliente c)
        {
            ModelState.Remove("saldoPendiente");

            if (ModelState.IsValid)
            {
                try
                {
                    // 1. Guardar el cliente
                    _contexto.Clientes.Add(c);
                    _contexto.SaveChanges();

                    // 2. Crear el Usuario automáticamente
                    var nuevoUsuario = new Usuario
                    {
                        email = c.email,
                        password = c.ruc, 
                        rol = "Cliente",
                        rucAsociado = c.ruc
                    };
                    _contexto.Usuarios.Add(nuevoUsuario);
                    _contexto.SaveChanges();

                    // 3. ENVIAR EMAIL REAL (Agregado)
                    try
                    {
                        EnviarEmailBienvenida(c.email, c.ruc, c.razonSocial);
                        TempData["Mensaje"] = $"¡Éxito! Cliente creado y notificado por email.";
                    }
                    catch (Exception exEmail)
                    {
                        // Si falla el mail, el cliente ya está creado, solo avisamos del error de red
                        TempData["Mensaje"] = $"Cliente creado, pero el correo falló: {exEmail.Message}";
                    }

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "Error al guardar: " + ex.Message;
                }
            }
            return View(c);
        }
        // 3. Eliminar Cliente
        public IActionResult Eliminar(int id)
        {
            var cliente = _contexto.Clientes.Find(id);
            if (cliente != null)
            {
                _contexto.Clientes.Remove(cliente);
                _contexto.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        // BUSCADOR
        public IActionResult Index(string buscar)
        {
            // Guardamos el término de búsqueda para que no se borre del input al recargar
            ViewData["FiltroActual"] = buscar;

            var clientes = from c in _contexto.Clientes select c;

            if (!string.IsNullOrEmpty(buscar))
            {
                // Filtra por RUC o por Razón Social
                clientes = clientes.Where(c => c.ruc.Contains(buscar) ||
                                               c.razonSocial.Contains(buscar));
            }

            return View(clientes.ToList());
        }

        // DETALLES
        public IActionResult Detalles(int id)
        {
            var cliente = _contexto.Clientes.FirstOrDefault(c => c.id == id);
            if (cliente == null) return NotFound();
            return View(cliente);
        }

        // EDITAR (GET)
        public IActionResult Editar(int id)
        {
            var cliente = _contexto.Clientes.Find(id);
            return View(cliente);
        }
        public IActionResult Portal()
        {
            // Obtenemos el RUC del cliente que inició sesión
            string ruc = HttpContext.Session.GetString("UsuarioRuc");

            // Buscamos sus datos
            var datosCliente = _contexto.Clientes.FirstOrDefault(c => c.ruc == ruc);

            if (datosCliente == null) return RedirectToAction("Ingresar", "Cuenta");

            return View(datosCliente);
        }
        //MANDAR EMAIL CON CREDENCIALES
        private void EnviarEmailBienvenida(string emailDestino, string ruc, string nombreEmpresa)
        {
            var emisor = "cernasayuri@gmail.com";
            var passwordApp = "jqcm ickw mcfg xyag";

            var mensaje = new System.Net.Mail.MailMessage();
            mensaje.From = new System.Net.Mail.MailAddress(emisor, "CobranzasPro");
            mensaje.To.Add(emailDestino);
            mensaje.Subject = "Bienvenido a CobranzasPro - Tus credenciales de acceso";

            // Diseño del correo
            mensaje.Body = $@"
        <div style='font-family: sans-serif; border: 1px solid #eee; padding: 20px; border-radius: 10px;'>
            <h2 style='color: #1F2B6C;'>¡Hola, {nombreEmpresa}!</h2>
            <p>Tu cuenta ha sido habilitada en nuestro sistema de gestión de cobranzas.</p>
            <p>Para revisar tu estado de cuenta, usa los siguientes datos:</p>
            <div style='background: #f6f8fc; padding: 15px; border-radius: 5px;'>
                <p><strong>Usuario:</strong> {emailDestino}</p>
                <p><strong>Contraseña temporal:</strong> {ruc}</p>
            </div>
            <p style='color: #666; font-size: 12px; margin-top: 20px;'>
                * Se recomienda cambiar tu contraseña al ingresar por primera vez.
            </p>
            <a href='#' style='background: #4F6BED; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; display: inline-block;'>Acceder al Portal</a>
        </div>";

            mensaje.IsBodyHtml = true;

            using (var clienteSmtp = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587))
            {
                clienteSmtp.Credentials = new System.Net.NetworkCredential(emisor, passwordApp);
                clienteSmtp.EnableSsl = true;
                clienteSmtp.Send(mensaje);
            }
        }
    }
}