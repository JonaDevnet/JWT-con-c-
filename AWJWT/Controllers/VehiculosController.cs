using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AWJWT.Models;
using Microsoft.EntityFrameworkCore;


namespace AWJWT.Controllers
{
    [Route("api/[controller]")]
    [Authorize] // solo usuarios verificados (AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)
    [ApiController]
    public class VehiculosController : ControllerBase  
    {
        private readonly BdjwtContext _bdjwtContext;
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
    }
}
