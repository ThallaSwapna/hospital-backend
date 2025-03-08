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
using System.Web.Http.Cors;// ✅ Required for ApiController


namespace WebApplication1.Controllers
{
    //[EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UserController : ApiController
    {
        private readonly string connectionString = "Server=DESKTOP-P5400J6;Database=hospital;User Id=sa;Password=admin123$;";

        [HttpPost]
        [Route("api/login")]
        public async Task<IHttpActionResult> Login([FromBody] LoginModel model)
        {
            if (model == null)
                return BadRequest("Invalid client request");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = "SELECT COUNT(*) FROM signup WHERE email = @Email AND password COLLATE Latin1_General_CS_AS = @Password";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Email", model.email);
                    cmd.Parameters.AddWithValue("@Password", model.password);

                    int count = (int)cmd.ExecuteScalar();
                    if (count > 0)
                        return Ok(new { message = "Login successful" });
                    else
                        return Unauthorized();
                }
            }
        }


        [HttpGet]
        [Route("api/getUsername")]
        public IHttpActionResult GetUsername(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email is required.");
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT username FROM signup WHERE email = @Email";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            return Ok(new { username = result.ToString() });
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
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("api/signupdetails")]
        public IHttpActionResult GetSignupDetails([FromUri] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email is required.");
            }

            EmployeeModel user = null;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT name, email, phonenumber, gender, username FROM signup WHERE email = @Email", con))
                {
                    cmd.Parameters.AddWithValue("@Email", email);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new EmployeeModel
                            {
                                name = reader.GetString(0),
                                phonenumber = reader.GetString(2),
                                gender = reader.GetString(3),
                                username = reader.GetString(4)
                            };
                        }
                    }
                }
            }

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPut]
        [Route("api/updatesignup")]
        public IHttpActionResult UpdateSignupDetails([FromUri] string email, [FromBody] EmployeeModel user)
        {
            if (string.IsNullOrWhiteSpace(email) || user == null)
            {
                return BadRequest("Invalid input data.");
            }

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(@"UPDATE signup 
                                                         SET name = @Name, 
                                                             phonenumber = @PhoneNumber, 
                                                             gender = @Gender, 
                                                             username = @Username 
                                                         WHERE email = @Email", con))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Name", user.name);
                        cmd.Parameters.AddWithValue("@PhoneNumber", user.phonenumber);
                        cmd.Parameters.AddWithValue("@Gender", user.gender);
                        cmd.Parameters.AddWithValue("@Username", user.username);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return Ok("User details updated successfully.");
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

        [HttpDelete]
        [Route("api/deleteprofile")]
        public IHttpActionResult DeleteProfile(string email)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("DELETE FROM signup WHERE email = @email", con))
                {
                    cmd.Parameters.AddWithValue("@email", email);
                    int rowsAffected = cmd.ExecuteNonQuery(); // Execute delete command

                    if (rowsAffected > 0)
                    {
                        return Ok(new { message = "Patient deleted successfully." });
                    }
                    else
                    {
                        return NotFound(); // Return 404 if no rows were deleted
                    }
                }
            }
        }

        [HttpPost]
        [Route("api/signup")]
        public async Task<string> SignUp([FromBody] UserModel signup)
        {
            if (signup == null)
                return "Invalid input data";
            string query = "INSERT INTO signup (name, username, email, phonenumber,password,confirmpassword,gender) VALUES (@name,@username, @email, @phonenumber, @password,@confirmpassword,@gender)";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@name", signup.name);
                    cmd.Parameters.AddWithValue("@username", signup.username);
                    cmd.Parameters.AddWithValue("@email", signup.email);
                    cmd.Parameters.AddWithValue("@phonenumber", signup.phonenumber);
                    cmd.Parameters.AddWithValue("@password", signup.password);
                    cmd.Parameters.AddWithValue("@confirmpassword", signup.confirmpassword);
                    cmd.Parameters.AddWithValue("@gender", signup.gender);

                    int result = await cmd.ExecuteNonQueryAsync();

                    return result > 0 ? "Data inserted successfully!" : "Something went wrong!";
                }
            }
        }
    }

}