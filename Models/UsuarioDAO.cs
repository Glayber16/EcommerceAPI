using EcommerceAPI.Models;
using Npgsql;
using System;
using System.Collections.Generic;

namespace EcommerceAPI.DataAccess
{
    public class UsuarioDAO
    {
        private readonly string CONNECTION_STRING = $"Host={Environment.GetEnvironmentVariable("DB_HOST")};" +
                                                    $"Port={Environment.GetEnvironmentVariable("DB_PORT")};" +
                                                    $"Username={Environment.GetEnvironmentVariable("DB_USER")};" +
                                                    $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};" +
                                                    $"Database={Environment.GetEnvironmentVariable("DB_NAME")};";


        public bool InserirUsuario(Usuario usuario)
        {
            try
            {
                using (var conn = new NpgsqlConnection(CONNECTION_STRING))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("INSERT INTO usuario (nome, endereco, email, login, senha, administrador) VALUES (@nome, @endereco, @email, @login, @senha, @adm)", conn))
                    {
                        cmd.Parameters.AddWithValue("@nome", usuario.Nome);
                        cmd.Parameters.AddWithValue("@endereco", usuario.Endereco);
                        cmd.Parameters.AddWithValue("@email", usuario.Email);
                        cmd.Parameters.AddWithValue("@login", usuario.Login);
                        cmd.Parameters.AddWithValue("@senha", usuario.Senha);
                        cmd.Parameters.AddWithValue("@adm", usuario.Adm);
                        return cmd.ExecuteNonQuery() == 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao inserir usuário: {ex.Message}");
                return false;
            }
        }


        public Usuario? Obter(string login, string senha)
        {
            Usuario? usuario = null;

            try
            {
                using (var conn = new NpgsqlConnection(CONNECTION_STRING))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT id, nome, endereco, email, login, senha, administrador FROM usuario WHERE login = @login AND senha = @senha", conn))
                    {
                        cmd.Parameters.AddWithValue("@login", login);
                        cmd.Parameters.AddWithValue("@senha", senha);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                usuario = new Usuario
                                {
                                    Id = reader.GetInt32(0),
                                    Nome = reader.GetString(1),
                                    Endereco = reader.GetString(2),
                                    Email = reader.GetString(3),
                                    Login = reader.GetString(4),
                                    Senha = reader.GetString(5),
                                    Adm = reader.GetBoolean(6)
                                };
                            }
                        }
                    }
                }
            }
            catch
            {
                usuario = null;
            }

            return usuario;
        }

        public bool Atualizar(string nome, string endereco, string email, string login, string senha, int id)
        {
            bool sucesso = false;

            try
            {
                using (var conn = new NpgsqlConnection(CONNECTION_STRING))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("UPDATE usuario SET nome = @nome, endereco = @endereco, email = @email, login = @login, senha = @senha WHERE id = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@nome", nome);
                        cmd.Parameters.AddWithValue("@endereco", endereco);
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@login", login);
                        cmd.Parameters.AddWithValue("@senha", senha);
                        cmd.Parameters.AddWithValue("@id", id);

                        sucesso = (cmd.ExecuteNonQuery() == 1);
                    }
                }
            }
            catch
            {
                sucesso = false;
            }

            return sucesso;
        }

        public bool Remover(int id)
        {
            bool sucesso = false;

            try
            {
                using (var conn = new NpgsqlConnection(CONNECTION_STRING))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("DELETE FROM usuario WHERE id = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        sucesso = (cmd.ExecuteNonQuery() == 1);
                    }
                }
            }
            catch
            {
                sucesso = false;
            }

            return sucesso;
        }


        public List<Usuario> ObterTodos()
        {
            List<Usuario> usuarios = new List<Usuario>();

            try
            {
                using (var conn = new NpgsqlConnection(CONNECTION_STRING)) 
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT id, nome, endereco, email, login, senha, administrador FROM usuario", conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read()) 
                            {
                                usuarios.Add(new Usuario
                                {
                                    Id = reader.GetInt32(0),
                                    Nome = reader.GetString(1),
                                    Endereco = reader.GetString(2),
                                    Email = reader.GetString(3),
                                    Login = reader.GetString(4),
                                    Senha = reader.GetString(5), 
                                    Adm = reader.GetBoolean(6)
                                });
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                Console.Error.WriteLine($"Erro de banco de dados ao obter todos os usuários: {ex.Message}");
                
                return new List<Usuario>();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ocorreu um erro inesperado ao obter todos os usuários: {ex.Message}");
                return new List<Usuario>(); 
            }

            return usuarios;
        }
    }
}