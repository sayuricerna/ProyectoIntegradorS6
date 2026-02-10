using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoIntegradorS6G7.Models;
using BCrypt.Net;
using System.Net.Mail;
using System.Net;

namespace ProyectoIntegradorS6G7.Controllers
{
    public class ConfiguracionController : Controller
    {
        private readonly AppDbContext _contexto;

        public ConfiguracionController(AppDbContext contexto)
        {
            _contexto = contexto;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var usuario = HttpContext.Session.GetString("UsuarioEmail");
            if (string.IsNullOrEmpty(usuario))
            {
                return RedirectToAction("Ingresar", "Cuenta");
            }

            // Obtener o crear configuración
            var config = _contexto.ConfiguracionIA.FirstOrDefault();
            if (config == null)
            {
                config = new ConfiguracionIA
                {
                    actualizadoPor = usuario,
                    fechaActualizacion = DateTime.Now
                };
                _contexto.ConfiguracionIA.Add(config);
                _contexto.SaveChanges();
            }

            // OBTENER ADMINISTRADORES DESDE USUARIOS
            var administradores = _contexto.Usuarios
                .Where(u => u.rol == "Administrador" && u.activo)
                .OrderBy(u => u.nombreCompleto)
                .ToList();

            ViewBag.Administradores = administradores;
            ViewBag.EmailActual = usuario;

            // Datos del admin actual
            var adminActual = _contexto.Usuarios.FirstOrDefault(u => u.email == usuario);
            ViewBag.AdminActual = adminActual;

            return View(config);
        }

