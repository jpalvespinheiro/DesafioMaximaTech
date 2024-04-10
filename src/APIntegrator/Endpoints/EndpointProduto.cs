using Adm.DataModelage.Entities;
using Newtonsoft.Json;
using Npgsql;

namespace Adm.Endpoints;

static class EndpointProduto
{
    internal static void Map(WebApplication app)
    {
        app.MapGet("api/produto_all", GetAll);
        app.MapGet("api/produto/{codigo}", Get);
        app.MapPut("api/produto_update/{codigo}", Update);
        app.MapPost("api/produto_create", Post);
        app.MapDelete("api/produto_delete/{codigo}", Delete);
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
                    select * from Produtos
                    """;

                List<Produto>? produtos = null;

                using var fetch = await cmd.ExecuteReaderAsync();
                if (fetch.Read() is false) return Results.NotFound($"""
                        Nao ha produtos cadastrados
                        """);

                produtos = [];

                do
                {
                    produtos.Add(new()
                    {
                        ID = fetch.GetGuid(0),
                        Codigo = fetch.GetString(1),
                        Descricao = fetch.GetString(2),
                        Preco = fetch.GetFloat(3),
                        Status = fetch.GetBoolean(4)
                    });
                } while (fetch.Read());

                dbconn.Close();

                return Results.Ok(produtos);
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
                    select * from Produtos where codigo = {ToSql(codigo)}
                    """;

                using var fetch = await cmd.ExecuteReaderAsync();
                if (fetch.Read() is false) return Results.NotFound($"""
                        Nao foi encontrado um produto com codigo: {codigo}
                        """);

                var produto = new Produto
                {
                    ID = fetch.GetGuid(0),
                    Codigo = fetch.GetString(1),
                    Descricao = fetch.GetString(2),
                    Preco = fetch.GetFloat(3),
                    Status = fetch.GetBoolean(4)
                };
                dbconn.Close();

                return Results.Ok(produto);
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

            var produto = JsonConvert.DeserializeObject<Produto>(content)!;
            if (produto.Validate() is string err)
            {
                return Results.BadRequest($"""
                    O produto fornecido nao é valido! Erros detectados:{'\n'}
                    {err}
                    """);
            }

            using (dbconn)
            {
                await dbconn.OpenAsync();
                using var cmd = dbconn.CreateCommand();
                cmd.CommandText = $"""
                    insert into produtos values (
                    /* id        */ {ToSql(produto.ID)},
                    /* codigo    */ {ToSql(produto.Codigo)},
                    /* descricao */ {ToSql(produto.Descricao)},
                    /* preco     */ {ToSql(produto.Preco)},
                    /* status    */ {ToSql(produto.Status)}
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

            var produto = JsonConvert.DeserializeObject<Produto>(content)!;
            if (produto.Validate() is string err)
            {
                return Results.BadRequest($"""
                    O produto fornecido nao é valido! Erros detectados:{'\n'}
                    {err}
                    """);
            }

            using (dbconn)
            {
                await dbconn.OpenAsync();
                using var cmd = dbconn.CreateCommand();
                cmd.CommandText = $"""
                    update produtos
                    set 
                        id = {ToSql(produto.ID)},
                        codigo = {ToSql(produto.Codigo)},
                        descricao = {ToSql(produto.Descricao)},
                        preco = {ToSql(produto.Preco)},
                        status = {ToSql(produto.Status)}
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
                        Nao foi encontrado produto com o codigo: {codigo}
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
                    delete from produtos where codigo = {ToSql(codigo)}
                    """;

                var lines = await cmd.ExecuteNonQueryAsync();
                dbconn.Close();

                if (lines is 0)
                {
                    return Results.NotFound($"""
                        Nao foi encontrado produto com o codigo: {codigo}
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
