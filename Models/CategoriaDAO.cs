using System;
using System.Collections.Generic;
using Npgsql;
using EcommerceAPI.Models;

namespace EcommerceAPI.DataAccess
{
    public class CategoriaDAO
    {
        private readonly string CONNECTION_STRING = $"Host={Environment.GetEnvironmentVariable("DB_HOST")};" +
                                             $"Port={Environment.GetEnvironmentVariable("DB_PORT")};" +
                                             $"Username={Environment.GetEnvironmentVariable("DB_USER")};" +
                                             $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};" +
                                             $"Database={Environment.GetEnvironmentVariable("DB_NAME")};";


        public List<Categoria> Listar()
        {
            List<Categoria> categorias = new List<Categoria>();

            try
            {
                using (var conn = new NpgsqlConnection(CONNECTION_STRING))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT id, descricao FROM categoria", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Categoria categoria = new Categoria
                            {
                                Id = reader.GetInt32(0),
                                descricao = reader.GetString(1)
                            };
                            categorias.Add(categoria);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return categorias;
        }

        public bool Inserir(string descricao)
        {
            bool sucesso = false;

            try
            {
                using (var conn = new NpgsqlConnection(CONNECTION_STRING))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("INSERT INTO categoria (descricao) VALUES (@descricao)", conn))
                    {
                        cmd.Parameters.AddWithValue("@descricao", descricao);
                        sucesso = (cmd.ExecuteNonQuery() == 1);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
                    using (var cmd = new NpgsqlCommand("DELETE FROM categoria WHERE id = @id", conn))
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
        
          public bool Atualizar(string descricao, int id)
        {
            bool sucesso = false;

            try
            {
                using (var conn = new NpgsqlConnection(CONNECTION_STRING))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("UPDATE categoria SET descricao = @descricao  WHERE id = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@descricao", descricao);
                        
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
