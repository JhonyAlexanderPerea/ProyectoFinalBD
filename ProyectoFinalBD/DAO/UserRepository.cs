/*using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;


namespace ProyectoFinalBD.DAO;

 public class UserRepository
    {
        private readonly string _connectionString;

        public UserRepository()
        {
            _connectionString = DbConnectionFactory.GetConnectionString();
        }

        public void AddUser(User user)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO Usuarios (nombre, correo, tipo_usuario, contraseña) VALUES (@nombre, @correo, @tipo_usuario, @contraseña)";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@nombre", user.Name);
                    cmd.Parameters.AddWithValue("@correo", user.Email);
                    cmd.Parameters.AddWithValue("@tipo_usuario", user.UserType);
                    cmd.Parameters.AddWithValue("@contraseña", user.Password);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<User> GetAllUsers()
        {
            var users = new List<User>();

            using (var conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Usuarios";
                using (var cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new User
                            {
                                UserId = (int)reader["id_usuario"],
                                Name = reader["nombre"].ToString(),
                                Email = reader["correo"].ToString(),
                                UserType = reader["tipo_usuario"].ToString(),
                                Password = reader["contraseña"].ToString()
                            });
                        }
                    }
                }
            }

            return users;
        }

        public User GetUserById(int id)
        {
            User user = null;

            using (var conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Usuarios WHERE id_usuario = @id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new User
                            {
                                UserId = (int)reader["id_usuario"],
                                Name = reader["nombre"].ToString(),
                                Email = reader["correo"].ToString(),
                                UserType = reader["tipo_usuario"].ToString(),
                                Password = reader["contraseña"].ToString()
                            };
                        }
                    }
                }
            }

            return user;
        }

        public void UpdateUser(User user)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = "UPDATE Usuarios SET nombre = @nombre, correo = @correo, tipo_usuario = @tipo_usuario, contraseña = @contraseña WHERE id_usuario = @id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@nombre", user.Name);
                    cmd.Parameters.AddWithValue("@correo", user.Email);
                    cmd.Parameters.AddWithValue("@tipo_usuario", user.UserType);
                    cmd.Parameters.AddWithValue("@contraseña", user.Password);
                    cmd.Parameters.AddWithValue("@id", user.UserId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteUser(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM Usuarios WHERE id_usuario = @id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }*/