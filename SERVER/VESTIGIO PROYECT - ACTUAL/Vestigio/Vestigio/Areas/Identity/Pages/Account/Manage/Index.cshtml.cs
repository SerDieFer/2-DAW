// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Vestigio.Models;

namespace Vestigio.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public IndexModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

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
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            /// 
            [Display(Name = "Nickname")]
            public string Nickname { get; set; }    

            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Display(Name = "ID")]
            [StringLength(9, MinimumLength = 9)]
            [RegularExpression(@"^((([A-Za-z])\d{8})|(\d{8}([A-Za-z])))$", ErrorMessage = "ID must contain a letter at the or at the beginning")]
            public string DNI { get; set; }

            [Display(Name = "Adress")]
            public string Address { get; set; }

            [Display(Name = "City")]
            public string City { get; set; }

            [Display(Name = "Postal Code")]
            [StringLength(5, MinimumLength = 5)]
            [RegularExpression(@"^\d{5}$", ErrorMessage = "Postal code must be a 5-digit number")]
            public string PostalCode { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

        }

        private async Task LoadAsync(User user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            Username = user.UserName;

            Input = new InputModel
            {
                Nickname = user.Nickname ?? string.Empty,
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                DNI = user.DNI ?? string.Empty,
                Address = user.Address ?? string.Empty,
                City = user.City ?? string.Empty,
                PostalCode = user.PostalCode ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Obtener el número de teléfono y actualizar si es necesario
            var currentPhoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != currentPhoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            // Si el nickname está vacío o es nulo, usar la parte del email antes del @
            user.Nickname = !string.IsNullOrWhiteSpace(Input.Nickname)
                ? Input.Nickname
                : user.Email.Split('@')[0];

            // Evita insertar valores NULL en campos obligatorios
            user.FirstName = Input.FirstName ?? string.Empty;
            user.LastName = Input.LastName ?? string.Empty;
            user.DNI = Input.DNI ?? string.Empty;
            user.Address = Input.Address ?? string.Empty;
            user.City = Input.City ?? string.Empty;
            user.PostalCode = Input.PostalCode ?? string.Empty;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Unexpected error when trying to update your profile.");
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }


    }
}
