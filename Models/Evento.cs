using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Eventos.Web.Models;

public class Evento
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El título es obligatorio.")]
    [StringLength(100, ErrorMessage = "El título no puede superar los 100 caracteres.")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha es obligatoria.")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha")]
    public DateTime Fecha { get; set; }

    [Required(ErrorMessage = "El lugar es obligatorio.")]
    [StringLength(100, ErrorMessage = "El lugar no puede superar los 100 caracteres.")]
    public string Lugar { get; set; } = string.Empty;

    [Range(1, 5000, ErrorMessage = "El aforo debe estar entre 1 y 5000.")]
    public int Aforo { get; set; }

    [Range(typeof(decimal), "0", "99999999.99", ErrorMessage = "El precio no puede ser negativo.")]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Precio { get; set; }

    [Required(ErrorMessage = "La modalidad es obligatoria.")]
    [StringLength(20, ErrorMessage = "La modalidad no puede superar los 20 caracteres.")]
    [RegularExpression(ModalidadesEvento.Patron, ErrorMessage = "La modalidad debe ser Presencial o Virtual.")]
    public string Modalidad { get; set; } = ModalidadesEvento.Presencial;

    [Range(1, int.MaxValue, ErrorMessage = "Seleccione una categoría.")]
    [Display(Name = "Categoría")]
    public int CategoriaId { get; set; }

    [ForeignKey(nameof(CategoriaId))]
    public Categoria? Categoria { get; set; }

    public bool Activo { get; set; } = true;

    public ICollection<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>();

    [NotMapped]
    public int InscritosConfirmados => Inscripciones?.Count(i => i.Estado == EstadosInscripcion.Confirmada) ?? 0;

    [NotMapped]
    public int CuposDisponibles => Aforo - InscritosConfirmados;
}
