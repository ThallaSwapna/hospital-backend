using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Threading.Tasks;
using WebApplication1.Models;
using System.Web.Http.Cors;

namespace WebApplication1.Controllers
{
    public class ForgotController : ApiController
    {
        private readonly string connectionString = "Server=DESKTOP-P5400J6;Database=hospital;User Id=sa;Password=admin123$;";
       
        [HttpGet]
        [Route("api/check-email")]
        public IHttpActionResult CheckEmailExists(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email is required.");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(1) FROM signup WHERE email = @Email", conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return Ok(count > 0); // Return true if email exists, otherwise false
                }
            }
        }
        [HttpGet]
        [Route("api/check-signup-email")]
        public IHttpActionResult CheckSignupEmailExists(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email is required.");

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(1) FROM signup WHERE email = @Email", conn))
                    {
                        cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 255).Value = email;
                        int count = Convert.ToInt32(cmd.ExecuteScalar());

                        // Return true if email does not exist, false if it exists
                        return Ok(count == 0);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error here (you can use a logging library like NLog or log4net)
                // For now, we can send the exception message for debugging
                return InternalServerError(new Exception($"An error occurred while checking the signup email: {ex.Message}"));
            }
        }

        [HttpGet]
        [Route("api/check-old-password")]
        public IHttpActionResult CheckOldPassword(string email, string oldPassword)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(oldPassword))
                return BadRequest("Email and Password are required.");

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(1) FROM signup WHERE email = @Email AND password COLLATE Latin1_General_CS_AS = @Password", conn))
                    {
                        cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 255).Value = email;
                        cmd.Parameters.Add("@Password", SqlDbType.NVarChar, 255).Value = oldPassword;

                        int count = Convert.ToInt32(cmd.ExecuteScalar());

                        // Return true if the old password is correct, otherwise false
                        return Ok(count == 1);
                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception($"An error occurred: {ex.Message}"));
            }
        }

        [HttpGet]
        [Route("api/changepassword")]
        public async Task<IHttpActionResult> ChangePassword(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return BadRequest("Email and Password are required.");

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("UPDATE signup SET password = @Password ,confirmpassword = @Password WHERE email = @Email", conn))
                    {
                        cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 255).Value = email;
                        cmd.Parameters.Add("@Password", SqlDbType.NVarChar, 255).Value = password; // 🔹 Hash this before storing

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = "Password updated successfully." });
                        }
                        else
                        {
                            return NotFound();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception($"An error occurred: {ex.Message}"));
            }
        }

    }
}
