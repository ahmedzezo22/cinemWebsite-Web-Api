using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using cinemaApp.Api.Data;
using cinemaApp.Api.Dtos;
using cinemaApp.Api.Models;
using cinemaApp.Api.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace cinemaApp.Api.Controllers
{
     [ApiController]
    [Route("[controller]")]
    
    public class AccountController:ControllerBase
    {
        private readonly DataContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<AppRole> _roleManager;

        public AccountController(DataContext context,UserManager<AppUser> userManager,RoleManager<AppRole> roleManager,SignInManager<AppUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager=roleManager;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto user)
        {
            if(user==null){
                return NotFound();
            }
            if(emailExist(user.Email)){
                return BadRequest("Email is used");
            }
            if(!isEmailValid(user.Email)){
                 return BadRequest("Email not valid");
            }
            if(userNameExist(user.UserName)){
                 return BadRequest("userName is used");
            }
            var userToCreate=new AppUser{
                Email=user.Email,
                UserName=user.UserName,
            };
          var result=  await _userManager.CreateAsync(userToCreate,user.Password);
          
          if(!result.Succeeded){
              return BadRequest(result.Errors);
          }
          //send email confirmation
          var token = await _userManager.GenerateEmailConfirmationTokenAsync(userToCreate);
                    var encodeToken = Encoding.UTF8.GetBytes(token);
                    var newToken = WebEncoders.Base64UrlEncode(encodeToken);

                    var confirmLink = $"http://localhost:4200/registerConfirm?ID={userToCreate.Id}&token={newToken}";
                    var txt = "Please confirm your registration at our site";
                    var link = "<a href=\"" + confirmLink + "\">Confirm registration</a>";
                    var title = "Registration Confirm";
         if(await sendGrid.Execute(userToCreate.Email,userToCreate.UserName,txt,title,link)){
              return Ok();
          }
        return Ok();
            
        }

        private bool userNameExist(string userName)
        {
           return _context.Users.Any(x=>x.UserName==userName);
        }

        private bool emailExist(string email)
        {
            return _context.Users.Any(x=>x.Email==email);
        }
        private bool isEmailValid(string Email){
            Regex reg=new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
            if(reg.IsMatch(Email)){
                return true;
            }
            return false;
        }
        //RegiterationConfirm
        [HttpGet("RegiterationConfirm")]
        public async Task<IActionResult> RegiterationConfirm(string ID,string token){
            if(string.IsNullOrEmpty(ID)||string.IsNullOrEmpty(token)){
                return NotFound();
            }
            var user=await _userManager.FindByIdAsync(ID);
            if(user==null){
                return NotFound();
            }
            var newToken = WebEncoders.Base64UrlDecode(token);
            var encodeToken = Encoding.UTF8.GetString(newToken);

            var result = await _userManager.ConfirmEmailAsync(user, encodeToken);
            if(result.Succeeded){
                
                // user.EmailConfirmed=true;
               return Ok();
            }else{
                return BadRequest(result.Errors);
            }
           
        }
        //login 

        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLoginDto model)
        {
            await CreateRole();
            await CreateAdmin();
            if(model==null){
                return NotFound("الايميل او كلمة السر غير صحيحه");
            }
            var user=await _userManager.FindByEmailAsync(model.Email);
            
            if(user==null)     return NotFound("الايميل او كلمة السر غير صحيحه");

             if(!user.EmailConfirmed) return BadRequest("لم يتم تأكيد حسابك ");
                 var result=await _signInManager.PasswordSignInAsync(user,model.Password,model.RememberMe,true);
            if(result.Succeeded){
                if(await _roleManager.RoleExistsAsync("User")){
                    if(!await _userManager.IsInRoleAsync(user,"User")&&!await _userManager.IsInRoleAsync(user,"Admin")){
                 await _userManager.AddToRoleAsync(user,"User");
               }
                }
                var roleName = await GetRoleNameByUserId(user.Id);
                if (roleName != null){
                AddCookies(user.UserName,roleName,user.Id,model.RememberMe,user.Email);
                }
                return Ok();
            }else if(result.IsLockedOut){
              
             return Unauthorized(" تم غلق الحساب موقتا نتبجه للتسجيل الخاطىء برجاء الانتظار عشر دقايق" );
            }
        
            return BadRequest("حدث خطأ ما");
        }
        // [Authorize]
    [HttpGet("GetAllUsers")]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetAllUsers(){
            return await _context.Users.ToListAsync();
    }
    private async Task CreateAdmin(){
        var admin=await _userManager.FindByNameAsync("Admin");
        if(admin==null){
            var user=new AppUser{
                Email="Admin@gmail.com",
                EmailConfirmed=true,
                UserName="Admin"
            };
           var result=await _userManager.CreateAsync(user,"Ahmedmandoo@154");
           if(result.Succeeded){
               if(await _roleManager.RoleExistsAsync("Admin")){
                 await _userManager.AddToRoleAsync(user,"Admin");
               }
        }
        }
    }
    private async Task CreateRole(){
        if(_roleManager.Roles.Count()<1){
            var role=new AppRole{
            Name="Admin"
        };
        await _roleManager.CreateAsync(role);
        role=new AppRole{
            Name="User"
        };
         await _roleManager.CreateAsync(role);
        }
        
    }
      private async Task<string> GetRoleNameByUserId(string userId)
        {
            var userRole = await _context.UserRoles.FirstOrDefaultAsync(x => x.UserId == userId);
            if (userRole != null)
            {
                return await _context.Roles.Where(x => x.Id == userRole.RoleId).Select(x => x.Name).FirstOrDefaultAsync();
            }
            return null;
        }
    //Cookies
     private async void AddCookies(string username, string roleName, string userId, bool remember, string email)
        {
            var claim = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, roleName),
            };

            var claimIdentity = new ClaimsIdentity(claim, CookieAuthenticationDefaults.AuthenticationScheme);

            if (remember)
            {
                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddDays(10)
                
                };

                await HttpContext.SignInAsync
                (
                   CookieAuthenticationDefaults.AuthenticationScheme,
                   new ClaimsPrincipal(claimIdentity),
                   authProperties
                );
            }
            else
            {
                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    IsPersistent = false,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                };

                await HttpContext.SignInAsync
                (
                   CookieAuthenticationDefaults.AuthenticationScheme,
                   new ClaimsPrincipal(claimIdentity),
                   authProperties
                );
            }
        }
        //logout
         [HttpGet]
        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddDays(10)
            };
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme, authProperties);
            return Ok();
        }
        //get role Name by email
        [HttpGet("GetRoleName/{email}")]
        public async Task<string> GetRoleNameByUserEmail(string email)
        {
            var user=await _userManager.FindByEmailAsync(email);
            if(user!=null){
                var userRole = await _context.UserRoles.
            FirstOrDefaultAsync(x => x.UserId == user.Id);
            if (userRole != null)
            {
                return await _context.Roles.Where(x => x.Id == userRole.RoleId).Select(x => x.Name).FirstOrDefaultAsync();
            }
            }
            
            return null;
        }
        //check user claim
    [Authorize]
    [HttpGet("checkUserClaim/{email}&{role}")]
        public   ActionResult checkUserClaim(string email,string role){
       var user=User.FindFirst(ClaimTypes.Email)?.Value;
       var userRole=User.FindFirst(ClaimTypes.Role)?.Value;
       var Id=User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
       if(user!=null&&userRole!=null && Id !=null){
          if(email!=user && role!=userRole){
              return Unauthorized();
              }
       }
       return Ok();
      }
      //forget password
      [HttpGet("forgetpassword/{email}")]
      public async Task<IActionResult> ForgetPassword(string Email){
          if(Email==null)return NotFound("برجاء ادخال ايميل ");
          var user= await _userManager.FindByEmailAsync(Email);

          if(user==null){return NotFound("ايميل غير صحيح");}
         var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var encodeToken = Encoding.UTF8.GetBytes(token);
                    var newToken = WebEncoders.Base64UrlEncode(encodeToken);
                    var confirmLink = $"http://localhost:4200/passwordconfirm?ID={user.Id}&token={newToken}";
                    var txt = "Please confirm your Email at our site to reset password";
                    var link = "<a href=\"" + confirmLink + "\">Confirm Password</a>";
                    var title = " password Confirm";
                                       
         if(await sendGrid.Execute(user.Email,user.UserName,txt,title,link)){
             
              return Ok(new{
                  
                  token=newToken
              });
          }
          return BadRequest("ايميل غير صحيح حاول مره اخرى");
      }
      //reset password
       [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPassword model){
            if(model==null){
                return NotFound();
            }
            var user=await _userManager.FindByIdAsync(model.ID);
            if(user==null){
                return NotFound();
            }
            var newToken = WebEncoders.Base64UrlDecode(model.Token);
            var encodeToken = Encoding.UTF8.GetString(newToken);
            var result = await _userManager.ResetPasswordAsync(user, encodeToken,model.Password);
            if(result.Succeeded){
               return Ok();
            }else{
                return BadRequest(result.Errors);
            }
        }
    }
}