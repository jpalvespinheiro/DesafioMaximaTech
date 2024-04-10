Projeto de Cadastro de Produtos para E-commerce 

## Processo de Instala��o

## Instala��o do Execut�vel:

Fa�a o download do execut�vel fornecido e siga as instru��es de instala��o padr�o em sua m�quina.
Configura��o do Banco de Dados:

## Configura��o do Banco de Dados:

Execute o script Batch do banco de dados fornecido abaixo no seu Terminal para configurar o banco de dados necess�rio para o projeto.
Inicializa��o dos Microservi�os:

## Inicie o Servi�o Web e o Servi�o de API:

Certifique-se de que todos os requisitos do ambiente (.NET Core 8.0) estejam atendidos.
Inicie o Servi�o Web (.NET Core MVC) para a interface do usu�rio e intera��es com o sistema.

Inicie o Servi�o de API (.NET Core 8.0) para fornecer endpoints para manipula��o dos dados de produtos e departamentos.

## Teste do Sistema**:

Com os microservi�os em execu��o, teste as funcionalidades do sistema:
Utilize o ApiIntegrador.http para testar as integra��es via API.
Acesse a interface Web para realizar testes adicionais.

## Descri��o

Esse Projeto consiste na codifica��o de uma tela de cadastro de produtos(CRUD) que faz parte de um m�dulo de administra��o de um E-commerce
Al�m disso, � implementada uma API para que os clientes possam consumir os dados via integra��o.

## MicroServi�os

No sistema foi composto por: 

Servi�o Web (.NET Core MVC): Respons�vel pela interface do usu�rio e intera��es com o sistema.
Servi�o de API (.NET Core 8.0): Fornecer� endpoints para manipula��o dos dados de produtos e departamentos.

De prefer�ncia decidi escolher o uso do .NET CORE 8.0 que � uma vers�o mais atualizada.

## Banco de Dados

PostgresSQL: Neste projeto decidi ultilizar o PostgresSQL para poder armazenar os dados pedidos no projeto.
Optei por N�O utilizar o Entity Framework, preferindo assim realizar consultas com Query Expl�citas para poder demostrar a qualidade
das consultas e meu conhecimento em SQL

## Funcionalidades do Cadastro de Produtos

No projeto desenvolvi uma tela de cadastro para poder Inserir, Alterar e Excluir 
E na tela de Listagem de Produtos coloquei uma funcionalidade para poder editar e excluir

As A��es do CRUD em geral s�o realizadas na tela de Produto e nop meno Cadastro exibe um formulario para fazer seguir com o processo
de cadastramento

## Decis�es de Projeto e Escolhas de Tecnologia



## Observa��es do Projeto

Durante o desenvolvimento, foram realizadas as seguintes implementa��es e pr�ticas:

Implementa��o de uma String de erros para validar cada propriedade da fun��o, garantindo consist�ncia e integridade dos dados.

Cria��o de uma nova tabela adicional para produtos de departamentos, a fim de estabelecer uma rela��o many-to-many entre produtos e departamentos, ampliando a flexibilidade e organiza��o da estrutura de dados.

Adi��o de constraints para verificar se os valores est�o corretos, de acordo com as especifica��es descritas no c�digo fonte, assegurando a consist�ncia dos dados armazenados no banco de dados.

Dentro da Fun��o ToSQL, na classe SqlConvenction eu usei uma defesa para poder ser contra o SQL INJECTION

Essas pr�ticas foram adotadas visando garantir a qualidade, robustez e efici�ncia do sistema desenvolvido.
