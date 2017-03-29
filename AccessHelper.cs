using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Windows;

namespace JW8307A
{
    internal class AccessHelper
    {
        public OleDbConnection Conn;
        public string ConnString; //连接字符串
        protected static OleDbConnection conn = new OleDbConnection();
        protected static OleDbCommand comm = new OleDbCommand();

        //打开数据库
        public static void OpenConnection()
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=.//data.mdb";
                comm.Connection = conn;
            }
            try
            {
                conn.Open();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //关闭数据库
        private static void CloseConnection()
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
                conn.Dispose();
                comm.Dispose();
            }
        }

        //执行sql语句
        public static void ExcuteSql(string sqlstr)
        {
            try
            {
                OpenConnection();
                comm.CommandType = CommandType.Text;
                comm.CommandText = sqlstr;
                comm.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                CloseConnection();
            }
        }

        //返回指定sql语句的dataset
        public static DataSet DataSet(string sqlstr)
        {
            DataSet ds = new DataSet();
            OleDbDataAdapter da = new OleDbDataAdapter();
            try
            {
                OpenConnection();
                comm.CommandType = CommandType.Text;
                comm.CommandText = sqlstr;
                da.SelectCommand = comm;
                da.Fill(ds);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                CloseConnection();
            }
            return ds;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="strDbName">数据库名称</param>
        public AccessHelper(string strDbName)
        {
            ConnString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + strDbName;
            if (Conn == null)
            {
                Conn = new OleDbConnection(ConnString);
            }
            if (Conn.State != ConnectionState.Open)
            {
                try
                {
                    Conn.Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="strTableName"></param>
        /// <param name="lstField"></param>
        /// <param name="lstFieldPara"></param>
        /// <returns></returns>
        public string InsertData(string strTableName, IList<string> lstField, IList<string> lstFieldPara)
        {
            var lstSqlString = new List<string> { "INSERT INTO ", strTableName, "(" };

            for (var j = 0; j < lstField.Count; j++)
            {
                lstSqlString.Add(lstField[j]);
                if (j != lstField.Count - 1)
                {
                    lstSqlString.Add(",");
                }
            }
            lstSqlString.Add(")");
            lstSqlString.Add("VALUES");
            lstSqlString.Add("(");
            for (var j = 0; j < lstFieldPara.Count; j++)
            {
                lstSqlString.Add(lstFieldPara[j]);
                if (j != lstFieldPara.Count - 1)
                {
                    lstSqlString.Add(",");
                }
            }
            lstSqlString.Add(")");
            var sqlString = string.Join(" ", lstSqlString.ToArray());

            return sqlString;
        }

        /// <summary>
        /// </summary>
        /// <param name="strTableName">表名称</param>
        /// <param name="lstValue">数据集</param>
        /// <returns></returns>
        public string InsertData(string strTableName, IList<object> lstValue)
        {
            var lstSqlString = new List<object> { "INSERT INTO ", strTableName, "VALUES", "(" };

            for (var j = 0; j < lstValue.Count; j++)
            {
                lstSqlString.Add(lstValue[j]);
                if (j != lstValue.Count - 1)
                {
                    lstSqlString.Add(",");
                }
            }
            lstSqlString.Add(")");
            var sqlString = string.Join(" ", lstSqlString.ToArray());

            return sqlString;
        }

        /// <summary>
        /// </summary>
        /// <param name="sqlString"></param>
        public void ExecuteSql(string sqlString)
        {
            int ret = 0;
            var conn = new OleDbConnection(ConnString);
            var cmd = new OleDbCommand(sqlString, conn);
            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sqlString"></param>
        /// <param name="parameters"></param>
        public void ExecuteSql(string sqlString, params OleDbParameter[] parameters)
        {
            var conn = new OleDbConnection(ConnString);
            var cmd = new OleDbCommand(sqlString, conn) { CommandType = CommandType.Text };
            if (parameters != null)
            {
                foreach (var p in parameters)
                {
                    if ((p.Direction == ParameterDirection.Output) && p.Value == null) p.Value = DBNull.Value;
                    cmd.Parameters.Add(p);
                }
            }
            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        ///     返回数据库中所有表名称
        /// </summary>
        /// <returns></returns>
        public string[] GetShemaTableName()
        {
            try
            {
                var conn = new OleDbConnection(ConnString);
                //获取数据表

                conn.Open();
                var shemaTable = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables,
                    new object[] { null, null, null, "TABLE" });
                var n = shemaTable.Rows.Count;
                var strTable = new string[n];
                var m = shemaTable.Columns.IndexOf("TABLE_NAME");
                for (var i = 0; i < n; i++)
                {
                    var dr = shemaTable.Rows[i];
                    strTable[i] = dr.ItemArray.GetValue(m).ToString();
                }
                return strTable;
            }
            catch (OleDbException ex)
            {
                MessageBox.Show("指定的限制集无效:\n" + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public DataSet SelectToDataSet(string cmdText)
        {
            //DataSet ds = new DataSet();
            //OleDbDataAdapter da = new OleDbDataAdapter();
            //try
            //{
            //    OpenConnection();
            //    comm.CommandType = CommandType.Text;
            //    comm.CommandText = cmdText;
            //    da.SelectCommand = comm;
            //    da.Fill(ds);
            //}
            //catch (Exception e)
            //{
            //    throw new Exception(e.Message);
            //}
            //finally
            //{
            //    CloseConnection();
            //}
            //return ds;
            var conn = new OleDbConnection(ConnString);
            var ds = new DataSet();
            try
            {
                var da = new OleDbDataAdapter();
                da.SelectCommand = new OleDbCommand();
                da.SelectCommand.Connection = conn;
                da.SelectCommand.CommandText = cmdText;
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return ds;
        }

        /// <summary>
        /// 返回数据集行数
        /// </summary>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public int GetDsRows(string cmdText)
        {
            var conn = new OleDbConnection(ConnString);
            var ds = new DataSet();
            try
            {
                var da = new OleDbDataAdapter();
                da.SelectCommand = new OleDbCommand();
                da.SelectCommand.Connection = conn;
                da.SelectCommand.CommandText = cmdText;
                da.Fill(ds);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                conn.Close();
            }
            return ds.Tables[0].Rows.Count;
        }

        /// <summary>
        ///     根据SQL命令返回数据DataSet数据集
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="subtableName">在返回的数据集中所添加的表的名称</param>
        /// <returns></returns>
        public DataSet SelectToDataSet(string cmdText, string subtableName)
        {
            var adapter = new OleDbDataAdapter();
            var command = new OleDbCommand(cmdText, Conn);
            adapter.SelectCommand = command;
            var ds = new DataSet();
            ds.Tables.Add(subtableName);
            adapter.Fill(ds, subtableName);
            return ds;
        }

        /// <summary>
        /// 返回数据库表的所有字段名称
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>

        public string[] GetTableColumn(string tableName)
        {
            DataTable dt = new DataTable();
            try
            {
                if (Conn.State == ConnectionState.Closed)
                {
                    Conn.Open();
                }
                dt = Conn.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new object[] { null, null, tableName, null });
                int n = dt.Rows.Count;
                string[] strTable = new string[n];
                int m = dt.Columns.IndexOf("COLUMN_NAME");
                for (int i = 0; i < n; i++)
                {
                    DataRow dr = dt.Rows[i];
                    strTable[i] = dr.ItemArray.GetValue(m).ToString();
                }
                return strTable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Conn.Close();
            }
        }

        public List<string> GetColumnField(string tableName)
        {
            List<string> columnList = new List<string>();
            try
            {
                if (Conn.State == ConnectionState.Closed)
                {
                    Conn.Open();
                }
                OleDbCommand cmd = new OleDbCommand("Select Name FROM SysColumns Where id=Object_Id('" + tableName + "')", Conn);
                OleDbDataReader reader = cmd.ExecuteReader();
                while (reader != null && reader.Read())
                {
                    columnList.Add(reader[0].ToString());
                }
            }
            catch (Exception)
            {
                throw;
            }
            Conn.Close();
            return columnList;
        }

        //public string ExecuteReader(string cmdText, string column)
        //{
        //    try
        //    {
        //        if (Conn.State == ConnectionState.Closed)
        //        {
        //            Conn.Open();
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //    OleDbCommand cmd = new OleDbCommand(cmdText, Conn);
        //    var dr = cmd.ExecuteReader();
        //    return dr[column].ToString();
        //}

        public List<string> ExecuteReader(string cmdText, string column)
        {
            List<string> list = new List<string>();
            try
            {
                if (Conn.State == ConnectionState.Closed)
                {
                    Conn.Open();
                }
            }
            catch (Exception)
            {
                throw;
            }
            OleDbCommand cmd = new OleDbCommand(cmdText, Conn);
            var dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(dr[column].ToString());
            }
            Conn.Close();
            return list;
        }
    }
}