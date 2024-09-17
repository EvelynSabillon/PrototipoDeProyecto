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

alter table Socio add DNI varchar(20) not null

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

create table Proveedor
(	
	ProveedorID int not null,
	Nombre      varchar(50) not null,
	Telefono    varchar(20) not null,
	Activo      bit not null,

	constraint pkProveedor primary key (ProveedorID)
)


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

