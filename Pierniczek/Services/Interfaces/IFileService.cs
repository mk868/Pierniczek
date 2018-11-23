using Pierniczek.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pierniczek.Services.Interfaces
{
    public interface IFileService
    {
        string PreviewFile(string filePath);
        string PreviewFile(string filePath, int lines);

        IList<ColumnModel> GenerateColumns(string filePath);
        IList<RowModel> GetRows(string filePath, IList<ColumnModel> columns);

        void SaveToFile(string filePath, IList<ColumnModel> columns, IList<RowModel> rows);
    }
}
