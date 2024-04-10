namespace Adm.DataModelage.Entities;

public sealed class Produto 
{
    /**
     * <doc>
     * <summary>Identificador unico do produto</summary>
     * </doc>
    */ public required Guid ID { get; init; }
    /**
     * <doc>
     * <summary>Codigo identificador user-friendly</summary>
     * </doc>
    */ public required string Codigo { get; init; }
    /**
     * <doc>
     * <summary>Descricao do produto</summary>
     * </doc>
    */ public required string Descricao { get; init; }
    /**
     * <doc>
     * <summary>Descricao do produto</summary>
     * </doc>
    */ public float Preco { get; init; }
    /**
     * <doc>
     * <summary>Situacao ativo/inativo do produto</summary>
     * </doc>
    */ public bool Status { get; init; }

    /**
     * <doc>
     * <summary>Valida o estado da instancia atual de <see cref="Produto"/><para>regras:</para></summary>
     * <remarks>
     * • ID nao pode ser <see langword="default"/><br/>
     * • Codigo nao pode ser um espaco vazio ou <see langword="null"/><br/>
     * • Descricao nao pode ser um espaco vazio ou <see langword="null"/><br/>
     * • Preco nao pode ser &lt; ou = a <strong>0</strong><br/>
     * </remarks>
     * <returns><see langword="null"/> se a instancia estiver OK se nao, os erros dectectados</returns>
     * </doc>
    */ public string? Validate()
    {
        string? erros = null;

        if (ID == default)
        {
            erros += "\nO identificador unico do produto nao pode ser vazio ou null";
        }
        if (string.IsNullOrWhiteSpace(Codigo))
        {
            erros += "\nO Codigo do produto nao pode ser vazio, um espaco ou null";
        }
        if (string.IsNullOrWhiteSpace(Descricao))
        {
            erros += "\nA Descricao do produto nao pode ser vazia, um espaco ou null";
        }
        if (Preco is 0)
        {
            erros += "\nO preco do produto nao pode ser 0";
        }
        if (Preco is < 0)
        {
            erros += "\nO preco do produto nao pode ser menor que 0";
        }

        return erros;
    }
}