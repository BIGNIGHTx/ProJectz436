using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace FinalProject.Pages
{
    [Authorize]  // Only authenticated users can access this page
    public class ComposeEmailModel : PageModel
    {
        [BindProperty]
        public string EmailSubject { get; set; }

        [BindProperty]
        public string EmailMessage { get; set; }

        [BindProperty]
        public string EmailSender { get; set; }

        [BindProperty]
        public string EmailReceiver { get; set; }

        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }


        // OnGet method for page rendering
        public void OnGet()
        {
        }

        // OnPostAsync for handling form submission
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Please fill in all fields correctly.";
                return Page(); // Return to the page with error message
            }

            string connectionString = "Server=tcp:vidit12345.database.windows.net,1433;Initial Catalog=Finalzz;Persist Security Info=False;User ID=vidit;Password=thep1234@;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    string sqlQuery = @"
                        INSERT INTO emails (EmailSubject, EmailMessage, EmailDate, EmailIsRead, EmailSender, EmailReceiver)
                        VALUES (@EmailSubject, @EmailMessage, @EmailDate, @EmailIsRead, @EmailSender, @EmailReceiver)";

                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        // Add parameters to avoid SQL injection
                        command.Parameters.AddWithValue("@EmailSubject", EmailSubject);
                        command.Parameters.AddWithValue("@EmailMessage", EmailMessage);
                        command.Parameters.AddWithValue("@EmailDate", DateTime.Now); // Current date and time
                        command.Parameters.AddWithValue("@EmailIsRead", 0); // Initially set as unread
                        command.Parameters.AddWithValue("@EmailSender", EmailSender);
                        command.Parameters.AddWithValue("@EmailReceiver", EmailReceiver);

                        // Execute the insert command
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            SuccessMessage = "Email sent successfully!";
                        }
                        else
                        {
                            ErrorMessage = "Failed to send email. Please try again.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"An error occurred: {ex.Message}";
                }
            }

            // Return the page with the success or error message
            return Page();
        }
    }
}
