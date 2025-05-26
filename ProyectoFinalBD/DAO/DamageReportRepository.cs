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
            command.CommandText = "SELECT * FROM DANIO"; // Nombre real de la tabla

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
            command.CommandText = "SELECT * FROM DAMAGE_REPORTS WHERE DAMAGE_REPORT_ID = :id";
            command.Parameters.Add(new OracleParameter("id", id));

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new DamageReport
                {
                    DamageReportId = reader["DAMAGE_REPORT_ID"].ToString(),
                    Date = Convert.ToDateTime(reader["DATE"]),
                    Cause = reader["CAUSE"].ToString(),
                    Description = reader["DESCRIPTION"]?.ToString(),
                    EquipmentId = reader["EQUIPMENT_ID"]?.ToString()
                };
            }

            return null;
        }

        public async Task Create(DamageReport report)
        {
            using var connection = new OracleConnection(_connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO DAMAGE_REPORTS 
                (DAMAGE_REPORT_ID, DATE, CAUSE, DESCRIPTION, EQUIPMENT_ID) 
                VALUES (:id, :date, :cause, :description, :equipmentId)";

            command.Parameters.Add(new OracleParameter("id", report.DamageReportId));
            command.Parameters.Add(new OracleParameter("date", report.Date));
            command.Parameters.Add(new OracleParameter("cause", report.Cause));
            command.Parameters.Add(new OracleParameter("description", report.Description ?? (object)DBNull.Value));
            command.Parameters.Add(new OracleParameter("equipmentId", report.EquipmentId ?? (object)DBNull.Value));

            await command.ExecuteNonQueryAsync();
        }

        public async Task Update(DamageReport report)
        {
            using var connection = new OracleConnection(_connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = @"UPDATE DAMAGE_REPORTS 
                SET DATE = :date, 
                    CAUSE = :cause, 
                    DESCRIPTION = :description, 
                    EQUIPMENT_ID = :equipmentId 
                WHERE DAMAGE_REPORT_ID = :id";

            command.Parameters.Add(new OracleParameter("date", report.Date));
            command.Parameters.Add(new OracleParameter("cause", report.Cause));
            command.Parameters.Add(new OracleParameter("description", report.Description ?? (object)DBNull.Value));
            command.Parameters.Add(new OracleParameter("equipmentId", report.EquipmentId ?? (object)DBNull.Value));
            command.Parameters.Add(new OracleParameter("id", report.DamageReportId));

            await command.ExecuteNonQueryAsync();
        }

        public async Task Delete(string id)
        {
            using var connection = new OracleConnection(_connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM DAMAGE_REPORTS WHERE DAMAGE_REPORT_ID = :id";
            command.Parameters.Add(new OracleParameter("id", id));

            await command.ExecuteNonQueryAsync();
        }
    }
}