using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using ProyectoFinalBD.Model;

namespace ProyectoFinalBD.DAO
{
    public class MaintenanceRepository
    {
        private readonly string _connectionString;

        public MaintenanceRepository()
        {
            _connectionString = DbConnectionFactory.GetConnectionString();
        }

        public async Task<List<Maintenance>> GetAll()
        {
            var maintenances = new List<Maintenance>();
            using var connection = new OracleConnection(_connectionString);
            const string query = @"
                SELECT m.*, e.nombreEquipo as equipment_name
                FROM Mantenimiento m
                JOIN Equipo e ON m.equipo = e.codigoEquipo";

            using var command = new OracleCommand(query, connection);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                maintenances.Add(MapMaintenanceFromReader(reader));
            }

            return maintenances;
        }

        public async Task<Maintenance> GetById(string maintenanceId)
        {
            using var connection = new OracleConnection(_connectionString);
            const string query = @"
                SELECT m.*, e.nombreEquipo as equipment_name
                FROM Mantenimiento m
                JOIN Equipo e ON m.equipo = e.codigoEquipo
                WHERE m.codigoMantenimiento = :maintenanceId";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add("maintenanceId", OracleDbType.Varchar2).Value = maintenanceId;

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapMaintenanceFromReader(reader);
            }

            return null;
        }

        public async Task Create(Maintenance maintenance)
        {
            using var connection = new OracleConnection(_connectionString);
            const string query = @"
                INSERT INTO Mantenimiento 
                (codigoMantenimiento, fecha, hallazgos, costo, equipo) 
                VALUES 
                (:maintenanceId, :date, :findings, :cost, :equipmentId)";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add("maintenanceId", OracleDbType.Varchar2).Value = maintenance.MaintenanceId;
            command.Parameters.Add("date", OracleDbType.Date).Value = maintenance.Date;
            command.Parameters.Add("findings", OracleDbType.Varchar2).Value = 
                maintenance.Findings ?? (object)DBNull.Value;
            command.Parameters.Add("cost", OracleDbType.Decimal).Value = maintenance.Cost;
            command.Parameters.Add("equipmentId", OracleDbType.Varchar2).Value = 
                maintenance.EquipmentId ?? (object)DBNull.Value;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task Update(Maintenance maintenance)
        {
            using var connection = new OracleConnection(_connectionString);
            const string query = @"
                UPDATE Mantenimiento 
                SET fecha = :date,
                    hallazgos = :findings,
                    costo = :cost,
                    equipo = :equipmentId
                WHERE codigoMantenimiento = :maintenanceId";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add("date", OracleDbType.Date).Value = maintenance.Date;
            command.Parameters.Add("findings", OracleDbType.Varchar2).Value = 
                maintenance.Findings ?? (object)DBNull.Value;
            command.Parameters.Add("cost", OracleDbType.Decimal).Value = maintenance.Cost;
            command.Parameters.Add("equipmentId", OracleDbType.Varchar2).Value = 
                maintenance.EquipmentId ?? (object)DBNull.Value;
            command.Parameters.Add("maintenanceId", OracleDbType.Varchar2).Value = maintenance.MaintenanceId;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task Delete(string maintenanceId)
        {
            using var connection = new OracleConnection(_connectionString);
            const string query = "DELETE FROM Mantenimiento WHERE codigoMantenimiento = :maintenanceId";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add("maintenanceId", OracleDbType.Varchar2).Value = maintenanceId;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        private static Maintenance MapMaintenanceFromReader(System.Data.IDataReader reader)
        {
            return new Maintenance
            {
                MaintenanceId = reader["codigoMantenimiento"].ToString()!,
                Date = Convert.ToDateTime(reader["fecha"]),
                Findings = reader["hallazgos"]?.ToString(),
                Cost = Convert.ToDecimal(reader["costo"]),
                EquipmentId = reader["equipo"]?.ToString(),
                Equipment = reader["equipo"] != DBNull.Value ? new Equipment 
                { 
                    Name = reader["equipment_name"].ToString()! 
                } : null
            };
        }
    }
}