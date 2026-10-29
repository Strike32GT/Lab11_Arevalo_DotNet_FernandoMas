namespace Eventos.Web.Models;

public static class EstadosInscripcion
{
    public const string Confirmada = "Confirmada";
    public const string Cancelada = "Cancelada";
    public const string Patron = "^(Confirmada|Cancelada)$";
}

public static class ModalidadesEvento
{
    public const string Presencial = "Presencial";
    public const string Virtual = "Virtual";
    public const string Patron = "^(Presencial|Virtual)$";

    public static readonly string[] Todas = { Presencial, Virtual };
}
