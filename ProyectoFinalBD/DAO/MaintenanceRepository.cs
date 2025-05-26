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
                SELECT 
                    m.CODIGOMANTENIMIENTO AS MaintenanceId,
                    m.FECHAMANTENIMIENTO AS Date,
                    m.HALLAZGOSMANTENIMIENTO AS Findings,
                    m.COSTOMANTENIMIENTO AS Cost,
                    m.EQUIPO AS EquipmentId
                FROM MANTENIMIENTO m";

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
                SELECT 
                    m.CODIGOMANTENIMIENTO AS MaintenanceId,
                    m.FECHAMANTENIMIENTO AS Date,
                    m.HALLAZGOSMANTENIMIENTO AS Findings,
                    m.COSTOMANTENIMIENTO AS Cost,
                    m.EQUIPO AS EquipmentId
                FROM MANTENIMIENTO m
                WHERE m.CODIGOMANT = :maintenanceId";

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
                INSERT INTO MANTENIMIENTO 
                (CODIGOMANTENIMIENTO, FECHAMANTENIMIENTO, HALLAZGOSMANTENIMIENTO, COSTOMANTENIMIENTO, EQUIPO) 
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
                UPDATE MANTENIMIENTO 
                SET FECHAMANTIMIENTO = :date,
                    HALLAZGOSMANTENIMIENTO = :findings,
                    COSTOMANTENIMIENTO = :cost,
                    EQUIPO = :equipmentId
                WHERE CODIGOMANT = :maintenanceId";

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
            const string query = "DELETE FROM MANTENIMIENTO WHERE CODIGOMANTENIMIENTO = :maintenanceId";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add("maintenanceId", OracleDbType.Varchar2).Value = maintenanceId;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        private static Maintenance MapMaintenanceFromReader(System.Data.IDataReader reader)
        {
            return new Maintenance
            {
                MaintenanceId = reader["MaintenanceId"].ToString()!,
                Date = Convert.ToDateTime(reader["Date"]),
                Findings = reader["Findings"]?.ToString(),
                Cost = Convert.ToDecimal(reader["Cost"]),
                EquipmentId = reader["EquipmentId"]?.ToString()
            };
        }
    }
}