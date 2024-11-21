using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace FinalProject.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public InputModel Input { get; set; }

        public string ErrorMessage { get; set; }

        public class InputModel
        {
            public string UserName { get; set; } // Can be email or username
            public string Password { get; set; } // Plain-text password entered by the user
        }

        public void OnGet()
        {
            // Handle GET request, if needed (e.g., clear session)
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Connection string (replace with your actual database credentials)
            string connectionString = "Server=tcp:vidit12345.database.windows.net,1433;Initial Catalog=Finalzz;Persist Security Info=False;User ID=vidit;Password=thep1234@;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    // SQL query to find user by username or email
                    string query = @"
                        SELECT Password 
                        FROM userInfo 
                        WHERE Username = @Username OR Email = @Username";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserName", Input.UserName);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                string storedPassword = reader["Password"].ToString();

                                // Verify password
                                if (VerifyPassword(storedPassword, Input.Password))
                                {
                                    // Redirect to ComposeEmail on successful login
                                    return RedirectToPage("/ComposeEmail");
                                }
                                else
                                {
                                    ErrorMessage = "Invalid username or password.";
                                }
                            }
                            else
                            {
                                ErrorMessage = "User not found.";
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                ErrorMessage = "An error occurred while connecting to the database.";
                // Log the exception (optional)
            }

            return Page(); // Reload the login page with an error message
        }

        // Helper method to verify the password (hash comparison)
        private bool VerifyPassword(string storedPassword, string enteredPassword)
        {
            using (var sha256 = SHA256.Create())
            {
                string enteredPasswordHash = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(enteredPassword)));
                return storedPassword == enteredPasswordHash;
            }
        }
    }
}
