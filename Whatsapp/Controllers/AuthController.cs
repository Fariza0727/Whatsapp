using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Whatsapp.Data;
using static Whatsapp.Data.MainModels;
using static Whatsapp.Models.AuthViewModels;

namespace Whatsapp.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly MainContext _context;
        public AuthController(MainContext context, UserManager<Usuario> userManager, SignInManager<Usuario> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        public async Task<IActionResult> Admin()
        {
            var user = new Usuario { Gerente = "admin@admin.com", Email = "admin@admin.com", Nome = "Admin", UserName = "admin@admin.com", Cargo = "Administrador", Imagem = "/app-assets/images/portrait/small/avatar-s-11.jpg" };
            var result = await _userManager.CreateAsync(user, "admin");
            if (result.Succeeded)
            {
                var u = _context.Users.First(p => p.Email == user.Email);
                var n = new Chatbot
                {
                    Usuario = u.Email
                };
                _context.Chatbots.Add(n);
                _context.SaveChanges();
            }
            return RedirectToAction("Login");
        }
        [AllowAnonymous]
        public async Task<IActionResult> Admin2()
        {
            var user = new Usuario { Gerente = "admin2@admin.com", Email = "admin2@admin.com", Nome = "Admin2", UserName = "admin2@admin.com", Cargo = "Administrador", Imagem = "/app-assets/images/portrait/small/avatar-s-11.jpg" };
            var result = await _userManager.CreateAsync(user, "admin");
            if (result.Succeeded)
            {
                var u = _context.Users.First(p => p.Email == user.Email);
                var n = new Chatbot
                {
                    Usuario = u.Email
                };
                _context.Chatbots.Add(n);
                _context.SaveChanges();
            }

            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Usuário ou senha incorretos.");
                    return View();
                }
            }
            return RedirectToAction("Index", "Dashboard");
        }
    }
}
