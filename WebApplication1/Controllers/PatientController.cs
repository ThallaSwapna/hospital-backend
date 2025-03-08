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
    public class PatientController : ApiController
    {
        private readonly string connectionString = "Server=DESKTOP-P5400J6;Database=hospital;User Id=sa;Password=admin123$;";
        [HttpPut]
        [Route("api/inactive/{id}")]
        public IHttpActionResult Put(int id,[FromBody] int active)
        {
            
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE addpatient SET active = @active WHERE patient_id = @PatientId", con))
                    {
                        cmd.Parameters.AddWithValue("@PatientId", id);
                        cmd.Parameters.AddWithValue("@active", active);

                        con.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        con.Close();

                        if (rowsAffected > 0)
                        {
                            return Ok("Patient Data updated successfully.");
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
        [Route("api/inactivepatients")]
        public IHttpActionResult GetInactievPatients()
        {
            List<AddPatient> patients = new List<AddPatient>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(
                    @"SELECT p.patient_id, p.name, p.email, p.phonenumber, 
                     p.disease, p.age, p.gender, p.active, p.dueamount, 
                     ISNULL(d.name, 'Not Assigned') AS doctor_name
              FROM addpatient p
              LEFT JOIN doctor d ON p.doctorId = d.doctor_id
              WHERE p.active = 0", con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            patients.Add(new AddPatient
                            {
                                patient_id = reader.GetInt32(0),
                                name = reader.GetString(1),
                                email = reader.GetString(2),
                                phonenumber = reader.GetString(3),
                                disease = reader.GetString(4),
                                age = reader.GetInt32(5),
                                gender = reader.GetString(6),
                                active = reader.GetByte(7),
                                dueamount = reader.GetDecimal(8),
                                doctor_name = reader.GetString(9)  // Now part of AddPatient model
                            });
                        }
                    }
                }
            }

            return Ok(patients.Count > 0 ? patients : new List<AddPatient>());
        }

        /* [HttpGet]
         [Route("api/activepatients")]
         public IHttpActionResult GetActivePatients()
         {
             List<AddPatient> patients = new List<AddPatient>();

             using (SqlConnection con = new SqlConnection(connectionString))
             {
                 con.Open();
                 using (SqlCommand cmd = new SqlCommand("SELECT * FROM addpatient where active=1", con))
                 {
                     using (SqlDataReader reader = cmd.ExecuteReader())
                     {
                         while (reader.Read())
                         {
                             patients.Add(new AddPatient
                             {
                                 patient_id = reader.GetInt32(0),  // Ensure correct column index
                                 name = reader.GetString(1),
                                 email = reader.GetString(2),
                                 phonenumber = reader.GetString(3),
                                 disease = reader.GetString(4),
                                 age = reader.GetInt32(5),
                                 gender = reader.GetString(6),
                                 active = reader.GetByte(7),
                                 dueamount = reader.GetDecimal(8),
                                 doctorId = reader.GetInt32(9),
                             });
                         }
                     }
                 }
             }

             return Ok(patients.Count > 0 ? patients : new List<AddPatient>()); // Return empty list if no data
         }
        */

        [HttpGet]
        [Route("api/activepatients")]
        public IHttpActionResult GetActivePatients()
        {
            List<AddPatient> patients = new List<AddPatient>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(
                    @"SELECT p.patient_id, p.name, p.email, p.phonenumber, 
                     p.disease, p.age, p.gender, p.active, p.dueamount, 
                     ISNULL(d.name, 'Not Assigned') AS doctor_name
              FROM addpatient p
              LEFT JOIN doctor d ON p.doctorId = d.doctor_id
              WHERE p.active = 1", con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            patients.Add(new AddPatient
                            {
                                patient_id = reader.GetInt32(0),
                                name = reader.GetString(1),
                                email = reader.GetString(2),
                                phonenumber = reader.GetString(3),
                                disease = reader.GetString(4),
                                age = reader.GetInt32(5),
                                gender = reader.GetString(6),
                                active = reader.GetByte(7),
                                dueamount = reader.GetDecimal(8),
                                doctor_name = reader.GetString(9)  // Now part of AddPatient model
                            });
                        }
                    }
                }
            }

            return Ok(patients.Count > 0 ? patients : new List<AddPatient>());
        }



        [HttpDelete]
        [Route("api/delete/{id}")]
        public IHttpActionResult DeletePatient(int id)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("DELETE FROM addpatient WHERE patient_id = @id", con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
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

        /*
                [HttpPost]
                [Route("api/addpatient")]
                public async Task<string> AddPatient([FromBody] AddPatient addpatient)
                {
                    if (addpatient == null)
                        return "Invalid input data";
                    string query = "INSERT INTO addpatient (name,email, phonenumber,disease,age,gender) VALUES (@name, @email, @phonenumber, @disease,@age,@gender)";

                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        await con.OpenAsync();
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.Parameters.AddWithValue("@name", addpatient.name);
                            cmd.Parameters.AddWithValue("@email", addpatient.email);
                            cmd.Parameters.AddWithValue("@phonenumber", addpatient.phonenumber);
                            cmd.Parameters.AddWithValue("@disease", addpatient.disease);
                            cmd.Parameters.AddWithValue("@age", addpatient.age);
                            cmd.Parameters.AddWithValue("@gender", addpatient.gender);

                            int result = await cmd.ExecuteNonQueryAsync();

                            return result > 0 ? "Patient Admitted successfully!" : "Something went wrong!";
                        }
                    }
                }
        */

        [HttpPost]
        [Route("api/addpatient")]
        public async Task<string> AddPatient([FromBody] AddPatient addpatient)
        {
            if (addpatient == null)
                return "Invalid input data";

            string query = "INSERT INTO addpatient (name, email, phonenumber, disease, age, gender, doctorId) " +
                           "VALUES (@name, @email, @phonenumber, @disease, @age, @gender, @doctorId)";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@name", addpatient.name);
                    cmd.Parameters.AddWithValue("@email", addpatient.email);
                    cmd.Parameters.AddWithValue("@phonenumber", addpatient.phonenumber);
                    cmd.Parameters.AddWithValue("@disease", addpatient.disease);
                    cmd.Parameters.AddWithValue("@age", addpatient.age);
                    cmd.Parameters.AddWithValue("@gender", addpatient.gender);
                    cmd.Parameters.AddWithValue("@doctorId", addpatient.doctorId); // New field for assigned doctor

                    int result = await cmd.ExecuteNonQueryAsync();
                    return result > 0 ? "Patient Admitted successfully!" : "Something went wrong!";
                }
            }
        }



        [HttpPut]
        [Route("api/updatepatient/{id}")]
        public IHttpActionResult UpdatePatient(int id, [FromBody] AddPatient patient)
        {
            if (patient == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE addpatient SET name = @Name, email = @Email, phoneNumber = @PhoneNumber, disease = @Disease, age = @Age, gender = @Gender, doctorId = @doctorId WHERE patient_id = @PatientId", con))
                    {
                        cmd.Parameters.AddWithValue("@PatientId", id);
                        cmd.Parameters.AddWithValue("@Name", patient.name);
                        cmd.Parameters.AddWithValue("@Email", patient.email);
                        cmd.Parameters.AddWithValue("@PhoneNumber", patient.phonenumber);
                        cmd.Parameters.AddWithValue("@Disease", patient.disease);
                        cmd.Parameters.AddWithValue("@Age", patient.age);
                        cmd.Parameters.AddWithValue("@Gender", patient.gender);
                        cmd.Parameters.AddWithValue("@doctorId", patient.doctorId);

                        con.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        con.Close();

                        if (rowsAffected > 0)
                        {
                            return Ok("Patient Data updated successfully.");
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
        [Route("api/patients/{id}")]
        public IHttpActionResult GetPatientById(int id)
        {
            AddPatient patient = null;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM addpatient WHERE patient_id = @id", con))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            patient = new AddPatient
                            {  // Ensure correct column index
                                patient_id = reader.GetInt32(0),
                                name = reader.GetString(1),
                                email = reader.GetString(2),
                                phonenumber = reader.GetString(3),
                                disease = reader.GetString(4),
                                age = reader.GetInt32(5),
                                gender = reader.GetString(6),
                                doctorId = reader.GetInt32(9),
                            };
                        }
                    }
                }
            }

            if (patient != null)
            {
                return Ok(patient);
            }
            else
            {
                return NotFound();
            }// Return 404 if patient not found
        }
    }
}