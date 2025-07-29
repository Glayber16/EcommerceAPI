using System;
using System.Collections.Generic;
using Npgsql;
using EcommerceAPI.Models;

namespace EcommerceAPI.DataAccess
{
    public class CarroDAO
    {
        private readonly string CONNECTION_STRING = $"Host={Environment.GetEnvironmentVariable("DB_HOST")};" +
                                                    $"Port={Environment.GetEnvironmentVariable("DB_PORT")};" +
                                                    $"Username={Environment.GetEnvironmentVariable("DB_USER")};" +
                                                    $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};" +
                                                    $"Database={Environment.GetEnvironmentVariable("DB_NAME")};";

        public List<Carro> Listar()
        {
            List<Carro> carros = new List<Carro>();

            try
            {
                using (var conn = new NpgsqlConnection(CONNECTION_STRING))
                {
                    conn.Open();

                    using (var cmd = new NpgsqlCommand(@"
                        SELECT c.id, c.modelo, c.marca, c.preco, c.foto, c.quantidade,
                               cat.id AS categoria_id, cat.descricao
                        FROM carro c
                        INNER JOIN categoria cat ON c.categoria_id = cat.id", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var categoria = new Categoria
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("categoria_id")),
                                descricao = reader.GetString(reader.GetOrdinal("descricao"))
                            };

                            var carro = new Carro
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                Modelo = reader.GetString(reader.GetOrdinal("modelo")),
                                Marca = reader.GetString(reader.GetOrdinal("marca")),
                                Preco = reader.GetDecimal(reader.GetOrdinal("preco")),
                                Foto = reader.GetString(reader.GetOrdinal("foto")),
                                Quantidade = reader.GetInt32(reader.GetOrdinal("quantidade")),

                                Categoria = categoria
                            };

                            carros.Add(carro);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao listar carros: {ex.Message}");
            }

            return carros;
        }

        public bool Inserir(Carro carro)
        {
            bool sucesso = false;

            try
            {
                using (var conn = new NpgsqlConnection(CONNECTION_STRING))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand(@"INSERT INTO carro
                        (modelo, marca, preco, foto, quantidade, categoria_id)
                        VALUES (@modelo, @marca, @preco, @foto, @quantidade, @categoria_id)", conn))
                    {
                        cmd.Parameters.AddWithValue("@modelo", carro.Modelo);
                        cmd.Parameters.AddWithValue("@marca", carro.Marca);
                        cmd.Parameters.AddWithValue("@preco", carro.Preco);
                        cmd.Parameters.AddWithValue("@foto", carro.Foto);
                        cmd.Parameters.AddWithValue("@quantidade", carro.Quantidade);

                        cmd.Parameters.AddWithValue("@categoria_id", carro.Categoria.Id);

                        sucesso = (cmd.ExecuteNonQuery() == 1);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao inserir carro: {ex.Message}");
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
                    using (var cmd = new NpgsqlCommand("DELETE FROM carro WHERE id = @id", conn))
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

        public bool Atualizar(Carro carro)
        {
            bool sucesso = false;

            try
            {
                using (var conn = new NpgsqlConnection(CONNECTION_STRING))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand(@"UPDATE carro
                        SET modelo = @modelo, marca = @marca, preco = @preco,
                            foto = @foto, quantidade = @quantidade, categoria_id = @categoria_id
                        WHERE id = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@modelo", carro.Modelo);
                        cmd.Parameters.AddWithValue("@marca", carro.Marca);
                        cmd.Parameters.AddWithValue("@preco", carro.Preco);
                        cmd.Parameters.AddWithValue("@foto", carro.Foto);
                        cmd.Parameters.AddWithValue("@quantidade", carro.Quantidade);

                        cmd.Parameters.AddWithValue("@categoria_id", carro.Categoria.Id);
                        cmd.Parameters.AddWithValue("@id", carro.Id);

                        sucesso = (cmd.ExecuteNonQuery() == 1);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar carro: {ex.Message}");
                sucesso = false;
            }

            return sucesso;
        }

        public List<Carro> Buscar(string termo)
        {
            List<Carro> carros = new List<Carro>();

            try
            {
                using (var conn = new NpgsqlConnection(CONNECTION_STRING))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand(@"
                        SELECT c.id, c.modelo, c.marca, c.preco, c.foto, c.quantidade,
                               cat.id AS categoria_id, cat.descricao
                        FROM carro c
                        INNER JOIN categoria cat ON c.categoria_id = cat.id
                        WHERE LOWER(c.modelo) LIKE LOWER(@termo)
                           OR LOWER(c.marca) LIKE LOWER(@termo)", conn))
                    {
                        cmd.Parameters.AddWithValue("@termo", "%" + termo + "%");

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var categoria = new Categoria
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("categoria_id")),
                                    descricao = reader.GetString(reader.GetOrdinal("descricao"))
                                };

                                var carro = new Carro
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                                    Modelo = reader.GetString(reader.GetOrdinal("modelo")),
                                    Marca = reader.GetString(reader.GetOrdinal("marca")),
                                    Preco = reader.GetDecimal(reader.GetOrdinal("preco")),
                                    Foto = reader.GetString(reader.GetOrdinal("foto")),
                                    Quantidade = reader.GetInt32(reader.GetOrdinal("quantidade")),

                                    Categoria = categoria
                                };

                                carros.Add(carro);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar carros: {ex.Message}");
            }

            return carros;
        }

  public Carro? ObterPorId(int id)
        {
            Carro? carro = null;
            try
            {
                using (var conn = new NpgsqlConnection(CONNECTION_STRING))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand(@"
                        SELECT c.id, c.modelo, c.marca, c.preco, c.foto, c.quantidade, c.categoria_id,
                               cat.id AS categoria_id_map, cat.descricao
                        FROM carro c
                        INNER JOIN categoria cat ON c.categoria_id = cat.id
                        WHERE c.id = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var categoria = new Categoria
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("categoria_id_map")),
                                    descricao = reader.GetString(reader.GetOrdinal("descricao"))
                                };
                                carro = new Carro
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                                    Modelo = reader.GetString(reader.GetOrdinal("modelo")),
                                    Marca = reader.GetString(reader.GetOrdinal("marca")),
                                    Preco = reader.GetDecimal(reader.GetOrdinal("preco")),
                                    Foto = reader.GetString(reader.GetOrdinal("foto")),
                                    Quantidade = reader.GetInt32(reader.GetOrdinal("quantidade")),
                                    CategoriaId = reader.GetInt32(reader.GetOrdinal("categoria_id")),
                                    Categoria = categoria
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Erro ao obter carro por ID: {ex.Message}");
            }
            return carro;
        }
    }
    

}