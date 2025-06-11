using EcommerceAPI.Models;
using Npgsql;
using System;
using System.Collections.Generic;

namespace EcommerceAPI.DataAccess
{
    public class UsuarioDAO
    {
        private const string CONNECTION_STRING = "Host=localhost;Port=5432;Username=postgres;Password=ufc123;Database=smdecommerce";

        public bool InserirCliente(string nome, string endereco, string email, string login, string senha)
        {
            bool sucesso = false;

            try
            {
                using (var conn = new NpgsqlConnection(CONNECTION_STRING))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("INSERT INTO usuario (nome, endereco, email, login, senha, administrador) VALUES (@nome, @endereco, @email, @login, @senha, FALSE)", conn))
                    {
                        cmd.Parameters.AddWithValue("@nome", nome);
                        cmd.Parameters.AddWithValue("@endereco", endereco);
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@login", login);
                        cmd.Parameters.AddWithValue("@senha", senha);
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

        public bool InserirAdministrador(string nome, string endereco, string email, string login, string senha)
        {
            bool sucesso = false;

            try
            {
                using (var conn = new NpgsqlConnection(CONNECTION_STRING))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("INSERT INTO usuario (nome, endereco, email, login, senha, administrador) VALUES (@nome, @endereco, @email, @login, @senha, TRUE)", conn))
                    {
                        cmd.Parameters.AddWithValue("@nome", nome);
                        cmd.Parameters.AddWithValue("@endereco", endereco);
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@login", login);
                        cmd.Parameters.AddWithValue("@senha", senha);
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
    }
}
