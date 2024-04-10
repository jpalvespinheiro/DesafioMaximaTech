using Adm.DataModelage.Entities;
using Newtonsoft.Json;
using Npgsql;
using System.Buffers;
using static System.Net.WebRequestMethods;

namespace Adm.Endpoints;

static class EndpointDepartamento
{
    internal static void Map(WebApplication app)
    {
        app.MapGet("api/departamento_all", GetAll);
        app.MapGet("api/departamento/{codigo}", Get);
        app.MapPut("api/departamento_update/{codigo}", Update);
        app.MapPost("api/departamento_create", Post);
        app.MapDelete("api/departamento_delete/{codigo}", Delete);
    }

    static async Task<IResult> GetAll(NpgsqlConnection dbconn)
    {
        try
        {
            using (dbconn)
            {
                await dbconn.OpenAsync();
                using var cmd = dbconn.CreateCommand();
                cmd.CommandText = $"""
                    select * from Departamentos
                    """;

                List<Departamento>? departamentos = null; 

                using var fetch = await cmd.ExecuteReaderAsync();
                if (fetch.Read() is false) return Results.NotFound($"""
                        Nao ha departamentos cadastrados
                        """);

                departamentos = [];

                do
                {
                    departamentos.Add(new()
                    {
                        ID = fetch.GetGuid(0),
                        Codigo = fetch.GetString(1),
                        Descricao = fetch.GetString(2)
                    });
                } while (fetch.Read());

                dbconn.Close();

                return Results.Ok(departamentos);
            }

        }
        catch (Exception e)
        {
#if DEBUG
            return Results.BadRequest(e.Message);
#else
            return Results.BadRequest("""
                Ocorreu um erro interno ao processar a requisicao
                """);
#endif
        }
    }
    static async Task<IResult> Get(string codigo, NpgsqlConnection dbconn)
    {
        try
        {
            using (dbconn)
            {
                await dbconn.OpenAsync();
                using var cmd = dbconn.CreateCommand();
                cmd.CommandText = $"""
                    select * from Departamentos where codigo = {ToSql(codigo)}
                    """;

                using var fetch = await cmd.ExecuteReaderAsync();
                if (fetch.Read() is false) return Results.NotFound($"""
                        Nao foi encontrado um departamento com codigo: {codigo}
                        """);

                Departamento departamento = new()
                {
                    ID = fetch.GetGuid(0),
                    Codigo = fetch.GetString(1),
                    Descricao = fetch.GetString(2),
                };
                dbconn.Close();

                return Results.Ok(departamento);
            }

        }
        catch (Exception e)
        {
#if DEBUG
            return Results.BadRequest(e.Message);
#else
            return Results.BadRequest("""
                Ocorreu um erro interno ao processar a requisicao
                """);
#endif
        }
    }
    static async Task<IResult> Post(HttpContext http, NpgsqlConnection dbconn)
    {
        if (http.Request.ContentLength is not long cttsize) return Results.BadRequest("""
            O novo objeto a ser criado nao foi fornecido no conteudo da requisicao 'ContentLength = 0'
            """);
        if (http.Request.Body is null) return Results.BadRequest("""
            O novo objeto a ser criado nao foi fornecido no conteudo da requisicao 'Content = null'
            """);
        try
        {
            string content;
            using (http.Request.Body)
            using (var read = new StreamReader(http.Request.Body))
            {
                content = await read.ReadToEndAsync();
            }

            var departamento = JsonConvert.DeserializeObject<Departamento>(content)!;
            if (departamento.Validate() is string err)
            {
                return Results.BadRequest($"""
                    O departamento fornecido nao é valido! Erros detectados:{'\n'}
                    {err}
                    """);
            }

            using (dbconn)
            {
                await dbconn.OpenAsync();
                using var cmd = dbconn.CreateCommand();
                cmd.CommandText = $"""
                    insert into departamentos values (
                    /* id        */ {ToSql(departamento.ID)},
                    /* codigo    */ {ToSql(departamento.Codigo)},
                    /* descricao */ {ToSql(departamento.Descricao)}
                    );
                    """;

                var lines = await cmd.ExecuteNonQueryAsync();
                dbconn.Close();
                if (lines is not 1)
                {
#if DEBUG
                    return Results.BadRequest("""
                        O banco de dados retornou uma quantidade modificada de linhas inesperada, deveria ser '1'
                        """);
#else
                    return Results.BadRequest("""
                        Ocorreu um erro interno ao processar a requisicao
                        """);
#endif
                }

                return Results.Ok();
            }

        }
        catch (Exception e)
        {
#if DEBUG
            return Results.BadRequest(e.Message);
#else
            return Results.BadRequest("""
                Ocorreu um erro interno ao processar a requisicao
                """);
#endif
        }
    }
    static async Task<IResult> Update(string codigo, HttpContext http, NpgsqlConnection dbconn)
    {
        if (http.Request.ContentLength is not long cttsize) return Results.BadRequest("""
            O objeto a ser editado nao foi fornecido no conteudo da requisicao 'ContentLength = 0'
            """);
        if (http.Request.Body is null) return Results.BadRequest("""
            O objeto a ser editado nao foi fornecido no conteudo da requisicao 'Content = null'
            """);
        try
        {
            string content;
            using (http.Request.Body)
            using (var read = new StreamReader(http.Request.Body))
            {
                content = await read.ReadToEndAsync();
            }

            var departamento = JsonConvert.DeserializeObject<Departamento>(content)!;
            if (departamento.Validate() is string err)
            {
                return Results.BadRequest($"""
                    O departamento fornecido nao é valido! Erros detectados:{'\n'}
                    {err}
                    """);
            }

            using (dbconn)
            {
                await dbconn.OpenAsync();
                using var cmd = dbconn.CreateCommand();
                cmd.CommandText = $"""
                    update departamentos
                    set 
                        id = {ToSql(departamento.ID)},
                        codigo = {ToSql(departamento.Codigo)},
                        descricao = {ToSql(departamento.Descricao)}
                    where codigo = {ToSql(codigo)}
                    """;

                var lines = await cmd.ExecuteNonQueryAsync();
                dbconn.Close();
                
                if (lines is > 1 or < 0)
                {
#if DEBUG
                    return Results.BadRequest("""
                        O banco de dados retornou uma quantidade modificada de linhas inesperada, deveria ser '1'
                        """);
#else
                    return Results.BadRequest("""
                        Ocorreu um erro interno ao processar a requisicao
                        """);
#endif
                }
                if (lines is 0)
                {
                    return Results.NotFound($"""
                        Nao foi encontrado departamento com o codigo: {codigo}
                        """);
                }

                return Results.Ok();
            }

        }
        catch (Exception e)
        {
#if DEBUG
            return Results.BadRequest(e.Message);
#else
            return Results.BadRequest("""
                Ocorreu um erro interno ao processar a requisicao
                """);
#endif
        }
    }
    static async Task<IResult> Delete(string codigo, NpgsqlConnection dbconn)
    {
        try
        {
            using (dbconn)
            {
                await dbconn.OpenAsync();
                using var cmd = dbconn.CreateCommand();
                cmd.CommandText = $"""
                    delete from Departamentos where codigo = {ToSql(codigo)}
                    """;

                var lines = await cmd.ExecuteNonQueryAsync();
                dbconn.Close();

                if (lines is 0)
                {
                    return Results.NotFound($"""
                        Nao foi encontrado departamento com o codigo: {codigo}
                        """);
                }
                if (lines is not 1)
                {
#if DEBUG
                    return Results.BadRequest("""
                        O banco de dados retornou uma quantidade modificada de linhas inesperada, deveria ser '1'
                        """);
#else
                    return Results.BadRequest("""
                        Ocorreu um erro interno ao processar a requisicao
                        """);
#endif
                }

                return Results.Ok();
            }
        }
        catch (Exception e)
        {
#if DEBUG
            return Results.BadRequest(e.Message);
#else
            return Results.BadRequest("""
                Ocorreu um erro interno ao processar a requisicao
                """);
#endif
        }
    }
}
