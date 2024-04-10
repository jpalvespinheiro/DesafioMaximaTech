create table ProdutosEmDepartamento
(
	id int GENERATED ALWAYS AS IDENTITY,
	ProdutosFK_ref_produto uuid not null,
	DepartamentosFK_ref_departamento uuid not null,
		
	constraint ProdutosEmDepartamento_id_primarykey primary key (id),
	constraint ProdutosEmDepartamento_ProdutosFKrefproduto foreign key (ProdutosFK_ref_produto) references Produtos(id),
	constraint ProdutosEmDepartamento_DepartamentosFKrefdepartamento foreign key (DepartamentosFK_ref_departamento) references Departamentos(id)
)