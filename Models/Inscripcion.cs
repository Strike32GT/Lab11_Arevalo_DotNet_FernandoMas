using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eventos.Web.Models;

public class Inscripcion
{
    public int Id { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un evento.")]
    [Display(Name = "Evento")]
    public int EventoId { get; set; }

    [ForeignKey(nameof(EventoId))]
    public Evento? Evento { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un asistente.")]
    [Display(Name = "Asistente")]
    public int AsistenteId { get; set; }

    [ForeignKey(nameof(AsistenteId))]
    public Asistente? Asistente { get; set; }

    [Display(Name = "Fecha de inscripción")]
    public DateTime FechaInscripcion { get; set; } = DateTime.Now;

    [Required(ErrorMessage = "El estado es obligatorio.")]
    [StringLength(20)]
    [RegularExpression(EstadosInscripcion.Patron, ErrorMessage = "El estado debe ser Confirmada o Cancelada.")]
    public string Estado { get; set; } = EstadosInscripcion.Confirmada;
}
