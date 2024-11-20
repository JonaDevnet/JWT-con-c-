using AWJWT.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AWJWT.Services
{
    public class JwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Genera un Access Token para el usuario proporcionado.
        /// </summary>
        /// <param name="user">Objeto Usuario con los datos necesarios para crear el token.</param>
        /// <returns>Access Token en formato string.</returns>
        public string GeneratorAccessToken(Usuario user)
        {
            // Validar que el usuario tenga los datos necesarios
            if (string.IsNullOrEmpty(user.Nombres) || string.IsNullOrEmpty(user.Correo) || user.IdUsuario <= 0)
                throw new ArgumentException("El usuario debe tener un nombre, un correo y un ID válidos.");

            // Validar que la clave JWT sea segura
            string key = _configuration["Jwt:key"]!;
            if (string.IsNullOrEmpty(key) || key.Length < 32)
                throw new ArgumentException("La clave JWT debe tener al menos 32 caracteres.");

            // Crear los claims del token
            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.IdUsuario.ToString()),
                new Claim(ClaimTypes.Email, user.Correo),
                new Claim(ClaimTypes.Name, user.Nombres),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            // Configurar la clave de seguridad y las credenciales de firma
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            // Configurar el tiempo de expiración
            var expirationMinutes = double.Parse(_configuration["Jwt:expiresInMinutes"]!);

            // Crear el token JWT
            var jwtConfig = new JwtSecurityToken(
                claims: userClaims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtConfig);
        }

        /// <summary>
        /// Genera un Refresh Token único.
        /// </summary>
        /// <returns>Refresh Token en formato jwt.</returns>
        public string GeneratorRefheshToken(int userId, string email, string name)
        {
            string key = _configuration["Jwt:key"]!;
            if (string.IsNullOrEmpty(key) || key.Length < 32)
                throw new ArgumentException("La clave JWT debe tener al menos 32 caracteres");

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Name, name),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // ID único para el Refresh Token
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64), // Fecha de creación en formato UNIX
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Configura un tiempo de expiración largo para el Refresh Token
            var jwtToken = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7), // Expiración del Refresh Token
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }



        /// <summary>
        /// Valida un token JWT y devuelve los claims si es válido.
        /// </summary>
        /// <param name="token">El token JWT a validar.</param>
        /// <returns>ClaimsPrincipal si el token es válido; null si no lo es.</returns>
        public ClaimsPrincipal? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:key"]!);

            try
            {
                // Validación de parámetros del token
                var claims = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false, // No validamos el issuer
                    ValidateAudience = false, // No validamos el audience
                    ClockSkew = TimeSpan.Zero // Desactivamos la tolerancia en expiración
                }, out var validatedToken);

                // Asegurarse de que el token sea un JWT válido
                if (validatedToken is not JwtSecurityToken jwtToken ||
                    !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return null; // Token no válido
                }

                return claims;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al validar el token: {ex.Message}"); // Log para depurar
                return null; // Token inválido
            }
        }
    }
}
