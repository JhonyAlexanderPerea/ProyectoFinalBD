using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using ProyectoFinalBD.Model;

namespace ProyectoFinalBD.DAO
{
    public class EquipmentStatusRepository
    {
        private readonly string _connectionString;

        public EquipmentStatusRepository()
        {
            _connectionString = DbConnectionFactory.GetConnectionString();
        }

        public async Task<List<EquipmentStatus>> GetAll()
        {
            var statuses = new List<EquipmentStatus>();
            using var connection = new OracleConnection(_connectionString);
            const string query = "SELECT codigoEstadoEquipo, nombreEstadoEquipo FROM EstadoEquipo";

            using var command = new OracleCommand(query, connection);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                statuses.Add(new EquipmentStatus
                {
                    EquipmentStatusId = reader["codigoEstadoEquipo"].ToString()!,
                    Name = reader["nombreEstadoEquipo"].ToString()!
                });
            }

            return statuses;
        }

        public async Task<EquipmentStatus?> GetById(string statusId)
        {
            using var connection = new OracleConnection(_connectionString);
            const string query = "SELECT codigoEstadoEquipo, nombreEstadoEquipo FROM EstadoEquipo WHERE codigoEstadoEquipo = :statusId";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add("statusId", OracleDbType.Varchar2).Value = statusId;

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new EquipmentStatus
                {
                    EquipmentStatusId = reader["codigoEstadoEquipo"].ToString()!,
                    Name = reader["nombreEstadoEquipo"].ToString()!
                };
            }

            return null;
        }

        public async Task Create(EquipmentStatus status)
        {
            using var connection = new OracleConnection(_connectionString);
            const string query = @"
                INSERT INTO EstadoEquipo 
                (codigoEstadoEquipo, nombreEstadoEquipo) 
                VALUES 
                (:statusId, :name)";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add("statusId", OracleDbType.Varchar2).Value = status.EquipmentStatusId;
            command.Parameters.Add("name", OracleDbType.Varchar2).Value = status.Name;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task Update(EquipmentStatus status)
        {
            using var connection = new OracleConnection(_connectionString);
            const string query = @"
                UPDATE EstadoEquipo 
                SET nombreEstadoEquipo = :name 
                WHERE codigoEstadoEquipo = :statusId";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add("name", OracleDbType.Varchar2).Value = status.Name;
            command.Parameters.Add("statusId", OracleDbType.Varchar2).Value = status.EquipmentStatusId;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task<bool> Delete(string equipmentStatusId)
        {
            try
            {
                using var connection = new OracleConnection(_connectionString);
                await connection.OpenAsync();

                // Query corregida usando el nombre exacto de columna de tu esquema
                const string deleteQuery = @"
            DELETE FROM EstadoEquipo 
            WHERE codigoEstadoEquipo = :CODIGO";

                using var command = new OracleCommand(deleteQuery, connection);
                command.Parameters.Add("CODIGO", OracleDbType.Varchar2).Value = equipmentStatusId;

                int rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch (OracleException ex) when (ex.Number == 2292) // Violación de integridad referencial
            {
                Console.WriteLine($"Error al eliminar: Existen equipos asociados a este estado. {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar EquipmentStatus: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> Exists(string statusId)
        {
            using var connection = new OracleConnection(_connectionString);
            const string query = "SELECT COUNT(1) FROM EstadoEquipo WHERE codigoEstadoEquipo = :CODIGOESTADOEQUIPO";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add("CODIGOESTADOEQUIPO", OracleDbType.Varchar2).Value = statusId;

            await connection.OpenAsync();
            return Convert.ToInt32(await command.ExecuteScalarAsync()) > 0;
        }
    }
}
