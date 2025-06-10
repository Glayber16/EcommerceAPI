using System;
using System.Collections.Generic;
using System.Data;
using Npgsql; // Certifique-se de adicionar o pacote Npgsql via NuGet
using Modelo.Categoria;

namespace Modelo
{
    public class CategoriaDAO
    {
        private const string CONNECTION_STRING = "Host=localhost;Port=5432;Username=postgres;Password=ufc123;Database=smdecommerce";

        /// <summary>
        /// Método para listar todas as categorias existentes
        /// </summary>
        /// <returns>Lista de categorias</returns>
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
                                Descricao = reader.GetString(1)
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

        /// <summary>
        /// Método para inserir uma nova categoria
        /// </summary>
        /// <param name="descricao">Descrição da categoria</param>
        /// <returns>True se a inserção for bem-sucedida</returns>
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
