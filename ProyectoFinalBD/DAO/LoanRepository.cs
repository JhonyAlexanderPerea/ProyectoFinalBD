using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using ProyectoFinalBD.Model;

namespace ProyectoFinalBD.DAO
{
    public class LoanRepository
    {
        private readonly string _connectionString;

        public LoanRepository()
        {
            _connectionString = DbConnectionFactory.GetConnectionString();
        }

        public async Task<List<Loan>> GetAll()
        {
            var loans = new List<Loan>();
            using var connection = new OracleConnection(_connectionString);
            const string query = @"
                SELECT p.*, e.nombreEquipo as equipment_name, 
                       u.nombreUsuario as user_name
                FROM Prestamo p
                JOIN Equipo e ON p.equipo = e.codigoEquipo
                JOIN Usuario u ON p.usuario = u.codigoUsuario";

            using var command = new OracleCommand(query, connection);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                loans.Add(MapLoanFromReader(reader));
            }

            return loans;
        }

        public async Task<Loan> GetById(string loanId)
        {
            using var connection = new OracleConnection(_connectionString);
            const string query = @"
                SELECT p.*, e.nombreEquipo as equipment_name, 
                       u.nombreUsuario as user_name
                FROM Prestamo p
                JOIN Equipo e ON p.equipo = e.codigoEquipo
                JOIN Usuario u ON p.usuario = u.codigoUsuario
                WHERE p.codigoPrestamo = :loanId";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add("loanId", OracleDbType.Varchar2).Value = loanId;

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapLoanFromReader(reader);
            }

            return null;
        }

        public async Task Create(Loan loan)
        {
            using var connection = new OracleConnection(_connectionString);
            const string query = @"
                INSERT INTO Prestamo 
                (codigoPrestamo, fechaPrestamo, fechaDevolucion, costoPenalizacion, 
                 equipo, usuario) 
                VALUES 
                (:loanId, :date, :dueDate, :penaltyCost, 
                 :equipmentId, :userId)";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add("loanId", OracleDbType.Varchar2).Value = loan.LoanId;
            command.Parameters.Add("date", OracleDbType.Date).Value = loan.Date;
            command.Parameters.Add("dueDate", OracleDbType.Date).Value = loan.DueDate;
            command.Parameters.Add("penaltyCost", OracleDbType.Decimal).Value = loan.PenaltyCost;
            command.Parameters.Add("equipmentId", OracleDbType.Varchar2).Value = loan.EquipmentId ?? (object)DBNull.Value;
            command.Parameters.Add("userId", OracleDbType.Varchar2).Value = loan.UserId ?? (object)DBNull.Value;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task Update(Loan loan)
        {
            using var connection = new OracleConnection(_connectionString);
            const string query = @"
                UPDATE Prestamo 
                SET fechaPrestamo = :date,
                    fechaDevolucion = :dueDate,
                    costoPenalizacion = :penaltyCost,
                    equipo = :equipmentId,
                    usuario = :userId
                WHERE codigoPrestamo = :loanId";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add("date", OracleDbType.Date).Value = loan.Date;
            command.Parameters.Add("dueDate", OracleDbType.Date).Value = loan.DueDate;
            command.Parameters.Add("penaltyCost", OracleDbType.Decimal).Value = loan.PenaltyCost;
            command.Parameters.Add("equipmentId", OracleDbType.Varchar2).Value = loan.EquipmentId ?? (object)DBNull.Value;
            command.Parameters.Add("userId", OracleDbType.Varchar2).Value = loan.UserId ?? (object)DBNull.Value;
            command.Parameters.Add("loanId", OracleDbType.Varchar2).Value = loan.LoanId;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task Delete(string loanId)
        {
            using var connection = new OracleConnection(_connectionString);
            const string query = "DELETE FROM Prestamo WHERE codigoPrestamo = :loanId";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add("loanId", OracleDbType.Varchar2).Value = loanId;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task<List<Loan>> GetByUser(string userId)
        {
            var loans = new List<Loan>();
            using var connection = new OracleConnection(_connectionString);
            const string query = @"
                SELECT p.*, e.nombreEquipo as equipment_name, 
                       u.nombreUsuario as user_name
                FROM Prestamo p
                JOIN Equipo e ON p.equipo = e.codigoEquipo
                JOIN Usuario u ON p.usuario = u.codigoUsuario
                WHERE p.usuario = :userId";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add("userId", OracleDbType.Varchar2).Value = userId;

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                loans.Add(MapLoanFromReader(reader));
            }

            return loans;
        }

        public async Task<List<Loan>> GetByEquipment(string equipmentId)
        {
            var loans = new List<Loan>();
            using var connection = new OracleConnection(_connectionString);
            const string query = @"
                SELECT p.*, e.nombreEquipo as equipment_name, 
                       u.nombreUsuario as user_name
                FROM Prestamo p
                JOIN Equipo e ON p.equipo = e.codigoEquipo
                JOIN Usuario u ON p.usuario = u.codigoUsuario
                WHERE p.equipo = :equipmentId";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add("equipmentId", OracleDbType.Varchar2).Value = equipmentId;

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                loans.Add(MapLoanFromReader(reader));
            }

            return loans;
        }

        private static Loan MapLoanFromReader(System.Data.IDataReader reader)
        {
            return new Loan
            {
                LoanId = reader["codigoPrestamo"].ToString()!,
                Date = Convert.ToDateTime(reader["fechaPrestamo"]),
                DueDate = Convert.ToDateTime(reader["fechaDevolucion"]),
                PenaltyCost = Convert.ToDecimal(reader["costoPenalizacion"]),
                EquipmentId = reader["equipo"].ToString(),
                UserId = reader["usuario"].ToString(),
                Equipment = new Equipment { Name = reader["equipment_name"].ToString()! },
                User = new User { Name = reader["user_name"].ToString()! }
            };
        }
    }
}