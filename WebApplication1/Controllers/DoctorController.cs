using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WebApplication1.Models;
using System.Data.SqlClient;
using System.Web.Http.Cors;

namespace WebApplication1.Controllers
{
    //[EnableCors(origins: "*", headers: "*", methods: "*")]
    public class DoctorController : ApiController
    {


        private readonly string connectionString = "Server=DESKTOP-P5400J6;Database=hospital;User Id=sa;Password=admin123$;";
        

        [HttpPost]
        [Route("api/doctor")]
        public async Task<string> AddDoctor([FromBody] DoctorModel doctor)
        {
            if (doctor == null)
                return "Invalid input data";
            string query = "INSERT INTO doctor (name, designation, phonenumber,age,gender) VALUES (@name,@designation, @phonenumber, @age,@gender)";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@name", doctor.name);
                    cmd.Parameters.AddWithValue("@designation", doctor.designation);
                    cmd.Parameters.AddWithValue("@phonenumber", doctor.phonenumber);
                    cmd.Parameters.AddWithValue("@age", doctor.age);
                    cmd.Parameters.AddWithValue("@gender", doctor.gender);

                    int result = await cmd.ExecuteNonQueryAsync();

                    return result > 0 ? "Doctor added successfully!" : "Something went wrong!";
                }
            }
        }


        [HttpGet]
        [Route("api/getdoctor")]
        public IHttpActionResult GetDoctors()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = "SELECT * FROM doctor"; // Adjust table name if needed
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<object> doctors = new List<object>();
                        while (reader.Read())
                        {
                            doctors.Add(new
                            {
                                doctor_id = reader["doctor_id"],
                                name = reader["name"],
                                designation = reader["designation"],
                                phonenumber = reader["phonenumber"],
                                age = reader["age"],
                                gender = reader["gender"],
                                patient_count=reader["patient_count"]
                            });
                        }
                        return Ok(doctors);
                    }
                }
            }
        }

        [HttpGet]
        [Route("api/doctors/{id}")]
        public IHttpActionResult GetDoctor(int id)
        {
            Doctor doctor = null;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM doctor WHERE doctor_id = @id", con))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            doctor = new Doctor
                            {  // Ensure correct column index
                                doctor_id = reader.GetInt32(0),
                                name = reader.GetString(1),
                                designation = reader.GetString(2),
                                phonenumber = reader.GetString(3),
                                age = reader.GetInt32(4),
                                gender = reader.GetString(5)
                            };
                        }
                    }
                }
            }

            if (doctor != null)
            {
                return Ok(doctor);
            }
            else
            {
                return NotFound();
            }// Return 404 if patient not found
        }

        [HttpPut]
        [Route("api/updatedoctor/{id}")]
        public IHttpActionResult UpdateDoctor(int id, [FromBody] Doctor doctor)
        {
            if (doctor == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE doctor SET name = @Name, designation = @designation, phoneNumber = @PhoneNumber, age = @Age, gender = @Gender WHERE doctor_id = @doctorId", con))
                    {
                        cmd.Parameters.AddWithValue("@doctorId", id);
                        cmd.Parameters.AddWithValue("@Name", doctor.name);
                        cmd.Parameters.AddWithValue("@designation", doctor.designation);
                        cmd.Parameters.AddWithValue("@PhoneNumber", doctor.phonenumber);
                        cmd.Parameters.AddWithValue("@Age", doctor.age);
                        cmd.Parameters.AddWithValue("@Gender", doctor.gender);

                        con.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        con.Close();

                        if (rowsAffected > 0)
                        {
                            return Ok("Doctor Data updated successfully.");
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

        [HttpDelete]
        [Route("api/deletedoctor/{id}")]
        public IHttpActionResult Deletedoctor(int id)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("DELETE FROM doctor WHERE doctor_id = @id", con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    int rowsAffected = cmd.ExecuteNonQuery(); // Execute delete command

                    if (rowsAffected > 0)
                    {
                        return Ok(new { message = "Doctor deleted successfully." });
                    }
                    else
                    {
                        return NotFound(); // Return 404 if no rows were deleted
                    }
                }
            }
        }


    }
}