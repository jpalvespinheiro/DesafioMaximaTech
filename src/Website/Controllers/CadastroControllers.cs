using Adm.DataModelage.Entities;
using Adm.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using Website.Models;

namespace Website.Controllers
{
    public class CadastroController : Controller
    {
        readonly HttpClient apiaccess;
        public CadastroController(IHttpClientFactory clientProvider)
        {
            apiaccess = clientProvider.CreateClient("API");
        }
        public async Task<IActionResult> Index(string? codigo)
        {
            HttpResponseMessage? response = null;
            try
            {
                response = await apiaccess.GetAsync("api/departamento_all");
                response.EnsureSuccessStatusCode();
                IReadOnlyCollection<Departamento> departamentos = JsonConvert.DeserializeObject<Departamento[]>(await response.Content.ReadAsStringAsync()!)!;
                
                if (codigo == null) return View(new CadastroModel() { Departamentos = departamentos });
                
                response = await apiaccess.GetAsync($"api/produto/{codigo}");
                response.EnsureSuccessStatusCode();
                var produto = JsonConvert.DeserializeObject<Produto>(await response.Content.ReadAsStringAsync())!;
                response = await apiaccess.GetAsync($"api/relacionar-produto-departamento/produto/{codigo}");
                string[]? departamentosAtivos = null;
                if (response.StatusCode is System.Net.HttpStatusCode.NotFound &&
                    await response.Content.ReadAsStringAsync() is string str && 
                    string.IsNullOrWhiteSpace(str) is false &&
                    str == $"\"Nao foi encontrado relacionamentos ao departamento com o codigo: {codigo}\"")
                {
                    departamentosAtivos = JsonConvert.DeserializeObject<string[]>(str);
                }

                var model = new CadastroModel() {
                    ProdutoPreExistente = produto,
                    Departamentos = departamentos,
                    DepartamentosAtivos = departamentosAtivos
                };
                return View(model);
            }
            catch (HttpRequestException) when (response?.Content is not null)
            {
                return BadRequest(await response.Content.ReadAsStringAsync());
            }
#if DEBUG
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
#else
            catch
            {
                return BadRequest("Ocorreu um erro inesperado");
            }
#endif
        }
        [HttpPost] public async Task<IActionResult> Index(string? codigoProdutoPreExistente, string? idExistente, string codigo, string descricao, string[] departamentos, float preco, bool status)
        {
            HttpResponseMessage? response = null;
            try {
                Produto produto = new()
                {
                    ID = idExistente is null or "null"
                            ? Guid.NewGuid()
                            : Guid.Parse(idExistente),
                    Codigo = codigo,
                    Descricao = descricao,
                    Preco = preco,
                    Status = status
                };
                if (produto.Validate() is string err) return BadRequest($"""
                    Houve(ram) o(s) seguinte(s) problema(s) ao atualizar o produto: {err}
                    """);
                StringContent content = new (
                    content: JsonConvert.SerializeObject(produto),
                    encoding: System.Text.Encoding.UTF8,
                    mediaType: "application/json"
                );
                if (codigoProdutoPreExistente is not null or "null")
                {
                    response = await apiaccess.PutAsync($"api/produto_update/{codigoProdutoPreExistente}", content);
                }
                else
                {
                    response = await apiaccess.PostAsync("api/produto_create", content);
                }
                response.EnsureSuccessStatusCode();

                List<string>? erros = null;
                foreach (var dep in departamentos)
                {
                    response = await apiaccess.PostAsync($"api/relacionar-produto-departamento_create?codigoproduto={produto.Codigo}&codigodepartamento={dep}", null);
                    if (response.IsSuccessStatusCode is false)
                    {
                        var erro = $"ocorrou um erro ao relacionar o produto {produto.Codigo} ao departamento {dep}, razao: {await response.Content.ReadAsStringAsync()}";
                        (erros ??= []).Add(erro);
                    }
                }

                if (erros?.Count is > 0)
                {
                    return BadRequest(erros);
                }
                return Redirect("~/Home/Index");
            }
            catch (HttpRequestException) when (response?.Content is not null)
            {
                return BadRequest(await response.Content.ReadAsStringAsync());
            }
#if DEBUG
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
#else
            catch
            {
                return BadRequest("Ocorreu um erro inesperado");
            }
#endif
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
