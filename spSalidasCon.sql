use CREL

create table SalidaCon
(
	SalidaID      int not null,
	SocioID		  int not null,
	Fecha         datetime not null,
	Activo        bit not null,
	constraint pkSalidaCon primary key (SalidaID),
	constraint fkSalidaSocioCon foreign key (SocioID) references Socio
)

create table SalidaDetalleCon
(
	SalidaDetID int not null,
	SalidaID    int not null,
	ArticuloID  int not null,
	Cantidad    int not null,
	Precio       float not null,
	Activo      bit not null,

	constraint pkSalidaDetalleCon primary key ( SalidaDetID ),
	constraint fkSalidaDetalleSalidaCon foreign key (SalidaID) references SalidaCon,
	constraint fkSalidaDetalleArticuloCon foreign key (ArticuloID) references ArticuloConcentrado,
)


go
create or alter procedure spSalidaConSelect @salidaid int = 0
as
	select *
	from SalidaCon where SalidaID = @salidaid or @salidaid = 0
go

create or alter procedure spSalidaConInsert 
@salidaid int output, 
@socioid int, 
@fecha datetime, 
@activo	bit
as
	select @salidaid = isnull(max(SalidaID), 0) + 1 from SalidaCon
	insert into SalidaCon
	values ( @salidaid, @socioid, @fecha, @activo)
go

create or alter procedure spSalidaConActivos @salidaid int = 0
as
	select *
	from SalidaCon where (SalidaID = @salidaid or @salidaid = 0 ) and Activo = 1
go

create or alter procedure spSalidaConInactivos @salidaid int = 0
as
	select *
	from SalidaCon where (SalidaID = @salidaid or @salidaid = 0 ) and Activo = 0
go



go
create or alter procedure spSalidaDetalleConSelect @salidaid int = 0
as
	select *
    from SalidaDetalleCon
    where SalidaID = @salidaid or @salidaid = 0
go

create or alter procedure spSalidaDetalleConInsert 
@salidaid int,
@articuloid int,
@cantidad int,
@precio float,
@activo bit
as
	declare @salidadetid int;
	select @salidadetid = isnull(max(SalidaDetID), 0) + 1 from SalidaDetalleCon
	insert into SalidaDetalleCon
	values (@salidadetid,@salidaid,@articuloid,@cantidad, @precio, @activo)
go

create or alter procedure spSalidaConDesactivarYDetalles @salidaid int
as
begin
    -- Desactivar la compra en la tabla SalidaCon
    update SalidaCon
    set Activo = 0
    where SalidaID = @salidaid;

    -- Desactivar los detalles de la compra en la tabla SalidaDetalleCon
    update SalidaDetalleCon
    set Activo = 0
    where SalidaID = @salidaid;
end
go