using EcommerceAPI.Models;
using Npgsql;
using System;
using System.Collections.Generic;

namespace EcommerceAPI.DataAccess
{
    public class VendaDAO
    {
        private readonly string CONNECTION_STRING;
        private readonly CarroDAO _carroDAO;

        public VendaDAO(string connectionString, CarroDAO carroDAO)
        {
            CONNECTION_STRING = connectionString;
            _carroDAO = carroDAO;
        }

        public VendaDAO()
        {
            CONNECTION_STRING = $"Host={Environment.GetEnvironmentVariable("DB_HOST")};" +
                                $"Port={Environment.GetEnvironmentVariable("DB_PORT")};" +
                                $"Username={Environment.GetEnvironmentVariable("DB_USER")};" +
                                $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};" +
                                $"Database={Environment.GetEnvironmentVariable("DB_NAME")};";
            _carroDAO = new CarroDAO();
        }

        public bool InserirComCarros(Venda venda)
        {
            try
            {
                using (var conn = new NpgsqlConnection(CONNECTION_STRING))
                {
                    conn.Open();
                    using (var trans = conn.BeginTransaction())
                    {
                        int vendaId;
                        using (var cmdVenda = new NpgsqlCommand("INSERT INTO venda (data, usuarioid) VALUES (@data, @usuarioid) RETURNING id", conn, trans))
                        {
                            cmdVenda.Parameters.AddWithValue("@data", venda.data);
                            cmdVenda.Parameters.AddWithValue("@usuarioid", venda.UsuarioId);
                            vendaId = Convert.ToInt32(cmdVenda.ExecuteScalar());
                        }

                        foreach (var item in venda.Itens)
                        {
                            var carroNoEstoque = _carroDAO.ObterPorId(item.CarroId);

                            if (carroNoEstoque == null || carroNoEstoque.Quantidade < item.Quantidade)
                            {
                                trans.Rollback();
                                return false;
                            }

                            decimal precoUnitarioVenda = carroNoEstoque.Preco;
                            decimal precoTotalItem = precoUnitarioVenda * item.Quantidade;

                            using (var cmdItem = new NpgsqlCommand("INSERT INTO vendacarro (vendaid, carroid, quantidade, preco) VALUES (@vendaid, @carroid, @quantidade, @preco)", conn, trans))
                            {
                                cmdItem.Parameters.AddWithValue("@vendaid", vendaId);
                                cmdItem.Parameters.AddWithValue("@carroid", item.CarroId);
                                cmdItem.Parameters.AddWithValue("@quantidade", item.Quantidade);
                                cmdItem.Parameters.AddWithValue("@preco", precoTotalItem);
                                cmdItem.ExecuteNonQuery();
                            }

                            int novaQuantidadeEstoque = carroNoEstoque.Quantidade - item.Quantidade;
                            using (var cmdAtualizarEstoque = new NpgsqlCommand("UPDATE carro SET quantidade = @novaquantidade WHERE id = @carroid", conn, trans))
                            {
                                cmdAtualizarEstoque.Parameters.AddWithValue("@novaquantidade", novaQuantidadeEstoque);
                                cmdAtualizarEstoque.Parameters.AddWithValue("@carroid", item.CarroId);
                                cmdAtualizarEstoque.ExecuteNonQuery();
                            }
                        }

                        trans.Commit();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Erro ao inserir venda com carros: {ex.Message}");
                return false;
            }
        }

        public List<Venda> ObterTodasComItens()
        {
            var vendas = new List<Venda>();

            try
            {
                using (var conn = new NpgsqlConnection(CONNECTION_STRING))
                {
                    conn.Open();

                    var vendasDict = new Dictionary<int, Venda>();
                    using (var cmdVendas = new NpgsqlCommand(
                        "SELECT v.id, v.data, v.usuarioid, u.nome, u.email " +
                        "FROM venda v JOIN usuario u ON v.usuarioid = u.id", conn))
                    {
                        using (var readerVendas = cmdVendas.ExecuteReader())
                        {
                            while (readerVendas.Read())
                            {
                                var venda = new Venda
                                {
                                    Id = readerVendas.GetInt32(0),
                                    data = readerVendas.GetDateTime(1),
                                    UsuarioId = readerVendas.GetInt32(2),
                                    Usuario = new Usuario
                                    {
                                        Id = readerVendas.GetInt32(2),
                                        Nome = readerVendas.GetString(3),
                                        Email = readerVendas.GetString(4)
                                    },
                                    Itens = new List<VendaCarro>()
                                };

                                vendas.Add(venda);
                                vendasDict.Add(venda.Id, venda);
                            }
                        }
                    }

                    using (var cmdItens = new NpgsqlCommand("SELECT id, vendaid, carroid, quantidade, preco FROM vendacarro", conn))
                    {
                        using (var readerItens = cmdItens.ExecuteReader())
                        {
                            while (readerItens.Read())
                            {
                                var vendaId = readerItens.GetInt32(1);
                                if (vendasDict.TryGetValue(vendaId, out var vendaParent))
                                {
                                    vendaParent.Itens.Add(new VendaCarro
                                    {
                                        Id = readerItens.GetInt32(0),
                                        VendaId = vendaId,
                                        CarroId = readerItens.GetInt32(2),
                                        Quantidade = readerItens.GetInt32(3),
                                        Preco = readerItens.GetDecimal(4)
                                    });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Erro ao obter vendas com itens: {ex.Message}");
                return new List<Venda>();
            }

            return vendas;
        }

           public List<Venda> ObterPorUsuario(int usuarioId)
{
    List<Venda> vendas = new List<Venda>();
    var vendasDict = new Dictionary<int, Venda>();

    try
    {
        using (var conn = new NpgsqlConnection(CONNECTION_STRING))
        {
            conn.Open();

            using (var cmd = new NpgsqlCommand(
                "SELECT v.id, v.data, v.usuarioid, u.id AS usuario_id_map, u.nome AS usuario_nome " +
                "FROM venda v JOIN usuario u ON v.usuarioid = u.id " +
                "WHERE v.usuarioid = @usuarioid", conn))
            {
                cmd.Parameters.AddWithValue("@usuarioid", usuarioId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var venda = new Venda
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            data = reader.GetDateTime(reader.GetOrdinal("data")),
                            UsuarioId = reader.GetInt32(reader.GetOrdinal("usuarioid")),
                            Usuario = new Usuario
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("usuario_id_map")),
                                Nome = reader.GetString(reader.GetOrdinal("usuario_nome")),
                            },
                            Itens = new List<VendaCarro>()
                        };

                        vendas.Add(venda);
                        vendasDict[venda.Id] = venda; // <-- ESSA LINHA É FUNDAMENTAL
                    }
                }
            }

            using (var cmdItens = new NpgsqlCommand(
                "SELECT id, vendaid, carroid, quantidade, preco FROM vendacarro", conn))
            {
                using (var readerItens = cmdItens.ExecuteReader())
                {
                    while (readerItens.Read())
                    {
                        var vendaId = readerItens.GetInt32(1);
                        if (vendasDict.TryGetValue(vendaId, out var vendaParent))
                        {
                            vendaParent.Itens.Add(new VendaCarro
                            {
                                Id = readerItens.GetInt32(0),
                                VendaId = vendaId,
                                CarroId = readerItens.GetInt32(2),
                                Quantidade = readerItens.GetInt32(3),
                                Preco = readerItens.GetDecimal(4)
                            });
                        }
                    }
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Erro ao obter vendas por usuário: {ex.Message}");
        return new List<Venda>();
    }

    return vendas;
}
public bool Deletar(int vendaId)
{
    try
    {
        using (var conn = new NpgsqlConnection(CONNECTION_STRING))
        {
            conn.Open();
            using (var trans = conn.BeginTransaction())
            {
                using (var cmdItens = new NpgsqlCommand("DELETE FROM vendacarro WHERE vendaid = @vendaid", conn, trans))
                {
                    cmdItens.Parameters.AddWithValue("@vendaid", vendaId);
                    cmdItens.ExecuteNonQuery();
                }

                using (var cmdVenda = new NpgsqlCommand("DELETE FROM venda WHERE id = @vendaid", conn, trans))
                {
                    cmdVenda.Parameters.AddWithValue("@vendaid", vendaId);
                    int linhasAfetadas = cmdVenda.ExecuteNonQuery();
                    if (linhasAfetadas == 0)
                    {
                        trans.Rollback();
                        return false;
                    }
                }

                trans.Commit();
                return true;
            }
        }
    }
    catch
    {
        return false;
    }
}

    }
}
