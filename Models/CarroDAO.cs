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
    }
}