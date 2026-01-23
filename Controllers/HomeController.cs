using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CompanyWebMarcBravo.Models;

namespace CompanyWebMarcBravo.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
    public IActionResult Register()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    
        [HttpPost]
    public async Task<IActionResult> Register(string Username, string Password, string ConfirmPassword)
    {
        // Validación simple de contraseñas
        if (Password != ConfirmPassword)
        {
            ViewBag.Error = "Las contraseñas no coinciden.";
            return View();
        }

        // Crear y guardar el usuario
        var user = new User { Username = Username, Password = Password };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Redirigir o mostrar mensaje de éxito
        return RedirectToAction("Index");
    }
}
