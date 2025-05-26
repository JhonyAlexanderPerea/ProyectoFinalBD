using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using ProyectoFinalBD.Model;

namespace ProyectoFinalBD.DAO
{
    public class ReturnRepository
    {
        private readonly string _connectionString;

        public ReturnRepository()
        {
            _connectionString = DbConnectionFactory.GetConnectionString();
        }

        public async Task<List<Return>> GetAll()
        {
            var returns = new List<Return>();
            using var connection = new OracleConnection(_connectionString);
            const string query = @"
                SELECT d.*, p.fechaPrestamo as loan_date
                FROM Devolucion d
                JOIN Prestamo p ON d.prestamo = p.codigoPrestamo";

            using var command = new OracleCommand(query, connection);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                returns.Add(MapReturnFromReader(reader));
            }

            return returns;
        }

        public async Task<Return> GetById(string returnId)
        {
            using var connection = new OracleConnection(_connectionString);
            const string query = @"
                SELECT 
    d.codigoDevolution AS ""return_id"",
    d.fechaDevolution AS ""return_date"",
    d.observacionesDevolution AS ""notes"",
    d.pagoMulta AS ""penalty_paid"",
    d.prestamo,
    p.fechaPrestamo AS ""loan_date""
FROM Devolucion d
JOIN Prestamo p ON d.prestamo = p.codigoPrestamo
";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add("returnId", OracleDbType.Varchar2).Value = returnId;

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapReturnFromReader(reader);
            }

            return null;
        }

        public async Task Create(Return returnObj)
        {
            using var connection = new OracleConnection(_connectionString);
            const string query = @"
                INSERT INTO Devolucion 
                (codigoDevolucion, fecha, notas, penalizacionPagada, prestamo) 
                VALUES 
                (:returnId, :date, :notes, :penaltyPaid, :loanId)";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add("returnId", OracleDbType.Varchar2).Value = returnObj.ReturnId;
            command.Parameters.Add("date", OracleDbType.Date).Value = returnObj.Date;
            command.Parameters.Add("notes", OracleDbType.Varchar2).Value = returnObj.Notes;
            command.Parameters.Add("penaltyPaid", OracleDbType.Decimal).Value = 
                returnObj.PenaltyPaid ?? (object)DBNull.Value;
            command.Parameters.Add("loanId", OracleDbType.Varchar2).Value = returnObj.LoanId;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task Update(Return returnObj)
        {
            using var connection = new OracleConnection(_connectionString);
            const string query = @"
                UPDATE Devolucion 
                SET fecha = :date,
                    notas = :notes,
                    penalizacionPagada = :penaltyPaid
                WHERE codigoDevolucion = :returnId";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add("date", OracleDbType.Date).Value = returnObj.Date;
            command.Parameters.Add("notes", OracleDbType.Varchar2).Value = returnObj.Notes;
            command.Parameters.Add("penaltyPaid", OracleDbType.Decimal).Value = 
                returnObj.PenaltyPaid ?? (object)DBNull.Value;
            command.Parameters.Add("returnId", OracleDbType.Varchar2).Value = returnObj.ReturnId;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task Delete(string returnId)
        {
            using var connection = new OracleConnection(_connectionString);
            const string query = "DELETE FROM Devolucion WHERE codigoDevolucion = :returnId";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add("returnId", OracleDbType.Varchar2).Value = returnId;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        private static Return MapReturnFromReader(System.Data.IDataReader reader)
        {
            return new Return
            {
                ReturnId = reader["CODIGODEVOLUTION"].ToString()!,
                Date = Convert.ToDateTime(reader["FECHADEVOLUTION"]),
                Notes = reader["OBSERVACIONESDEVOLUTION"].ToString()!,
                PenaltyPaid = reader["PAGOMULTA"] != DBNull.Value ? 
                    Convert.ToDecimal(reader["PAGOMULTA"]) : null,
                LoanId = reader["PRESTAMO"].ToString()!,
                Loan = new Loan 
                { 
                    Date = Convert.ToDateTime(reader["loan_date"])
                }
            };
        }

    }
}