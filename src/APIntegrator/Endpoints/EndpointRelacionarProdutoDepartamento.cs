using Adm.DataModelage.Entities;
using Newtonsoft.Json;
using Npgsql;

namespace Adm.Endpoints;

static class EndpointRelacionarProdutoDepartamento
{
    internal static void Map(WebApplication app)
    {
        app.MapPost("api/relacionar-produto-departamento_create", Post);
        app.MapGet("api/relacionar-produto-departamento/departamento", GetPorDepartamento);
        app.MapGet("api/relacionar-produto-departamento/produto", GetPorProduto);
        app.MapDelete("api/relacionar-produto-departamento_delete/departamento/{codigo}", DeletePorDepartamento);
        app.MapDelete("api/relacionar-produto-departamento_delete/produto/{codigo}", DeletePorProduto);
    }

    static async Task<IResult> Post(string? codigoProduto, string? codigoDepartamento, HttpContext http, NpgsqlConnection dbconn)
    {
        if (string.IsNullOrWhiteSpace(codigoProduto) || string.IsNullOrWhiteSpace(codigoDepartamento)) return Results.BadRequest("""
            Codigo do produto/departamneto nao informado! por favor forneca-o em '?codigoproduto=X&codigoDepartamento=Y'
            """);
        try
        {
            using (dbconn)
            {
                Guid produtoId, departamentoId;
                await dbconn.OpenAsync();
                var queryProdutoId = dbconn.CreateCommand();
                queryProdutoId.CommandText = $"""
                    select id from produtos
                    where codigo = {ToSql(codigoProduto)}
                    fetch first 1 rows only
                    """;

                var queryDepartamentoId = dbconn.CreateCommand();
                queryDepartamentoId.CommandText = $"""
                    select id from departamentos
                    where codigo = {ToSql(codigoDepartamento)}
                    fetch first 1 rows only
                    """;
                
                produtoId = ((Guid?) await queryProdutoId.ExecuteScalarAsync()) ?? default;
                departamentoId = ((Guid?) await queryDepartamentoId.ExecuteScalarAsync()) ?? default;

                if (produtoId == default || departamentoId == default) return Results.BadRequest($"""
                        Os ids de departamento e/ou produto nao foram encontrados
                        """);

                var insert = dbconn.CreateCommand();
                insert.CommandText = $"""
                    Insert into produtosemdepartamento values (
                    /* id                               */ default,
                    /* ProdutosFK_ref_produto           */ {ToSql(produtoId)},
                    /* DepartamentosFK_ref_departamento */ {ToSql(departamentoId)}
                    );
                    """;

                var lines = await insert.ExecuteNonQueryAsync();

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
    static async Task<IResult> GetPorDepartamento(string codigo, NpgsqlConnection dbconn)
    {
        try
        {
            using (dbconn)
            {
                await dbconn.OpenAsync();
                using var cmd = dbconn.CreateCommand();
                cmd.CommandText = $"""
                    select dep.codigo from produtosemdepartamento
                    left join produtos dep on dep.id = ProdutosFK_ref_produto
                    where ProdutosFK_ref_produto in (
                        select id from departamentos
                        where codigo = {ToSql(codigo)}
                    )
                    """;

                var values = await cmd.ExecuteReaderAsync();
                if (values.Read() is false) return Results.NotFound($"""
                        Nao foi encontrado relacionamentos aos produto com o codigo: {codigo}
                        """);
                List<string> codigos = [];
                do
                {
                    codigos.Add(values.GetString(0));
                } while (values.Read());

                dbconn.Close();

                return Results.Ok(codigos);
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
    static async Task<IResult> GetPorProduto(string codigo, NpgsqlConnection dbconn)
    {
        try
        {
            using (dbconn)
            {
                await dbconn.OpenAsync();
                using var cmd = dbconn.CreateCommand();
                cmd.CommandText = $"""
                    select dep.codigo from produtosemdepartamento
                    left join departamentos dep on dep.id = departamentosFK_ref_departamento
                    where DepartamentosFK_ref_departamento in (
                        select id from produtos
                        where codigo = {ToSql(codigo)}
                    )
                    """;

                var values = await cmd.ExecuteReaderAsync();
                if (values.Read() is false) return Results.NotFound($"""
                        Nao foi encontrado relacionamentos ao departamento com o codigo: {codigo}
                        """);
                List<string> codigos = [];
                do
                {
                    codigos.Add(values.GetString(0));
                } while (values.Read());

                dbconn.Close();

                return Results.Ok(codigos);
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
    static async Task<IResult> DeletePorDepartamento(string codigo, NpgsqlConnection dbconn)
    {
        try
        {
            using (dbconn)
            {
                await dbconn.OpenAsync();
                using var cmd = dbconn.CreateCommand();
                cmd.CommandText = $"""
                    delete from produtosemdepartamento where DepartamentosFK_ref_departamento in (
                        select id from departamentos
                        where codigo = {ToSql(codigo)}
                    )
                    """;

                var lines = await cmd.ExecuteNonQueryAsync();
                dbconn.Close();

                if (lines is 0)
                {
                    return Results.NotFound($"""
                        Nao foi encontrado relacionamentos ao departamento com o codigo: {codigo}
                        """);
                }

                return Results.Ok($"removidos: {lines}");
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
    static async Task<IResult> DeletePorProduto(string codigo, NpgsqlConnection dbconn)
    {
        try
        {
            using (dbconn)
            {
                await dbconn.OpenAsync();
                using var cmd = dbconn.CreateCommand();
                cmd.CommandText = $"""
                    delete from produtosemdepartamento where ProdutosFK_ref_produto in (
                        select id from produtos
                        where codigo = {ToSql(codigo)}
                    )
                    """;

                var lines = await cmd.ExecuteNonQueryAsync();
                dbconn.Close();

                if (lines is 0)
                {
                    return Results.NotFound($"""
                        Nao foi encontrado relacionamentos ao produto com o codigo: {codigo}
                        """);
                }

                return Results.Ok($"removidos: {lines}");
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
 