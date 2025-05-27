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
                SELECT 
                    d.codigoDevolution AS CODIGODEVOLUTION,
                    d.fechaDevolution AS FECHADEVOLUTION,
                    d.observacionesDevolution AS OBSERVACIONESDEVOLUTION,
                    d.pagoMulta AS PAGOMULTA,
                    d.prestamo AS PRESTAMO,
                    p.fechaPrestamo AS FECHAPRESTAMO
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

        public async Task<Return?> GetById(string returnId)
        {
            if (string.IsNullOrEmpty(returnId))
                throw new ArgumentException("returnId no puede ser nulo o vacío", nameof(returnId));

            using var connection = new OracleConnection(_connectionString);
            const string query = @"
                SELECT 
                    d.codigoDevolution AS CODIGODEVOLUTION,
                    d.fechaDevolution AS FECHADEVOLUTION,
                    d.observacionesDevolution AS OBSERVACIONESDEVOLUTION,
                    d.pagoMulta AS PAGOMULTA,
                    d.prestamo AS PRESTAMO,
                    p.fechaPrestamo AS FECHAPRESTAMO
                FROM Devolucion d
                JOIN Prestamo p ON d.prestamo = p.codigoPrestamo
                WHERE d.codigoDevolution = :returnId";

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
       public async Task Create(Return devolucion)
{
    // Validaciones básicas
    if (string.IsNullOrWhiteSpace(devolucion.ReturnId))
        throw new ArgumentException("El código de devolución es obligatorio");
    
    if (devolucion.ReturnId.Length > 10)
        throw new ArgumentException("El código no puede exceder 10 caracteres");

    if (string.IsNullOrWhiteSpace(devolucion.Notes))
        throw new ArgumentException("Las observaciones son obligatorias");

    if (string.IsNullOrWhiteSpace(devolucion.LoanId))
        throw new ArgumentException("El préstamo asociado es obligatorio");

    using var connection = new OracleConnection(_connectionString);
    
    const string query = @"
        INSERT INTO DEVOLUCION (
            CODIGODEVOLUTION,      -- VARCHAR2(10)
            FECHADEVOLUTION,       -- DATE
            OBSERVACIONESDEVOLUTION, -- CLOB NOT NULL
            PAGOMULTA,            -- NUMBER(15,2) NULL
            PRESTAMO              -- VARCHAR2(10) NOT NULL
        ) VALUES (
            :CodigoDevolucion,
            TO_DATE(:FechaDevolucion, 'DD/MM/YYYY'),
            :Observaciones,
            :PagoMulta,
            :PrestamoId
        )";

    try
    {
        await connection.OpenAsync();
        
        using var command = new OracleCommand(query, connection);
        
        // Parámetros exactamente como en la consulta SQL
        command.Parameters.Add("CodigoDevolucion", OracleDbType.Varchar2, 10).Value = devolucion.ReturnId;
        
        command.Parameters.Add("FechaDevolucion", OracleDbType.Varchar2).Value = 
            devolucion.Date.ToString("dd/MM/yyyy");
        
        command.Parameters.Add("Observaciones", OracleDbType.Clob).Value = 
            devolucion.Notes ?? (object)DBNull.Value;
        
        command.Parameters.Add("PagoMulta", OracleDbType.Decimal, 15).Value = 
            devolucion.PenaltyPaid ?? (object)DBNull.Value;
        command.Parameters["PagoMulta"].Scale = 2; // Para NUMBER(15,2)
        
        command.Parameters.Add("PrestamoId", OracleDbType.Varchar2, 10).Value = 
            devolucion.LoanId;

        await command.ExecuteNonQueryAsync();
    }
    catch (OracleException ex) when (ex.Number == 1)
    {
        throw new Exception($"Ya existe una devolución con código {devolucion.ReturnId}", ex);
    }
    catch (OracleException ex) when (ex.Number == 2291)
    {
        throw new Exception($"No existe el préstamo {devolucion.LoanId}", ex);
    }
    catch (OracleException ex)
    {
        throw new Exception($"Error Oracle (Code: {ex.Number}): {ex.Message}", ex);
    }
}

        public async Task Update(Return returnObj)
        {
            if (returnObj == null)
                throw new ArgumentNullException(nameof(returnObj));

            if (string.IsNullOrEmpty(returnObj.ReturnId))
                throw new ArgumentException("ReturnId no puede ser nulo o vacío", nameof(returnObj.ReturnId));

            using var connection = new OracleConnection(_connectionString);
            const string query = @"
                UPDATE Devolucion 
                SET fechaDevolution = :date,
                    observacionesDevolution = :notes,
                    pagoMulta = :penaltyPaid
                WHERE codigoDevolution = :returnId";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add("date", OracleDbType.Date).Value = returnObj.Date;
            command.Parameters.Add("notes", OracleDbType.Clob).Value = (object?)returnObj.Notes ?? DBNull.Value;
            command.Parameters.Add("penaltyPaid", OracleDbType.Decimal).Value = returnObj.PenaltyPaid ?? (object)DBNull.Value;
            command.Parameters.Add("returnId", OracleDbType.Varchar2).Value = returnObj.ReturnId;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task Delete(string returnId)
        {
            if (string.IsNullOrEmpty(returnId))
                throw new ArgumentException("returnId no puede ser nulo o vacío", nameof(returnId));

            using var connection = new OracleConnection(_connectionString);
            const string query = "DELETE FROM Devolucion WHERE codigoDevolution = :returnId";

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
                Notes = reader["OBSERVACIONESDEVOLUTION"]?.ToString() ?? string.Empty,
                PenaltyPaid = reader["PAGOMULTA"] != DBNull.Value ? Convert.ToDecimal(reader["PAGOMULTA"]) : null,
                LoanId = reader["PRESTAMO"].ToString()!,
                Loan = new Loan
                {
                    Date = Convert.ToDateTime(reader["FECHAPRESTAMO"])
                }
            };
        }
    }
}