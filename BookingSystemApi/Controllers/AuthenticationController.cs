using BookingSystemApi.Authentication;
using BookingSystemApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BookingSystemApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RestaurantBookingContext _context;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthenticationController(RestaurantBookingContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
            _configuration = configuration;
            _signInManager = signInManager;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromForm] RegisterModel model, [FromForm] string clientlink)
        {
            var userExist = await userManager.FindByNameAsync(model.Username);
            if (userExist != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "UserExist", Message = " User Already Exist" });

            var emailexist = await userManager.FindByEmailAsync(model.Email);
            if (emailexist != null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "EmailExist", Message = " Email Already Exist" });
            }
            ApplicationUser user = new ApplicationUser
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                PhoneNumber = model.PhoneNumber
            };

            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Failed to register new user" });

            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var code = System.Web.HttpUtility.UrlEncode(token);
            Uri confirmationLink = new Uri(clientlink + "?token=" + code + "&email=" + user.Email);
            EmailHelper emailHelper = new EmailHelper();
            bool emailResponse = emailHelper.SendEmail(user.Email, confirmationLink);
            await userManager.AddToRoleAsync(user, UserRoles.User);
            if (emailResponse)
            {
                return Ok(new Response { Status = "Success", Message = "User Created Successfully" });
            }
            else
            {
                return Ok(new Response { Status = "Fail Email", Message = "Fail to send mail" });
            }


        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromForm] LoginModel model)
        {
            var user = await userManager.FindByNameAsync(model.Username);
            var result = await _signInManager.PasswordSignInAsync(model.Username,
                model.Password, false, lockoutOnFailure: true);
            Response rp = new Response();
            //if (user!=null && await userManager.CheckPasswordAsync(user, model.Password))
            if (result.Succeeded)
            {

                JsonToken jwt = new JsonToken();
                jwt.token = "";

                var userRoles = await userManager.GetRolesAsync(user);
                jwt.role = userRoles;
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),

                };
                foreach (var userrole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userrole));
                }
                var authSigninKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(5),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigninKey, SecurityAlgorithms.HmacSha256)
                    );


                var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);

                jwt.token = tokenStr;
                return Ok(jwt);
            }

            else if (result.IsLockedOut)
            {
                return Unauthorized(new Response { Status = "Locked", Message = "User has been locked" });
            }
            else
            {
                Response response = new Response { Status = "Invalid", Message = "User and password invalid" };
                if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
                {
                    bool emailStatus = await userManager.IsEmailConfirmedAsync(user);
                    if (emailStatus == false)
                    {
                        response.Status = "UnconfirmedEmail";
                        response.Message = "Please confirm your email before login";
                    }
                }

                return Unauthorized(response);
            }
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        [Route("RegisterAdmin")]
        public async Task<IActionResult> RegisterAdmin([FromForm] RegisterModel model, [FromForm] string clientlink)
        {
            var userExist = await userManager.FindByNameAsync(model.Username);
            if (userExist != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "UserExist", Message = " User Already Exist" });
            var emailexist = await userManager.FindByEmailAsync(model.Email);
            if (emailexist != null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "EmailExist", Message = " Email Already Exist" });
            }
            ApplicationUser user = new ApplicationUser
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                PhoneNumber = model.PhoneNumber
            };

            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Failed to register new user" });
            }

            //create user role in database
            if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            }
            if (!await roleManager.RoleExistsAsync(UserRoles.User))
            {
                await roleManager.CreateAsync(new IdentityRole(UserRoles.User));
            }

            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var code = System.Web.HttpUtility.UrlEncode(token);
            Uri confirmationLink = new Uri(clientlink + "?token=" + code + "&email=" + user.Email);
            EmailHelper emailHelper = new EmailHelper();
            bool emailResponse = emailHelper.SendEmail(user.Email, confirmationLink);








            await userManager.AddToRoleAsync(user, UserRoles.Admin);
            if (emailResponse)
            {
                return Ok(new Response { Status = "Success", Message = "Admin Created Successfully" });
            }
            else
            {
                return Ok(new Response { Status = "Fail Email", Message = "Fail to send mail" });
            }

        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet]
        [Route("validate")]
        public async Task<ActionResult> Validate()
        {
            return Ok();
        }

        [Authorize]
        [HttpPut]
        [Route("UserUpdate")]
        public async Task<IActionResult> UserUpdate([FromForm] UserUpdate model)
        {
            var identity = User.Identity as ClaimsIdentity;
            IEnumerable<Claim> claims = identity.Claims;
            string username = User.FindFirstValue(ClaimTypes.Name);
            var user = await userManager.FindByNameAsync(username);
            var checkPassword = await userManager.ChangePasswordAsync(user, model.old_password, model.password);
            if (checkPassword.Succeeded)
            {
                return Ok(new Response { Status = "Success", Message = "Password updated successfully" });
            }
            else
            {
                return Ok(new Response { Status = "Fail", Message = "Old password is incorrect" });

            }
            //var token = await userManager.GeneratePasswordResetTokenAsync(user);
            //var result = await userManager.ResetPasswordAsync(user, token, model.password);


            //var user1 = await userManager.FindByIdAsync(id);

            //var token1 = await userManager.GeneratePasswordResetTokenAsync(user);

            //var result = await UserManager.ResetPasswordAsync(user, token, "MyN3wP@ssw0rd");
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet]
        [Route("GetUser")]
        public async Task<ActionResult<PagedList<List<UserDetailStatus>>>> GetUser([FromQuery] PaginationFilter paginationFilter)
        {
            var userList = userManager.GetUsersInRoleAsync("User");

            var pagedata = _context.AspNetUserRoles.Include(x => x.User).Include(p => p.Role).Where(x => x.Role.Name == "User")

                .Select(x => new UserDetailStatus
                {
                    username = x.User.UserName,
                    email = x.User.Email,
                    phoneNumber = x.User.PhoneNumber,
                    //roles = x.Role.AspNetUserRoles.Select(x=>x.Role.Name), 
                    roles = x.Role.Name,
                    status = x.User.LockoutEnd > DateTime.UtcNow ? "lockedout" : "active"
                }).OrderBy(x => x.username).Skip((paginationFilter.PageNumber - 1) * paginationFilter.PageSize)
                .Take(paginationFilter.PageSize).ToList();

            //var pagedata = _context.AspNetUsers.Include(x=>x.AspNetUserRoles).ThenInclude(model=>model.Role)

            //    .Select(x=>new UserDetailStatus{ 
            //        username=x.UserName, 
            //        email=x.Email, 
            //        phoneNumber=x.PhoneNumber,
            //        roles= x.AspNetUserRoles.Where(x=>x.Role.Name=="User").Select(x=>x.Role.Name),
            //        status = x.LockoutEnd > DateTime.UtcNow? "lockedout" : "active"
            //    }).OrderBy(x=>x.username).Skip((paginationFilter.PageNumber - 1) * paginationFilter.PageSize)
            //    .Take(paginationFilter.PageSize).ToList();
            var totalRecords = await _context.AspNetUsers.CountAsync();

            return Ok(new PagedList<List<UserDetailStatus>>(pagedata, paginationFilter.PageNumber, paginationFilter.PageSize, totalRecords));

            // string username = User.FindFirstValue(ClaimTypes.Name);
            //var user = await userManager.FindByNameAsync(username);
            //user.Email = "asdf";
            //user.LockoutEnd
            //await userManager.UpdateAsync(user);
            //await userManager.SetLockoutEndDateAsync(user, null);
        }


        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet]
        [Route("GetUserByUsername")]
        public async Task<ActionResult<UserDetailStatus>> GetUserByUsername([FromQuery] string username)
        {
            if (_context.AspNetUsers.Any(x => x.UserName == username))
            {
                UserDetailStatus user = _context.AspNetUsers.Where(x => x.UserName == username).Select(x => new UserDetailStatus
                {
                    username = x.UserName,
                    email = x.Email,
                    phoneNumber = x.PhoneNumber,
                    roles = x.AspNetUserRoles.Select(x => x.Role.Name).FirstOrDefault(),
                    //roles = x.AspNetUserRoles.Select(x => x.Role.Name),
                    status = x.LockoutEnd > DateTime.UtcNow ? "lockedout" : "active"
                }).FirstOrDefault();

                return Ok(user);
            }
            else
            {
                return NotFound();
            }
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPut]
        [Route("EditUser")]
        public async Task UpdateUser([FromForm] returnUserDetailStatus returnUserDetailStatus)
        {
            // string username = User.FindFirstValue(ClaimTypes.Name);
            var user = await userManager.FindByNameAsync(returnUserDetailStatus.username);
            user.Email = returnUserDetailStatus.email;
            user.PhoneNumber = returnUserDetailStatus.phoneNumber;
            await userManager.UpdateAsync(user);
            if (returnUserDetailStatus.status == "active")
            {
                await userManager.SetLockoutEndDateAsync(user, null);
            }
            else
            {
                await userManager.SetLockoutEndDateAsync(user, DateTime.UtcNow.AddDays(5000));
            }
        }

        [HttpPost]
        [Route("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword([FromForm] string email, [FromForm] string clientlink)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound();
            }
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            Uri link = new Uri(clientlink + "?token=" + token + "&email=" + user.Email);

            EmailHelper emailHelper = new EmailHelper();
            bool emailResponse = emailHelper.SendEmailPasswordReset(user.Email, link);
            if (emailResponse)
            {
                return Ok(new Response { Status = "Success", Message = "Email send successfully" });
            }
            else
            {
                // log email failed 
                return Ok(new Response { Status = "Fail", Message = "Fail to send email" });
            }
        }

        [HttpPost]
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordModel resetPasswordModel)
        {
            var user = await userManager.FindByEmailAsync(resetPasswordModel.Email);
            resetPasswordModel.Token = resetPasswordModel.Token.Replace(' ', '+');
            var resetPassResult = await userManager.ResetPasswordAsync(user, resetPasswordModel.Token, resetPasswordModel.Password);
            if (resetPassResult.Succeeded)
            {
                return Ok(new Response { Status = "Success", Message = "Password Reset successfully" });
            }
            else
            {
                return Ok(new Response { Status = "Fail", Message = "Fail to reset password" });
            }
        }

        [HttpPost]
        [Route("EmailConfirmation")]
        public async Task<IActionResult> EmailConfirmation([FromForm] string token, [FromForm] string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            var result = await userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return Ok(new Response { Status = "Success", Message = "Email Confirm successfully" });
            }
            else
            {
                return Ok(new Response { Status = "Fail", Message = "Fail to confirm email" });
            }
        }

    }

}
