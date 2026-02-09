using Microsoft.AspNetCore.Mvc;

namespace ProyectoIntegradorS6G7.Controllers
{
    public class ReportesController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            // URL de tu reporte publicado en Power BI
            ViewBag.PowerBIUrl = "TU_URL_DE_POWER_BI_AQUI";
            return View();
        }
    }
}