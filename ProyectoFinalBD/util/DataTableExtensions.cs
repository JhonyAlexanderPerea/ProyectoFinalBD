using System.Collections.Generic;
using System.Data;

namespace ProyectoFinalBD.util
{
    public static class DataTableExtensions
    {
        public static List<Dictionary<string, string>> ToDictionaryList(this DataTable dt)
        {
            var list = new List<Dictionary<string, string>>();
            foreach (DataRow row in dt.Rows)
            {
                var dict = new Dictionary<string, string>();
                foreach (DataColumn col in dt.Columns)
                {
                    dict[col.ColumnName] = row[col]?.ToString() ?? "";
                }
                list.Add(dict);
            }
            return list;
        }
    }
}