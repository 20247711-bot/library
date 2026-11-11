using Npgsql;
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

    public async Task<IActionResult> Edit(long id)
    {
        var member = await _member.GetByIdAsync(id);
        if (member == null)
        {
            return NotFound();
        }
        return View(member);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Member member)
    {
        if (!ModelState.IsValid)
        {
            return View(member);
        }

        await _member.UpdateAsync(member);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(long id)
    {
        var member = await _member.GetByIdAsync(id);
        if (member == null)
        {
            return NotFound();
        }
        return View(member);
    }

    // POST /Members/Delete/5 : the delete was confirmed.
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(long id)
    {
        try
        {
            await _member.DeleteAsync(id);
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.ForeignKeyViolation)
        {
            var member = await _member.GetByIdAsync(id);
            if (member == null)
            {
                return NotFound();
            }
            ViewBag.Error = "This member has loans on record, so the database will not delete them.";
            return View("Delete", member);
        }

        return RedirectToAction(nameof(Index));
    }







}
