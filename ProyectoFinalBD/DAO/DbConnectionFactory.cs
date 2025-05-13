/*using System.Data.SqlClient;
using System.Configuration;

namespace ProyectoFinalBD.DAO;

public class DbConnectionFactory
{
    public static class DbConnectionFactory
    {
        public static SqlConnection CreateConnection()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SqlConnection"].ConnectionString;
            return new SqlConnection(connectionString);
        }
    }
}*/