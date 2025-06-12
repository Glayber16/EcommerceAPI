using System;
using System.Collections.Generic;
using Npgsql;
using EcommerceAPI.Models;

namespace EcommerceAPI.DataAccess
{
    public class CategoriaDAO
    {
        private const string CONNECTION_STRING = "Host=localhost;Port=5432;Username=postgres;Password=ufc123;Database=smdecommerce";

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
    }
}
