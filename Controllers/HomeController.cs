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
            var posts = _context.Posts
        .OrderByDescending(p => p.CreatedAt)
        .ToList();
        return View(posts);
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
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index");
    }
    [HttpPost]
    public async Task<IActionResult> Post(string Text, IFormFile Image)
    {
        // Aquí puedes guardar el post en la base de datos y la imagen si se sube
        // Por ahora solo redirige al dashboard

        string imagePath = null;

        //Guardar la imagen si se proporciona 
        if (Image != null && Image.Length > 0)
    {
        var fileName = Guid.NewGuid() + Path.GetExtension(Image.FileName);
        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

        using (var stream = new FileStream(path, FileMode.Create))
        {
            await Image.CopyToAsync(stream);
        }
        imagePath = "/images/" + fileName;
    }
        
    // Crear el post
    var post = new Post
    {
        Username = User.Identity.Name ?? "Anonimo",
        Text = Text,
        ImagePath = imagePath,
        CreatedAt = DateTime.Now
    };
        _context.Posts.Add(post);
    await _context.SaveChangesAsync();

        return RedirectToAction("Dashboard");
    }
    [HttpPost]
public async Task<IActionResult> DeletePost(int id)
{
    var post = await _context.Posts.FindAsync(id);
    if (post == null)
        return NotFound();

    // Solo permite borrar si el usuario es el creador
    if (User.Identity?.Name != post.Username)
        return Forbid();

    _context.Posts.Remove(post);
    await _context.SaveChangesAsync();
    return RedirectToAction("Dashboard");
}

}
