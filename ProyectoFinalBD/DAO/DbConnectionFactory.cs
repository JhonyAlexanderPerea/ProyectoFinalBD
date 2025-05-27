using System;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;

namespace ProyectoFinalBD.DAO
{
    public static class DbConnectionFactory
    {
        public static string GetConnectionString()
        {
            return "User Id=sys;Password=admin;Data Source=localhost:1521/xe;DBA Privilege=SYSDBA";
        }

        public static async Task TestConnection()
        {
            string connectionString = GetConnectionString();
            using var connection = new OracleConnection(connectionString);
            try
            {
                await connection.OpenAsync();
                // Si llega aquí, la conexión fue exitosa
                Console.WriteLine("Conexión exitosa a SmartLoan");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error de conexión a la base de datos: {ex.Message}");
            }
        }
    }
}