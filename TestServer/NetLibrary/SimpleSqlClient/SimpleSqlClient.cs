using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.SqlClient;

namespace NetLibrary.SimpleSqlClient
{
    public class SimpleDB
    {
        private string connection_string = "";

        private SqlDbType GetSqlDbType(string type_string)
        {
            switch (type_string)
            {
                case "System.Int64":
                    return SqlDbType.BigInt;
                case "System.int":
                case "System.Int32":
                    return SqlDbType.Int;
                case "System.String":
                    return SqlDbType.NVarChar;
            }

            return 0;
        }

        public SimpleDB(string server, string uid, string pwd, string db)
        {
            connection_string = string.Format("server = {0}; uid = {1}; pwd = {2}; database = {3};", server, uid, pwd, db);
        }

        public int ExecSP(string sp_name, params QueryParam[] exec_params)
        {
            if (connection_string == "")
                return -1;

            using (SqlConnection conn = new SqlConnection(connection_string))
            {
                try
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sp_name, conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    List<QueryParam> output_params = null;

                    foreach (var param in exec_params)
                    {
                        SqlParameter sql_param = new SqlParameter(param.name_, GetSqlDbType(param.value_.GetType().ToString()));

                        switch (param.direction_)
                        {
                            case ParameterDirection.Input:
                                {
                                    sql_param.Value = param.value_;
                                }
                                break;
                            case ParameterDirection.InputOutput:
                                {
                                    if (output_params == null)
                                        output_params = new List<QueryParam>();

                                    output_params.Add(param);
                                    sql_param.Value = param.value_;
                                }
                                break;
                            case ParameterDirection.Output:
                                {
                                    if (output_params == null)
                                        output_params = new List<QueryParam>();

                                    output_params.Add(param);
                                }
                                break;
                        }

                        sql_param.Direction = param.direction_;
                        cmd.Parameters.Add(sql_param);
                    }

                    SqlParameter return_value = new SqlParameter();
                    return_value.Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add(return_value);

                    cmd.ExecuteNonQuery();

                    if (output_params != null)
                    {
                        foreach (var output_param in output_params)
                        {
                            output_param.value_ = cmd.Parameters[output_param.name_].Value;
                        }
                    }

                    return Convert.ToInt32(return_value.Value);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return -2;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public int ExecSP(string sp_name, QueryRecord record, params QueryParam[] exec_params)
        {
            if (connection_string == "")
                return -1;

            using (SqlConnection conn = new SqlConnection(connection_string))
            {
                try
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sp_name, conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    List<QueryParam> output_params = null;

                    foreach (var param in exec_params)
                    {
                        SqlParameter sql_param = new SqlParameter(param.name_, GetSqlDbType(param.value_.GetType().ToString()));

                        switch (param.direction_)
                        {
                            case ParameterDirection.Input:
                                {
                                    sql_param.Value = param.value_;
                                }
                                break;
                            case ParameterDirection.InputOutput:
                                {
                                    if (output_params == null)
                                        output_params = new List<QueryParam>();

                                    output_params.Add(param);
                                    sql_param.Value = param.value_;
                                }
                                break;
                            case ParameterDirection.Output:
                                {
                                    if (output_params == null)
                                        output_params = new List<QueryParam>();

                                    output_params.Add(param);
                                }
                                break;
                        }

                        sql_param.Direction = param.direction_;
                        cmd.Parameters.Add(sql_param);
                    }

                    SqlParameter return_value = new SqlParameter();
                    return_value.Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add(return_value);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            record.SetRecord((string name) => { return reader[name]; });
                        }
                    }

                    foreach (var output_param in output_params)
                    {
                        output_param.value_ = cmd.Parameters[output_param.name_].Value;
                    }

                    return Convert.ToInt32(return_value.Value);
                }
                catch (Exception e)
                {
                    record.message_ = e.Message;
                    return -2;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        // 제작중
        public int ExecDQuery(string sql, params QueryParam[] exec_params)
        {
            return 0;
        }

        public int ExecDQuery(string sql, QueryRecord record, params QueryParam[] exec_params)
        {
            return 0;
        }
    }
}
