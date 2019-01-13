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
        
        
        FileDataSeparatorEnum DetectSeparatorType(string fileName);
        void Save(string filePath, DataModel data, FileDataSeparatorEnum separator);
        DataModel Load(string filePath, FileDataSeparatorEnum separator);
        DataModel LoadDefinitions(string filePath, FileDataSeparatorEnum separator);
        DataModel ReloadRows(string filePath, DataModel dataModel, FileDataSeparatorEnum separator);
    }
}
