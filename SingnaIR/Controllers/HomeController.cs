﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SingnaIR.DAL;
using SingnaIR.Hubs;
using SingnaIR.Models;
using SingnaIR.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SingnaIR.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly Context _context;
        private readonly IHubContext<ChatHub> _hubContext;

        public HomeController(IHubContext<ChatHub> hubContext, ILogger<HomeController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, DAL.Context context)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _hubContext = hubContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public IActionResult Chat()
        {

            var users = _userManager.Users.ToList();
            ViewBag.Users = users;
            return View();
        }




        public async Task<IActionResult> CreateUser()
        {
            var user1 = new AppUser { FullName = "Habib", UserName = "_Habib" };
            var user2 = new AppUser { FullName = "Gunel", UserName = "_Gunel" };
            var user3 = new AppUser { FullName = "Rasul", UserName = "_Rasul" };

            await _userManager.CreateAsync(user1, "12345@Hi");
            await _userManager.CreateAsync(user2, "12345@Gt");
            await _userManager.CreateAsync(user3, "12345@Rr");

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Login()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            AppUser user = await _userManager.FindByNameAsync(loginVM.UserName);

            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, true, true);


            if (user == null) return NotFound();

            return RedirectToAction(nameof(Chat));
        }

        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public async Task ShowUserAlert(string id)
        {
            AppUser user = await _userManager.FindByIdAsync(id);
            await _hubContext.Clients.Client(user.ConnectionId).SendAsync("ShowAlert", user.Id);

        }

    }
}
