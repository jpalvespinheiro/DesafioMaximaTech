Projeto de Cadastro de Produtos para E-commerce 

## Processo de Instalação

## Instalação do Executável:

Faça o download do executável fornecido e siga as instruções de instalação padrão em sua máquina.
Configuração do Banco de Dados:

## Configuração do Banco de Dados:

Execute o script Batch do banco de dados fornecido abaixo no seu Terminal para configurar o banco de dados necessário para o projeto.
Inicialização dos Microserviços:

## Inicie o Serviço Web e o Serviço de API:

Certifique-se de que todos os requisitos do ambiente (.NET Core 8.0) estejam atendidos.
Inicie o Serviço Web (.NET Core MVC) para a interface do usuário e interações com o sistema.

Inicie o Serviço de API (.NET Core 8.0) para fornecer endpoints para manipulação dos dados de produtos e departamentos.

## Teste do Sistema**:

Com os microserviços em execução, teste as funcionalidades do sistema:
Utilize o ApiIntegrador.http para testar as integrações via API.
Acesse a interface Web para realizar testes adicionais.

## Descrição

Esse Projeto consiste na codificação de uma tela de cadastro de produtos(CRUD) que faz parte de um módulo de administração de um E-commerce
Além disso, é implementada uma API para que os clientes possam consumir os dados via integração.

## MicroServiços

No sistema foi composto por: 

Serviço Web (.NET Core MVC): Responsável pela interface do usuário e interações com o sistema.
Serviço de API (.NET Core 8.0): Fornecerá endpoints para manipulação dos dados de produtos e departamentos.

De preferência decidi escolher o uso do .NET CORE 8.0 que é uma versão mais atualizada.

## Banco de Dados

PostgresSQL: Neste projeto decidi ultilizar o PostgresSQL para poder armazenar os dados pedidos no projeto.
Optei por NÃO utilizar o Entity Framework, preferindo assim realizar consultas com Query Explícitas para poder demostrar a qualidade
das consultas e meu conhecimento em SQL

## Funcionalidades do Cadastro de Produtos

No projeto desenvolvi uma tela de cadastro para poder Inserir, Alterar e Excluir 
E na tela de Listagem de Produtos coloquei uma funcionalidade para poder editar e excluir

As Ações do CRUD em geral são realizadas na tela de Produto e nop meno Cadastro exibe um formulario para fazer seguir com o processo
de cadastramento

## Decisões de Projeto e Escolhas de Tecnologia



## Observações do Projeto

Durante o desenvolvimento, foram realizadas as seguintes implementações e práticas:

Implementação de uma String de erros para validar cada propriedade da função, garantindo consistência e integridade dos dados.

Criação de uma nova tabela adicional para produtos de departamentos, a fim de estabelecer uma relação many-to-many entre produtos e departamentos, ampliando a flexibilidade e organização da estrutura de dados.

Adição de constraints para verificar se os valores estão corretos, de acordo com as especificações descritas no código fonte, assegurando a consistência dos dados armazenados no banco de dados.

Dentro da Função ToSQL, na classe SqlConvenction eu usei uma defesa para poder ser contra o SQL INJECTION

Essas práticas foram adotadas visando garantir a qualidade, robustez e eficiência do sistema desenvolvido.
