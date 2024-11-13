using Microsoft.Data.SqlClient;
using Second_Step.Models;

namespace Second_Step.Data
{
    public class EmployeeRepos
    {
        private readonly string _connectionString;

        public EmployeeRepos(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Employee> GetEmployees()
        {
            var employees = new List<Employee>();

            string query = "SELECT Id, Payroll_Number, Forenames, Surname, Date_of_Birth, Telephone, Mobile, Address, " +
                           "Address_2, Postcode, EMail_Home, Start_Date FROM Employees";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            employees.Add(new Employee
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Payroll_Number = reader.GetString(reader.GetOrdinal("Payroll_Number")),
                                Forenames = reader.GetString(reader.GetOrdinal("Forenames")),
                                Surname = reader.GetString(reader.GetOrdinal("Surname")),
                                Date_of_Birth = reader.GetDateTime(reader.GetOrdinal("Date_of_Birth")),
                                Telephone = reader.GetString(reader.GetOrdinal("Telephone")),
                                Mobile = reader.GetString(reader.GetOrdinal("Mobile")),
                                Address = reader.GetString(reader.GetOrdinal("Address")),
                                Address_2 = reader.GetString(reader.GetOrdinal("Address_2")),
                                Postcode = reader.GetString(reader.GetOrdinal("Postcode")),
                                EMail_Home = reader.GetString(reader.GetOrdinal("EMail_Home")),
                                Start_Date = reader.GetDateTime(reader.GetOrdinal("Start_Date"))
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }

            return employees;
        }
    }
}
