using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace GameWIndowTest1.Global.SQL
{
    class base_sql
    {
        protected string _table, _filename, cs;
        protected SQLiteConnection con;
        public base_sql(string E_table, string E_filename)
        {
            _table = E_table;
            _filename = E_filename;
            cs = $"URI=file:{E_filename}";
            con = new SQLiteConnection(cs);
        }


    }
}
