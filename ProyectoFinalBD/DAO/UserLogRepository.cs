﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using ProyectoFinalBD.Model;

namespace ProyectoFinalBD.DAO
{
    public class UserLogRepository
    {
        private readonly string _connectionString;

        public UserLogRepository()
        {
            _connectionString = DbConnectionFactory.GetConnectionString();
        }

        public async Task<List<UserLog>> GetAll()
        {
            var logs = new List<UserLog>();
            using var connection = new OracleConnection(_connectionString);
            const string query = @"
                SELECT l.*, u.nombreUser AS user_name
                FROM LogUsuario l
                LEFT JOIN Usuario u ON l.usuario = u.codigoUser";

            using var command = new OracleCommand(query, connection);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                logs.Add(MapUserLogFromReader(reader));
            }

            return logs;
        }

        public async Task<UserLog> GetById(string userLogId)
        {
            using var connection = new OracleConnection(_connectionString);
            const string query = @"
                SELECT l.*, u.nombreUser AS user_name
                FROM LogUsuario l
                LEFT JOIN Usuario u ON l.usuario = u.codigoUser
                WHERE l.codigoLogUser = :userLogId";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add("userLogId", OracleDbType.Varchar2).Value = userLogId;

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapUserLogFromReader(reader);
            }

            return null;
        }

        public async Task Create(UserLog userLog)
        {
            using var connection = new OracleConnection(_connectionString);
            const string query = @"
        INSERT INTO LogUsuario 
        (codigoLogUser, fecha, registro, usuario) 
        VALUES 
        (:codigoLogUser, :fecha, :registro, :usuario)";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add("codigoLogUser", OracleDbType.Varchar2).Value = userLog.UserLogId;
            command.Parameters.Add("fecha", OracleDbType.Date).Value = userLog.Date;
            command.Parameters.Add("registro", OracleDbType.Clob).Value = userLog.Entry;
            command.Parameters.Add("usuario", OracleDbType.Varchar2).Value = userLog.UserId ?? (object)DBNull.Value;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }
        public async Task Delete(string userLogId)
        {
            using var connection = new OracleConnection(_connectionString);
            const string query = "DELETE FROM LogUsuario WHERE codigoLogUser = :userLogId";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add("userLogId", OracleDbType.Varchar2).Value = userLogId;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task<List<UserLog>> GetByUser(string userId)
        {
            var logs = new List<UserLog>();
            using var connection = new OracleConnection(_connectionString);
            const string query = @"
                SELECT l.*, u.nombreUser AS user_name
                FROM LogUsuario l
                LEFT JOIN Usuario u ON l.usuario = u.codigoUser
                WHERE l.usuario = :userId";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add("userId", OracleDbType.Varchar2).Value = userId;

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                logs.Add(MapUserLogFromReader(reader));
            }

            return logs;
        }

        private static UserLog MapUserLogFromReader(IDataReader reader)
        {
            return new UserLog
            {
                UserLogId = reader["codigoLogUser"].ToString()!,
                Date = Convert.ToDateTime(reader["fecha"]),
                Entry = reader["registro"].ToString()!, // Changed to match column name
                UserId = reader["usuario"]?.ToString(), // Changed to match column name
            };
        }

        public async Task Update(UserLog userLog)
        {
            if (userLog == null)
                throw new ArgumentNullException(nameof(userLog));

            // Validaciones adicionales recomendadas
            if (string.IsNullOrWhiteSpace(userLog.UserLogId))
                throw new ArgumentException("ID de log de usuario es requerido");
    
            if (userLog.UserLogId.Length > 10)
                throw new ArgumentException("ID no puede exceder 10 caracteres");

            using var connection = new OracleConnection(_connectionString);
            const string query = @"
        UPDATE LogUsuario
        SET 
            fecha = :fecha,
            registro = :registro,
            usuario = :usuario
        WHERE codigoLogUser = :codigoLogUser";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add("fecha", OracleDbType.Date).Value = userLog.Date;
            command.Parameters.Add("registro", OracleDbType.Clob).Value = userLog.Entry;
            command.Parameters.Add("usuario", OracleDbType.Varchar2).Value = 
                userLog.UserId ?? (object)DBNull.Value;
            command.Parameters.Add("codigoLogUser", OracleDbType.Varchar2).Value = userLog.UserLogId;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }
    }
}