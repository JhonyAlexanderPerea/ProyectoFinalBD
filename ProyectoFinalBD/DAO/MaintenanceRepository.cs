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
                    CODIGOMANTENIMIENTO,
                    FECHAMANTENIMIENTO,
                    HALLAZGOSMANTENIMIENTO,
                    COSTOMANTENIMIENTO,
                    EQUIPO
                FROM MANTENIMIENTO";

            try
            {
                await connection.OpenAsync();
                using var command = new OracleCommand(query, connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    maintenances.Add(MapMaintenanceFromReader(reader));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetAll: {ex}");
                throw;
            }

            return maintenances;
        }

        public async Task<Maintenance> GetById(string maintenanceId)
        {
            using var connection = new OracleConnection(_connectionString);
            const string query = @"
                SELECT 
                    CODIGOMANTENIMIENTO,
                    FECHAMANTENIMIENTO,
                    HALLAZGOSMANTENIMIENTO,
                    COSTOMANTENIMIENTO,
                    EQUIPO
                FROM MANTENIMIENTO
                WHERE CODIGOMANTENIMIENTO = :maintenanceId";

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
    // Validaciones según esquema
    if (string.IsNullOrWhiteSpace(maintenance.MaintenanceId))
        throw new ArgumentException("El código de mantenimiento es obligatorio");
    
    if (maintenance.MaintenanceId.Length > 10)
        throw new ArgumentException("El código no puede exceder 10 caracteres");

    if (maintenance.Cost < 0)
        throw new ArgumentException("El costo no puede ser negativo");

    using var connection = new OracleConnection(_connectionString);
    
    // Query 100% compatible con el esquema
    const string query = @"
        INSERT INTO MANTENIMIENTO (
            CODIGOMANTENIMIENTO,    -- VARCHAR2(10) PRIMARY KEY
            FECHAMANTENIMIENTO,     -- DATE NOT NULL
            HALLAZGOSMANTENIMIENTO, -- CLOB
            COSTOMANTENIMIENTO,     -- NUMBER(12,2) NOT NULL
            EQUIPO                  -- VARCHAR2(10) (FK)
        ) VALUES (
            :CodigoMantenimiento,
            :FechaMantenimiento,
            :Hallazgos,
            :Costo,
            :Equipo
        )";

    try
    {
        await connection.OpenAsync();
        
        using var command = new OracleCommand(query, connection);
        
        // Parámetros exactos como en el esquema
        command.Parameters.Add("CodigoMantenimiento", OracleDbType.Varchar2, 10).Value = maintenance.MaintenanceId;
        
        command.Parameters.Add("FechaMantenimiento", OracleDbType.Date).Value = maintenance.Date;
        
        command.Parameters.Add("Hallazgos", OracleDbType.Clob).Value = 
            string.IsNullOrWhiteSpace(maintenance.Findings) ? (object)DBNull.Value : maintenance.Findings;
        
        command.Parameters.Add("Costo", OracleDbType.Decimal, 12).Value = maintenance.Cost;
        command.Parameters["Costo"].Scale = 2; // Para NUMBER(12,2)
        
        command.Parameters.Add("Equipo", OracleDbType.Varchar2, 10).Value = 
            string.IsNullOrWhiteSpace(maintenance.EquipmentId) ? (object)DBNull.Value : maintenance.EquipmentId;

        await command.ExecuteNonQueryAsync();
    }
    catch (OracleException ex) when (ex.Number == 1)
    {
        throw new Exception($"Ya existe un mantenimiento con código {maintenance.MaintenanceId}", ex);
    }
    catch (OracleException ex) when (ex.Number == 2291)
    {
        throw new Exception($"El equipo {maintenance.EquipmentId} no existe", ex);
    }
}

        public async Task Update(Maintenance maintenance)
        {
            using var connection = new OracleConnection(_connectionString);
            const string query = @"
                UPDATE MANTENIMIENTO 
                SET FECHAMANTENIMIENTO = :FechaMantenimiento,
                    HALLAZGOSMANTENIMIENTO = :Hallazgos,
                    COSTOMANTENIMIENTO = :Costo,
                    EQUIPO = :Equipo
                WHERE CODIGOMANTENIMIENTO = :CodigoMantenimiento";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add("FechaMantenimiento", OracleDbType.Date).Value = maintenance.Date;
            command.Parameters.Add("Hallazgos", OracleDbType.Varchar2).Value = 
                maintenance.Findings ?? (object)DBNull.Value;
            command.Parameters.Add("Costo", OracleDbType.Decimal).Value = maintenance.Cost;
            command.Parameters.Add("Equipo", OracleDbType.Varchar2).Value = 
                maintenance.EquipmentId ?? (object)DBNull.Value;
            command.Parameters.Add("CodigoMantenimiento", OracleDbType.Varchar2).Value = maintenance.MaintenanceId;

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
                MaintenanceId = reader["CODIGOMANTENIMIENTO"].ToString()!,
                Date = Convert.ToDateTime(reader["FECHAMANTENIMIENTO"]),
                Findings = reader["HALLAZGOSMANTENIMIENTO"]?.ToString(),
                Cost = Convert.ToDecimal(reader["COSTOMANTENIMIENTO"]),
                EquipmentId = reader["EQUIPO"]?.ToString()
            };
        }
    }
}
