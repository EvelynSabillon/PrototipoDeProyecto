use CREL
go
sp_help ArticuloConcentrado

alter table ArticuloConcentrado add constraint ukNombreArticuloConcentrado unique (Nombre)

go
create table ArticuloConcentrado 
(
	ArticuloID int not null,
	Nombre     varchar(50) not null,
	Codigo     varchar(50) null,
	Precio	   float not null,
	Entrada    int not null,
	Salida     int not null,
	Existencia int not null,
	Activo     bit not null,
	Costo      float not null,
	constraint pkArticuloCon primary key (ArticuloID)
)
go

create or alter procedure spConcentradoSelect @articuloid int = 0
as
	select ArticuloID, Nombre, Codigo, Precio, Costo, Entrada, Salida, Existencia, Activo
	from ArticuloConcentrado where (ArticuloID = @articuloid or @articuloid = 0)
go

create or alter procedure spConcentradoActivos @articuloid int = 0
as
	select ArticuloID, Nombre, Codigo, Precio, Costo, Entrada, Salida, Existencia, Activo
	from ArticuloConcentrado where (ArticuloID = @articuloid or @articuloid = 0) and Activo = 1
go

create or alter procedure spConcentradoInsert 
@nombre varchar(50),
@codigo varchar(50),
@precio float,
@entrada int,
@salida int,
@existencia int,
@activo	  bit,
@costo float
as
	declare @articuloid int; 
	select @articuloid = isnull(max(ArticuloID),0)+1 from ArticuloConcentrado
	insert into ArticuloConcentrado
	values ( @articuloid,@nombre, @codigo, @precio, @entrada, @salida, @existencia, @activo, @costo )
go

create or alter procedure spConcentradoUpdate
@articuloid int, 
@nombre varchar(50),
@codigo varchar(50),
@precio float,
@entrada int,
@salida int,
@existencia int,
@activo	  bit,
@costo float
as
	update ArticuloConcentrado
	set Nombre = @nombre, Codigo = @codigo, Precio = @precio, Entrada = @entrada, Salida = @salida, Existencia = @existencia, Activo = @activo, Costo = @costo
	where ArticuloID = @articuloid
go


create or alter procedure spConcentradoDesactivar @articuloid int
as
	update ArticuloConcentrado set Activo = 0 where ArticuloID = @articuloid
go


create or alter procedure spConcentradoInactivos @articuloid int = 0
as
	select ArticuloID, Nombre, Codigo, Precio, Costo, Entrada, Salida, Existencia, Activo
	from ArticuloConcentrado where (ArticuloID = @articuloid or @articuloid = 0) and Activo = 0
go