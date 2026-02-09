using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoIntegradorS6G7.Models;
using BCrypt.Net;

namespace ProyectoIntegradorS6G7.Controllers
{
    public class ConfiguracionController : Controller
    {
        private readonly AppDbContext _contexto;

        public ConfiguracionController(AppDbContext contexto)
        {
            _contexto = contexto;
        }

        // GET: /Configuracion/Index
        [HttpGet]
        public IActionResult Index()
        {
            var usuario = HttpContext.Session.GetString("UsuarioEmail");

            if (string.IsNullOrEmpty(usuario))
            {
                return RedirectToAction("Login", "Cuenta");
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

            // Obtener administradores
            var administradores = _contexto.Administradores
                .Where(a => a.activo)
                .OrderBy(a => a.nombreCompleto)
                .ToList();

            ViewBag.Administradores = administradores;
            ViewBag.EmailActual = usuario;

            return View(config);
        }

        //[HttpGet]
        //public IActionResult Index()
        //{
        //    var config = _contexto.ConfiguracionIA.FirstOrDefault();

        //    if (config == null)
        //    {

        //        config = new ConfiguracionIA();
        //        _contexto.ConfiguracionIA.Add(config);
        //        _contexto.SaveChanges();
        //    }

        //    var administradores = _contexto.Administradores
        //        .Where(a => a.activo)
        //        .OrderBy(a => a.nombreCompleto)
        //        .ToList();

        //    ViewBag.Administradores = administradores;

        //    ViewBag.EmailActual = HttpContext.Session.GetString("UsuarioEmail");


        //    return View(config);
        //}

        // POST: /Configuracion/GuardarParametrosIA
        //[HttpPost]
        [HttpPost]
        public IActionResult GuardarParametrosIA(ConfiguracionIA modelo)
        {
            try
            {
                // Validar sesión
                var usuario = HttpContext.Session.GetString("UsuarioEmail");

                if (string.IsNullOrEmpty(usuario))
                {
                    TempData["Error"] = "Sesión expirada. Inicie sesión nuevamente.";
                    return RedirectToAction("Login", "Cuenta");
                }

                var config = _contexto.ConfiguracionIA.FirstOrDefault();

                if (config == null)
                {
                    config = new ConfiguracionIA();
                    _contexto.ConfiguracionIA.Add(config);
                }

                // Actualizar valores
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
                config.actualizadoPor = usuario; // ya validado

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

        //public IActionResult GuardarParametrosIA(ConfiguracionIA modelo)
        //{
        //    try
        //    {
        //        var config = _contexto.ConfiguracionIA.FirstOrDefault();

        //        if (config == null)
        //        {
        //            config = new ConfiguracionIA();
        //            _contexto.ConfiguracionIA.Add(config);
        //        }

        //        // Actualizar valores
        //        config.interesMinimo = modelo.interesMinimo;
        //        config.interesMaximo = modelo.interesMaximo;
        //        config.interesRiesgoBajo = modelo.interesRiesgoBajo;
        //        config.interesRiesgoMedio = modelo.interesRiesgoMedio;
        //        config.interesRiesgoAlto = modelo.interesRiesgoAlto;
        //        config.diasParaMorosidad = modelo.diasParaMorosidad;
        //        config.diasParaRiesgoAlto = modelo.diasParaRiesgoAlto;
        //        config.cuotasMinimasPorDefecto = modelo.cuotasMinimasPorDefecto;
        //        config.cuotasMaximasPorDefecto = modelo.cuotasMaximasPorDefecto;

        //        config.fechaActualizacion = DateTime.Now;
        //        config.actualizadoPor = HttpContext.Session.GetString("UsuarioEmail");

        //        _contexto.SaveChanges();

        //        TempData["Mensaje"] = "Parámetros de IA actualizados correctamente";
        //        return RedirectToAction("Index");
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["Error"] = "Error: " + ex.Message;
        //        return RedirectToAction("Index");
        //    }
        //}

        // POST: /Configuracion/CrearAdministrador
        [HttpPost]
        public IActionResult CrearAdministrador(string nombreCompleto, string cedula, string email, string telefono, string password)
        {
            try
            {
                // Validar que no existe
                var existe = _contexto.Administradores.Any(a => a.email == email || a.cedula == cedula);
                if (existe)
                {
                    TempData["Error"] = "Ya existe un administrador con ese email o cédula";
                    return RedirectToAction("Index");
                }

                // Validar límite de 3 administradores
                var totalAdmins = _contexto.Administradores.Count(a => a.activo);
                if (totalAdmins >= 3)
                {
                    TempData["Error"] = "Ya existen 3 administradores activos. Desactiva uno para crear otro.";
                    return RedirectToAction("Index");
                }

                // Crear administrador
                var admin = new Administrador
                {
                    nombreCompleto = nombreCompleto,
                    cedula = cedula,
                    email = email,
                    telefono = telefono,
                    password = BCrypt.Net.BCrypt.HashPassword(password), // Hash de contraseña
                    activo = true,
                    creadoPor = HttpContext.Session.GetString("UsuarioEmail")
                };

                _contexto.Administradores.Add(admin);
                _contexto.SaveChanges();

                TempData["Mensaje"] = "Administrador creado exitosamente";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // POST: /Configuracion/DesactivarAdministrador
        [HttpPost]
        public IActionResult DesactivarAdministrador(int id)
        {
            try
            {
                var admin = _contexto.Administradores.Find(id);
                if (admin != null)
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
    }
}