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
        public const string FileFilter = "txt files (*.txt)|*.txt|csv files (*.csv)|*.csv|All files|*.*";

        private readonly IFileService _fileService;

        public OpenFileWindowViewModel(IFileService fileService)
        {
            this._fileService = fileService;

            Open = new TaskCommand(OnOpenExecute);
            Accept = new TaskCommand(OnAcceptExecute);
            Reload = new TaskCommand(OnReloadExecute);
            SelectAll = new TaskCommand(OnSelectAllExecute);
            UnselectAll = new TaskCommand(OnUnselectAllExecute);
            ToggleSelection = new TaskCommand(OnToggleSelectionExecute);
        }


        public DataModel Data { get; set; }
        public string FilePreview { get; set; }
        public string FilePath { get; private set; }
        public string FileName { get; private set; }
        public FileDataSeparatorEnum SeparatorType { get; set; }

        public bool SeparatorWhiteChars
        {
            get => SeparatorType.HasFlag(FileDataSeparatorEnum.WhiteChars);
            set => SetSeparatorTypeFlag(FileDataSeparatorEnum.WhiteChars, value);
        }
        public bool SeparatorTab
        {
            get => SeparatorType.HasFlag(FileDataSeparatorEnum.Tab);
            set => SetSeparatorTypeFlag(FileDataSeparatorEnum.Tab, value);
        }
        public bool SeparatorDot
        {
            get => SeparatorType.HasFlag(FileDataSeparatorEnum.Dot);
            set => SetSeparatorTypeFlag(FileDataSeparatorEnum.Dot, value);
        }
        public bool SeparatorComma
        {
            get => SeparatorType.HasFlag(FileDataSeparatorEnum.Comma);
            set => SetSeparatorTypeFlag(FileDataSeparatorEnum.Comma, value);
        }
        public bool SeparatorSemicolon
        {
            get => SeparatorType.HasFlag(FileDataSeparatorEnum.Semicolon);
            set => SetSeparatorTypeFlag(FileDataSeparatorEnum.Semicolon, value);
        }
        public bool SeparatorSpace
        {
            get => SeparatorType.HasFlag(FileDataSeparatorEnum.Space);
            set => SetSeparatorTypeFlag(FileDataSeparatorEnum.Space, value);
        }

        private void SetSeparatorTypeFlag(FileDataSeparatorEnum flag, bool value)
        {
            if (value)
            {
                SeparatorType |= flag;
            }
            else
            {
                SeparatorType &= ~flag;
            }
        }

        private void Load(string path)
        {
            FilePreview = _fileService.PreviewFile(path);
            Data = _fileService.LoadDefinitions(path, SeparatorType);
            FileName = Path.GetFileName(path);
            FilePath = path;
        }

        private async Task OnOpenExecute()
        {
            var dependencyResolver = this.GetDependencyResolver();
            var openFileService = dependencyResolver.Resolve<IOpenFileService>();
            openFileService.Filter = FileFilter;
            if (await openFileService.DetermineFileAsync())
            {
                SeparatorType = _fileService.DetectSeparatorType(openFileService.FileName);
                Load(openFileService.FileName);
            }
        }

        private async Task OnAcceptExecute()
        {
            await this.SaveAndCloseViewModelAsync();
        }

        private async Task OnReloadExecute()
        {
            Load(FilePath);
        }

        private async Task OnSelectAllExecute()
        {
            foreach (var column in this.Data.Columns)
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
        public TaskCommand Reload { get; private set; }
        public TaskCommand SelectAll { get; private set; }
        public TaskCommand UnselectAll { get; private set; }
        public TaskCommand ToggleSelection { get; private set; }

        public DataModel LoadData()
        {
            if (FilePath == null || Data == null)
                return null;

            return _fileService.ReloadRows(FilePath, Data, SeparatorType);
        }
    }
}
