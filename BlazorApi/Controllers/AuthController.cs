using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Supabase.Gotrue;
using System;
using System.Threading.Tasks;

namespace WeatherApp.Controllers
{
    [EnableCors("AllowBlazorWasm")]
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly SupabaseAuthService _authService;

        public AuthController(SupabaseAuthService authService)
        {
            _authService = authService;
        }

        // ? Get the current user from the authentication session
        [HttpGet("current-user")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var session = await _authService.GetSessionAsync();

            if (session == null || session.User == null)
                return Unauthorized(new { message = "No user is currently logged in." });

            return Ok(new
            {
                session.User.Id,
                session.User.Email,
                session.User.UserMetadata
            });
        }

        // ? User login - stores authentication token in a secure cookie
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(new { message = "Email and password are required." });

            try
            {
                var session = await _authService.LoginAsync(request.Email, request.Password);

                if (session != null && session.User != null)
                {
                    // ? Retrieve access token from the session
                    var accessToken = session.AccessToken;

                    // ? Store authentication token in an HTTP-only cookie
                    var authCookieOptions = new CookieOptions
                    {
                        HttpOnly = true,  // Prevents JavaScript access
                        Secure = true,    // Ensures HTTPS-only usage
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTime.UtcNow.AddDays(7) // Session expires in 7 days
                    };

                    Response.Cookies.Append("auth_token", accessToken!, authCookieOptions);

                    return Ok(new
                    {
                        message = "Login successful.",
                        user = new
                        {
                            session.User.Id,
                            session.User.Email,
                            session.User.UserMetadata
                        }
                    });
                }

                return Unauthorized(new { message = "Invalid email or password." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during login.", error = ex.Message });
            }
        }

        // ? User logout - clears authentication session
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _authService.LogoutAsync();

                // ? Remove authentication cookie
                Response.Cookies.Delete("auth_token");

                return Ok(new { message = "Logout successful." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during logout.", error = ex.Message });
            }
        }

        // ? User registration
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(new { message = "Email and password are required." });

            try
            {
                var session = await _authService.RegisterAsync(request.Email, request.Password);

                if (session != null && session.User != null)
                {
                    return Ok(new
                    {
                        message = "Registration successful.",
                        user = new
                        {
                            session.User.Id,
                            session.User.Email,
                            session.User.UserMetadata
                        }
                    });
                }

                return BadRequest(new { message = "Registration failed. Please try again." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during registration.", error = ex.Message });
            }
        }

        // ? Send password recovery email
        [HttpPost("recover-password")]
        public async Task<IActionResult> RecoverPassword([FromBody] RecoverPasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                return BadRequest(new { message = "Email is required for password recovery." });

            try
            {
                await _authService.SendPasswordRecoveryEmailAsync(request.Email);
                return Ok(new { message = "Password recovery email sent." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while sending password recovery email.", error = ex.Message });
            }
        }
    }

    // ? Request models
    public class LoginRequest
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }

    public class RegisterRequest
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }

    public class RecoverPasswordRequest
    {
        public string? Email { get; set; }
    }
}
