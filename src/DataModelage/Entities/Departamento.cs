namespace Adm.DataModelage.Entities;

public sealed class Departamento
{
    /**
     * <doc>
     * <summary>Identificador unico do departamento</summary>
     * </doc>
    */ public required Guid ID { get; init; }
    /**
     * <doc><summary>Codigo user-friendly do departamento</summary></doc>
    */ public required string Codigo { get; init; }
    /**
     * <doc>
     * <summary>Descricao do departamento</summary>
     * </doc>
    */ public required string Descricao { get; init; }

    /**
     * <doc>
     * <summary>Valida o estado da instancia atual de <see cref="Produto"/><para>regras:</para></summary>
     * <remarks>
     * • ID nao pode ser <see langword="default"/><br/>
     * • Descricao nao pode ser um espaco vazio ou <see langword="null"/><br/>
     * • UserCode nao pode ser '000' e deve conter somente digitos<br/>
     * </remarks>
     * <returns><see langword="null"/> se a instancia estiver OK se nao, os erros dectectados</returns>
     * </doc>
    */ public string? Validate()
    {
        string? erros = null;

        if (ID == default)
        {
            erros += "\nO identificador unico do departamento nao pode ser vazio ou null";
        }
        if (string.IsNullOrWhiteSpace(Descricao))
        {
            erros += "\nA Descricao do departamento nao pode ser vazia, um espaco ou null";
        }
        if (
            Codigo is "000" or { Length: not 3 } ||
            char.IsDigit(Codigo[0]) is false ||
            char.IsDigit(Codigo[1]) is false ||
            char.IsDigit(Codigo[2]) is false
        ) {
            erros += "\nO codigo do usuario deve conter apenas digitos e nao poderá ser 000";
        }

        return erros;
    }
}