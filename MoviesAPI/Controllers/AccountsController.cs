using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MoviesAPI.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MoviesAPI.Controllers
{

    public class AccountsController : CustomBaseController
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly ApplicationDbContext context;
        private readonly IDataProtector dataProtector;
        public AccountsController(
            UserManager<IdentityUser> userManager,
            IConfiguration configuration,
            SignInManager<IdentityUser> signInManager,
            ApplicationDbContext context,
            IMapper mapper) : base(context, mapper)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
            this.context = context;
        }

        [HttpGet("Users")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult<List<UserDTO>>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            var queryable = context.Users.AsQueryable();
            queryable = queryable.OrderBy(x => x.Email);
            return await Get<IdentityUser, UserDTO>(paginationDTO);

        }
        [HttpGet("Roles")]
        [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme,Roles ="Admin")]
        public async Task<ActionResult>AssignRole(RoleEditDTO roleEdit)
        {
            var user = await userManager.FindByIdAsync(roleEdit.UserId);
            if (user==null)
            {
                return NotFound();
            }
            await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, roleEdit.RoleName));
            return NoContent();
        }


        [HttpPost("Login")]
        public async Task<ActionResult<UserToken>> Login([FromBody]UserInfo model)
        {
            var result = await signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                isPersistent: false,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return await BuildToken(model);

            }
            return BadRequest("Incorrect lgin");
        }


        [HttpPost("Register")]
        public async Task<ActionResult<UserToken>> Register(UserInfo userCredentials)
        {
            var user = new IdentityUser { UserName = userCredentials.Email, Email = userCredentials.Email };
            var result = await userManager.CreateAsync(user, userCredentials.Password);

            if (result.Succeeded)
            {
                return await BuildToken(userCredentials);
            }
            return BadRequest(result.Errors);


        }
        [HttpGet("RenewToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<UserToken>> RenewToken()
        {
            var userInfo = new UserInfo
            {
                Email = HttpContext.User.Identity.Name
            };
            return await BuildToken(userInfo);
        }
        private async Task<UserToken> BuildToken(UserInfo userInfo)
        {
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userInfo.Email),
                    new Claim(ClaimTypes.Email, userInfo.Email)
                };

            var identityUser = await userManager.FindByEmailAsync(userInfo.Email);
            claims.Add(new Claim(ClaimTypes.NameIdentifier, identityUser.Id));
            var claimsDB = await userManager.GetClaimsAsync(identityUser);
            claims.AddRange(claimsDB);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["jwt:key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddYears(1);
            JwtSecurityToken token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: expiration,
                signingCredentials: creds);
            return new UserToken()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };
        }
        [HttpPost("AssignRole")]
        public async Task<ActionResult> AssignRole(EditRoleDTO editRoleDTO)
        {
            var user = await userManager.FindByIdAsync(editRoleDTO.UserId);
            if (user == null)
            {
                return NotFound();
            }
            await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, editRoleDTO.RoleName));
            return NoContent();
        }
        [HttpPost("RemoveRol")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> RemoveRole(EditRoleDTO editRoleDTO)
        {
            var user = await userManager.FindByIdAsync(editRoleDTO.UserId);
            if (user == null)
            {
                return NotFound();
            }
            await userManager.RemoveClaimAsync(user, new Claim(ClaimTypes.Role, editRoleDTO.RoleName));
            return NoContent();
        }
        
	

	}


}




