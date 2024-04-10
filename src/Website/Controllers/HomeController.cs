using Adm.DataModelage.Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using Website.Models;

namespace Website.Controllers
{
    public class HomeController : Controller
    {
        readonly HttpClient apiaccess;
        public HomeController(IHttpClientFactory clientProvider)
        {
            apiaccess = clientProvider.CreateClient("API");
        }

        public async Task<IActionResult> Index()
        {
            HttpResponseMessage? response = null;
            try
            {
                response = await apiaccess.GetAsync("api/produto_all");
                if (response.StatusCode is System.Net.HttpStatusCode.NotFound &&
                    await response.Content.ReadAsStringAsync() is string content &&
                    content is "\"Nao ha produtos cadastrados\"")
                {
                    return View(Array.Empty<Produto>());
                }
                
                response.EnsureSuccessStatusCode();
                var produtos = JsonConvert.DeserializeObject<Produto[]>(await response.Content.ReadAsStringAsync())!;
                return View(produtos);
            }
            catch (HttpRequestException) when(response?.Content is not null)
            {
                return BadRequest(await response.Content.ReadAsStringAsync());
            }
#if DEBUG
            catch(Exception e)
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
        public async Task<IActionResult> Delete(string codigo)
        {
            HttpResponseMessage? response = null;
            try
            {
                response = await apiaccess.DeleteAsync($"api/relacionar-produto-departamento_delete/produto/{codigo}");
                response.EnsureSuccessStatusCode();
                response = await apiaccess.DeleteAsync($"api/produto_delete/{codigo}");
                response.EnsureSuccessStatusCode();

                return Redirect("Index");
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
