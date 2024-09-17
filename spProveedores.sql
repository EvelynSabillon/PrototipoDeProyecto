use CREL
sp_help proveedor

alter table Proveedor add constraint ukNombreProveedor unique (Nombre)

create table Proveedor
(	
	ProveedorID int not null,
	Nombre      varchar(50) not null,
	Telefono    varchar(20) not null,
	Activo      bit not null,

	constraint pkProveedor primary key (ProveedorID)
)

alter table Proveedor add Direccion varchar(100) null

go
create or alter procedure spProveedorSelect @proveedorid int = 0
as
	select ProveedorID, Nombre, Direccion, Telefono, Activo
	from Proveedor where (ProveedorID = @proveedorid or @proveedorid = 0)
go

create or alter procedure spProveedorActivos @proveedorid int = 0
as
	select ProveedorID, Nombre, Direccion, Telefono, Activo
	from Proveedor where (ProveedorID = @proveedorid or @proveedorid = 0) and Activo = 1
go

create or alter procedure spProveedorInsert
@nombre varchar(50),
@telefono varchar(20),
@activo	  bit,
@direccion varchar(100)
as
	declare @proveedorid int;
	select @proveedorid = isnull(max(ProveedorID),0)+1 from Proveedor
	insert into Proveedor
	values ( @proveedorid, @nombre, @telefono, @activo, @direccion)
go

create or alter procedure spProveedorUpdate
@proveedorid int, 
@nombre varchar(50),
@telefono varchar(20),
@activo	  bit,
@direccion varchar(100)
as
	update Proveedor 
	set Nombre = @nombre, Telefono = @telefono, Activo = @activo, Direccion = @direccion
	where ProveedorID = @proveedorid
go


create or alter procedure spProveedorDesactivar @proveedorid int
as
	update Proveedor set Activo = 0 where ProveedorID = @proveedorid
go


create or alter procedure spProveedorInactivos @proveedorid int = 0
as
	select ProveedorID, Nombre, Direccion, Telefono, Activo
	from Proveedor where (ProveedorID = @proveedorid or @proveedorid = 0) and Activo = 0
go

