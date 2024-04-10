create table Departamentos
(
	id uuid not null,
	codigo char(3) not null,
	descricao varchar(64) not null,
	
	constraint Departamentos_id_primarykey primary key (id),
	constraint Departamentos_codigo_unique unique(codigo),
	constraint Departamentos_codigo_digits check(codigo ~ '^[0-9\.]+$'),
	constraint Departamentos_codigo_notempty check(not codigo ~ '^ *$'),
	constraint Departamentos_descricao_notempty check(not descricao ~ '^ *$')
)