using Dapper;
using ErrorLogReader.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ErrorLogReader.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void InsertLogEntry(LogEntryDto logEntryDto)
        {
            try
            {
                using IDbConnection dbConnection = new SqlConnection(_connectionString);
                dbConnection.Open();

                string sql = "INSERT INTO Logentries (Text, Timestamp, CreatedDate, CreatedUser, UpdatedDate, UpdatedUser, ModuleName) " +
                             "VALUES (@Text, @Timestamp, @CreatedDate, @CreatedUser, @UpdatedDate, @UpdatedUser, @ModuleName)";

                dbConnection.Execute(sql, logEntryDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while saving changes: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner Exception: " + ex.InnerException.Message);
                }
            }
        }
    }
}