        [HttpPost]
        public IActionResult GuardarParametrosIA(ConfiguracionIA modelo)
        {
            try
            {
                var usuario = HttpContext.Session.GetString("UsuarioEmail");
                if (string.IsNullOrEmpty(usuario))
                {
                    TempData["Error"] = "Sesión expirada. Inicie sesión nuevamente.";
                    return RedirectToAction("Ingresar", "Cuenta");
                }

                var config = _contexto.ConfiguracionIA.FirstOrDefault();
                if (config == null)
                {
                    config = new ConfiguracionIA();
                    _contexto.ConfiguracionIA.Add(config);
                }

                config.interesMinimo = modelo.interesMinimo;
                config.interesMaximo = modelo.interesMaximo;
                config.interesRiesgoBajo = modelo.interesRiesgoBajo;
                config.interesRiesgoMedio = modelo.interesRiesgoMedio;
                config.interesRiesgoAlto = modelo.interesRiesgoAlto;
                config.diasParaMorosidad = modelo.diasParaMorosidad;
                config.diasParaRiesgoAlto = modelo.diasParaRiesgoAlto;
                config.cuotasMinimasPorDefecto = modelo.cuotasMinimasPorDefecto;
                config.cuotasMaximasPorDefecto = modelo.cuotasMaximasPorDefecto;
                config.fechaActualizacion = DateTime.Now;
                config.actualizadoPor = usuario;

                _contexto.SaveChanges();

                TempData["Mensaje"] = "Parámetros de IA actualizados correctamente";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // crud
        [HttpPost]
        public IActionResult CrearAdministrador(string nombreCompleto, string cedula, string email, string telefono, string password)
        {
            try
            {
                // Validar que no existe
                var existe = _contexto.Usuarios.Any(u => u.email == email);
                if (existe)
                {
                    TempData["Error"] = "Ya existe un usuario con ese email";
                    return RedirectToAction("Index");
                }

                // Validar límite de 3 administradores
                var totalAdmins = _contexto.Usuarios.Count(u => u.rol == "Administrador" && u.activo);
                if (totalAdmins >= 3)
                {
                    TempData["Error"] = "Ya existen 3 administradores activos. Desactiva uno para crear otro.";
                    return RedirectToAction("Index");
                }

                // Crear usuario administrador
                var nuevoAdmin = new Usuario
                {
                    email = email,
                    password = BCrypt.Net.BCrypt.HashPassword(password),
                    rol = "Administrador",
                    nombreCompleto = nombreCompleto,
                    cedula = cedula,
                    telefono = telefono,
                    activo = true,
                    creadoPor = HttpContext.Session.GetString("UsuarioEmail"),
                    fechaCreacion = DateTime.Now,
                    puedeConfigurar = false
                };

                _contexto.Usuarios.Add(nuevoAdmin);
                _contexto.SaveChanges();

                // ENVIAR EMAIL CON CREDENCIALES
                try
                {
                    EnviarEmailNuevoAdministrador(email, password, nombreCompleto);
                    TempData["Mensaje"] = "Administrador creado y notificado por email";
                }
                catch
                {
                    TempData["Mensaje"] = "Administrador creado (email no enviado)";
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // 🆕 DESACTIVAR ADMINISTRADOR
        [HttpPost]
        public IActionResult DesactivarAdministrador(int id)
        {
            try
            {
                var admin = _contexto.Usuarios.Find(id);
                if (admin != null && admin.rol == "Administrador")
                {
                    admin.activo = false;
                    _contexto.SaveChanges();
                    TempData["Mensaje"] = "Administrador desactivado";
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // Actualizar
        [HttpPost]
        public IActionResult ActualizarPerfil(string telefono, string passwordActual, string passwordNueva)
        {
            try
            {
                var email = HttpContext.Session.GetString("UsuarioEmail");
                var admin = _contexto.Usuarios.FirstOrDefault(u => u.email == email);

                if (admin == null)
                {
                    TempData["Error"] = "Usuario no encontrado";
                    return RedirectToAction("Index");
                }

                // Actualizar teléfono si se proporcionó
                if (!string.IsNullOrEmpty(telefono))
                {
                    admin.telefono = telefono;
                }

                // Cambiar contraseña si se proporcionó
                if (!string.IsNullOrEmpty(passwordActual) && !string.IsNullOrEmpty(passwordNueva))
                {
                    // Verificar contraseña actual
                    if (!BCrypt.Net.BCrypt.Verify(passwordActual, admin.password))
                    {
                        TempData["Error"] = "La contraseña actual es incorrecta";
                        return RedirectToAction("Index");
                    }

                    admin.password = BCrypt.Net.BCrypt.HashPassword(passwordNueva);
                }

                _contexto.SaveChanges();
                TempData["Mensaje"] = "Perfil actualizado correctamente";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        //email
        private void EnviarEmailNuevoAdministrador(string emailDestino, string password, string nombre)
        {
            var emisor = "cernasayuri@gmail.com";
            var passwordApp = "jqcm ickw mcfg xyag";

            var mensaje = new MailMessage();
            mensaje.From = new MailAddress(emisor, "CobranzasPro");
            mensaje.To.Add(emailDestino);
            mensaje.Subject = "Bienvenido al equipo de CobranzasPro";

            mensaje.Body = $@"
                <div style='font-family: sans-serif; border: 1px solid #eee; padding: 20px; border-radius: 10px;'>
                    <h2 style='color: #1F2B6C;'>¡Hola, {nombre}!</h2>
                    <p>Has sido agregado como <strong>Administrador</strong> en CobranzasPro.</p>
                    <p>Tus credenciales de acceso son:</p>
                    <div style='background: #f6f8fc; padding: 15px; border-radius: 5px;'>
                        <p><strong>Usuario:</strong> {emailDestino}</p>
                        <p><strong>Contraseña temporal:</strong> {password}</p>
                    </div>
                    <p style='color: #dc3545; font-weight: bold;'>
                        ⚠️ Por seguridad, cambia tu contraseña al iniciar sesión.
                    </p>
                    <a href='#' style='background: #4F6BED; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; display: inline-block; margin-top: 15px;'>
                        Acceder al Sistema
                    </a>
                </div>";

            mensaje.IsBodyHtml = true;

            using (var clienteSmtp = new SmtpClient("smtp.gmail.com", 587))
            {
                clienteSmtp.Credentials = new NetworkCredential(emisor, passwordApp);
                clienteSmtp.EnableSsl = true;
                clienteSmtp.Send(mensaje);
            }
        }
    }
}