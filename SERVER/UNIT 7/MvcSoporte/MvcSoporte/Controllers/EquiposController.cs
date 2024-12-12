using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcSoporte.Data;
using MvcSoporte.Models;

namespace MvcSoporte.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class EquiposController : Controller
    {
        private readonly MvcSoporteContexto _context;

        public EquiposController(MvcSoporteContexto context)
        {
            _context = context;
        }   

        // GET: Equipos
        public async Task<IActionResult> Index(string strCadenaDeBusqueda, int? intLocalizacionesId, string busquedaActual, int? localizacionesIdActual, int? pageNumber)
        {
            //var mvcSoporteContexto = _context.Equipos.Include(e => e.Localizacion);
            //return View(await mvcSoporteContexto.ToListAsync());

            pageNumber = (strCadenaDeBusqueda != null) ? 1 : pageNumber;
            strCadenaDeBusqueda ??= busquedaActual;

            ViewData["BusquedaActual"] = strCadenaDeBusqueda;

            // CARGAR TODOS LOS TIPOS DE LOCALIZACIONES PARA EL FILTRO
            var localizaciones = _context.Localizaciones.ToList();
            ViewBag.LocalizacionId = new SelectList(localizaciones, "Id", "Descripcion", intLocalizacionesId);

            // CARGAR LOS EQUIPOS
            var equipos = _context.Equipos.AsQueryable();

            // ORDENAR LOS EQUIPOS POR FECHAAVISO DE FORMA DESCENDENTE
            equipos = equipos.OrderByDescending(s => s.FechaCompra);

            // FILTRAR POR ID, HACIENDO LA BÚSQUEDA INSENSIBLE A MAYÚSCULAS/MINÚSCULAS
            if (!String.IsNullOrEmpty(strCadenaDeBusqueda))
            {
                // USAMOS TOLOWER() PARA CONVERTIR AMBOS LADOS DE LA COMPARACIÓN A MINÚSCULAS
                equipos = equipos.Where(s => s.CodigoEquipo.ToLower().Contains(strCadenaDeBusqueda.ToLower()));
            }

            // FILTRAR POR TIPO DE LOCALIZACION SI ES NECESARIO
            if (intLocalizacionesId != null)
            {
                equipos = equipos.Where(x => x.Localizacion.Id == intLocalizacionesId);
            }

            // INCLUIR LAS RELACIONES NECESARIAS
            equipos = equipos.Include(a => a.Localizacion);


            int pageSize = 3;
            return View(await PaginatedList<Equipo>.CreateAsync(equipos.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Equipos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // CARGAR EL EQUIPO CON SUS RELACIONES
            var equipo = await _context.Equipos
                .Include(e => e.Localizacion)
                .Include(e => e.Avisos)
                .ThenInclude(a => a.Empleado)
                .Include(e => e.Avisos)
                .ThenInclude(b => b.TipoAveria)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (equipo == null)
            {
                return NotFound();
            }

            return View(equipo);
        }

        // GET: Equipos/Create
        public IActionResult Create()
        {
            ViewData["LocalizacionId"] = new SelectList(_context.Localizaciones, "Id", "Descripcion");
            return View();
        }

        // POST: Equipos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CodigoEquipo,Marca,Modelo,NumeroSerie,Caracteristicas,Precio,PrecioCadena,FechaCompra,FechaBaja,LocalizacionId")] Equipo equipo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(equipo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LocalizacionId"] = new SelectList(_context.Localizaciones, "Id", "Descripcion", equipo.LocalizacionId);
            return View(equipo);
        }

        // GET: Equipos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipo = await _context.Equipos.FindAsync(id);
            if (equipo == null)
            {
                return NotFound();
            }
            ViewData["LocalizacionId"] = new SelectList(_context.Localizaciones, "Id", "Descripcion", equipo.LocalizacionId);
            return View(equipo);
        }

        // POST: Equipos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CodigoEquipo,Marca,Modelo,NumeroSerie,Caracteristicas,Precio,PrecioCadena,FechaCompra,FechaBaja,LocalizacionId")] Equipo equipo)
        {
            if (id != equipo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(equipo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EquipoExists(equipo.Id))
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
            ViewData["LocalizacionId"] = new SelectList(_context.Localizaciones, "Id", "Descripcion", equipo.LocalizacionId);
            return View(equipo);
        }

        // GET: Equipos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipo = await _context.Equipos
                .Include(e => e.Localizacion)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (equipo == null)
            {
                return NotFound();
            }

            return View(equipo);
        }

        // POST: Equipos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var equipo = await _context.Equipos.FindAsync(id);
            if (equipo != null)
            {
                _context.Equipos.Remove(equipo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EquipoExists(int id)
        {
            return _context.Equipos.Any(e => e.Id == id);
        }
    }
}
