using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    public class MySQL
    {
        // 存储多个数据库连接字符串
        private List<string> _connectionStrings = new List<string>();

        // 用于轮询选择连接字符串的索引
        private int _connectionIndex = 0;

        // 线程安全的锁对象
        private readonly object _lock = new object();
        public void Init(string[] connectionStrings)
        {
            if (connectionStrings == null || connectionStrings.Length == 0)
                throw new ArgumentException("连接字符串数组不能为空。");

            _connectionStrings.AddRange(connectionStrings);
        }
        public async Task<List<Dictionary<string, object>>> ExeQuery(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                throw new ArgumentException("SQL 查询不能为空。");

            // 获取下一个连接字符串
            string connectionString = GetNextConnectionString();

            // 创建连接和命令
            using (var connection = new MySqlConnection(connectionString))
            using (var command = new MySqlCommand(sql, connection))
            {
                try
                {
                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var results = new List<Dictionary<string, object>>();

                        while (await reader.ReadAsync())
                        {
                            var row = new Dictionary<string, object>();

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[reader.GetName(i)] = await reader.IsDBNullAsync(i) ? null : reader.GetValue(i);
                            }

                            results.Add(row);
                        }

                        return results;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"执行查询时出错：{ex.Message}");
                    throw;
                }
            }
        }

        /// <summary>
        /// 获取下一个连接字符串，实现简单的轮询负载均衡
        /// </summary>
        /// <returns>数据库连接字符串</returns>
        private string GetNextConnectionString()
        {
            lock (_lock)
            {
                var connStr = _connectionStrings[_connectionIndex];
                _connectionIndex = (_connectionIndex + 1) % _connectionStrings.Count;
                return connStr;
            }
        }
    }
}
