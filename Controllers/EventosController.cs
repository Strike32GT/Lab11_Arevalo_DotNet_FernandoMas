using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Eventos.Web.Controllers;

public class EventosController : Controller
{
    private readonly EventoRepositorio _eventos;

    public EventosController(EventoRepositorio eventos)
    {
        _eventos = eventos;
    }

    public async Task<IActionResult> Index(int? categoriaId)
    {
        var eventos = await _eventos.ObtenerActivosAsync(categoriaId);
        await CargarCategoriasAsync(categoriaId);
        return View(eventos);
    }

    public async Task<IActionResult> Details(int id)
    {
        var evento = await _eventos.ObtenerPorIdAsync(id);
        if (evento is null)
        {
            return NotFound();
        }

        return View(evento);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await CargarCategoriasAsync();
        CargarModalidades();
        return View(new Evento { Fecha = DateTime.Today.AddDays(7), Modalidad = ModalidadesEvento.Presencial });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Evento evento)
    {
        if (!ModelState.IsValid)
        {
            await CargarCategoriasAsync(evento.CategoriaId);
            CargarModalidades(evento.Modalidad);
            return View(evento);
        }

        await _eventos.AgregarAsync(evento);
        TempData["Mensaje"] = "Evento registrado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var evento = await _eventos.ObtenerParaEditarAsync(id);
        if (evento is null)
        {
            return NotFound();
        }

        await CargarCategoriasAsync(evento.CategoriaId);
        CargarModalidades(evento.Modalidad);
        return View(evento);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Evento evento)
    {
        if (id != evento.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            await CargarCategoriasAsync(evento.CategoriaId);
            CargarModalidades(evento.Modalidad);
            return View(evento);
        }

        await _eventos.ActualizarAsync(evento);
        TempData["Mensaje"] = "Evento actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var evento = await _eventos.ObtenerPorIdAsync(id);
        if (evento is null)
        {
            return NotFound();
        }

        return View(evento);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _eventos.EliminarLogicoAsync(id);
        TempData["Mensaje"] = "Evento desactivado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    private async Task CargarCategoriasAsync(int? seleccionada = null)
    {
        var categorias = await _eventos.ObtenerCategoriasActivasAsync();
        ViewBag.Categorias = new SelectList(categorias, "Id", "Nombre", seleccionada);
    }

    private void CargarModalidades(string? seleccionada = null)
    {
        ViewBag.Modalidades = new SelectList(ModalidadesEvento.Todas, seleccionada ?? ModalidadesEvento.Presencial);
    }
}
