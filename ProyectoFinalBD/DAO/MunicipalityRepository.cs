using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using ProyectoFinalBD.Model;

namespace ProyectoFinalBD.DAO
{
    public class MunicipalityRepository
    {
        private readonly string _connectionString;

        public MunicipalityRepository()
        {
            _connectionString = DbConnectionFactory.GetConnectionString();
        }

        public async Task<List<Municipality>> GetAll()
        {
            var municipalities = new List<Municipality>();
            using var connection = new OracleConnection(_connectionString);
            const string query = "SELECT * FROM Municipio";

            using var command = new OracleCommand(query, connection);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                municipalities.Add(new Municipality
                {
                    MunicipalityId = reader["codigoMunicipio"].ToString()!,
                    Name = reader["nombreMunicipio"].ToString()!,
                    Description = reader["descripcionMunicipio"]?.ToString()
                });
            }

            return municipalities;
        }

        public async Task<Municipality> GetById(string municipalityId)
        {
            using var connection = new OracleConnection(_connectionString);
            const string query = "SELECT * FROM Municipio WHERE codigoMunicipio = :municipalityId";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add("municipalityId", OracleDbType.Varchar2).Value = municipalityId;

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Municipality
                {
                    MunicipalityId = reader["codigoMunicipio"].ToString()!,
                    Name = reader["nombreMunicipio"].ToString()!,
                    Description = reader["descripcionMunicipio"]?.ToString()
                };
            }

            return null;
        }

        public async Task Create(Municipality municipality)
        {
            using var connection = new OracleConnection(_connectionString);
            const string query = @"
                INSERT INTO Municipio 
                (codigoMunicipio, nombreMunicipio, descripcionMunicipio) 
                VALUES 
                (:municipalityId, :name, :description)";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add("municipalityId", OracleDbType.Varchar2).Value = municipality.MunicipalityId;
            command.Parameters.Add("name", OracleDbType.Varchar2).Value = municipality.Name;
            command.Parameters.Add("description", OracleDbType.Varchar2).Value = 
                municipality.Description ?? (object)DBNull.Value;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task Update(Municipality municipality)
        {
            using var connection = new OracleConnection(_connectionString);
            const string query = @"
                UPDATE Municipio 
                SET nombreMunicipio = :name,
                    descripcionMunicipio = :description
                WHERE codigoMunicipio = :municipalityId";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add("name", OracleDbType.Varchar2).Value = municipality.Name;
            command.Parameters.Add("description", OracleDbType.Varchar2).Value = 
                municipality.Description ?? (object)DBNull.Value;
            command.Parameters.Add("municipalityId", OracleDbType.Varchar2).Value = municipality.MunicipalityId;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task Delete(string municipalityId)
        {
            using var connection = new OracleConnection(_connectionString);
            const string query = "DELETE FROM Municipio WHERE codigoMunicipio = :municipalityId";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add("municipalityId", OracleDbType.Varchar2).Value = municipalityId;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }
    }
}