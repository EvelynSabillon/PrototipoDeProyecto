use CREL

alter table Socio add constraint ukNombre unique (Nombre)
sp_help Socio

go
create or alter procedure spSocioSelect @socioid int = 0
as
	select SocioID, Nombre, Direccion, Telefono, Activo 
	from Socio where (SocioID = @socioid or @socioid = 0)
go

create or alter procedure spSocioActivos @socioid int = 0
as
	select SocioID, Nombre, Direccion, Telefono, Activo 
	from Socio where (SocioID = @socioid or @socioid = 0) and Activo = 1
go

create or alter procedure spSocioInsert 
@nombre varchar(50),
@direccion varchar(100),
@telefono varchar(20),
@activo	  bit
as
	declare @socioid as int;
	select @socioid = isnull(max(SocioID),0)+1 from Socio
	insert into Socio
	values ( @socioid,@nombre, @direccion, @telefono, @activo )
go

create or alter procedure spSocioUpdate
@socioid int, 
@nombre varchar(50),
@direccion varchar(100),
@telefono varchar(20),
@activo	  bit
as
	update Socio 
	set Nombre = @nombre, Direccion = @direccion, Telefono = @telefono, Activo = @activo 
	where SocioID = @socioid
go


create or alter procedure spSocioDesactivar @socioid int
as
	update Socio set Activo = 0 where SocioID = @socioid
go


create or alter procedure spSocioInactivos @socioid int = 0
as
	select SocioID, Nombre, Direccion, Telefono, Activo 
	from Socio where (SocioID = @socioid or @socioid = 0) and Activo = 0
go

