use CREL

create table SalidaMed
(
	SalidaID      int not null,
	SocioID   int not null,
	Fecha         datetime not null,
	Activo        bit not null,
	constraint pkSalidaMed primary key (SalidaID),
	constraint fkSalidaSocioMed foreign key (SocioID) references Socio
)

create table SalidaDetalleMed
(
	SalidaDetID int not null,
	SalidaID    int not null,
	ArticuloID  int not null,
	Cantidad    int not null,
	Precio       float not null,
	Activo      bit not null,

	constraint pkSalidaDetalleMed primary key ( SalidaDetID ),
	constraint fkSalidaDetalleSalidaMed foreign key (SalidaID) references SalidaMed,
	constraint fkSalidaDetalleArticuloMed foreign key (ArticuloID) references ArticuloMedicamentos,
)


go
create or alter procedure spSalidaMedSelect @salidaid int = 0
as
	select *
	from SalidaMed where SalidaID = @salidaid or @salidaid = 0
go

create or alter procedure spSalidaMedInsert 
@salidaid int output, 
@socioid int, 
@fecha datetime, 
@activo	bit
as
	select @salidaid = isnull(max(SalidaID), 0) + 1 from SalidaMed
	insert into SalidaMed
	values ( @salidaid, @socioid, @fecha, @activo)
go

create or alter procedure spSalidaMedActivos @salidaid int = 0
as
	select *
	from SalidaMed where (SalidaID = @salidaid or @salidaid = 0 ) and Activo = 1
go

create or alter procedure spSalidaMedInactivos @salidaid int = 0
as
	select *
	from SalidaMed where (SalidaID = @salidaid or @salidaid = 0 ) and Activo = 0
go



go
create or alter procedure spSalidaDetalleMedSelect @salidaid int = 0
as
	select *
    from SalidaDetalleMed
    where SalidaID = @salidaid or @salidaid = 0
go

create or alter procedure spSalidaDetalleMedInsert 
@salidaid int,
@articuloid int,
@cantidad int,
@precio float,
@activo bit
as
	declare @salidadetid int;
	select @salidadetid = isnull(max(SalidaDetID), 0) + 1 from SalidaDetalleMed
	insert into SalidaDetalleMed
	values (@salidadetid,@salidaid,@articuloid,@cantidad, @precio, @activo)
go

create or alter procedure spSalidaMedDesactivarYDetalles @salidaid int
as
begin
    -- Desactivar la compra en la tabla SalidaMed
    update SalidaMed
    set Activo = 0
    where SalidaID = @salidaid;

    -- Desactivar los detalles de la compra en la tabla SalidaDetalleMed
    update SalidaDetalleMed
    set Activo = 0
    where SalidaID = @salidaid;
end
go
