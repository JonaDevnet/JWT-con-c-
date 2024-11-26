using AWJWT.Context;
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
        public async Task<IActionResult> Registrarse([FromBody] UsuarioDTO objecto)
        {
            if (objecto == null || string.IsNullOrEmpty(objecto.Nombre) || string.IsNullOrEmpty(objecto.Clave))
            {
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = false, message = "Datos inválidos" });
            }

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
        public async Task<IActionResult> Login([FromBody] LoginDTO objeto)
        {
            var usuarioEncontrado = await _DbContext.Usuarios.Where(
                u =>
                u.Correo == objeto.Correo &&
                u.Clave == _Utilities.encriptarSHA256(objeto.Clave))
                .FirstOrDefaultAsync();

            if (usuarioEncontrado == null)
                return Unauthorized(new { isSucces = false, messege = "Credenciales incorrectas" });

            // Generar los tokens
            var accessToken = _Services.GeneratorAccessToken(usuarioEncontrado);
            var refreshToken = _Services.GeneratorRefheshToken(usuarioEncontrado.IdUsuario, usuarioEncontrado.Correo!, usuarioEncontrado.Nombres!);

            // Configurar la cookie HttpOnly con el refresh token
            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true, 
                Secure = HttpContext.Request.IsHttps, // Solo en HTTPS 
                SameSite = SameSiteMode.Strict, // Evita CSRF
                Expires = DateTime.UtcNow.AddDays(7) // expiración
            });

            return Ok(new { isSucces = true, accessToken });
        }


        [HttpPost]
        [Route("refresh-token")]
        public IActionResult RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized(new { message = "Refresh token faltante" });

            var principal = _Services.ValidateToken(refreshToken);
            if (principal == null)
                return Unauthorized(new { message = "Refresh token inválido" });

            // Verificar expiración del token
            var expClaim = principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp);
            if (expClaim != null && long.TryParse(expClaim.Value, out long expTimestamp))
            {
                var expirationDate = DateTimeOffset.FromUnixTimeSeconds(expTimestamp).UtcDateTime;
                if (expirationDate <= DateTime.UtcNow)
                    return Unauthorized(new { message = "Refresh token expirado" });
            }

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

            return Ok(new { accessToken = newAccessToken });
        }


    }
}
