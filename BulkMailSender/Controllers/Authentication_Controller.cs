using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using BulkMailSender.Authentication;
using BulkMailSender.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;

namespace RestroBarandPubsAPI.Controllers.Authentication
{
    [Route("api/[controller]")]
    [ApiController]
    public class Authentication_Controller : ControllerBase
    {
        private readonly UserManager<Application_User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        public Authentication_Controller(UserManager<Application_User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
        }

        

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] Login_Model model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo,
                    UserName=user.UserName,
                    Roles= userRoles,
                    Status = "success"
                });
            }
            return Ok(new Response { Status = "failed", Message = "invalid username or password." });
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] Register_Model model)
        {
            try
            {
                var userExists = await _userManager.FindByNameAsync(model.Username);
                if (userExists != null)
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

                Application_User user = new Application_User()
                {
                    Email = model.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = model.Username
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details or password policy and try again." });

                }
                return Ok(new Response { Status = "Success", Message = "User created successfully!" });
            }
            catch (Exception Exc)
            {

                throw Exc;
            }
            
        }

        [HttpPost]
        [Route("CreateRole")]
        public async Task<IActionResult> CreateRole([Required] string RoleName)
        {
            if (ModelState.IsValid)
            {
                var RoleExists = await _roleManager.FindByNameAsync(RoleName);
                if (RoleExists != null)
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Role already exists!" });

                IdentityResult result = await _roleManager.CreateAsync(new IdentityRole(RoleName));
                if (!result.Succeeded)
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Role creation failed! Please check Role Name and try again." });
            }
            return Ok(new Response { Status = "Success", Message = "Role created successfully!" });
        }

        [HttpPost]
        [Route("CreateUserRoles")]
        public async Task<IActionResult> CreateUserRoles( [Required] string UserName, [Required] string RoleName)
        {

            var RoleManager = _serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = _serviceProvider.GetRequiredService<UserManager<Application_User>>();

            //Adding Admin Role
            var roleCheck = await RoleManager.RoleExistsAsync(RoleName);
            if (!roleCheck)
            {
                //create the roles and seed them to the database
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Role does not exists!" });
            }
            //Assign Admin role to the main User here we have given our newly registered 
            //login id for Admin management
            Application_User user = await UserManager.FindByNameAsync(UserName);
            
            // Add User To Role
            var userInRole = await _userManager.IsInRoleAsync(user, RoleName);

            if (!userInRole)
            {
                await UserManager.AddToRoleAsync(user, RoleName);
                return Ok(new Response { Status = "Success", Message = "Role assigned successfully!" });
            }
            else
            {
                return Ok(new Response { Status = "Not added", Message = "Same role already assigned for this user." });
            }
        }
        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword(UserPasswordChangeModel model)
        {
            Application_User appUser = _userManager.FindByNameAsync(model.UserName).Result;
            await _userManager.ChangePasswordAsync(appUser, model.OldPassword, model.NewPassword);
            return Ok(new Response { Status = "Password Changed", Message = "Password changed successfully." });
        }
    }
}
