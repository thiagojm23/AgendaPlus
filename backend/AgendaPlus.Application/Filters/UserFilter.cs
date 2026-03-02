namespace AgendaPlus.Application.Filters;

public class UserFilter
{
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? SecondName { get; set; }
    public bool? SomenteAtivos { get; set; }
    public DateTime? DataCadastroInicio { get; set; }
    public DateTime? DataCadastroFim { get; set; }
}