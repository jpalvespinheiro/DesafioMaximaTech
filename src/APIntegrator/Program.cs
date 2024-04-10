using Adm.Endpoints;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql;
namespace Adm.APIntegrator;

static class Program
{
    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var appsettings = JsonConvert.DeserializeObject<JObject>(File.ReadAllText("appsettings.json"))!;
        var dbconnection = appsettings["ConnectionStrings"]!["Database"]!;

        builder.Services.AddTransient(services => new NpgsqlConnection(dbconnection.Value<string>()));
        var app = builder.Build();
        app.UseHttpsRedirection();

        EndpointProduto.Map(app);
        EndpointDepartamento.Map(app);
        EndpointRelacionarProdutoDepartamento.Map(app);

        app.Run("https://localhost:7135");
    }
}