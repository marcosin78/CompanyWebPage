using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using CompanyWebMarcBravo.Models;
using System.Security.Claims;

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

            if (User.Identity != null && User.Identity.IsAuthenticated)
                {
                return RedirectToAction("Dashboard");
                }

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
    public IActionResult Dashboard()
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
    [HttpPost]
    public async Task<IActionResult> Login(string user, string pass)
    {
        //Reemplazar con la logica de acceso a la base de datos para validar el usuario
        bool isValid=false;

        isValid = _context.Users.Any(u => u.Username == user && u.Password == pass);

        Console.WriteLine($"isValid: {isValid}");

        if (isValid)
        {

            //Crear claims

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user)
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            //Guardar cookie

              // Guardar la cookie
            await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Dashboard");
            
        }
        else
        {
            ViewBag.LoginError = "Usuario o contraseña incorrectos.";

            return View("Index");
        }

    }
}
