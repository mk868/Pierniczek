using Catel.Fody;
using Catel.IoC;
using Catel.MVVM;
using Catel.Services;
using Pierniczek.Models;
using Pierniczek.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pierniczek.ViewModels
{
    public class OpenFileWindowViewModel : ViewModelBase
    {
        public const string FileFilter = "All files|*.*";

        private readonly IFileService _fileService;

        public OpenFileWindowViewModel(IFileService fileService)
        {
            this._fileService = fileService;

            Open = new TaskCommand(OnOpenExecute);
            Accept = new TaskCommand(OnAcceptExecute);
        }


        public IList<ColumnModel> Columns { get; set; }
        public string FilePreview { get; set; }
        public string FilePath { get; private set; }
        public string FileName { get; private set; }

        private async Task OnOpenExecute()
        {
            var dependencyResolver = this.GetDependencyResolver();
            var openFileService = dependencyResolver.Resolve<IOpenFileService>();
            openFileService.Filter = FileFilter;
            if (await openFileService.DetermineFileAsync())
            {
                var filePath = openFileService.FileName;
                FilePreview = _fileService.PreviewFile(filePath);
                Columns = _fileService.GenerateColumns(filePath);
                FileName = Path.GetFileName(filePath);
                FilePath = filePath;
            }
        }

        private async Task OnAcceptExecute()
        {
            await this.SaveAndCloseViewModelAsync();
        }

        public TaskCommand Open { get; private set; }
        public TaskCommand Accept { get; private set; }

    }
}
