use CREL

create table Socio
(
	SocioID int not null,
	Nombre     varchar(50) not null,
	Direccion  varchar(100) not null,
	Telefono   varchar(20) not null,
	Activo     bit not null,

	constraint pkSocio primary key (SocioID),
)

alter table Socio drop column DNI
alter table Socio add constraint ukTelefono unique (Telefono)

create table Proveedor
(	
	ProveedorID int not null,
	Nombre      varchar(50) not null,
	Telefono    varchar(20) not null,
	Activo      bit not null,

	constraint pkProveedor primary key (ProveedorID)
)

create table ArticuloMedicamentos 
(
	ArticuloID int not null,
	Nombre     varchar(50) not null,
	Codigo      varchar(50) null,
	Precio	   float not null,
	Entrada    int not null,
	Salida     int not null,
	Existencia int not null,
	Activo     bit not null,
	Costo      float not null,
	constraint pkArticuloMed primary key (ArticuloID)
)

alter table ArticuloMedicamentos add Costo float not null

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



alter table ArticuloConcentrado add Costo float not null




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


create table CompraCon
(
	CompraID      int not null,
	ProveedorID   int not null,
	Fecha         datetime not null,
	Documento	  varchar(20) not null,
    Tipo          varchar(1) not null,
	Activo        bit not null,
	constraint pkCompraCon primary key (CompraID),
	constraint fkCompraProveedorCon foreign key (ProveedorID) references Proveedor
)

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

create table Prestamo 
(
	PrestamoID      int not null,
	SocioID		    int not null,
	Fecha		    datetime not null,
	Monto		    float not null,
	CuotaQuincenal  float not null,
	MontoRestante   float null,
	PagoParcial     float null,
	Pagado			bit not null,
	Activo			bit not null
)


create table Anticipos