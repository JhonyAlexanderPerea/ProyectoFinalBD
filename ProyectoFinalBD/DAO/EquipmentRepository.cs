using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using ProyectoFinalBD.Model;

namespace ProyectoFinalBD.DAO
{
    public class EquipmentRepository
    {
        private readonly string _connectionString;

        public EquipmentRepository()
        {
            _connectionString = DbConnectionFactory.GetConnectionString();
        }

        public async Task<List<Equipment>> GetAll()
        {
            using var connection = new OracleConnection(_connectionString);
            const string query = @"
                SELECT 
                    e.CODIGOEQUIPO,
                    e.NOMBREEQUIPO,
                    e.COSTOEQUIPO,
                    e.CARACTERISTICASEQUIPO,
                    e.TIPOEQUIPO,
                    e.UBICACION,
                    e.ESTADOEQUIPO,
                    e.PROVEEDOR,
                    te.NOMBRETIPOEQUIPO AS ""equipment_type_name"",
                    u.LUGARUBICACION AS ""location_name"",
                    ee.NOMBREESTADOEQUIPO AS ""status_name"",
                    p.NOMBREPROVEEDOR AS ""supplier_name""
                FROM 
                    EQUIPO e
                    LEFT JOIN TIPOEQUIPO te ON e.TIPOEQUIPO = te.CODIGOTIPOEQUIPO
                    LEFT JOIN UBICACION u ON e.UBICACION = u.CODIGOUBICACION
                    LEFT JOIN ESTADOEQUIPO ee ON e.ESTADOEQUIPO = ee.CODIGOESTADOEQUIPO
                    LEFT JOIN PROVEEDOR p ON e.PROVEEDOR = p.CODIGOPROVEEDOR";

            using var command = new OracleCommand(query, connection);
            var equipments = new List<Equipment>();

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                equipments.Add(MapEquipmentFromReader(reader));
            }

            return equipments;
        }

        public async Task<Equipment?> GetById(string equipmentId)
        {
            using var connection = new OracleConnection(_connectionString);
            const string query = @"
                SELECT 
                    e.CODIGOEQUIPO,
                    e.NOMBREEQUIPO,
                    e.COSTOEQUIPO,
                    e.CARACTERISTICASEQUIPO,
                    e.TIPOEQUIPO,
                    e.UBICACION,
                    e.ESTADOEQUIPO,
                    e.PROVEEDOR,
                    te.NOMBRETIPOEQUIPO AS ""equipment_type_name"",
                    u.LUGARUBICACION AS ""location_name"",
                    ee.NOMBREESTADOEQUIPO AS ""status_name"",
                    p.NOMBREPROVEEDOR AS ""supplier_name""
                FROM 
                    EQUIPO e
                    LEFT JOIN TIPOEQUIPO te ON e.TIPOEQUIPO = te.CODIGOTIPOEQUIPO
                    LEFT JOIN UBICACION u ON e.UBICACION = u.CODIGOUBICACION
                    LEFT JOIN ESTADOEQUIPO ee ON e.ESTADOEQUIPO = ee.CODIGOESTADOEQUIPO
                    LEFT JOIN PROVEEDOR p ON e.PROVEEDOR = p.CODIGOPROVEEDOR
                WHERE 
                    e.CODIGOEQUIPO = :equipmentId";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add("equipmentId", OracleDbType.Varchar2).Value = equipmentId;

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
                return MapEquipmentFromReader(reader);

            return null;
        }

