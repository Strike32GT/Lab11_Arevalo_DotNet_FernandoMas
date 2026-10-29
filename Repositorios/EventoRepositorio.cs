using Eventos.Web.Data;
using Eventos.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Eventos.Web.Repositorios;

public class EventoRepositorio
{
    private readonly EventosContext _context;

    public EventoRepositorio(EventosContext context)
    {
        _context = context;
    }

    public async Task<List<Evento>> ObtenerActivosAsync(int? categoriaId = null)
    {
        var consulta = _context.Eventos
            .AsNoTracking()
            .Include(e => e.Categoria)
            .Include(e => e.Inscripciones)
            .Where(e => e.Activo);

        if (categoriaId.HasValue && categoriaId.Value > 0)
        {
            consulta = consulta.Where(e => e.CategoriaId == categoriaId.Value);
        }

        return await consulta.OrderBy(e => e.Fecha).ToListAsync();
    }

    public async Task<List<Evento>> ObtenerActivosParaListaAsync()
    {
        return await _context.Eventos
            .AsNoTracking()
            .Where(e => e.Activo)
            .OrderBy(e => e.Fecha)
            .ToListAsync();
    }

    public async Task<Evento?> ObtenerPorIdAsync(int id)
    {
        return await _context.Eventos
            .AsNoTracking()
            .Include(e => e.Categoria)
            .Include(e => e.Inscripciones)
                .ThenInclude(i => i.Asistente)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Evento?> ObtenerParaEditarAsync(int id)
    {
        return await _context.Eventos
            .AsNoTracking()
            .Include(e => e.Categoria)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<List<Categoria>> ObtenerCategoriasActivasAsync()
    {
        return await _context.Categorias
            .AsNoTracking()
            .Where(c => c.Activo)
            .OrderBy(c => c.Nombre)
            .ToListAsync();
    }

    public async Task AgregarAsync(Evento evento)
    {
        _context.Eventos.Add(evento);
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(Evento evento)
    {
        var existente = await _context.Eventos.FindAsync(evento.Id);
        if (existente is null)
        {
            return;
        }

        existente.Titulo = evento.Titulo;
        existente.Fecha = evento.Fecha;
        existente.Lugar = evento.Lugar;
        existente.Aforo = evento.Aforo;
        existente.Precio = evento.Precio;
        existente.Modalidad = evento.Modalidad;
        existente.CategoriaId = evento.CategoriaId;
        existente.Activo = evento.Activo;

        await _context.SaveChangesAsync();
    }

    public async Task EliminarLogicoAsync(int id)
    {
        var evento = await _context.Eventos.FindAsync(id);
        if (evento is null)
        {
            return;
        }

        evento.Activo = false;
        await _context.SaveChangesAsync();
    }
}
