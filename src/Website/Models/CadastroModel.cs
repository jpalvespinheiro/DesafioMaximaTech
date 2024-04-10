using Adm.DataModelage.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Adm.Models;

public sealed class CadastroModel
{
    public Produto? ProdutoPreExistente { get; init; }
    public required IReadOnlyCollection<Departamento> Departamentos { get; init; }
    public string[]? DepartamentosAtivos;
}
