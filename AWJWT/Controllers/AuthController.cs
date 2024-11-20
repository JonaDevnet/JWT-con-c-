using AWJWT.Customs;
using AWJWT.DTOs;
using AWJWT.Models;
using AWJWT.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AWJWT.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous] // no solo usuarios autorizados, no tiene sentido xd
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly BdjwtContext _DbContext;
        private readonly Utilities _Utilities;
        private readonly JwtService _Services;    
        public AuthController(BdjwtContext context, Utilities utilities, JwtService services)
        {
            _DbContext = context;
            _Utilities = utilities;
            _Services = services;
        }

        [HttpPost]
        [Route("Registrarse")]
        public async Task<IActionResult> Registrarse(UsuarioDTO objecto)
        {
            //if (objecto == null || string.IsNullOrEmpty(objecto.Nombre) || string.IsNullOrEmpty(objecto.Clave))
            //{
            //    return StatusCode(StatusCodes.Status200OK, new { isSuccess = false, message = "Datos inválidos" });
            //}

            var modeloUsuario = new Usuario
            {
                Nombres = objecto.Nombre,
                Apellidos = objecto.Apellido,
                Correo = objecto.Correo,
                Celular = objecto.NumeroCelular,
                Clave = _Utilities.encriptarSHA256(objecto.Clave),
            };

            await _DbContext.Usuarios.AddAsync(modeloUsuario);
            await _DbContext.SaveChangesAsync();

            if (modeloUsuario.IdUsuario != 0)
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = true });
            else
                return StatusCode(StatusCodes.Status200OK, new { isSucces = false });

        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginDTO objeto)
        {
            var usuarioEncontrado = await _DbContext.Usuarios.Where(
                                   u => 
                                   u.Correo == objeto.Correo &&
                                   u.Clave == _Utilities.encriptarSHA256(objeto.Clave))
                                    .FirstOrDefaultAsync();
            if (usuarioEncontrado == null)
                return Unauthorized(new { isSucces = false, messege = "Credenciales incorrectas" });

            //generamos el token
            var accessToken = _Services.GeneratorAccessToken(usuarioEncontrado);
            var refreshToken = _Services.GeneratorRefheshToken(usuarioEncontrado.IdUsuario, usuarioEncontrado.Correo!, usuarioEncontrado.Nombres!);

            //Establecemos la cookie HttpOnly con el refresh token

            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = HttpContext.Request.IsHttps,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Ok(new { isSucces = true, accessToken, refreshToken });
        }

        [HttpPost]
        [Route("refresh-token")]
        public IActionResult RefreshToken()
        {
            // Obtener el Refresh Token de las cookies
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized(new { message = "Refresh token faltante" });

            // Validar el Refresh Token
            var principal = _Services.ValidateToken(refreshToken);
            if (principal == null)
                return Unauthorized(new { message = "Refresh token inválido" });

            // Verificar que el token no haya expirado
            var expClaim = principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp);
            if (expClaim != null && long.TryParse(expClaim.Value, out long expTimestamp))
            {
                var expirationDate = DateTimeOffset.FromUnixTimeSeconds(expTimestamp).UtcDateTime;
                if (expirationDate <= DateTime.UtcNow)
                    return Unauthorized(new { message = "Refresh token expirado" });
            }
            else
            {
                return Unauthorized(new { message = "Refresh token inválido 2" });
            }

            // Extraer datos del usuario desde los claims
            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = principal.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(name))
                return Unauthorized(new { message = "Información del usuario no válida" });

            // Generar un nuevo Access Token
            var newAccessToken = _Services.GeneratorAccessToken(new Usuario
            {
                IdUsuario = int.Parse(userId),
                Correo = email,
                Nombres = name
            });

            // Generar un nuevo Refresh Token y actualizar la cookie
            var newRefreshToken = _Services.GeneratorRefheshToken(int.Parse(userId), email, name); // Asegúrate de pasar estos valores
            Response.Cookies.Append("refreshToken", newRefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = false, // Cambiar esto a `true` cuando esté en producción (si usas HTTPS)
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7) // Configura el tiempo de expiración según sea necesario
            });

            return Ok(new { accessToken = newAccessToken });
        }

    }
}
