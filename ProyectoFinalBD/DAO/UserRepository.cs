using System;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Threading.Tasks;
using ProyectoFinalBD.Model;
using ProyectoFinalBD.DAO;

namespace ProyectoFinalBD.DAO;

public class UserRepository
{
    private readonly string _connectionString;

    public UserRepository()
    {
        _connectionString = DbConnectionFactory.GetConnectionString();
    }

    public async Task CreateUser(User user)
    {
        using var connection = new OracleConnection(_connectionString);
        const string query = @"
            INSERT INTO Usuario 
            (codigoUser, cedulaUser, nombreUser, correoUser, contrasenaUser, rolUsuario, municipio) 
            VALUES 
            (:codigoUser, :cedulaUser, :nombreUser, :correoUser, :contrasenaUser, :rolUsuario, :municipio)";

        using var command = new OracleCommand(query, connection);

        command.Parameters.Add("codigoUser", OracleDbType.Varchar2).Value = user.UserId;
        command.Parameters.Add("cedulaUser", OracleDbType.Varchar2).Value = user.Cedula;
        command.Parameters.Add("nombreUser", OracleDbType.Varchar2).Value = user.Name;
        command.Parameters.Add("correoUser", OracleDbType.Varchar2).Value = user.Email;
        command.Parameters.Add("contrasenaUser", OracleDbType.Varchar2).Value = user.Password;
        command.Parameters.Add("rolUsuario", OracleDbType.Varchar2).Value =
            user.UserRoleId ?? (object)DBNull.Value;
        command.Parameters.Add("municipio", OracleDbType.Varchar2).Value =
            user.MunicipalityId ?? (object)DBNull.Value;

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    public async Task<User?> GetUserById(string userId)
    {
        using var connection = new OracleConnection(_connectionString);
        const string query = @"
            SELECT u.*, r.nombreRolUsuario, m.nombreMunicipio
            FROM Usuario u
            LEFT JOIN RolUsuario r ON u.rolUsuario = r.codigoRolUsuario
            LEFT JOIN Municipio m ON u.municipio = m.codigoMunicipio
            WHERE u.codigoUser = :userId";

        using var command = new OracleCommand(query, connection);
        command.Parameters.Add("userId", OracleDbType.Varchar2).Value = userId;

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return MapUserFromReader(reader);
    }

    public async Task<List<User>> GetAllUsers()
    {
        using var connection = new OracleConnection(_connectionString);
        const string query = @"
            SELECT u.*, r.nombreRolUsuario, m.nombreMunicipio
            FROM Usuario u
            LEFT JOIN RolUsuario r ON u.rolUsuario = r.codigoRolUsuario
            LEFT JOIN Municipio m ON u.municipio = m.codigoMunicipio";

        using var command = new OracleCommand(query, connection);
        var users = new List<User>();

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            users.Add(MapUserFromReader(reader));
        }

        return users;
    }

    public async Task Update(User user)
    {
        using var connection = new OracleConnection(_connectionString);
        const string query = @"
            UPDATE Usuario 
            SET cedulaUser = :cedulaUser,
                nombreUser = :nombreUser, 
                correoUser = :correoUser, 
                contrasenaUser = :contrasenaUser,
                rolUsuario = :rolUsuario,
                municipio = :municipio
            WHERE codigoUser = :codigoUser";

        using var command = new OracleCommand(query, connection);
        command.Parameters.Add("cedulaUser", OracleDbType.Varchar2).Value = user.Cedula;
        command.Parameters.Add("nombreUser", OracleDbType.Varchar2).Value = user.Name;
        command.Parameters.Add("correoUser", OracleDbType.Varchar2).Value = user.Email;
        command.Parameters.Add("contrasenaUser", OracleDbType.Varchar2).Value = user.Password;
        command.Parameters.Add("rolUsuario", OracleDbType.Varchar2).Value =
            user.UserRoleId ?? (object)DBNull.Value;
        command.Parameters.Add("municipio", OracleDbType.Varchar2).Value =
            user.MunicipalityId ?? (object)DBNull.Value;
        command.Parameters.Add("codigoUser", OracleDbType.Varchar2).Value = user.UserId;

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    public async Task Delete(string userId)
    {
        using var connection = new OracleConnection(_connectionString);
        const string query = "DELETE FROM Usuario WHERE codigoUser = :userId";

        using var command = new OracleCommand(query, connection);
        command.Parameters.Add("userId", OracleDbType.Varchar2).Value = userId;

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    private static User MapUserFromReader(IDataReader reader)
    {
        return new User
        {
            UserId = reader["codigoUser"].ToString()!,
            Cedula = reader["cedulaUser"].ToString(),
            Name = reader["nombreUser"].ToString()!,
            Email = reader["correoUser"].ToString()!,
            Password = reader["contrasenaUser"].ToString()!,
            UserRoleId = reader["rolUsuario"].ToString(),
            MunicipalityId = reader["municipio"].ToString()
        };
    }

    public async Task<User> ValidateLogin(string email, string password)
    {
        using var connection = new OracleConnection(_connectionString);
        const string query = @"
        SELECT u.*, r.nombreRolUsuario AS role_name 
        FROM Usuario u
        LEFT JOIN RolUsuario r ON u.rolUsuario = r.codigoRolUsuario
        WHERE u.correoUser = :email 
        AND u.contrasenaUser = :password";

        using var command = new OracleCommand(query, connection);
        command.Parameters.Add("email", OracleDbType.Varchar2).Value = email;
        command.Parameters.Add("password", OracleDbType.Varchar2).Value = password;

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return MapUserFromReader(reader);
        }

        return null;
    }
}