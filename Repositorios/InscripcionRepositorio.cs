using Eventos.Web.Data;
using Eventos.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Eventos.Web.Repositorios;

public class InscripcionRepositorio
{
    private readonly EventosContext _context;

    public InscripcionRepositorio(EventosContext context)
    {
        _context = context;
    }

    public async Task<List<Inscripcion>> ObtenerTodasAsync()
    {
        return await _context.Inscripciones
            .AsNoTracking()
            .Include(i => i.Evento)
            .Include(i => i.Asistente)
            .OrderByDescending(i => i.FechaInscripcion)
            .ToListAsync();
    }

    public async Task<Inscripcion?> ObtenerPorIdAsync(int id)
    {
        return await _context.Inscripciones
            .AsNoTracking()
            .Include(i => i.Evento)
            .Include(i => i.Asistente)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<bool> ExisteConfirmadaAsync(int eventoId, int asistenteId, int? excluirId = null)
    {
        return await _context.Inscripciones
            .AnyAsync(i => i.EventoId == eventoId
                        && i.AsistenteId == asistenteId
                        && i.Estado == EstadosInscripcion.Confirmada
                        && (!excluirId.HasValue || i.Id != excluirId.Value));
    }

    public async Task<int> ContarConfirmadasAsync(int eventoId, int? excluirId = null)
    {
        return await _context.Inscripciones
            .CountAsync(i => i.EventoId == eventoId
                          && i.Estado == EstadosInscripcion.Confirmada
                          && (!excluirId.HasValue || i.Id != excluirId.Value));
    }

    public async Task AgregarAsync(Inscripcion inscripcion)
    {
        _context.Inscripciones.Add(inscripcion);
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(Inscripcion inscripcion)
    {
        var existente = await _context.Inscripciones.FindAsync(inscripcion.Id);
        if (existente is null)
        {
            return;
        }

        existente.EventoId = inscripcion.EventoId;
        existente.AsistenteId = inscripcion.AsistenteId;
        existente.Estado = inscripcion.Estado;

        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(int id)
    {
        var inscripcion = await _context.Inscripciones.FindAsync(id);
        if (inscripcion is null)
        {
            return;
        }

        _context.Inscripciones.Remove(inscripcion);
        await _context.SaveChangesAsync();
    }

    public async Task CancelarAsync(int id)
    {
        var inscripcion = await _context.Inscripciones.FindAsync(id);
        if (inscripcion is null || inscripcion.Estado != EstadosInscripcion.Confirmada)
        {
            return;
        }

        inscripcion.Estado = EstadosInscripcion.Cancelada;
        await _context.SaveChangesAsync();
    }
}
