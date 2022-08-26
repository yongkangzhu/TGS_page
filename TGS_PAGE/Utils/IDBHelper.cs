using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace TGS_PAGE.Utils
{
    public interface IDBHelper
    {
        string cstr { get; set; }

        DataTable Query(string sql);
        DataTable Query(string sql, string[] key, object[] value);
        void Execute(string sql);

    }
}
