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
                SELECT e.*, 
                       et.nombreTipoEquipo AS equipment_type_name,
                       l.nombreUbicacion AS location_name,
                       es.nombreEstado AS status_name,
                       s.nombreProveedor AS supplier_name
                FROM Equipo e
                LEFT JOIN TipoEquipo et ON e.tipoEquipo = et.codigoTipoEquipo
                LEFT JOIN Ubicacion l ON e.ubicacion = l.codigoUbicacion
                LEFT JOIN EstadoEquipo es ON e.estadoEquipo = es.codigoEstado
                LEFT JOIN Proveedor s ON e.proveedor = s.codigoProveedor";

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
                SELECT e.*, 
                       et.nombreTipoEquipo AS equipment_type_name,
                       l.nombreUbicacion AS location_name,
                       es.nombreEstado AS status_name,
                       s.nombreProveedor AS supplier_name
                FROM Equipo e
                LEFT JOIN TipoEquipo et ON e.tipoEquipo = et.codigoTipoEquipo
                LEFT JOIN Ubicacion l ON e.ubicacion = l.codigoUbicacion
                LEFT JOIN EstadoEquipo es ON e.estadoEquipo = es.codigoEstado
                LEFT JOIN Proveedor s ON e.proveedor = s.codigoProveedor
                WHERE e.codigoEquipo = :equipmentId";

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
                INSERT INTO Equipo 
                (codigoEquipo, nombreEquipo, costo, caracteristicas, tipoEquipo, ubicacion, estadoEquipo, proveedor)
                VALUES 
                (:equipmentId, :name, :cost, :features, :equipmentTypeId, :locationId, :statusId, :supplierId)";

            using var command = new OracleCommand(query, connection);
            
            command.Parameters.Add("equipmentId", OracleDbType.Varchar2).Value = equipment.EquipmentId;
            command.Parameters.Add("name", OracleDbType.Varchar2).Value = equipment.Name;
            command.Parameters.Add("cost", OracleDbType.Decimal).Value = equipment.Cost;
            command.Parameters.Add("features", OracleDbType.Varchar2).Value = equipment.Features;
            command.Parameters.Add("equipmentTypeId", OracleDbType.Varchar2).Value = 
                equipment.EquipmentTypeId ?? (object)DBNull.Value;
            command.Parameters.Add("locationId", OracleDbType.Varchar2).Value = 
                equipment.LocationId ?? (object)DBNull.Value;
            command.Parameters.Add("statusId", OracleDbType.Varchar2).Value = 
                equipment.EquipmentStatusId ?? (object)DBNull.Value;
            command.Parameters.Add("supplierId", OracleDbType.Varchar2).Value = 
                equipment.SupplierId ?? (object)DBNull.Value;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task Update(Equipment equipment)
        {
            using var connection = new OracleConnection(_connectionString);
            const string query = @"
                UPDATE Equipo 
                SET nombreEquipo = :name,
                    costo = :cost,
                    caracteristicas = :features,
                    tipoEquipo = :equipmentTypeId,
                    ubicacion = :locationId,
                    estadoEquipo = :statusId,
                    proveedor = :supplierId
                WHERE codigoEquipo = :equipmentId";

            using var command = new OracleCommand(query, connection);
            
            command.Parameters.Add("name", OracleDbType.Varchar2).Value = equipment.Name;
            command.Parameters.Add("cost", OracleDbType.Decimal).Value = equipment.Cost;
            command.Parameters.Add("features", OracleDbType.Varchar2).Value = equipment.Features;
            command.Parameters.Add("equipmentTypeId", OracleDbType.Varchar2).Value = 
                equipment.EquipmentTypeId ?? (object)DBNull.Value;
            command.Parameters.Add("locationId", OracleDbType.Varchar2).Value = 
                equipment.LocationId ?? (object)DBNull.Value;
            command.Parameters.Add("statusId", OracleDbType.Varchar2).Value = 
                equipment.EquipmentStatusId ?? (object)DBNull.Value;
            command.Parameters.Add("supplierId", OracleDbType.Varchar2).Value = 
                equipment.SupplierId ?? (object)DBNull.Value;
            command.Parameters.Add("equipmentId", OracleDbType.Varchar2).Value = equipment.EquipmentId;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task Delete(string equipmentId)
        {
            using var connection = new OracleConnection(_connectionString);
            const string query = "DELETE FROM Equipo WHERE codigoEquipo = :equipmentId";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add("equipmentId", OracleDbType.Varchar2).Value = equipmentId;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task<List<Equipment>> GetByLocation(string locationId)
        {
            using var connection = new OracleConnection(_connectionString);
            const string query = @"
                SELECT e.*, 
                       et.nombreTipoEquipo AS equipment_type_name,
                       l.nombreUbicacion AS location_name,
                       es.nombreEstado AS status_name,
                       s.nombreProveedor AS supplier_name
                FROM Equipo e
                LEFT JOIN TipoEquipo et ON e.tipoEquipo = et.codigoTipoEquipo
                LEFT JOIN Ubicacion l ON e.ubicacion = l.codigoUbicacion
                LEFT JOIN EstadoEquipo es ON e.estadoEquipo = es.codigoEstado
                LEFT JOIN Proveedor s ON e.proveedor = s.codigoProveedor
                WHERE e.ubicacion = :locationId";

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
                EquipmentId = reader["codigoEquipo"].ToString()!,
                Name = reader["nombreEquipo"].ToString()!,
                Cost = Convert.ToDecimal(reader["costo"]),
                Features = reader["caracteristicas"].ToString()!,
                EquipmentTypeId = reader["tipoEquipo"].ToString(),
                LocationId = reader["ubicacion"].ToString(),
                EquipmentStatusId = reader["estadoEquipo"].ToString(),
                SupplierId = reader["proveedor"].ToString(),
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