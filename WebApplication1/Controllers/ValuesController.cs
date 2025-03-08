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
using Microsoft.Extensions.Configuration;

namespace WebApplication1.Controllers
{
   // [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ValuesController : ApiController
    {
        private readonly string connectionString = "Server=DESKTOP-P5400J6;Database=hospital;User Id=sa;Password=admin123$;";



        [HttpPost]
        [Route("api/contact")]
        public async Task<string> Post([FromBody] ContactModel contact)
        {
            if (contact == null)
                return "Invalid input data";
            string query = "INSERT INTO contactinfo (name,  email, phonenumber,message) VALUES (@name, @email, @phonenumber, @message)";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@name", contact.name);
                    cmd.Parameters.AddWithValue("@email", contact.email);
                    cmd.Parameters.AddWithValue("@phonenumber", contact.phonenumber);
                    cmd.Parameters.AddWithValue("@message", contact.message);

                    int result = await cmd.ExecuteNonQueryAsync();

                    return result > 0 ? "Data inserted successfully!" : "Something went wrong!";
                }
            }
        }

      


        [HttpGet]
        [Route("api/dashboard")]
        public IHttpActionResult Dashboard()
        {
            try
            {
                int patientCount = 0, signupCount = 0, doctorCount = 0;

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM addpatient", con))
                    {
                        patientCount = (int)cmd.ExecuteScalar();
                    }
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM signup", con))
                    {
                        signupCount = (int)cmd.ExecuteScalar();
                    }
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM doctor", con))
                    {
                        doctorCount = (int)cmd.ExecuteScalar();
                    }
                }

                return Ok(new { patients = patientCount, users = signupCount, doctors = doctorCount });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("api/getfeedback")]
        public IHttpActionResult GetFeedback()
        {
            List<ContactModel> feedbacks = new List<ContactModel>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM contactinfo", con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            feedbacks.Add(new ContactModel
                            { 
                                name = reader.GetString(0),
                                email = reader.GetString(1),
                                phonenumber = reader.GetString(2),
                                message = reader.GetString(3),
                            });
                        }
                    }
                }
            }

            return Ok(feedbacks.Count > 0 ? feedbacks : new List<ContactModel>()); // Return empty list if no data
        }



        [HttpPost]
        [Route("api/payment")]
        public IHttpActionResult ProcessPayment([FromBody] PaymentData payment)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString)) // ✅ Use the existing connectionString
                {
                    connection.Open();

                    // Fetch current due amount
                    string fetchQuery = "SELECT dueamount FROM addpatient WHERE patient_id = @PatientId";
                    decimal currentDueAmount = 0;

                    using (var cmd = new SqlCommand(fetchQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@PatientId", payment.PatientId);
                        var result = cmd.ExecuteScalar();

                        if (result == null)
                        {
                            return BadRequest("Invalid Patient ID. No record found.");
                        }

                        if (!decimal.TryParse(result.ToString(), out currentDueAmount))
                        {
                            return BadRequest("Error fetching due amount. Invalid data format.");
                        }
                    }

                    // Calculate new due amount
                    decimal newDueAmount = currentDueAmount - payment.Amount;
                    if (newDueAmount < 0) newDueAmount = 0; // Ensure it doesn’t go negative

                    // Update due amount in the database
                    string updateQuery = "UPDATE addpatient SET dueamount = @NewDueAmount WHERE patient_id = @PatientId";
                    using (var cmd = new SqlCommand(updateQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@NewDueAmount", newDueAmount);
                        cmd.Parameters.AddWithValue("@PatientId", payment.PatientId);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            return BadRequest("Failed to update due amount. Patient ID might be incorrect.");
                        }
                    }
                }

                return Ok(new { message = "Payment processed successfully!" });
            }
            catch (SqlException ex)
            {
                return InternalServerError(new Exception("Database error occurred: " + ex.Message));
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("An error occurred: " + ex.Message));
            }
        }


        [HttpGet]
        [Route("api/getDueAmount/{patientId}")]
        public IHttpActionResult GetDueAmount(int patientId)
        {
            try
            {
                decimal dueAmount = 0;

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "SELECT dueamount FROM addpatient WHERE patient_id = @PatientId";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@PatientId", patientId);
                        var result = cmd.ExecuteScalar();

                        if (result == null)
                        {
                            return NotFound(); // No patient found
                        }

                        dueAmount = Convert.ToDecimal(result);
                    }
                }

                return Ok(new { dueAmount });
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("Error fetching due amount: " + ex.Message));
            }
        }

    }


}




