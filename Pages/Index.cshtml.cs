using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace FinalProject.Pages
{
    public class IndexModel : PageModel
    {
        public List<EmailInfo> listEmails = new List<EmailInfo>();

        public void OnGet()
        {
            try
            {
                string connectionString = "Server=tcp:vidit12345.database.windows.net,1433;Initial Catalog=Finalzz;Persist Security Info=False;User ID=vidit;Password=thep1234@;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Console.WriteLine("Database connection successful!");

                    string currentUserEmail = User.Identity?.Name ?? "guest@example.com";
                    Console.WriteLine($"Current User Email: {currentUserEmail}");

                    string sql = @"
                SELECT 
                    EmailID, EmailSubject, EmailMessage, EmailDate, EmailIsRead, EmailSender, EmailReceiver
                FROM emails
                WHERE EmailReceiver = @EmailReceiver";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@EmailReceiver", currentUserEmail);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine("Email found!");

                                EmailInfo email = new EmailInfo
                                {
                                    EmailID = reader["EmailID"].ToString(),
                                    EmailSubject = reader["EmailSubject"].ToString(),
                                    EmailMessage = reader["EmailMessage"].ToString(),
                                    EmailDate = reader["EmailDate"].ToString(),
                                    EmailIsRead = reader["EmailIsRead"].ToString(),
                                    EmailSender = reader["EmailSender"].ToString(),
                                    EmailReceiver = reader["EmailReceiver"].ToString()
                                };

                                listEmails.Add(email);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }


        public class EmailInfo
    {
        public string EmailID { get; set; }
        public string EmailSubject { get; set; }
        public string EmailMessage { get; set; }
        public string EmailDate { get; set; }
        public string EmailIsRead { get; set; }
        public string EmailSender { get; set; }
        public string EmailReceiver { get; set; }
    }
}

    public class UserInfo
    {
        public String FirstName;
        public String LastName;
        public String MobilePhone;        
        public String Username;
        public String Password;
        public String ConfirmPassword;
        public String Email;
        public String Department;

    }

}
