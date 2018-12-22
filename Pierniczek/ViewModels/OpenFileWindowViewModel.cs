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
            SelectAll = new TaskCommand(OnSelectAllExecute);
            UnselectAll = new TaskCommand(OnUnselectAllExecute);
            ToggleSelection = new TaskCommand(OnToggleSelectionExecute);
        }


        public DataModel Data { get; set; }
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
                Data = _fileService.LoadDefinitions(filePath);
                FileName = Path.GetFileName(filePath);
                FilePath = filePath;
            }
        }

        private async Task OnAcceptExecute()
        {
            await this.SaveAndCloseViewModelAsync();
        }

        private async Task OnSelectAllExecute()
        {
            foreach(var column in this.Data.Columns)
            {
                column.Use = true;
            }
        }

        private async Task OnUnselectAllExecute()
        {
            foreach (var column in this.Data.Columns)
            {
                column.Use = false;
            }
        }

        private async Task OnToggleSelectionExecute()
        {
            foreach (var column in this.Data.Columns)
            {
                column.Use = !column.Use;
            }
        }

        public TaskCommand Open { get; private set; }
        public TaskCommand Accept { get; private set; }
        public TaskCommand SelectAll { get; private set; }
        public TaskCommand UnselectAll { get; private set; }
        public TaskCommand ToggleSelection { get; private set; }

        public DataModel LoadData()
        {
            if (FilePath == null || Data == null)
                return null;

            return _fileService.ReloadRows(FilePath, Data);
        }
    }
}
