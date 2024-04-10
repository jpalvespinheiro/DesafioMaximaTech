create table Produtos
(
	id uuid not null,
	codigo varchar(16) not null,
	descricao varchar(64) not null,
	preco decimal(10, 2),
	status boolean,
	
	constraint Produtos_id_primarykey primary key (id),
	constraint Produtos_codigo_notempty check(not codigo ~ '^ *$'),
	constraint Produtos_descricao_notempty check(not descricao ~ '^ *$'),
	constraint Produtos_preco_notzeroorless check(preco > 0)
)