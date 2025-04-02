using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;
using MvcCoreSasAzureStorage.Helpers;
using MvcCoreSasAzureStorage.Models;

namespace MvcCoreSasAzureStorage.Controllers
{
    public class MigracionController : Controller
    {
        private HelperXML helper;

        public MigracionController(HelperXML helper)
        {
            this.helper = helper;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string accion)
        {
            string azureKeys = "";
            TableServiceClient serviceClient = new TableServiceClient(azureKeys);
            TableClient tableClient = serviceClient.GetTableClient("alumnos");
            await tableClient.CreateIfNotExistsAsync();
            List<Alumno> alumnos = this.helper.GetAlumnos();
            foreach (Alumno alum in alumnos)
            {
                await tableClient.AddEntityAsync<Alumno>(alum);
            }
            ViewData["MENSAJE"] = "Migración completada";
            return View();
        }
    }
}
