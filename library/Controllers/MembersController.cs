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

    public IActionResult Create()
    {
        return View(new Member());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Member member)
    {
        if (!ModelState.IsValid)                        // a required field was left empty
        {
            return View(member);
        }

        await _member.AddAsync(member);
        return RedirectToAction(nameof(Index));
    }





}