        public async Task Create(Equipment equipment)
        {
            using var connection = new OracleConnection(_connectionString);
            const string query = @"
                INSERT INTO EQUIPO 
                    (CODIGOEQUIPO, NOMBREEQUIPO, COSTOEQUIPO, CARACTERISTICASEQUIPO, TIPOEQUIPO, UBICACION, ESTADOEQUIPO, PROVEEDOR)
                VALUES 
                    (:equipmentId, :name, :cost, :features, :equipmentTypeId, :locationId, :statusId, :supplierId)";

            using var command = new OracleCommand(query, connection);

            command.Parameters.Add("equipmentId", OracleDbType.Varchar2).Value = equipment.EquipmentId;
            command.Parameters.Add("name", OracleDbType.Varchar2).Value = equipment.Name;
            command.Parameters.Add("cost", OracleDbType.Decimal).Value = equipment.Cost;
            command.Parameters.Add("features", OracleDbType.Varchar2).Value = equipment.Features;
            command.Parameters.Add("equipmentTypeId", OracleDbType.Varchar2).Value = equipment.EquipmentTypeId ?? (object)DBNull.Value;
            command.Parameters.Add("locationId", OracleDbType.Varchar2).Value = equipment.LocationId ?? (object)DBNull.Value;
            command.Parameters.Add("statusId", OracleDbType.Varchar2).Value = equipment.EquipmentStatusId ?? (object)DBNull.Value;
            command.Parameters.Add("supplierId", OracleDbType.Varchar2).Value = equipment.SupplierId ?? (object)DBNull.Value;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task Update(Equipment equipment)
        {
            using var connection = new OracleConnection(_connectionString);
            const string query = @"
                UPDATE EQUIPO 
                SET 
                    NOMBREEQUIPO = :name,
                    COSTOEQUIPO = :cost,
                    CARACTERISTICASEQUIPO = :features,
                    TIPOEQUIPO = :equipmentTypeId,
                    UBICACION = :locationId,
                    ESTADOEQUIPO = :statusId,
                    PROVEEDOR = :supplierId
                WHERE 
                    CODIGOEQUIPO = :equipmentId";

            using var command = new OracleCommand(query, connection);

            command.Parameters.Add("name", OracleDbType.Varchar2).Value = equipment.Name;
            command.Parameters.Add("cost", OracleDbType.Decimal).Value = equipment.Cost;
            command.Parameters.Add("features", OracleDbType.Varchar2).Value = equipment.Features;
            command.Parameters.Add("equipmentTypeId", OracleDbType.Varchar2).Value = equipment.EquipmentTypeId ?? (object)DBNull.Value;
            command.Parameters.Add("locationId", OracleDbType.Varchar2).Value = equipment.LocationId ?? (object)DBNull.Value;
            command.Parameters.Add("statusId", OracleDbType.Varchar2).Value = equipment.EquipmentStatusId ?? (object)DBNull.Value;
            command.Parameters.Add("supplierId", OracleDbType.Varchar2).Value = equipment.SupplierId ?? (object)DBNull.Value;
            command.Parameters.Add("equipmentId", OracleDbType.Varchar2).Value = equipment.EquipmentId;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task Delete(string equipmentId)
        {
            using var connection = new OracleConnection(_connectionString);
            const string query = "DELETE FROM EQUIPO WHERE CODIGOEQUIPO = :equipmentId";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add("equipmentId", OracleDbType.Varchar2).Value = equipmentId;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task<List<Equipment>> GetByLocation(string locationId)
        {
            using var connection = new OracleConnection(_connectionString);
            const string query = @"
                SELECT 
                    e.CODIGOEQUIPO,
                    e.NOMBREEQUIPO,
                    e.COSTOEQUIPO,
                    e.CARACTERISTICASEQUIPO,
                    e.TIPOEQUIPO,
                    e.UBICACION,
                    e.ESTADOEQUIPO,
                    e.PROVEEDOR,
                    te.NOMBRETIPOEQUIPO AS ""equipment_type_name"",
                    u.LUGARUBICACION AS ""location_name"",
                    ee.NOMBREESTADOEQUIPO AS ""status_name"",
                    p.NOMBREPROVEEDOR AS ""supplier_name""
                FROM 
                    EQUIPO e
                    LEFT JOIN TIPOEQUIPO te ON e.TIPOEQUIPO = te.CODIGOTIPOEQUIPO
                    LEFT JOIN UBICACION u ON e.UBICACION = u.CODIGOUBICACION
                    LEFT JOIN ESTADOEQUIPO ee ON e.ESTADOEQUIPO = ee.CODIGOESTADOEQUIPO
                    LEFT JOIN PROVEEDOR p ON e.PROVEEDOR = p.CODIGOPROVEEDOR
                WHERE 
                    e.UBICACION = :locationId";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add("locationId", OracleDbType.Varchar2).Value = locationId;

            var equipments = new List<Equipment>();
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                equipments.Add(MapEquipmentFromReader(reader));
            }

            return equipments;
        }

        private static Equipment MapEquipmentFromReader(IDataReader reader)
        {
            return new Equipment
            {
                EquipmentId = reader["CODIGOEQUIPO"].ToString()!,
                Name = reader["NOMBREEQUIPO"].ToString()!,
                Cost = Convert.ToDecimal(reader["COSTOEQUIPO"]),
                Features = reader["CARACTERISTICASEQUIPO"].ToString()!,
                EquipmentTypeId = reader["TIPOEQUIPO"]?.ToString(),
                LocationId = reader["UBICACION"]?.ToString(),
                EquipmentStatusId = reader["ESTADOEQUIPO"]?.ToString(),
                SupplierId = reader["PROVEEDOR"]?.ToString(),

                EquipmentType = reader["equipment_type_name"] != DBNull.Value ? new EquipmentType
                {
                    Name = reader["equipment_type_name"].ToString()!
                } : null,

                Location = reader["location_name"] != DBNull.Value ? new Location
                {
                    Name = reader["location_name"].ToString()!
                } : null,

                EquipmentStatus = reader["status_name"] != DBNull.Value ? new EquipmentStatus
                {
                    Name = reader["status_name"].ToString()!
                } : null,

                Supplier = reader["supplier_name"] != DBNull.Value ? new Supplier
                {
                    Name = reader["supplier_name"].ToString()!
                } : null
            };
        }
    }
}
