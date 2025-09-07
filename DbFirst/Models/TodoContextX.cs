using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using DbFirst.Dtos;

#nullable disable

namespace DbFirst.Models
{
    public partial class TodoContext : DbContext
    {
        public List<T> ExecSQL<T>(string query)
        {
            using (var command = Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                command.CommandType = CommandType.Text;
                Database.OpenConnection();
                List<T> list = new List<T>();
                using (var result = command.ExecuteReader())
                {
                    // 获取所有列名
                    var columnNames = new Dictionary<string, int>();
                    for (int i = 0; i < result.FieldCount; i++)
                    {
                        columnNames[result.GetName(i)] = i;
                    }

                    while (result.Read())
                    {
                        T obj = Activator.CreateInstance<T>();
                        foreach (PropertyInfo prop in obj.GetType().GetProperties())
                        {
                            if (columnNames.ContainsKey(prop.Name))
                            {
                                // 获取列的索引
                                int columnIndex = columnNames[prop.Name];
                                if (!object.Equals(result[columnIndex], DBNull.Value))
                                {
                                    prop.SetValue(obj, result[columnIndex], null);
                                }
                            }
                        }
                        list.Add(obj);
                    }
                }
                Database.CloseConnection();
                return list;
            }
        }
    }
}
