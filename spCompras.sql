

create table CompraMed
(
	CompraID      int not null,
	ProveedorID   int not null,
	Fecha         datetime not null,
	Documento	  varchar(20) not null,
    Tipo          varchar(1) not null,
	Activo        bit not null,
	constraint pkCompraMed primary key (CompraID),
	constraint fkCompraProveedorMed foreign key (ProveedorID) references Proveedor
)

create table CompraDetalleMed
(
	CompraDetID int not null,
	CompraID    int not null,
	ArticuloID  int not null,
	Cantidad    int not null,
	Costo       float not null,
	Activo      bit not null,

	constraint pkCompraDetalleMed primary key ( CompraDetID ),
	constraint fkCompraDetalleCompraMed foreign key (CompraID) references CompraMed,
	constraint fkCompraDetalleArticuloMed foreign key (ArticuloID) references ArticuloMedicamentos,
)

sp_help CompraMed

go
create or alter procedure spCompraMedSelect @compraid int = 0
as
	select *
	from CompraMed where CompraID = @compraid or @compraid = 0
go

create or alter procedure spCompraMedInsert 
@compraid int output, 
@proveedorid int, 
@fecha datetime, 
@documento varchar(20),
@tipo varchar(1),
@activo	bit
as
	select @compraid = isnull(max(CompraID), 0) + 1 from CompraMed
	insert into CompraMed
	values ( @compraid, @proveedorid, @fecha, @documento, 
			@tipo, @activo)
go

create or alter procedure spCompraMedActivos @compraid int = 0
as
	select *
	from CompraMed where (CompraID = @compraid or @compraid = 0 ) and Activo = 1
go

create or alter procedure spCompraMedInactivos @compraid int = 0
as
	select *
	from CompraMed where (CompraID = @compraid or @compraid = 0 ) and Activo = 0
go

go
create or alter procedure spCompraDetalleMedSelect @compraid int = 0
as
	select *
    from CompraDetalleMed
    where CompraID = @compraid or @compraid = 0
go

create or alter procedure spCompraDetalleMedInsert 
@compradetid int,
@compraid int,
@articuloid int,
@cantidad int,
@costo float,
@activo bit
as
	select @compradetid = isnull(max(CompraDetID), 0) + 1 from CompraDetalleMed
	insert into CompraDetalleMed
	values (@compradetid,@compraid,@articuloid,@cantidad, @costo, @activo)
go

create or alter procedure spCompraMedDesactivarYDetalles @compraid int
as
begin
    -- Desactivar la compra en la tabla CompraMed
    update CompraMed
    set Activo = 0
    where CompraID = @compraid;

    -- Desactivar los detalles de la compra en la tabla CompraDetalleMed
    update CompraDetalleMed
    set Activo = 0
    where CompraID = @compraid;
end
go

