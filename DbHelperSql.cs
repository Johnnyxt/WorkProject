using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace JW8307A
{
    internal class DbHelperSql
    {
        private static string connString;

        public DbHelperSql(string serverName, string dbName, string userId, string password)
        {
            connString = string.Format("SERVER={0};database={1};user={2};pwd={3}", serverName, dbName, userId, password);
            using (SqlConnection connection = new SqlConnection(connString))
            {
                try
                {
                    connection.Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    IsConnect = false;
                    return;
                }
                IsConnect = true;
            }
        }

        public bool IsConnect { get; set; }

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="sqlString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSql(string sqlString)
        {
            using (SqlConnection connection = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlString, connection))
                {
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (SqlException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }

        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="sqlString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public static object GetSingle(string sqlString)
        {
            using (SqlConnection connection = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlString, connection))
                {
                    try
                    {
                        connection.Open();
                        object obj = cmd.ExecuteScalar();
                        if ((Equals(obj, null)) || (Equals(obj, DBNull.Value)))
                        {
                            return null;
                        }
                        return obj;
                    }
                    catch (SqlException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }

        /// <summary>
        /// 执行查询语句，返回SqlDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="strSql">查询语句</param>
        /// <returns>SqlDataReader</returns>
        public static SqlDataReader ExecuteReader(string strSql)
        {
            SqlConnection connection = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(strSql, connection);
            try
            {
                connection.Open();
                SqlDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return myReader;
            }
            catch (SqlException e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="cmdText">查询语句</param>
        /// <returns>DataSet</returns>
        public static DataSet SelectToDataSet(string cmdText)
        {
            using (SqlConnection connection = new SqlConnection(connString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmdText, connection);
                    da.Fill(ds);
                }
                catch (SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
        }

        /// <summary>
        /// 执行查询语句，返回DataTable
        /// </summary>
        /// <param name="cmdText">查询语句</param>
        /// <param name="index">表索引</param>
        /// <returns></returns>
        public static DataTable SelectToDataTable(string cmdText, int index)
        {
            using (SqlConnection connection = new SqlConnection(connString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmdText, connection);
                    da.Fill(ds);
                }
                catch (SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds.Tables[index];
            }
        }

        /// <summary>
        /// 执行查询语句，返回DataTable
        /// </summary>
        /// <param name="cmdText">查询语句</param>
        /// <param name="tableName">表名称</param>
        /// <returns></returns>
        public static DataTable SelectToDataTable(string cmdText, string tableName)
        {
            using (SqlConnection connection = new SqlConnection(connString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmdText, connection);
                    da.Fill(ds);
                }
                catch (SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds.Tables[tableName];
            }
        }

        /// <summary>
        /// 执行查询语句，返回第一个DataTable
        /// </summary>
        /// <param name="cmdText">查询语句</param>
        /// <returns></returns>
        public static DataTable SelectToDataTable(string cmdText)
        {
            using (SqlConnection connection = new SqlConnection(connString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmdText, connection);
                    da.Fill(ds);
                }
                catch (SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds.Tables[0];
            }
        }

        /// <summary>
        /// 写入数据库
        /// </summary>
        /// <param name="table">datatable</param>
        /// <param name="tableName">数据库表名</param>
        public static void WriteToServer(DataTable table, string tableName)
        {
            SqlBulkCopy sqlbulkcopy = new SqlBulkCopy(connString) { DestinationTableName = tableName };
            for (int i = 0; i < table.Columns.Count; i++)
            {
                sqlbulkcopy.ColumnMappings.Add(table.Columns[i].ColumnName, table.Columns[i].ColumnName);
            }
            sqlbulkcopy.WriteToServer(table);
            sqlbulkcopy.Close();
        }

        public static string InsertData(string strTableName, IList<object> lstValue)
        {
            var lstSqlString = new List<object> { "INSERT INTO ", strTableName, "VALUES", "(" };

            for (var j = 0; j < lstValue.Count; j++)
            {
                if (lstValue[j] is string)
                {
                    lstSqlString.Add("'" + lstValue[j] + "'");
                }
                else
                {
                    lstSqlString.Add(lstValue[j]);
                }
                if (j != lstValue.Count - 1)
                {
                    lstSqlString.Add(",");
                }
            }
            lstSqlString.Add(")");
            var sqlString = string.Join(" ", lstSqlString.ToArray());

            return sqlString;
        }
    }
}