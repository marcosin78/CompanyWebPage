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
    public IActionResult Admin()
    {
        ViewBag.Departments = _context.Departments.ToList();   

        return View();
    }
    public IActionResult Register()
    {
        return View();
    }
    public IActionResult Dashboard(string filter = "general")
    {
        var usuario = _context.Users.FirstOrDefault(u => u.Username == User.Identity.Name);
        ViewBag.UserDepartmentId = usuario?.DepartmentId;
        ViewBag.SelectedDepartment = filter;

        List<Post> posts;

        if (filter == "department" && usuario?.DepartmentId != null)
        {
            posts = _context.Posts
                .Where(p => p.DepartmentId == usuario.DepartmentId)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();
        }
        else
        {
            posts = _context.Posts
                .Where(p => p.DepartmentId == null)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();
        }

        return View(posts);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    
        [HttpPost]
    public async Task<IActionResult> Register(string Username, string Password, string ConfirmPassword, string InviteCode)
    {
        if (Password != ConfirmPassword)
        {
            ViewBag.Error = "Las contraseñas no coinciden.";
            return View();
        }

        var invite = _context.InviteCodes.FirstOrDefault(i => i.Code == InviteCode);
        if (invite == null)
        {
            ViewBag.Error = "Código de invitación inválido o ya usado.";
            return View();
        }

        var user = new User
        {
            Username = Username,
            Password = Password,
            DepartmentId = invite.DepartmentId
        };
        _context.Users.Add(user);

        // Eliminar el código tras usarlo
        _context.InviteCodes.Remove(invite);

        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }
    [HttpPost]
    public async Task<IActionResult> Login(string user, string pass)
    {
        var usuario = _context.Users.FirstOrDefault(u => u.Username == user && u.Password == pass);

        if (usuario != null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user),
                new Claim(ClaimTypes.Role, usuario.Role.ToString()) // <-- Usa el rol real
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

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
    public async Task<IActionResult> Post(string Text, IFormFile Image, int? departmentId)
    {
        string imagePath = null;

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
        
    var post = new Post
    {
        Username = User.Identity.Name ?? "Anonimo",
        Text = Text,
        ImagePath = imagePath,
        CreatedAt = DateTime.Now,
        DepartmentId = departmentId // Aquí se guarda el ID del departamento
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
[HttpPost]
public IActionResult GenerateUserCode(int DepartmentId, string Code)
{
    var invite = new InviteCode
    {
        Code = Code,
        DepartmentId = DepartmentId
    };
    _context.InviteCodes.Add(invite);
    _context.SaveChanges();
    TempData["Success"] = "Código guardado correctamente.";
    return RedirectToAction("Admin");
}
}
