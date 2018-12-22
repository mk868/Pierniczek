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
        
        
        void Save(string filePath, DataModel data);
        DataModel Load(string filePath);
        DataModel LoadDefinitions(string filePath);
        DataModel ReloadRows(string filePath, DataModel dataModel);
    }
}
