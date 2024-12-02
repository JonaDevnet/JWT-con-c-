# 🚗 Vehicle API  

API RESTful desarrollada con **ASP.NET Core 8** que permite la gestión de vehículos. Este proyecto utiliza **SQL Server** como base de datos y está diseñado para conectarse con un cliente desarrollado en **React TypeScript**.  

## ✨ Características  

- **Endpoints CRUD**:
  - Obtener lista de vehículos.
  - Crear un nuevo vehículo.
  - Actualizar un vehículo existente.
  - Eliminar un vehículo.  
- **Base de datos SQL Server** para almacenamiento confiable y eficiente.  
- Preparada para interactuar con un cliente en **React TypeScript**.  

## 🚀 Tecnologías  

- **Backend**:  
  - ASP.NET Core 8  
  - Entity Framework Core  
- **Base de Datos**:  
  - SQL Server  
- **Cliente**:  
  - React con TypeScript (proyecto relacionado)  

## 📂 Estructura del Proyecto  

📁 VehicleAPI
├── 📂 Controllers
├── 📂 Models
├── 📂 Data
├── 📂 Services
└── 📂 Migrations


## 📜 Endpoints  

| Método | Endpoint        | Descripción                      |  
|--------|-----------------|----------------------------------|  
| GET    | /api/vehicles   | Obtiene la lista de vehículos.   |  
| POST   | /api/vehicles   | Crea un nuevo vehículo.          |  
| PUT    | /api/vehicles   | Actualiza un vehículo existente. |  
| DELETE | /api/vehicles   | Elimina un vehículo.             |  

## ⚙️ Requisitos Previos  

1. **Software necesario**:  
   - [SDK de .NET 8](https://dotnet.microsoft.com/download/dotnet/8.0)  
   - [SQL Server](https://www.microsoft.com/sql-server)  
   - [Postman](https://www.postman.com/) (opcional, para pruebas de la API)  

2. **Configuración de la base de datos**:  
   - Actualiza la cadena de conexión en `appsettings.json` para que coincida con tu entorno:  
     ```json
     "ConnectionStrings": {
       "DefaultConnection": "Server=TU_SERVIDOR;Database=VehicleDB;Trusted_Connection=True;"
     }
     ```  

## 🛠️ Instrucciones de Instalación  

1. Clona este repositorio:  
```bash
   git clone https://github.com/tuusuario/vehicle-api.git  
   cd vehicle-api
```

2. Restaura los paquetes NuGet:

```bash
   dotnet restore  
```
3. Aplica las migraciones de la base de datos:

```bash
   dotnet ef database update    
```
4. Ejecuta el proyecto:

```bash
   dotnet run     
```
## 🌐 Conexión con el Cliente  

El cliente de esta API está desarrollado en **React con TypeScript** y actúa como la interfaz de usuario para gestionar vehículos.  

- **Repositorio del Cliente**:  
  [Enlace al repositorio del cliente](#) _(Reemplaza con el enlace real de tu repositorio de React TypeScript)._  

### Configuración para la Conexión  

1. **URL Base de la API**:  
   Asegúrate de configurar la URL base de esta API en tu cliente. Puedes hacerlo en un archivo de configuración o en una constante:  
```typescript
   const API_BASE_URL = "https://localhost:5001/api";
```

