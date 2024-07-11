// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using DAL;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace POS_BeautySalon.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        //private readonly EmailSender _emailSender;
        private readonly SalonContext _context; //Para poder crear lista de deseo al momento que un usuario se registre

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            //EmailSender emailSender,
            SalonContext context)  //Para poder crear lista de deseo al momento que un usuario se registre
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            //_emailSender = emailSender;
            _context = context; //Para poder crear lista de deseo al momento que un usuario se registre
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {

            [Required(ErrorMessage = "El campo de nombre es requerido")]
            [MaxLength(20, ErrorMessage = "El largo maximo es de 20")]
            public string Nombre { get; set; }

            [Required(ErrorMessage = "El campo de apellido es requerido")]
            [MaxLength(100)]
            public string Apellido { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }


       


         public async Task<IActionResult> OnPostAsync(string returnUrl = null)
         {
             returnUrl ??= Url.Content("~/");
             ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

             if (ModelState.IsValid)
             {
                 var user = CreateUser();

                 await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                 await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                 user.Nombre = Input.Nombre;
                 user.Apellido = Input.Apellido;
                 user.Estado = 1;
                 var result = await _userManager.CreateAsync(user, Input.Password);

                 if (result.Succeeded)
                 {
                     _logger.LogInformation("User created a new account with password.");
                     //Para agregar el tipo de rol
                     var resultRole = await _userManager.AddToRoleAsync(user, "Cliente");

                     //Para poder crear lista de deseo al momento que un usuario se registre
                     var listaDeseos = new ListaDeseo
                     {
                         ClienteId = user.Id
                         
                     };
                     _context.ListaDeseos.Add(listaDeseos);
                     await _context.SaveChangesAsync();

                     await _signInManager.SignInAsync(user, isPersistent: false);
                     return LocalRedirect(returnUrl);


                 }
                 foreach (var error in result.Errors)
                 {
                     ModelState.AddModelError(string.Empty, error.Description);
                 }
             }

             // If we got this far, something failed, redisplay form
             return Page();
         }



        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>(); 

            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}
