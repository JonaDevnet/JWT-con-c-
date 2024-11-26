using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AWJWT.Context;
using AWJWT.OpVehiculo;
using AWJWT.Models;
using AWJWT.DTOs;


namespace AWJWT.Controllers
{
    [Route("api/[controller]")]
    [Authorize] // solo usuarios verificados (AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)
    [ApiController]
    public class VehiculosController : ControllerBase  
    {
        private readonly BdjwtContext _bdjwtContext;
        private VehiculosDAO _vehiculoDAO = new VehiculosDAO();
        public VehiculosController(BdjwtContext bdjwtContext)
        {
            _bdjwtContext = bdjwtContext;
        }

        [HttpGet]
        [Route("Lista")]
        public async Task<IActionResult> Lista()
        {
            var lista = await _bdjwtContext.Vehiculos.ToListAsync();
            return StatusCode(StatusCodes.Status200OK, new { Value  = lista });
        }

        [HttpPost]
        [Route("NuevoVehiculo")]
        public async Task<IActionResult> NuevoVehiculo([FromBody] VehiculosDTO vehiculo)
        {
            try
            {
                if (vehiculo == null)
                    return StatusCode(StatusCodes.Status200OK, new { isSuccess = false, message = "Vehiculo nullo" });

                var nuevoVehiiculo = new Vehiculo
                {
                    Tipo = vehiculo.Tipo,
                    Patente = vehiculo.Patente,
                    Marca = vehiculo.Marca,
                    Modelo = vehiculo.Modelo,
                    Anio = vehiculo.Anio,
                };

                await _bdjwtContext.Vehiculos.AddAsync(nuevoVehiiculo);
                await _bdjwtContext.SaveChangesAsync();

                if (nuevoVehiiculo.IdVehiculo != 0)
                    return StatusCode(StatusCodes.Status200OK, new { isSuccess = true });
                else
                    return StatusCode(StatusCodes.Status200OK, new { isSuccess = false });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpDelete]
        [Route("EliminarVehiculo")]
        public IActionResult EliminarVehiculo(int id)
        {
            try
            {
                if (id == 0)
                    return StatusCode(StatusCodes.Status200OK, new { isSuccess = false, message = "No existe el vehiculo" });

                var vehiculo =  _bdjwtContext.Vehiculos.Where(v => v.IdVehiculo == id).FirstOrDefault();
                if (vehiculo == null)
                    return StatusCode(StatusCodes.Status200OK, new { isSuccess = false, message = "Vehiculo no encontrado" });

                _bdjwtContext.Vehiculos.Remove(vehiculo);
                _bdjwtContext.SaveChanges();
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = true, message = "Vehiculo eliminado" });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }    
        }
        [HttpPost]
        [Route("EditarVehiculo")]
        public IActionResult EditarVehiculo([FromBody] Vehiculo vehiculo)
        {
            try
            {
                if (vehiculo == null)
                    return StatusCode(StatusCodes.Status200OK, new { isSuccess = false, message = "No existe el vehiculo" });

                var vehiculoUpdate = _bdjwtContext.Vehiculos.Where(v => v.IdVehiculo == vehiculo.IdVehiculo).FirstOrDefault();
                if (vehiculoUpdate == null)
                    return StatusCode(StatusCodes.Status200OK, new { isSuccess = false, message = "No existe el vehiculo" });

                vehiculoUpdate.Tipo = vehiculo.Tipo;
                vehiculoUpdate.Patente = vehiculo.Patente;
                vehiculoUpdate.Marca = vehiculo.Marca;
                vehiculoUpdate.Modelo = vehiculo.Modelo;
                vehiculoUpdate.Anio = vehiculo.Anio;

                _bdjwtContext.SaveChanges();
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = true, message = "Vehiculo editado" });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
