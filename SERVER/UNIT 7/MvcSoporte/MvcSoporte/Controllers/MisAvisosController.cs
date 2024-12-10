using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcSoporte.Data;
using MvcSoporte.Models;

namespace MvcSoporte.Controllers
{
    // CONTROLADOR DE AVISOS CUSTOMIZADO QUE HEREDA DE LA CLASE CONTROLADOR BÁSICA
    public class MisAvisosController : Controller
    {
        private readonly MvcSoporteContexto _context;

        public MisAvisosController(MvcSoporteContexto context)
        {
            _context = context;
        }

        // ACCIÓN GET QUE OBTIENE LOS AVISOS DE UN EMPLEADO Y DEVUELVE LA VISTA DE ESTOS
        // GET: MisAvisos
        public async Task<IActionResult> Index()
        {
            // SE SELECCIONA EL EMPLEADO CORRESPONDIENTE AL USUARIO ACTUAL, MEDIANTE SU IDENTITY
            var emailUsuario = User.Identity.Name;
            var empleado = await _context.Empleados.Where(e => e.Email == emailUsuario)
                    .FirstOrDefaultAsync();

            // SI EL EMPLEADO NO ES ENCONTRADO, TE DEVUELVE A LA VISTA DE INDEX DEL CONTROLADOR HOME
            if (empleado == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // SE SELECCIONAN LOS AVISOS DEL EMPLEADO CORRESPONDIENTE AL USUARIO ACTUAL 
            var misAvisos = _context.Avisos
                   .Where(a => a.EmpleadoId == empleado.Id)
                   .OrderByDescending(a => a.FechaAviso)
                   .Include(a => a.Empleado).Include(a => a.Equipo).Include(a => a.TipoAveria);

            // DEVOLVERÁ LA VISTA DE LOS AVISOS ACTUALES DEL EMPLEADO SELECCIONADO UNA VEZ SE HAYA COMPLETADO LA CARGA DE DATOS DE ESTOS
            return View(await misAvisos.ToListAsync());

            //var mvcSoporteContexto = _context.Avisos.Include(a => a.Empleado).Include(a => a.Equipo).Include(a => a.TipoAveria);
            //return View(await mvcSoporteContexto.ToListAsync());
        }


        // ACCIÓN GET QUE OBTIENE LOS DETALLES DE EL AVISO SELECCIONADO DE UN USUARIO POR SU ID Y DEVUELVE LA VISTA DE ESTE DETALLE
        // GET: MisAvisos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            // SI EL ID NO EXISTE DE ESA TAREA, DEVUELVE ERROR
            if (id == null)
            {
                return NotFound();
            }

            // BUSCA EL AVISO CON SU ID CORRESPONDIENTE EN EL CONTEXTO DE DATOS DE AVISOS
            var aviso = await _context.Avisos
                .Include(a => a.Empleado)
                .Include(a => a.Equipo)
                .Include(a => a.TipoAveria)
                .FirstOrDefaultAsync(m => m.Id == id);

            // SI EL AVISO CUYO ID APORTADO NO SE ENCUENTRA DEVOLVERÁ ERROR
            if (aviso == null)
            {
                return NotFound();
            }

            // PARA EVITAR EL ACCESO A LOS AVISOS DE OTROS EMPLEADOS 
            var emailUsuario = User.Identity.Name;
            var empleado = await _context.Empleados
                        .Where(e => e.Email == emailUsuario)
                        .FirstOrDefaultAsync();

            // SI EL EMPLEADO NO SE ENCUENTRA DEVOLVERÁ ERROR
            if (empleado == null)
            {
                return NotFound();
            }

            // SI EL ID DEL AVISO Y SU EMPLEADO NO COINCIDE CON EL ID DEL EMPLEADO NOS DEVOLVERÁ A LA VISTA INDEX
            if (aviso.EmpleadoId != empleado.Id)
            {
                return RedirectToAction(nameof(Index));
            }

            // DEVUELVE LA VISTA DEL AVISO SELECCIONADO Y SUS DETALLES
            return View(aviso);
        }

        // ACCIÓN GET PARA LA CREACIÓN DE LA VISTA DEL EQUIPO Y EL TIPO DE AVERÍA SELECCIONADO POR SUS RESPECTIVOS ID
        // DONDE EL USUARIO LE PASARÁ LOS DATOS
        // GET: MisAvisos/Create
        public IActionResult Create()
        {
            // ViewData["EmpleadoId"] = new SelectList(_context.Empleados, "Id", "Nombre"); 
            ViewData["EquipoId"] = new SelectList(_context.Equipos, "Id", "CodigoEquipo");
            ViewData["TipoAveriaId"] = new SelectList(_context.TipoAverias, "Id", "Descripcion");

            return View();
        }

