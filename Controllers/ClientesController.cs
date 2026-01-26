using System;
using System.Linq;
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

        // Listado de clientes
        public IActionResult Index()
        {
            var listaClientes = _contexto.Clientes.ToList();
            return View(listaClientes);
        }

        // Vista para crear un nuevo cliente
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Crear(Cliente nuevoCliente)
        {
            if (ModelState.IsValid)
            {
                _contexto.Clientes.Add(nuevoCliente);
                _contexto.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(nuevoCliente);
        }
    }
}