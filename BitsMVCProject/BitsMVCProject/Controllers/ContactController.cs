using BitsMVCProject.Models;
using BitsMVCProject.Services;
using Microsoft.AspNetCore.Mvc;

namespace BitsMVCProject.Controllers;

public class ContactController : Controller
{
    private readonly IContactService _service;

    public ContactController(IContactService service)
    {
        _service = service;
    }

    public async Task<IActionResult> Index()
    {
        var contacts = await _service.GetAllAsync();
        return View(contacts);
    }

    [HttpPost]
    public async Task<IActionResult> Import(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            TempData["Error"] = "Please select a valid CSV file.";
            return RedirectToAction("Index");
        }

        try
        {
            await _service.ImportFromCsvAsync(file);
            TempData["Success"] = "CSV file imported successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = "An error occurred while importing the CSV file.";
        }

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Edit(int id)
    {
        var contact = await _service.GetByIdAsync(id);
        if (contact == null) return NotFound();
        return View(contact);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(ContactModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var updated = await _service.UpdateAsync(model);
        if (updated == null) return NotFound();

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound();

        return RedirectToAction("Index");
    }
}

