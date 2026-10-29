using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Eventos.Web.Controllers;

public class InscripcionesController : Controller
{
    private readonly InscripcionRepositorio _inscripciones;
    private readonly EventoRepositorio _eventos;
    private readonly AsistenteRepositorio _asistentes;

    public InscripcionesController(
        InscripcionRepositorio inscripciones,
        EventoRepositorio eventos,
        AsistenteRepositorio asistentes)
    {
        _inscripciones = inscripciones;
        _eventos = eventos;
        _asistentes = asistentes;
    }

    public async Task<IActionResult> Index()
    {
        var inscripciones = await _inscripciones.ObtenerTodasAsync();
        return View(inscripciones);
    }

    public async Task<IActionResult> Details(int id)
    {
        var inscripcion = await _inscripciones.ObtenerPorIdAsync(id);
        if (inscripcion is null)
        {
            return NotFound();
        }

        return View(inscripcion);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await CargarListasAsync();
        return View(new Inscripcion());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Inscripcion inscripcion)
    {
        inscripcion.Estado = EstadosInscripcion.Confirmada;
        await ValidarInscripcionAsync(inscripcion);

        if (!ModelState.IsValid)
        {
            await CargarListasAsync(inscripcion.EventoId, inscripcion.AsistenteId);
            return View(inscripcion);
        }

        inscripcion.FechaInscripcion = DateTime.Now;
        await _inscripciones.AgregarAsync(inscripcion);
        TempData["Mensaje"] = "Inscripción registrada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var inscripcion = await _inscripciones.ObtenerPorIdAsync(id);
        if (inscripcion is null)
        {
            return NotFound();
        }

        await CargarListasAsync(inscripcion.EventoId, inscripcion.AsistenteId, inscripcion.Estado);
        return View(inscripcion);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Inscripcion inscripcion)
    {
        if (id != inscripcion.Id)
        {
            return NotFound();
        }

        var existente = await _inscripciones.ObtenerPorIdAsync(id);
        if (existente is null)
        {
            return NotFound();
        }

        await ValidarInscripcionAsync(inscripcion, id);

        if (!ModelState.IsValid)
        {
            await CargarListasAsync(inscripcion.EventoId, inscripcion.AsistenteId, inscripcion.Estado);
            return View(inscripcion);
        }

        await _inscripciones.ActualizarAsync(inscripcion);
        TempData["Mensaje"] = "Inscripción actualizada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancelar(int id)
    {
        await _inscripciones.CancelarAsync(id);
        TempData["Mensaje"] = "Inscripción cancelada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var inscripcion = await _inscripciones.ObtenerPorIdAsync(id);
        if (inscripcion is null)
        {
            return NotFound();
        }

        return View(inscripcion);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _inscripciones.EliminarAsync(id);
        TempData["Mensaje"] = "Inscripción eliminada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    private async Task ValidarInscripcionAsync(Inscripcion inscripcion, int? excluirId = null)
    {
        var evento = await _eventos.ObtenerPorIdAsync(inscripcion.EventoId);
        var asistente = await _asistentes.ObtenerPorIdAsync(inscripcion.AsistenteId);

        if (evento is null || !evento.Activo)
        {
            ModelState.AddModelError(nameof(Inscripcion.EventoId), "Seleccione un evento activo.");
        }

        if (asistente is null || !asistente.Activo)
        {
            ModelState.AddModelError(nameof(Inscripcion.AsistenteId), "Seleccione un asistente activo.");
        }

        if (evento is null || inscripcion.Estado != EstadosInscripcion.Confirmada)
        {
            return;
        }

        if (evento.Fecha.Date < DateTime.Today)
        {
            ModelState.AddModelError(nameof(Inscripcion.EventoId), "No se puede inscribir a un evento que ya pasó.");
        }

        if (asistente is not null
            && await _inscripciones.ExisteConfirmadaAsync(evento.Id, asistente.Id, excluirId))
        {
            ModelState.AddModelError(nameof(Inscripcion.AsistenteId), "El asistente ya está inscrito en este evento.");
        }

        var confirmadas = await _inscripciones.ContarConfirmadasAsync(evento.Id, excluirId);
        if (confirmadas >= evento.Aforo)
        {
            ModelState.AddModelError(nameof(Inscripcion.EventoId), "El evento ya no tiene cupos disponibles.");
        }
    }

    private async Task CargarListasAsync(int? eventoId = null, int? asistenteId = null, string? estado = null)
    {
        var eventos = await _eventos.ObtenerActivosParaListaAsync();
        var asistentes = await _asistentes.ObtenerActivosAsync();
        ViewBag.Eventos = new SelectList(eventos, "Id", "Titulo", eventoId);
        ViewBag.Asistentes = new SelectList(asistentes, "Id", "Nombres", asistenteId);
        ViewBag.Estados = new SelectList(
            new[] { EstadosInscripcion.Confirmada, EstadosInscripcion.Cancelada },
            estado ?? EstadosInscripcion.Confirmada);
    }
}
