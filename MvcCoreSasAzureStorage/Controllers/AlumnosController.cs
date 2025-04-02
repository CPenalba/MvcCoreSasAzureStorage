using Microsoft.AspNetCore.Mvc;
using MvcCoreSasAzureStorage.Models;
using MvcCoreSasAzureStorage.Services;

namespace MvcCoreSasAzureStorage.Controllers
{
    public class AlumnosController : Controller
    {
        private ServiceAzureAlumnos service;

        public AlumnosController(ServiceAzureAlumnos service)
        {
            this.service = service;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string curso)
        {
            List<Alumno> alumnos = await this.service.GetAlumnosAsync(curso);
            return View(alumnos);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Alumno a)
        {
            await this.service.CreateAlumno(a.IdAlumno, a.Nombre, a.Apellidos, a.Nota);
            return RedirectToAction("Index");
        }
    }
}
