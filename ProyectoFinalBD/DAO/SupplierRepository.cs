using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using ProyectoFinalBD.Model;

namespace ProyectoFinalBD.DAO
{
    public class SupplierRepository
    {
        private readonly string _connectionString;

        public SupplierRepository()
        {
            _connectionString = DbConnectionFactory.GetConnectionString();
        }

        public async Task<List<Supplier>> GetAll()
        {
            var suppliers = new List<Supplier>();
            using var connection = new OracleConnection(_connectionString);
            const string query = @"
                SELECT p.*, m.nombreMunicipio AS municipality_name
                FROM Proveedor p
                LEFT JOIN Municipio m ON p.municipio = m.codigoMunicipio";

            using var command = new OracleCommand(query, connection);
            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                suppliers.Add(MapSupplierFromReader(reader));
            }

            return suppliers;
        }

        public async Task<Supplier?> GetById(string supplierId)
        {
            if (string.IsNullOrEmpty(supplierId))
                throw new ArgumentException("supplierId no puede ser nulo o vacío", nameof(supplierId));

            using var connection = new OracleConnection(_connectionString);
            const string query = @"
                SELECT p.*, m.nombreMunicipio AS municipality_name
                FROM Proveedor p
                LEFT JOIN Municipio m ON p.municipio = m.codigoMunicipio
                WHERE p.codigoProveedor = :supplierId";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add("supplierId", OracleDbType.Varchar2).Value = supplierId;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapSupplierFromReader(reader);
            }

            return null;
        }

        public async Task Create(Supplier supplier)
        {
            if (supplier == null)
                throw new ArgumentNullException(nameof(supplier));
            if (string.IsNullOrEmpty(supplier.SupplierId))
                throw new ArgumentException("SupplierId no puede ser nulo o vacío", nameof(supplier.SupplierId));

            using var connection = new OracleConnection(_connectionString);
            const string query = @"
                INSERT INTO Proveedor 
                (codigoProveedor, nombreProveedor, contacto, correoElectronico, mesesGarantia, municipio) 
                VALUES 
                (:supplierId, :name, :contact, :email, :warrantyMonths, :municipalityId)";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add("supplierId", OracleDbType.Varchar2).Value = supplier.SupplierId;
            command.Parameters.Add("name", OracleDbType.Varchar2).Value = supplier.Name;
            command.Parameters.Add("contact", OracleDbType.Varchar2).Value = supplier.Contact ?? (object)DBNull.Value;
            command.Parameters.Add("email", OracleDbType.Varchar2).Value = supplier.Email ?? (object)DBNull.Value;
            command.Parameters.Add("warrantyMonths", OracleDbType.Int32).Value = supplier.WarrantyMonths;
            command.Parameters.Add("municipalityId", OracleDbType.Varchar2).Value = supplier.MunicipalityId ?? (object)DBNull.Value;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task Update(Supplier supplier)
        {
            if (supplier == null)
                throw new ArgumentNullException(nameof(supplier));
            if (string.IsNullOrEmpty(supplier.SupplierId))
                throw new ArgumentException("SupplierId no puede ser nulo o vacío", nameof(supplier.SupplierId));

            using var connection = new OracleConnection(_connectionString);
            const string query = @"
                UPDATE Proveedor 
                SET nombreProveedor = :name,
                    contacto = :contact,
                    correoElectronico = :email,
                    mesesGarantia = :warrantyMonths,
                    municipio = :municipalityId
                WHERE codigoProveedor = :supplierId";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add("name", OracleDbType.Varchar2).Value = supplier.Name;
            command.Parameters.Add("contact", OracleDbType.Varchar2).Value = supplier.Contact ?? (object)DBNull.Value;
            command.Parameters.Add("email", OracleDbType.Varchar2).Value = supplier.Email ?? (object)DBNull.Value;
            command.Parameters.Add("warrantyMonths", OracleDbType.Int32).Value = supplier.WarrantyMonths;
            command.Parameters.Add("municipalityId", OracleDbType.Varchar2).Value = supplier.MunicipalityId ?? (object)DBNull.Value;
            command.Parameters.Add("supplierId", OracleDbType.Varchar2).Value = supplier.SupplierId;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task Delete(string supplierId)
        {
            if (string.IsNullOrEmpty(supplierId))
                throw new ArgumentException("supplierId no puede ser nulo o vacío", nameof(supplierId));

            using var connection = new OracleConnection(_connectionString);
            const string query = "DELETE FROM Proveedor WHERE codigoProveedor = :supplierId";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add("supplierId", OracleDbType.Varchar2).Value = supplierId;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        private static Supplier MapSupplierFromReader(System.Data.IDataReader reader)
        {
            return new Supplier
            {
                SupplierId = reader["CODIGOPROVEEDOR"].ToString()!,
                Name = reader["NOMBREPROVEEDOR"].ToString()!,
                Contact = reader["CONTACTO"]?.ToString(),
                Email = reader["CORREOELECTRONICO"]?.ToString(),
                WarrantyMonths = reader["MESESGARANTIA"] != DBNull.Value ? Convert.ToInt32(reader["MESESGARANTIA"]) : 0,
                MunicipalityId = reader["MUNICIPIO"]?.ToString(),
                Municipality = reader["MUNICIPIO"] != DBNull.Value ? new Municipality 
                { 
                    Name = reader["municipality_name"].ToString()! 
                } : null
            };
        }
    }
}