        // ACCIÓN POST PARA LA CREACIÓN DE LA VISTA QUE DEVOLVERÁ EL SERVIDOR LOS DATOS DEL CLIENTE,
        // POSTERIORMENTE SERÁ COMPROBADO PARA FINALMENTE SI ES CORRECTO, REALIZAR LAS CONSULTAS DEMANDADAS
        // CREANDO EN EL MODELO DE DATOS EL AVISO COMPROBADO Y DEVOLVIENDO AL FINAL LA VISTA CORRESPONDIENTE DEL AVISO

        // POST: MisAvisos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> 
        Create([Bind("Id,Descripcion,FechaAviso,FechaCierre,Observaciones,EmpleadoId,TipoAveriaId,EquipoId")] Aviso aviso)
        {
            // SE ASIGNA AL AVISO EL ID DEL EMPLEADO CORRESPONDIENTE AL USUARIO ACTUAL 
            var emailUsuario = User.Identity.Name;
            var empleado = await _context.Empleados
                 .Where(e => e.Email == emailUsuario)
                 .FirstOrDefaultAsync();

            // SI ESTE NO ES NULO, SE LIGARÁ EL AVISO CON EL ID DEL EMPLEADO
            if (empleado != null)
            {
                aviso.EmpleadoId = empleado.Id;
            }

            // SI FINALMENTE EL ESTADO DEL MODELO ES VÁLIDO, SE AÑADIRÁ EL AVISO AL CONTEXTO,
            // APLICANDO LOS CAMBIOS A LA BD DE ESTE
            if (ModelState.IsValid)
            {
                _context.Add(aviso);
                await _context.SaveChangesAsync();

                // REDIRIGE AL INDEX DE MIS AVISOS
                return RedirectToAction(nameof(Index));
            }

            // GUARDA EN EL VIEW DATA LOS ELEMENTOS DE DATOS DE ESTE AVISO, PARA PASARSELO A LA CORRESPONDIENTE VISTA

            // ViewData["EmpleadoId"] = new SelectList(_context.Empleados, "Id", "Nombre", aviso.EmpleadoId);
            ViewData["EquipoId"] = new SelectList(_context.Equipos, "Id", "CodigoEquipo", aviso.EquipoId);
            ViewData["TipoAveriaId"] = new SelectList(_context.TipoAverias, "Id", "Descripcion", aviso.TipoAveriaId);

            // FINALMENTE DEVUELVE LA VISTA
            return View(aviso);
        }


        // ACCION GET QUE EDITARÁ LA TAREA EN BASE AL ID QUE RECIBA

        // GET: MisAvisos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aviso = await _context.Avisos.FindAsync(id);
            if (aviso == null)
            {
                return NotFound();
            }

            // PARA EVITAR EL ACCESO A LOS AVISOS DE OTROS EMPLEADOS 
            var emailUsuario = User.Identity.Name;
            var empleado = await _context.Empleados
                        .Where(e => e.Email == emailUsuario)
                        .FirstOrDefaultAsync();

            if (empleado == null)
            {
                return NotFound();
            }
            if (aviso.EmpleadoId != empleado.Id)
            {
                return RedirectToAction(nameof(Index));
            }

            ViewData["EmpleadoId"] = new SelectList(_context.Empleados, "Id", "Nombre", aviso.EmpleadoId);
            ViewData["EquipoId"] = new SelectList(_context.Equipos, "Id", "CodigoEquipo", aviso.EquipoId);
            ViewData["TipoAveriaId"] = new SelectList(_context.TipoAverias, "Id", "Descripcion", aviso.TipoAveriaId);
            return View(aviso);
        }

        // POST: MisAvisos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Descripcion,FechaAviso,FechaCierre,Observaciones,EmpleadoId,TipoAveriaId,EquipoId")] Aviso aviso)
        {
            if (id != aviso.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(aviso);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AvisoExists(aviso.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmpleadoId"] = new SelectList(_context.Empleados, "Id", "Nombre", aviso.EmpleadoId);
            ViewData["EquipoId"] = new SelectList(_context.Equipos, "Id", "CodigoEquipo", aviso.EquipoId);
            ViewData["TipoAveriaId"] = new SelectList(_context.TipoAverias, "Id", "Descripcion", aviso.TipoAveriaId);
            return View(aviso);
        }

        // GET: MisAvisos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aviso = await _context.Avisos
                .Include(a => a.Empleado)
                .Include(a => a.Equipo)
                .Include(a => a.TipoAveria)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (aviso == null)
            {
                return NotFound();
            }

            // PARA EVITAR EL ACCESO A LOS AVISOS DE OTROS EMPLEADOS 
            var emailUsuario = User.Identity.Name;
            var empleado = await _context.Empleados
                        .Where(e => e.Email == emailUsuario)
                        .FirstOrDefaultAsync();

            if (empleado == null)
            {
                return NotFound();
            }
            if (aviso.EmpleadoId != empleado.Id)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(aviso);
        }

        // POST: MisAvisos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var aviso = await _context.Avisos.FindAsync(id);
            if (aviso != null)
            {
                _context.Avisos.Remove(aviso);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AvisoExists(int id)
        {
            return _context.Avisos.Any(e => e.Id == id);
        }
    }
}
