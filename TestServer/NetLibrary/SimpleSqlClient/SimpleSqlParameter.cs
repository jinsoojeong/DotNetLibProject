using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.SqlClient;

namespace NetLibrary.SimpleSqlClient
{
    public enum ParamType
    {
        INPUT,
        INPUTOUTPUT,
        OUTPUT
    }

    public class QueryParam
    {
        public QueryParam(ParamType type, string name, object value)
        {
            switch (type)
            {
                case ParamType.INPUT:
                    this.direction_ = ParameterDirection.Input;
                    break;
                case ParamType.INPUTOUTPUT:
                    this.direction_ = ParameterDirection.InputOutput;
                    break;
                case ParamType.OUTPUT:
                    this.direction_ = ParameterDirection.Output;
                    break;
            }

            this.name_ = name;
            this.value_ = value;
        }

        public readonly ParameterDirection direction_;
        public readonly string name_;
        public object value_;
    }
}
