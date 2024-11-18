using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using MySqlConnector;

public static class MySqlHelper
{

    public static void ExecuteTransaction(string connectionString, Action<MySqlCommand> action)
    {
        using (var conn = new MySqlConnection(connectionString))
        {
            conn.Open();
            using (var transaction = conn.BeginTransaction())
            {
                try
                {
                    var command = new MySqlCommand
                    {
                        Connection = conn,
                        Transaction = transaction
                    };

                    action(command);

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }

    public static async Task ExecuteTransactionAsync(string connectionString, Func<MySqlCommand, Task> action)
    {
        await using (var conn = new MySqlConnection(connectionString))
        {
            await conn.OpenAsync();
            await using (var transaction = await conn.BeginTransactionAsync())
            {
                try
                {
                    var command = new MySqlCommand
                    {
                        Connection = conn,
                        Transaction = transaction
                    };

                    await action(command);

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }
    }

    public static object ExecuteScalar(string connectionString, string sql, params MySqlParameter[] parameters)
    {
        using (var conn = new MySqlConnection(connectionString))
        {
            conn.Open();
            using (var cmd = new MySqlCommand(sql, conn))
            {
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                return cmd.ExecuteScalar();
            }
        }
    }

    public static void ExecuteNonQuery(string connectionString, string sql, params MySqlParameter[] parameters)
    {
        using (var conn = new MySqlConnection(connectionString))
        {
            conn.Open();
            using (var cmd = new MySqlCommand(sql, conn))
            {
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                cmd.ExecuteNonQuery();
            }
        }
    }

    public static async Task ExecuteNonQueryAsync(string connectionString, string sql, params MySqlParameter[] parameters)
    {
        using (var conn = new MySqlConnection(connectionString))
        {
            await conn.OpenAsync();
            using (var cmd = new MySqlCommand(sql, conn))
            {
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }

    public static DataRow ExecuteDataRow(string connectionString, string sql, params MySqlParameter[] parameters)
    {
        var dt = new DataTable();
        using (var conn = new MySqlConnection(connectionString))
        {
            conn.Open();
            using (var cmd = new MySqlCommand(sql, conn))
            {
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                using (var reader = cmd.ExecuteReader())
                {
                    dt.Load(reader);
                    if (dt.Rows.Count > 0)
                    {
                        return dt.Rows[0];
                    }
                    return null;
                }
            }
        }
    }

    public static async Task<DataRow> ExecuteDataRowAsync(string connectionString, string sql, params MySqlParameter[] parameters)
    {
        var dt = new DataTable();
        using (var conn = new MySqlConnection(connectionString))
        {
            await conn.OpenAsync();
            using (var cmd = new MySqlCommand(sql, conn))
            {
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                    if (dt.Rows.Count > 0)
                    {
                        return dt.Rows[0];
                    }
                    return null;
                }
            }
        }
    }

    public static DataSet ExecuteDataSet(string connectionString, string sql, params MySqlParameter[] parameters)
    {
        var ds = new DataSet();
        using (var conn = new MySqlConnection(connectionString))
        {
            conn.Open();
            using (var cmd = new MySqlCommand(sql, conn))
            {
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                using (var adapter = new MySqlDataAdapter(cmd))
                {
                    adapter.Fill(ds);
                }
            }
        }
        return ds;
    }

    public static async Task<DataSet> ExecuteDataSetAsync(string connectionString, string sql, params MySqlParameter[] parameters)
    {
        var ds = new DataSet();
        using (var conn = new MySqlConnection(connectionString))
        {
            await conn.OpenAsync();
            using (var cmd = new MySqlCommand(sql, conn))
            {
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                using (var adapter = new MySqlDataAdapter(cmd))
                {
                    await Task.Run(() => adapter.Fill(ds));
                }
            }
        }
        return ds;
    }

    public static List<List<object>> ExecuteReader(string connectionString, string sql, params MySqlParameter[] parameters)
    {
        var result = new List<List<object>>();
        using (var conn = new MySqlConnection(connectionString))
        {
            conn.Open();
            using (var cmd = new MySqlCommand(sql, conn))
            {
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var row = new List<object>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row.Add(reader.GetValue(i));
                        }
                        result.Add(row);
                    }
                }
            }
        }
        return result;
    }

    public static async Task<List<List<object>>> ExecuteReaderAsync(string connectionString, string sql, params MySqlParameter[] parameters)
    {
        var result = new List<List<object>>();
        using (var conn = new MySqlConnection(connectionString))
        {
            await conn.OpenAsync();
            using (var cmd = new MySqlCommand(sql, conn))
            {
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var row = new List<object>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row.Add(reader.GetValue(i));
                        }
                        result.Add(row);
                    }
                }
            }
        }
        return result;
    }

    public static MySqlDataReader GetReader(string connectionString, string sql, params MySqlParameter[] parameters)
    {
        var conn = new MySqlConnection(connectionString);
        conn.Open();
        using (var cmd = new MySqlCommand(sql, conn))
        {
            if (parameters != null)
            {
                cmd.Parameters.AddRange(parameters);
            }
            return cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
        }
    }
    
    private static async Task<MySqlDataReader> GetReaderAsync(string connectionString, string sql, params MySqlParameter[] parameters)
    {
        var conn = new MySqlConnection(connectionString);
        await conn.OpenAsync();
        using (var cmd = new MySqlCommand(sql, conn))
        {
            if (parameters != null)
            {
                cmd.Parameters.AddRange(parameters);
            }
            return await cmd.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection);
        }
    } 
}
