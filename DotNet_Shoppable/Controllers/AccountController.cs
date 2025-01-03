using DotNet_Shoppable.Data;
using DotNet_Shoppable.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DotNet_Shoppable.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IConfiguration configuration; // access keys from appsetting.json

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration) 
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
        }

        public IActionResult Register()
        {
            //check if the user is already authenticated
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        //Add user
        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            //check if the user is already authenticated
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View(registerDto);
            }

            //create new account and authenticate the user
            var user = new ApplicationUser()
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                UserName = registerDto.Email, // registerDto.FirstName.ToLower() + "_" + registerDto.LastName.ToLower(), // UserName will be used to authenticate the user
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
                Address = registerDto.Address,
                CreatedAt = DateTime.UtcNow,
                //CreatedAt = DateTime.Now, // <==== uncomment when using sqlserver
            };

            var result = await userManager.CreateAsync(user, registerDto.Password);

            if (result.Succeeded) 
            {
                // successful user registration
                await userManager.AddToRoleAsync(user, "client");

                // sign in the new user
                await signInManager.SignInAsync(user, false);

                return RedirectToAction("Index", "Home");
            }

            // registration failed +> show registration errors
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(registerDto);
        }


        //User logout
        public async Task<IActionResult> Logout()
        {
            //check if user is authenticated 
            if (signInManager.IsSignedIn(User))
            {
                // signs out user when logout is clicked
                await signInManager.SignOutAsync();
            }

            return RedirectToAction("Index", "Home");
        }


        //User login 
        public IActionResult Login()
        {
            //check if the user is already authenticated
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        //User login Auth
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            //check if the user is already authenticated
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View(loginDto);
            }

            var result = await signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password, loginDto.RememberMe, false); // change 'false' to 'true' to lock account, if login fails too many times

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid login attempt! Please try again."; // display error in login view
            }

            return View(loginDto);
        }

        //User profile
        [Authorize]  // the 'Authorize' parameter only allows authenticated user to access the profile view
        public async Task<IActionResult> Profile()
        {
            // check if profile exist
            var appUser = await userManager.GetUserAsync(User);
            if (appUser == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // get data in modal
            var profileDto = new ProfileDto()
            {
                FirstName = appUser.FirstName,
                LastName = appUser.LastName,
                Email = appUser.Email ?? "", // could be null
                PhoneNumber = appUser.PhoneNumber,
                Address = appUser.Address,
            };


            return View(profileDto);
        }

        // Upate user profile
        [Authorize]  // the 'Authorize' parameter only allows authenticated user to access the profile view
        [HttpPost]
        public async Task<IActionResult> Profile(ProfileDto profileDto)
        {
            if (!ModelState.IsValid) 
            {
                ViewBag.ErrorMessage = "Please fill all the required fields with valid values";
                return View(profileDto);
            }

            // get the current user
            var appUser = await userManager.GetUserAsync(User);
            if (appUser == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // update the user profile
            appUser.FirstName = profileDto.FirstName;
            appUser.LastName = profileDto.LastName;
            appUser.Email = profileDto.Email;
            appUser.PhoneNumber = profileDto.PhoneNumber;
            appUser.Address = profileDto.Address;

            var result = await userManager.UpdateAsync(appUser);

            if (result.Succeeded) 
            {
                ViewBag.SuccessMessage = "Profile updated successfully";
            }
            else
            {
                ViewBag.ErrorMessage = "Unable to update the profile: " + result.Errors.First().Description; // display the description of the error     
            }

            return View(profileDto);
        }

        //User password
        [Authorize]
        public IActionResult Password()
        {
            return View();
        }

        // Update profile password
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Password(PasswordDto passwordDto)
        {
            if (!ModelState.IsValid) 
            {
                return View();
            }

            // get the current user
            var appUser = await userManager.GetUserAsync(User);
            if(appUser == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // update the password
            var result = await userManager.ChangePasswordAsync(appUser, passwordDto.CurrentPassword, passwordDto.NewPassword);

            if (result.Succeeded) 
            {
                ViewBag.SuccessMessage = "Password updated successfully";

            }
            else
            {
                ViewBag.ErrorMessage = "Error: " + result.Errors.First().Description;
            }

            return View();
        }


        // Restricted page access for users not authorized
        public IActionResult AccessDenied()
        {
            // If user role is not admin redirect to home page
            return RedirectToAction("Index", "Home");
        }


        //Forgot password
        public IActionResult ForgotPassword()
        {
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        //Send email password reset link - data model will not be used for action
        [HttpPost]
        public async Task<IActionResult> ForgotPassword([Required, EmailAddress] string email)
        {
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }

            // get the email data from view
            ViewBag.Email = email;

            // check email validation
            if (!ModelState.IsValid)
            {
                ViewBag.EmailError = ModelState["email"]?.Errors.First().ErrorMessage ?? "Invalid Email Address"; 
                return View();
            }

            // search for user email address
            var user = await userManager.FindByEmailAsync(email);

            if (user != null)
            {
                // generate password reset token
                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                string resetUrl = Url.ActionLink("ResetPassword", "Account", new { token }) ?? "URL Error";

                //Console.WriteLine("Password reset link: " + resetUrl);

                // send url by email
                string senderName = configuration["BrevoSettings:SenderName"] ?? "";
                string senderEmail = configuration["BrevoSettings:SenderEmail"] ?? "";
                string username = user.FirstName + " " + user.LastName;
                string subject = "Password Reset";
                string message = "Dear " + username + ",\n\n" + "You can reset your password using the following link: \n\n" + resetUrl + "\n\n" + "Best Regards";

                EmailSender.SendEmail(senderName, senderEmail, username, email, subject, message);   
            }

            ViewBag.SuccessMessage = "Please check your Email account and click on the 'Password Reset link'";

            return View();
        }


        //Password Reset link
        public IActionResult ResetPassword(string? token)
        {
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }

            if (token == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string? token, PasswordResetDto model)
        {
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }

            if (token == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // find user and user email 
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ViewBag.ErrorMessage = "Token not valid!";
                return View(model);
            }

            // reset password
            var result = await userManager.ResetPasswordAsync(user, token, model.Password);

            if (result.Succeeded) 
            {
                ViewBag.SuccessMessage = "Password reset successfully!";
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

            }

            return View(model);
        }



    }
}
