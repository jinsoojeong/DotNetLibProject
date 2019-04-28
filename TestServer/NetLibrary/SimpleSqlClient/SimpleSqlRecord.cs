using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLibrary.SimpleSqlClient
{
    using Columns = Dictionary<string, object>;
    using ColumnNames = List<string>;
    using Rows = List<Row>;
    delegate object GetValue(string name);

    public class Row
    {
        public Row()
        {
            columns_ = new Columns();
        }

        public void Add(string name, object o)
        {
            columns_.Add(name, o);
        }

        public object Get(string name)
        {
            object o;

            if (!columns_.TryGetValue(name, out o))
                return null;

            return o;
        }

        Columns columns_;
    }

    public abstract class QueryRecord
    {
        public QueryRecord()
        {
            column_names_ = new ColumnNames();
            rows_ = new Rows();
        }

        protected void AddColumn(string name)
        {
            column_names_.Add(name);
        }

        internal void SetRecord(GetValue getvalue)
        {
            Row row = new Row();

            foreach (string column_name in column_names_)
            {
                row.Add(column_name, getvalue(column_name));
            }

            rows_.Add(row);
        }

        public string message_ { get; internal set; }
        public ColumnNames column_names_;
        protected Rows rows_;
    }
}
