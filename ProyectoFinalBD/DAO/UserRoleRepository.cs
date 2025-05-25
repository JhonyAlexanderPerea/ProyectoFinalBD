using System.Collections.Generic;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using ProyectoFinalBD.Model;

namespace ProyectoFinalBD.DAO
{
    public class UserRoleRepository
    {
        private readonly string _connectionString;

        public UserRoleRepository()
        {
            _connectionString = DbConnectionFactory.GetConnectionString();
        }

        public async Task<List<UserRole>> GetAll()
        {
            var roles = new List<UserRole>();
            using var connection = new OracleConnection(_connectionString);
            const string query = "SELECT * FROM RolUsuario";

            using var command = new OracleCommand(query, connection);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                roles.Add(MapUserRoleFromReader(reader));
            }

            return roles;
        }

        public async Task<UserRole> GetById(string userRoleId)
        {
            using var connection = new OracleConnection(_connectionString);
            const string query = "SELECT * FROM RolUsuario WHERE codigoRol = :userRoleId";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add("userRoleId", OracleDbType.Varchar2).Value = userRoleId;

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapUserRoleFromReader(reader);
            }

            return null;
        }

        public async Task Create(UserRole userRole)
        {
            using var connection = new OracleConnection(_connectionString);
            const string query = @"
                INSERT INTO RolUsuario 
                (codigoRol, nombreRol) 
                VALUES 
                (:userRoleId, :name)";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add("userRoleId", OracleDbType.Varchar2).Value = userRole.UserRoleId;
            command.Parameters.Add("name", OracleDbType.Varchar2).Value = userRole.Name;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task Update(UserRole userRole)
        {
            using var connection = new OracleConnection(_connectionString);
            const string query = @"
                UPDATE RolUsuario 
                SET nombreRol = :name
                WHERE codigoRol = :userRoleId";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add("name", OracleDbType.Varchar2).Value = userRole.Name;
            command.Parameters.Add("userRoleId", OracleDbType.Varchar2).Value = userRole.UserRoleId;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task Delete(string userRoleId)
        {
            using var connection = new OracleConnection(_connectionString);
            const string query = "DELETE FROM RolUsuario WHERE codigoRol = :userRoleId";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add("userRoleId", OracleDbType.Varchar2).Value = userRoleId;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        private static UserRole MapUserRoleFromReader(System.Data.IDataReader reader)
        {
            return new UserRole
            {
                UserRoleId = reader["codigoRol"].ToString()!,
                Name = reader["nombreRol"].ToString()!
            };
        }
    }
}