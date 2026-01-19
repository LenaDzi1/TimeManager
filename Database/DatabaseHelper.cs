// Import przestrzeni nazw
using System.Data;              // Typy bazodanowe
using System.Data.SqlClient;    // Klient SQL Server
using System;                   // DateTime

namespace TimeManager.Database
{
    /// <summary>
    /// Helper do operacji na bazie danych SQL Server.
    /// 
    /// Udostępnia statyczne metody do:
    /// - Tworzenia połączeń (GetConnection, OpenConnection)
    /// - Wykonywania zapytań (ExecuteQuery, ExecuteScalar, ExecuteNonQuery)
    /// - Testowania połączenia (TestConnection)
    /// </summary>
    public class DatabaseHelper
    {
        // Connection string do bazy danych
        private static string _connectionString = @"Server=localhost\SQLEXPRESS;Database=TimeManagerDB;Integrated Security=true;TrustServerCertificate=true;";

        /// <summary>
        /// Obcina milisekundy z parametrów DateTime, aby zapewnić spójność bazy danych.
        /// </summary>
        private static void SanitizeParameters(SqlParameter[] parameters)
        {
            if (parameters == null) return;
            
            foreach (var p in parameters)
            {
                if (p.Value is DateTime dt)
                {
                    // Zostawiamy tylko sekundy (ucinamy milisekundy)
                    p.Value = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Kind);
                }
            }
        }

        /// <summary>
        /// Tworzy nowe połączenie SQL (nieotwarte).
        /// </summary>
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        /// <summary>
        /// Tworzy i otwiera nowe połączenie SQL.
        /// </summary>
        public static SqlConnection OpenConnection()
        {
            var conn = new SqlConnection(_connectionString);
            conn.Open();
            return conn;
        }

        public static void ExecuteNonQuery(string query, params SqlParameter[] parameters)
        {
            SanitizeParameters(parameters);
            using (var connection = GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public static object ExecuteScalar(string query, params SqlParameter[] parameters)
        {
            SanitizeParameters(parameters);
            using (var connection = GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    connection.Open();
                    return command.ExecuteScalar();
                }
            }
        }

        public static DataTable ExecuteQuery(string query, params SqlParameter[] parameters)
        {
            SanitizeParameters(parameters);
            using (var connection = GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    using (var adapter = new SqlDataAdapter(command))
                    {
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
        }
    }
}





