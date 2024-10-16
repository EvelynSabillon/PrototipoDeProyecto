
use CREL

create table CompraCon
(
	CompraID      int not null,
	ProveedorID   int not null,
	Fecha         datetime not null,
	Documento	  varchar(20) not null,
    Tipo          varchar(10) not null,
	Activo        bit not null,
	constraint pkCompraCon primary key (CompraID),
	constraint fkCompraProveedorCon foreign key (ProveedorID) references Proveedor
)

alter table CompraCon alter column Tipo varchar(10) not null

create table CompraDetalleCon
(
	CompraDetID int not null,
	CompraID    int not null,
	ArticuloID  int not null,
	Cantidad    int not null,
	Costo       float not null,
	Activo      bit not null,

	constraint pkCompraDetalleCon primary key ( CompraDetID ),
	constraint fkCompraDetalleCompraCon foreign key (CompraID) references CompraCon,
	constraint fkCompraDetalleArticuloCon foreign key (ArticuloID) references ArticuloConcentrado,
)


go
create or alter procedure spCompraConSelect @compraid int = 0
as
	select *
	from CompraCon where CompraID = @compraid or @compraid = 0
go

create or alter procedure spCompraConInsert 
@compraid int output, 
@proveedorid int, 
@fecha datetime, 
@documento varchar(20),
@tipo varchar(10),
@activo	bit
as
	select @compraid = isnull(max(CompraID), 0) + 1 from CompraCon
	insert into CompraCon
	values ( @compraid, @proveedorid, @fecha, @documento, 
			@tipo, @activo)
go



create or alter procedure spCompraConActivos @compraid int = 0
as
	select *
	from CompraCon where (CompraID = @compraid or @compraid = 0 ) and Activo = 1
go

create or alter procedure spCompraConInactivos @compraid int = 0
as
	select *
	from CompraCon where (CompraID = @compraid or @compraid = 0 ) and Activo = 0
go


create or alter procedure spCompraDetalleConSelect @compraid int = 0
as
	select *
    from CompraDetalleCon
    where CompraID = @compraid or @compraid = 0
go

create or alter procedure spCompraDetalleConInsert 
@compraid int,
@articuloid int,
@cantidad int,
@costo float,
@activo bit
as
	declare @compradetid int;
	select @compradetid = isnull(max(CompraDetID), 0) + 1 from CompraDetalleCon
	insert into CompraDetalleCon
	values (@compradetid,@compraid,@articuloid,@cantidad, @costo, @activo)
go



create or alter procedure spCompraDetalleConDesactivar @compraid int
as
	update CompraDetalleCon set Activo = 0
	from CompraDetalleCon cdc
	inner join CompraCon cc on cc.CompraID = cdc.CompraID
	where cc.CompraID = @compraid and cc.Activo = 0
go


create or alter procedure spCompraConDesactivarYDetalles @compraid int
as
begin
    -- Desactivar la compra en la tabla CompraCon
    update CompraCon
    set Activo = 0
    where CompraID = @compraid;

    -- Desactivar los detalles de la compra en la tabla CompraDetalleCon
    update CompraDetalleCon
    set Activo = 0
    where CompraID = @compraid;
end
go

create or alter procedure spCompraConArticulosActivosSelect @articuloid int = 0
as
	select * 
	from ArticuloConcentrado where (ArticuloID = @articuloid or @articuloid = 0 ) and Activo = 1
go