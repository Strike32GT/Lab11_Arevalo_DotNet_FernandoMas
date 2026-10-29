using Eventos.Web.Data;
using Eventos.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Eventos.Web.Repositorios;

public class AsistenteRepositorio
{
    private readonly EventosContext _context;

    public AsistenteRepositorio(EventosContext context)
    {
        _context = context;
    }

    public async Task<List<Asistente>> ObtenerActivosAsync()
    {
        return await _context.Asistentes
            .AsNoTracking()
            .Where(a => a.Activo)
            .OrderBy(a => a.Nombres)
            .ToListAsync();
    }

    public async Task<Asistente?> ObtenerPorIdAsync(int id)
    {
        return await _context.Asistentes
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<bool> ExisteDniAsync(string dni, int? excluirId = null)
    {
        return await _context.Asistentes
            .AnyAsync(a => a.Dni == dni && (!excluirId.HasValue || a.Id != excluirId.Value));
    }

    public async Task<bool> ExisteCorreoAsync(string correo, int? excluirId = null)
    {
        return await _context.Asistentes
            .AnyAsync(a => a.Correo == correo && (!excluirId.HasValue || a.Id != excluirId.Value));
    }

    public async Task AgregarAsync(Asistente asistente)
    {
        _context.Asistentes.Add(asistente);
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(Asistente asistente)
    {
        var existente = await _context.Asistentes.FindAsync(asistente.Id);
        if (existente is null)
        {
            return;
        }

        existente.Nombres = asistente.Nombres;
        existente.Dni = asistente.Dni;
        existente.Correo = asistente.Correo;
        existente.Telefono = asistente.Telefono;
        existente.Activo = asistente.Activo;

        await _context.SaveChangesAsync();
    }

    public async Task EliminarLogicoAsync(int id)
    {
        var asistente = await _context.Asistentes.FindAsync(id);
        if (asistente is null)
        {
            return;
        }

        asistente.Activo = false;
        await _context.SaveChangesAsync();
    }
}
