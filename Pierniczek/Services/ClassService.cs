using Pierniczek.Models;
using Pierniczek.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pierniczek.Services
{
    public class ClassService : IClassService
    {

        private ISet<string> GetGroups(string columnName, IList<RowModel> rows)
        {
            var groups = new HashSet<string>();
            foreach (var row in rows)
            {
                groups.Add(row[columnName] as string);
            }

            return groups;
        }

        private Dictionary<string, int> CreateMapping(ISet<string> groups)
        {
            var result = new Dictionary<string, int>();
            int i = 1;
            foreach(var group in groups)
            {
                result.Add(group, i);
                i++;
            }

            return result;
        }

        public IList<RowModel> GroupAlphabetically(ColumnModel strColumn, ColumnModel newColumn, IList<RowModel> rows)
        {
            var groups = GetGroups(strColumn.Name, rows);

            groups = new SortedSet<string>(groups);
            
            var mapping = CreateMapping(groups);

            foreach(var row in rows)
            {
                var gValue = row[strColumn.Name] as string;
                row[newColumn.Name] = mapping[gValue];
            }

            return rows;
        }

        public IList<RowModel> GroupByOrder(ColumnModel strColumn, ColumnModel newColumn, IList<RowModel> rows)
        {
            var groups = GetGroups(strColumn.Name, rows);

            var mapping = CreateMapping(groups);

            foreach (var row in rows)
            {
                var gValue = row[strColumn.Name] as string;
                row[newColumn.Name] = mapping[gValue];
            }

            return rows;
        }
    }
}
