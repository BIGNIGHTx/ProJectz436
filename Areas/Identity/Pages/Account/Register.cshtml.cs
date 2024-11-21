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
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using FinalProject.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography.Xml;
using Microsoft.Data.SqlClient;

namespace FinalProject.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<FinalProjectUser> _signInManager;
        private readonly UserManager<FinalProjectUser> _userManager;
        private readonly IUserStore<FinalProjectUser> _userStore;
        private readonly IUserEmailStore<FinalProjectUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<FinalProjectUser> userManager,
            IUserStore<FinalProjectUser> userStore,
            SignInManager<FinalProjectUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(255, ErrorMessage = "The First Name must be between 1 and 255 characters.", MinimumLength = 1)]
            public string FirstName { get; set; }

            [Required]
            [StringLength(255, ErrorMessage = "The Last Name must be between 1 and 255 characters.", MinimumLength = 1)]
            public string LastName { get; set; }

            [Required]
            [StringLength(15, ErrorMessage = "The Mobile Phone must be between 7 and 15 characters.", MinimumLength = 7)]
            public string MobilePhone { get; set; }

            [Required]
            [StringLength(255, ErrorMessage = "The Username must be between 1 and 255 characters.", MinimumLength = 1)]
            public string UserName { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            public string Department { get; set; }

            [Required]
            [EmailAddress(ErrorMessage = "The Email field is not a valid e-mail address.")]
            public string Email { get; set; }
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

            if (!ModelState.IsValid)
            {
                ErrorMessage = "Please correct the errors in the form.";
                return Page();
            }

            try
            {
                string connectionString = "Server=tcp:vidit12345.database.windows.net,1433;Initial Catalog=Finalzz;Persist Security Info=False;User ID=vidit;Password=thep1234@;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    string sql = @"
                        INSERT INTO userInfo 
                        (FirstName, LastName, MobilePhone, Username, Password, Email, Department) 
                        VALUES (@FirstName, @LastName, @MobilePhone, @Username, @Password, @Email, @Department);";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@FirstName", Input.FirstName);
                        command.Parameters.AddWithValue("@LastName", Input.LastName);
                        command.Parameters.AddWithValue("@MobilePhone", Input.MobilePhone);
                        command.Parameters.AddWithValue("@Username", Input.UserName);
                        command.Parameters.AddWithValue("@Password", Input.Password); // Use hashing in production!
                        command.Parameters.AddWithValue("@Email", Input.Email);
                        command.Parameters.AddWithValue("@Department", Input.Department);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            SuccessMessage = "Registration successful!";
                            return RedirectToPage("/Account/Login");
                        }
                        else
                        {
                            ErrorMessage = "Failed to register the user. Please try again.";
                            return Page();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
                return Page();
            }
        }

        private FinalProjectUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<FinalProjectUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(FinalProjectUser)}'. " +
                    $"Ensure that '{nameof(FinalProjectUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<FinalProjectUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<FinalProjectUser>)_userStore;
        }
    }
}
