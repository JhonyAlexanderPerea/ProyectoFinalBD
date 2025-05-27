using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using ProyectoFinalBD.Model;

namespace ProyectoFinalBD.DAO
{
    public class EquipmentTypeRepository
    {
        private readonly string _connectionString;

        public EquipmentTypeRepository()
        {
            _connectionString = DbConnectionFactory.GetConnectionString();
        }

        public async Task<List<EquipmentType>> GetAll()
        {
            var types = new List<EquipmentType>();
            using var connection = new OracleConnection(_connectionString);
            const string query = "SELECT codigoTipoEquipo, nombreTipoEquipo, descripcionTipoEquipo FROM TipoEquipo";

            using var command = new OracleCommand(query, connection);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                types.Add(new EquipmentType
                {
                    EquipmentTypeId = reader["codigoTipoEquipo"].ToString()!,
                    Name = reader["nombreTipoEquipo"].ToString()!,
                    Description = reader["descripcionTipoEquipo"]?.ToString()
                });
            }

            return types;
        }

        public async Task<EquipmentType?> GetById(string typeId)
        {
            using var connection = new OracleConnection(_connectionString);
            const string query = "SELECT codigoTipoEquipo, nombreTipoEquipo, descripcionTipoEquipo FROM TipoEquipo WHERE codigoTipoEquipo = :typeId";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add("typeId", OracleDbType.Varchar2).Value = typeId;

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new EquipmentType
                {
                    EquipmentTypeId = reader["codigoTipoEquipo"].ToString()!,
                    Name = reader["nombreTipoEquipo"].ToString()!,
                    Description = reader["descripcionTipoEquipo"]?.ToString()
                };
            }

            return null;
        }

        public async Task Create(EquipmentType type)
        {
            using var connection = new OracleConnection(_connectionString);
            const string query = @"
                INSERT INTO TipoEquipo 
                (codigoTipoEquipo, nombreTipoEquipo, descripcionTipoEquipo) 
                VALUES 
                (:typeId, :name, :description)";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add("typeId", OracleDbType.Varchar2).Value = type.EquipmentTypeId;
            command.Parameters.Add("name", OracleDbType.Varchar2).Value = type.Name;
            command.Parameters.Add("description", OracleDbType.Varchar2).Value = 
                type.Description ?? (object)DBNull.Value;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task Update(EquipmentType type)
        {
            using var connection = new OracleConnection(_connectionString);
            const string query = @"
                UPDATE TipoEquipo 
                SET nombreTipoEquipo = :name,
                    descripcionTipoEquipo = :description
                WHERE codigoTipoEquipo = :typeId";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add("name", OracleDbType.Varchar2).Value = type.Name;
            command.Parameters.Add("description", OracleDbType.Varchar2).Value = 
                type.Description ?? (object)DBNull.Value;
            command.Parameters.Add("typeId", OracleDbType.Varchar2).Value = type.EquipmentTypeId;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task Delete(string typeId)
        {
            using var connection = new OracleConnection(_connectionString);
            const string query = "DELETE FROM TipoEquipo WHERE codigoTipoEquipo = :typeId";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add("typeId", OracleDbType.Varchar2).Value = typeId;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task<bool> Exists(string typeId)
        {
            using var connection = new OracleConnection(_connectionString);
            const string query = "SELECT COUNT(1) FROM TipoEquipo WHERE codigoTipoEquipo = :typeId";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add("typeId", OracleDbType.Varchar2).Value = typeId;

            await connection.OpenAsync();
            return Convert.ToInt32(await command.ExecuteScalarAsync()) > 0;
        }
    }
}
