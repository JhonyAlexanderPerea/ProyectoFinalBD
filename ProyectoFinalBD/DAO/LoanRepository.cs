using System;
using System.Collections.Generic;
using System.Data;
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
                SELECT 
                    p.codigoPrestamo,
                    p.fechaPrestamo,
                    p.fechaLimitePrestamo,
                    p.costoMultaPrestamo,
                    p.equipo,
                    p.usuario,
                    e.nombreEquipo AS equipment_name,
                    u.nombreUser AS user_name
                FROM Prestamo p
                JOIN Equipo e ON p.equipo = e.codigoEquipo
                JOIN Usuario u ON p.usuario = u.codigoUser";

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
                SELECT 
                    p.codigoPrestamo,
                    p.fechaPrestamo,
                    p.fechaLimitePrestamo,
                    p.costoMultaPrestamo,
                    p.equipo,
                    p.usuario,
                    e.nombreEquipo AS equipment_name,
                    u.nombreUser AS user_name
                FROM Prestamo p
                JOIN Equipo e ON p.equipo = e.codigoEquipo
                JOIN Usuario u ON p.usuario = u.codigoUser
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
            if (loan == null)
                throw new ArgumentNullException(nameof(loan));

            if (string.IsNullOrEmpty(loan.LoanId))
                throw new ArgumentException("Loan ID cannot be null or empty", nameof(loan.LoanId));

            if (loan.Date >= loan.DueDate)
                throw new ArgumentException("Due date must be after loan date");

            using var connection = new OracleConnection(_connectionString);
            const string query = @"
        INSERT INTO Prestamo 
        (codigoPrestamo, fechaPrestamo, fechaLimitePrestamo, costoMultaPrestamo, 
         equipo, usuario) 
        VALUES 
        (:codigoPrestamo, :fechaPrestamo, :fechaLimitePrestamo, :costoMultaPrestamo, 
         :equipo, :usuario)";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add("codigoPrestamo", OracleDbType.Varchar2).Value = loan.LoanId;
            command.Parameters.Add("fechaPrestamo", OracleDbType.Date).Value = loan.Date;
            command.Parameters.Add("fechaLimitePrestamo", OracleDbType.Date).Value = loan.DueDate;
            command.Parameters.Add("costoMultaPrestamo", OracleDbType.Decimal).Value = loan.PenaltyCost;
            command.Parameters.Add("equipo", OracleDbType.Varchar2).Value = loan.EquipmentId ?? (object)DBNull.Value;
            command.Parameters.Add("usuario", OracleDbType.Varchar2).Value = loan.UserId ?? (object)DBNull.Value;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }


        public async Task Update(Loan loan)
{
    // Validaciones mejoradas
    if (loan == null)
        throw new ArgumentNullException(nameof(loan));

    if (string.IsNullOrWhiteSpace(loan.LoanId))
        throw new ArgumentException("El ID del préstamo es requerido", nameof(loan.LoanId));

    if (loan.LoanId.Length > 10)
        throw new ArgumentException("El ID del préstamo no puede exceder 10 caracteres", nameof(loan.LoanId));

    if (loan.Date >= loan.DueDate)
        throw new ArgumentException("La fecha límite debe ser posterior a la fecha de préstamo");

    if (loan.PenaltyCost < 0)
        throw new ArgumentException("El costo de multa no puede ser negativo", nameof(loan.PenaltyCost));

    using var connection = new OracleConnection(_connectionString);
    const string query = @"
        UPDATE Prestamo 
        SET fechaPrestamo = :fechaPrestamo,
            fechaLimitePrestamo = :fechaLimitePrestamo,
            costoMultaPrestamo = :costoMultaPrestamo,
            equipo = :equipo,
            usuario = :usuario
        WHERE codigoPrestamo = :codigoPrestamo";

    using var command = new OracleCommand(query, connection);
    
    // Parámetros alineados con nombres de columna
    command.Parameters.Add("fechaPrestamo", OracleDbType.Date).Value = loan.Date;
    command.Parameters.Add("fechaLimitePrestamo", OracleDbType.Date).Value = loan.DueDate;
    command.Parameters.Add("costoMultaPrestamo", OracleDbType.Decimal).Value = loan.PenaltyCost;
    command.Parameters.Add("equipo", OracleDbType.Varchar2).Value = loan.EquipmentId ?? (object)DBNull.Value;
    command.Parameters.Add("usuario", OracleDbType.Varchar2).Value = loan.UserId ?? (object)DBNull.Value;
    command.Parameters.Add("codigoPrestamo", OracleDbType.Varchar2).Value = loan.LoanId;

    try
    {
        await connection.OpenAsync();
        
        // Usar transacción para mayor seguridad
        using var transaction = await connection.BeginTransactionAsync();
        try
        {
            int affectedRows = await command.ExecuteNonQueryAsync();
            
            if (affectedRows == 0)
                throw new KeyNotFoundException("No se encontró el préstamo con el ID especificado");
                
            await transaction.CommitAsync();
        }
        catch (OracleException ex) when (ex.Number == 2290) // Violación de CHECK constraint
        {
            await transaction.RollbackAsync();
            throw new ArgumentException("Las fechas no cumplen con la validación: la fecha límite debe ser posterior a la fecha de préstamo");
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    catch (OracleException ex) when (ex.Number == 1) // Violación de clave única
    {
        throw new ArgumentException("El ID del préstamo ya existe", nameof(loan.LoanId), ex);
    }
}

        public async Task Delete(string loanId)
        {
            if (string.IsNullOrEmpty(loanId))
                throw new ArgumentException("Loan ID cannot be null or empty", nameof(loanId));

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
                SELECT 
                    p.codigoPrestamo,
                    p.fechaPrestamo,
                    p.fechaLimitePrestamo,
                    p.costoMultaPrestamo,
                    p.equipo,
                    p.usuario,
                    e.nombreEquipo AS equipment_name,
                    u.nombreUser AS user_name
                FROM Prestamo p
                JOIN Equipo e ON p.equipo = e.codigoEquipo
                JOIN Usuario u ON p.usuario = u.codigoUser
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
                SELECT 
                    p.codigoPrestamo,
                    p.fechaPrestamo,
                    p.fechaLimitePrestamo,
                    p.costoMultaPrestamo,
                    p.equipo,
                    p.usuario,
                    e.nombreEquipo AS equipment_name,
                    u.nombreUser AS user_name
                FROM Prestamo p
                JOIN Equipo e ON p.equipo = e.codigoEquipo
                JOIN Usuario u ON p.usuario = u.codigoUser
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

        private static Loan MapLoanFromReader(IDataReader reader)
        {
            return new Loan
            {
                LoanId = reader["codigoPrestamo"].ToString()!,
                Date = Convert.ToDateTime(reader["fechaPrestamo"]),
                DueDate = Convert.ToDateTime(reader["fechaLimitePrestamo"]),
                PenaltyCost = Convert.ToDecimal(reader["costoMultaPrestamo"]),
                EquipmentId = reader["equipo"]?.ToString(),
                UserId = reader["usuario"]?.ToString(),
                Equipment = new Equipment { Name = reader["equipment_name"].ToString() },
                User = new User { Name = reader["user_name"].ToString() }
            };
        }
    }
}