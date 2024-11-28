using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MvcSoporte.Areas.Identity.Pages.Account;
using MvcSoporte.Data;
using MvcSoporte.Models;

namespace MvcSoporte.Controllers
{
    // AUTORIZACIÓN DE USO SOLO PARA USUARIOS REGISTRADOS COMO ADMINISTRADORES
    [Authorize(Roles = "Administrador")]
    public class UsuariosController : Controller
    {
        // PASAMOS COMO PROPIEDADES DE CONTROLADOR DE USUARIOS EL CONTEXTO DE LA BD QUE MANEJA
        // EL SISTEMA DE AUTENTIFICACIÓN, REGISTRO E INICIO DE USUARIOS
        private readonly ApplicationDbContext _context;

        // AQUI HACE USO DE UNA API DE MANEJO DE USUARIOS, DONDE LE PASARÁ LA IDENTIDAD ACTUAL DEL USUARIO
        private readonly UserManager<IdentityUser> _userManager;

        public UsuariosController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            var usuarios = from user in _context.Users
                           join userRoles in _context.UserRoles on user.Id equals userRoles.UserId
                           join role in _context.Roles on userRoles.RoleId equals role.Id
                           select new ViewUsuarioConRol
                           {
                               Email = user.Email,
                               NombreUsuario = user.UserName,
                               RolDeUsuario = role.Name
                           };

            return View(usuarios.ToList());
        }

        // GET: Usuarios/Create 
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Email,Password")] RegisterModel.InputModel model)
        {
            // SE CREA EL NUEVO USUARIO 
            var user = new IdentityUser();
            user.UserName = model.Email;
            user.Email = model.Email;
            string usuarioPWD = model.Password;
            var result = await _userManager.CreateAsync(user, usuarioPWD);

            // SE ASIGNA EL ROL DE "ADMINISTRADOR" AL USUARIO 
            if (result.Succeeded)
            {
                var result1 = await _userManager.AddToRoleAsync(user, "Administrador");
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }
    }
}