﻿using System;
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
    public class EmpleadosController : Controller
    {
        private readonly MvcSoporteContexto _context;

        public EmpleadosController(MvcSoporteContexto context)
        {
            _context = context;
        }

        // GET: Empleados
        public async Task<IActionResult> Index(string strCadenaDeBusqueda, string busquedaActual, int? pageNumber)
        {
            pageNumber = (strCadenaDeBusqueda != null) ? 1 : pageNumber;
            strCadenaDeBusqueda ??= busquedaActual;

            ViewData["BusquedaActual"] = strCadenaDeBusqueda;

            var empleados = from s in _context.Empleados
                            select s;

            // CARGAR LOS EMPLEADOS
            empleados = _context.Empleados.AsQueryable();

            // ORDENAR LOS EMPLEADOS POR FECHA NACIMIENTO DE FORMA DESCENDENTE
            empleados = empleados.OrderByDescending(s => s.FechaNacimiento);

            // FILTRAR POR CORREO, HACIENDO LA BÚSQUEDA INSENSIBLE A MAYÚSCULAS/MINÚSCULAS
            if (!String.IsNullOrEmpty(strCadenaDeBusqueda))
            {
                // USAMOS TOLOWER() PARA CONVERTIR AMBOS LADOS DE LA COMPARACIÓN A MINÚSCULAS
                empleados = empleados.Where(s => s.Email.ToLower().Contains(strCadenaDeBusqueda.ToLower()));
            }

            int pageSize = 3;

            return View(await PaginatedList<Empleado>.CreateAsync(empleados.AsNoTracking(),pageNumber ?? 1, pageSize));
            //return View(await _context.Empleados.ToListAsync());
        }

        // GET: Empleados/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleado = await _context.Empleados
                .FirstOrDefaultAsync(m => m.Id == id);
            if (empleado == null)
            {
                return NotFound();
            }

            return View(empleado);
        }

        // GET: Empleados/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Empleados/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Email,Telefono,FechaNacimiento")] Empleado empleado)
        {
            if (ModelState.IsValid)
            {
                _context.Add(empleado);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(empleado);
        }

        // GET: Empleados/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado == null)
            {
                return NotFound();
            }
            return View(empleado);
        }

        // POST: Empleados/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Email,Telefono,FechaNacimiento")] Empleado empleado)
        {
            if (id != empleado.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(empleado);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmpleadoExists(empleado.Id))
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
            return View(empleado);
        }

        // GET: Empleados/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleado = await _context.Empleados
                .FirstOrDefaultAsync(m => m.Id == id);
            if (empleado == null)
            {
                return NotFound();
            }

            return View(empleado);
        }

        // POST: Empleados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado != null)
            {
                _context.Empleados.Remove(empleado);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmpleadoExists(int id)
        {
            return _context.Empleados.Any(e => e.Id == id);
        }
    }
}
