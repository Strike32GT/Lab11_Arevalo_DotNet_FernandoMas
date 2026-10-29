using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace Eventos.Web.Controllers;

public class AsistentesController : Controller
{
    private readonly AsistenteRepositorio _asistentes;

    public AsistentesController(AsistenteRepositorio asistentes)
    {
        _asistentes = asistentes;
    }

    public async Task<IActionResult> Index()
    {
        var asistentes = await _asistentes.ObtenerActivosAsync();
        return View(asistentes);
    }

    public async Task<IActionResult> Details(int id)
    {
        var asistente = await _asistentes.ObtenerPorIdAsync(id);
        if (asistente is null)
        {
            return NotFound();
        }

        return View(asistente);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new Asistente());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Asistente asistente)
    {
        await ValidarDuplicadosAsync(asistente);

        if (!ModelState.IsValid)
        {
            return View(asistente);
        }

        asistente.Activo = true;
        await _asistentes.AgregarAsync(asistente);
        TempData["Mensaje"] = "Asistente registrado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var asistente = await _asistentes.ObtenerPorIdAsync(id);
        if (asistente is null)
        {
            return NotFound();
        }

        return View(asistente);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Asistente asistente)
    {
        if (id != asistente.Id)
        {
            return NotFound();
        }

        await ValidarDuplicadosAsync(asistente, id);

        if (!ModelState.IsValid)
        {
            return View(asistente);
        }

        await _asistentes.ActualizarAsync(asistente);
        TempData["Mensaje"] = "Asistente actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var asistente = await _asistentes.ObtenerPorIdAsync(id);
        if (asistente is null)
        {
            return NotFound();
        }

        return View(asistente);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _asistentes.EliminarLogicoAsync(id);
        TempData["Mensaje"] = "Asistente desactivado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    private async Task ValidarDuplicadosAsync(Asistente asistente, int? excluirId = null)
    {
        if (!string.IsNullOrWhiteSpace(asistente.Dni)
            && await _asistentes.ExisteDniAsync(asistente.Dni, excluirId))
        {
            ModelState.AddModelError(nameof(Asistente.Dni), "El DNI ya está registrado.");
        }

        if (!string.IsNullOrWhiteSpace(asistente.Correo)
            && await _asistentes.ExisteCorreoAsync(asistente.Correo, excluirId))
        {
            ModelState.AddModelError(nameof(Asistente.Correo), "El correo ya está registrado.");
        }
    }
}
