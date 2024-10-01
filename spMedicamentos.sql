use CREL
go
sp_help ArticuloMedicamentos

alter table ArticuloMedicamentos add constraint ukNombreArticuloMedicamentos unique (Nombre)
go
create table ArticuloMedicamentos 
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
	constraint pkArticuloMed primary key (ArticuloID)
)
go

create or alter procedure spMedicamentosSelect @articuloid int = 0
as
	select ArticuloID, Nombre, Codigo, Precio, Costo, Entrada, Salida, Existencia, Activo
	from ArticuloMedicamentos where (ArticuloID = @articuloid or @articuloid = 0)
go

create or alter procedure spMedicamentosActivos @articuloid int = 0
as
	select ArticuloID, Nombre, Codigo, Precio, Costo, Entrada, Salida, Existencia, Activo
	from ArticuloMedicamentos where (ArticuloID = @articuloid or @articuloid = 0) and Activo = 1
go

create or alter procedure spMedicamentosInsert  
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
	select @articuloid = isnull(max(ArticuloID),0)+1 from ArticuloMedicamentos
	insert into ArticuloMedicamentos
	values ( @articuloid,@nombre, @codigo, @precio, @entrada, @salida, @existencia, @activo, @costo )
go

create or alter procedure spMedicamentosUpdate
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
	update ArticuloMedicamentos
	set Nombre = @nombre, Codigo = @codigo, Precio = @precio, Entrada = @entrada, Salida = @salida, Existencia = @existencia, Activo = @activo, Costo = @costo
	where ArticuloID = @articuloid
go


create or alter procedure spMedicamentosDesactivar @articuloid int
as
	update ArticuloMedicamentos set Activo = 0 where ArticuloID = @articuloid
go


create or alter procedure spMedicamentosInactivos @articuloid int = 0
as
	select ArticuloID, Nombre, Codigo, Precio, Costo, Entrada, Salida, Existencia, Activo
	from ArticuloMedicamentos where (ArticuloID = @articuloid or @articuloid = 0) and Activo = 0
go