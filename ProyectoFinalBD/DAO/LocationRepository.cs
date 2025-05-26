using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using ProyectoFinalBD.Model;

namespace ProyectoFinalBD.DAO
{
    public class LocationRepository
    {
        private readonly string _connectionString;

        public LocationRepository()
        {
            _connectionString = DbConnectionFactory.GetConnectionString();
        }

        public async Task<List<Location>> GetAll()
        {
            var locations = new List<Location>();
            using var connection = new OracleConnection(_connectionString);
            const string query = "SELECT * FROM Ubicacion";

            using var command = new OracleCommand(query, connection);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                locations.Add(MapLocationFromReader(reader));
            }

            return locations;
        }

        public async Task<Location> GetById(string locationId)
        {
            using var connection = new OracleConnection(_connectionString);
            const string query = "SELECT * FROM Ubicacion WHERE codigoUbicacion = :locationId";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add("locationId", OracleDbType.Varchar2).Value = locationId;

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapLocationFromReader(reader);
            }

            return null;
        }

        public async Task Create(Location location)
        {
            using var connection = new OracleConnection(_connectionString);
            const string query = @"
                INSERT INTO Ubicacion 
                (codigoUbicacion, nombreUbicacion) 
                VALUES 
                (:locationId, :name)";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add("locationId", OracleDbType.Varchar2).Value = location.LocationId;
            command.Parameters.Add("name", OracleDbType.Varchar2).Value = location.Name;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task Update(Location location)
        {
            using var connection = new OracleConnection(_connectionString);
            const string query = @"
                UPDATE Ubicacion 
                SET nombreUbicacion = :name
                WHERE codigoUbicacion = :locationId";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add("name", OracleDbType.Varchar2).Value = location.Name;
            command.Parameters.Add("locationId", OracleDbType.Varchar2).Value = location.LocationId;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task Delete(string locationId)
        {
            using var connection = new OracleConnection(_connectionString);
            const string query = "DELETE FROM Ubicacion WHERE codigoUbicacion = :locationId";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add("locationId", OracleDbType.Varchar2).Value = locationId;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        private static Location MapLocationFromReader(IDataReader reader)
        {
            return new Location
            {
                LocationId = reader["CODIGOUBICACION"].ToString()!,
                Name = reader["LUGARUBICACION"].ToString()!
            };
        }

    }
}