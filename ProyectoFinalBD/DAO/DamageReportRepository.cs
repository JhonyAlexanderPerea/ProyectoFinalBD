using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using ProyectoFinalBD.Model;

namespace ProyectoFinalBD.DAO
{
    public class DamageReportRepository
    {
        private readonly string _connectionString;

        public DamageReportRepository()
        {
            _connectionString = DbConnectionFactory.GetConnectionString();
        }

        public async Task<List<DamageReport>> GetAll()
        {
            var reports = new List<DamageReport>();
            using var connection = new OracleConnection(_connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM DANIO";

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                reports.Add(new DamageReport
                {
                    DamageReportId = reader["CODIGODANIO"].ToString(),
                    Date = Convert.ToDateTime(reader["FECHADANIO"]),
                    Cause = reader["CAUSADANIO"].ToString(),
                    Description = reader["DESCRIPCIONDANIO"]?.ToString(),
                    EquipmentId = reader["EQUIPO"]?.ToString()
                });
            }

            return reports;
        }

        public async Task<DamageReport> GetById(string id)
        {
            using var connection = new OracleConnection(_connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM DANIO WHERE CODIGODANIO = :id";
            command.Parameters.Add(new OracleParameter("id", id));

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new DamageReport
                {
                    DamageReportId = reader["CODIGODANIO"].ToString(),
                    Date = Convert.ToDateTime(reader["FECHADANIO"]),
                    Cause = reader["CAUSADANIO"].ToString(),
                    Description = reader["DESCRIPCIONDANIO"]?.ToString(),
                    EquipmentId = reader["EQUIPO"]?.ToString()
                };
            }

            return null;
        }

        public async Task Create(DamageReport report)
        {
            using var connection = new OracleConnection(_connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO DANIO 
                (CODIGODANIO, FECHADANIO, CAUSADANIO, DESCRIPCIONDANIO, EQUIPO) 
                VALUES (:codigoDanio, :fechaDanio, :causaDanio, :descripcionDanio, :equipo)";

            command.Parameters.Add(new OracleParameter("codigoDanio", report.DamageReportId));
            command.Parameters.Add(new OracleParameter("fechaDanio", report.Date));
            command.Parameters.Add(new OracleParameter("causaDanio", report.Cause));
            command.Parameters.Add(new OracleParameter("descripcionDanio",
                string.IsNullOrEmpty(report.Description) ? (object)DBNull.Value : report.Description));
            command.Parameters.Add(new OracleParameter("equipo",
                string.IsNullOrEmpty(report.EquipmentId) ? (object)DBNull.Value : report.EquipmentId));

            await command.ExecuteNonQueryAsync();
        }

        public async Task Update(DamageReport report)
        {
            using var connection = new OracleConnection(_connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = @"UPDATE DANIO 
                SET FECHADANIO = :fechaDanio, 
                    CAUSADANIO = :causaDanio, 
                    DESCRIPCIONDANIO = :descripcionDanio, 
                    EQUIPO = :equipo 
                WHERE CODIGODANIO = :codigoDanio";

            command.Parameters.Add(new OracleParameter("fechaDanio", report.Date));
            command.Parameters.Add(new OracleParameter("causaDanio", report.Cause));
            command.Parameters.Add(new OracleParameter("descripcionDanio", report.Description ?? (object)DBNull.Value));
            command.Parameters.Add(new OracleParameter("equipo", report.EquipmentId ?? (object)DBNull.Value));
            command.Parameters.Add(new OracleParameter("codigoDanio", report.DamageReportId));

            await command.ExecuteNonQueryAsync();
        }

        public async Task Delete(string id)
        {
            using var connection = new OracleConnection(_connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM DANIO WHERE CODIGODANIO = :id";
            command.Parameters.Add(new OracleParameter("id", id));

            await command.ExecuteNonQueryAsync();
        }
    }
}
