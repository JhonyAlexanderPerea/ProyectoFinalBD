using System;
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

            try
            {
                using var connection = new OracleConnection(_connectionString);
                // Especificar columnas explícitamente para evitar problemas
                const string query = "SELECT codigoRolUsuario, nombreRolUsuario FROM RolUsuario";

                using var command = new OracleCommand(query, connection);
                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    roles.Add(new UserRole
                    {
                        // Asegúrate que estas propiedades existan en tu clase UserRole
                        UserRoleId = reader["codigoRolUsuario"].ToString(),
                        Name = reader["nombreRolUsuario"].ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving user roles: {ex.Message}");
                throw; // Relanzar para manejo superior
            }

            return roles;
        }

        public async Task<UserRole?> GetById(string userRoleId)
        {
            try
            {
                using var connection = new OracleConnection(_connectionString);
                const string query = "SELECT * FROM RolUsuario WHERE codigoRolUsuario = :userRoleId";

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
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetById UserRole: {ex.Message}");
                throw;
            }
        }

        public async Task Create(UserRole userRole)
        {
            try
            {
                using var connection = new OracleConnection(_connectionString);
                const string query = @"
                    INSERT INTO RolUsuario 
                    (codigoRolUsuario, nombreRol) 
                    VALUES 
                    (:userRoleId, :name)";

                using var command = new OracleCommand(query, connection);
                command.Parameters.Add("userRoleId", OracleDbType.Varchar2).Value = userRole.UserRoleId;
                command.Parameters.Add("name", OracleDbType.Varchar2).Value = userRole.Name;

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Create UserRole: {ex.Message}");
                throw;
            }
        }

        public async Task Update(UserRole userRole)
        {
            try
            {
                using var connection = new OracleConnection(_connectionString);
                const string query = @"
                    UPDATE RolUsuario 
                    SET nombreRol = :name
                    WHERE codigoRolUsuario = :userRoleId";

                using var command = new OracleCommand(query, connection);
                command.Parameters.Add("name", OracleDbType.Varchar2).Value = userRole.Name;
                command.Parameters.Add("userRoleId", OracleDbType.Varchar2).Value = userRole.UserRoleId;

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Update UserRole: {ex.Message}");
                throw;
            }
        }

        public async Task Delete(string userRoleId)
        {
            try
            {
                using var connection = new OracleConnection(_connectionString);
                const string query = "DELETE FROM RolUsuario WHERE codigoRolUsuario = :userRoleId";

                using var command = new OracleCommand(query, connection);
                command.Parameters.Add("userRoleId", OracleDbType.Varchar2).Value = userRoleId;

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Delete UserRole: {ex.Message}");
                throw;
            }
        }

        private static UserRole MapUserRoleFromReader(System.Data.IDataReader reader)
        {
            return new UserRole
            {
                UserRoleId = reader["codigoRolUsuario"].ToString()!,
                Name = reader["nombreRol"].ToString()!
            };
        }
    }
}
