-- creacion de base de datos 'BDJWT'
create database BDJWT

go


use BDJWT
-- creamos la tabla para el usuario
create table Usuarios(
	IdUsuario int primary key identity,
	Nombres varchar(50),
	Apellidos varchar(50),
	Correo varchar(50),
	Celular float,
	Clave varchar(100),
)
-- creamos la tabla para los vehiculos
create table Vehiculos(
	IdVehiculo int primary key identity,
	Tipo varchar(50), -- correspode a automovil, motovehiculo y pick-up
	Patente varchar(10),
	Marca varchar(50),
	Modelo varchar(50),
	Anio int,
)

-- comprobamos la existencia de ambas tablas
select * from Usuarios, Vehiculos


-- insertamos datos a vehiculos
insert into Vehiculos
(
	Tipo,
	Patente,
	Marca,
	Modelo,
	Anio
) 
values 
(
	'Automovil',
	'AD098DS',
	'Ford',
	'Focus',
	2024
),
(
    'Motovehiculo',
    'ME456BC',
    'Honda',
    'CB500X',
    2022
),
(
    'Pick-up',
    'PK789EF',
    'Chevrolet',
    'S10',
    2021
)

--comprobamos la insercion a vehiculos
select * from Vehiculos

select NEWID();