using library.Data;
using library.Models;
using Microsoft.AspNetCore.Mvc;

namespace library.Controllers;

public class MembersController : Controller
{
    private readonly MemberRepository _member;

    // ASP.NET Core hands in the repository, because Program.cs registered it.
    public MembersController(MemberRepository member)
    {
        _member = member;
    }

    // GET /Books : the list of books.
    public async Task<IActionResult> Index()
    {
        var member = await _member.GetAllAsync();

        return View(member);                             // Views/Books/Index.cshtml
    }

    

}
