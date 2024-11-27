using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcSoporte.Data;
using MvcSoporte.Models;

namespace MvcSoporte.Controllers
{
    [Authorize(Roles = "Usuario")]
    public class MisDatosController : Controller
    {
        private readonly MvcSoporteContexto _context;

        public MisDatosController(MvcSoporteContexto context)
        {
            _context = context;
        }

        // GET: MisDatos/Create 
        public IActionResult Create()
        {
            return View();
        }

        // POST: MisDatos/Create 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>

        Create([Bind("Id,Nombre,Email,Telefono,FechaNacimiento")] Empleado empleado)
        {
            // ASIGNAR EL EMAIL DEL USUARIO ACTUAL 
            empleado.Email = User.Identity.Name;

            if (ModelState.IsValid)
            {
                _context.Add(empleado);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return View(empleado);
        }
    }
}

