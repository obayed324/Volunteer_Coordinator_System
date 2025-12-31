using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Volunteer_Coordinator_System
{
    public class Result
    {
        public DataTable Data { get; set; }
        public bool HasError { get; set; }
        public string Message { get; set; }
    }

    internal class DbHelper
    {
        private static string connectionString = "Data Source=R-01\\SQLEXPRESS;Initial Catalog=DisasterReliefDB;Integrated Security=True;TrustServerCertificate=True";

        public static Result GetQueryData(string query, Dictionary<string, object> parameters = null)
        {
            var result = new Result();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    if (parameters != null)
                        foreach (var p in parameters)
                            cmd.Parameters.AddWithValue(p.Key, p.Value);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    result.Data = dt;
                }
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Message = ex.Message;
            }
            return result;
        }

        public static Result ExecuteNonQuery(string query, Dictionary<string, object> parameters = null)
        {
            var result = new Result();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    if (parameters != null)
                        foreach (var p in parameters)
                            cmd.Parameters.AddWithValue(p.Key, p.Value);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Message = ex.Message;
            }
            return result;
        }
    }
}
