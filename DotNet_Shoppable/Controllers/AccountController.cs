using DotNet_Shoppable.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DotNet_Shoppable.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) 
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
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



        // Restricted page access for users not authorized
        public IActionResult AccessDenied()
        {
            // If user role is not admin redirect to home page
            return RedirectToAction("Index", "Home");
        }



    }
}
