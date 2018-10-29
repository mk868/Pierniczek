using Catel.IoC;
using Catel.MVVM;
using Catel.Services;
using Pierniczek.Models;
using Pierniczek.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace Pierniczek.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private string _filePath;
        private IList<ColumnModel> _columns;
        private readonly IUIVisualizerService _uiVisualizerService;
        private readonly IFileService _fileService;
        private readonly IMessageService _messageService;
        private readonly IClassService _classService;
        private readonly IScaleService _scaleService;

        public MainWindowViewModel(IUIVisualizerService uiVisualizerService, IFileService fileService, IMessageService messageService, IClassService classService, IScaleService scaleService)
        {
            this._uiVisualizerService = uiVisualizerService;
            this._fileService = fileService;
            this._messageService = messageService;
            this._classService = classService;
            this._scaleService = scaleService;

            OpenFile = new TaskCommand(OnOpenFileExecute);
            GroupAlphabetically = new TaskCommand(OnGroupAlphabeticallyExecute);
            GroupByOrder = new TaskCommand(OnGroupByOrderExecute);
            NewRange = new TaskCommand(OnNewRangeExecute);
            Discretization = new TaskCommand(OnDiscretizationExecute);
            Normalization = new TaskCommand(OnNormalizationExecute);
            ShowPercent = new TaskCommand(OnShowPercentExecute);
            Scatter = new TaskCommand(OnScatterExecute);
        }

        public IList<RowModel> Rows { get; private set; }
        public ObservableCollection<DataGridColumn> Columns { get; private set; }

        private void SetColumns(IList<ColumnModel> fields)
        {
            var columns = new List<DataGridColumn>();

            foreach (var field in fields)
            {
                var column = new DataGridTextColumn();
                column.Header = field.Name;
                column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                column.Binding = new Binding($"[{field.Name}]");
                columns.Add(column);
            }

            Columns = new ObservableCollection<DataGridColumn>(columns);
        }

        private void ReloadFile()
        {
            var rows = _fileService.GetRows(_filePath, _columns);
            Rows = (rows);
        }


        private async Task OnOpenFileExecute()
        {
            var typeFactory = this.GetTypeFactory();
            var openFileWindowViewModel = typeFactory.CreateInstanceWithParametersAndAutoCompletion<OpenFileWindowViewModel>();

            if (await _uiVisualizerService.ShowDialogAsync(openFileWindowViewModel) ?? false)
            {
                _filePath = openFileWindowViewModel.FilePath;
                _columns = openFileWindowViewModel.Columns;
                SetColumns(openFileWindowViewModel.Columns);
                ReloadFile();
            }
        }


        private async Task<ColumnModel> SelectColumn(IList<ColumnModel> columns, string title = null)
        {
            var typeFactory = this.GetTypeFactory();

            var selectColumnDataWindowViewModel = typeFactory.CreateInstanceWithParametersAndAutoCompletion<SelectColumnDataWindowViewModel>();
            selectColumnDataWindowViewModel.Columns = columns;
            if (title != null)
                selectColumnDataWindowViewModel.SetTitle(title);

            if (!await _uiVisualizerService.ShowDialogAsync(selectColumnDataWindowViewModel) ?? false)
            {
                return null;
            }

            return selectColumnDataWindowViewModel.SelectedColumn;
        }


        private async Task<string> CreateColumn(string suggestedName)
        {
            var typeFactory = this.GetTypeFactory();

            var newColumnDataWindowViewModel = typeFactory.CreateInstanceWithParametersAndAutoCompletion<NewColumnDataWindowViewModel>();
            newColumnDataWindowViewModel.ColumnName = suggestedName;
            if (!await _uiVisualizerService.ShowDialogAsync(newColumnDataWindowViewModel) ?? false)
            {
                return null;
            }

            var newName = newColumnDataWindowViewModel.ColumnName;
            if (_columns.Any(s => s.Name == newName))
            {
                await _messageService.ShowErrorAsync("Column name already exist!");
                return null;
            }

            return newName;
        }




        private async Task OnNewRangeExecute()
        {
            var typeFactory = this.GetTypeFactory();

            var column = await SelectColumn(_columns.Where(s => s.Type != TypeEnum.String).ToList());
            if (column == null)
            {
                return;
            }

            var newRangeDataWindowViewModel = typeFactory.CreateInstanceWithParametersAndAutoCompletion<NewRangeDataWindowViewModel>();
            if (!await _uiVisualizerService.ShowDialogAsync(newRangeDataWindowViewModel) ?? false)
            {
                return;
            }

            var min = newRangeDataWindowViewModel.Min;
            var max = newRangeDataWindowViewModel.Max;

            if (max <= min)
            {
                await _messageService.ShowErrorAsync("Max <= Min!");
                return;
            }


            var newName = await CreateColumn(column.Name + "_NewRange");
            if (newName == null)
            {
                return;
            }

            var newColumn = CreateColumn(newName, TypeEnum.Number);

            Rows = _scaleService.ChangeRange(Rows, column.Name, newColumn.Name, min, max);
            _columns.Add(newColumn);
            SetColumns(_columns);

        }

        private async Task OnGroupAlphabeticallyExecute()
        {
            var typeFactory = this.GetTypeFactory();

            var column = await SelectColumn(_columns.Where(s => s.Type == TypeEnum.String).ToList());
            if (column == null)
            {
                return;
            }

            var newName = await CreateColumn(column.Name + "_GroupAlp");
            if (newName == null)
            {
                return;
            }

            var newColumn = CreateColumn(newName, TypeEnum.Number);

            Rows = _classService.GroupAlphabetically(column.Name, newColumn.Name, Rows);
            _columns.Add(newColumn);
            SetColumns(_columns);
        }

        private ColumnModel CreateColumn(string name, TypeEnum type)
        {
            return new ColumnModel()
            {
                Name = name,
                Type = type,
                Use = true
            };
        }

        private async Task OnGroupByOrderExecute()
        {
            var typeFactory = this.GetTypeFactory();

            var column = await SelectColumn(_columns.Where(s => s.Type == TypeEnum.String).ToList());
            if (column == null)
            {
                return;
            }

            var newName = await CreateColumn(column.Name + "_GroupByOrder");
            if (newName == null)
            {
                return;
            }

            var newColumn = CreateColumn(newName, TypeEnum.Number);

            Rows = _classService.GroupByOrder(column.Name, newColumn.Name, Rows);
            _columns.Add(newColumn);
            SetColumns(_columns);
        }

        private async Task OnDiscretizationExecute()
        {
            var typeFactory = this.GetTypeFactory();

            var column = await SelectColumn(_columns.Where(s => s.Type == TypeEnum.Number).ToList());
            if (column == null)
            {
                return;
            }

            var setRangesDataWindowViewModel = typeFactory.CreateInstanceWithParametersAndAutoCompletion<SetRangesDataWindowViewModel>();
            setRangesDataWindowViewModel.Ranges = new List<RangeModel>();
            if (!await _uiVisualizerService.ShowDialogAsync(setRangesDataWindowViewModel) ?? false)
            {
                return;
            }

            var ranges = setRangesDataWindowViewModel.Ranges;

            var newName = await CreateColumn(column.Name + "_Discretization");
            if (newName == null)
            {
                return;
            }

            var newColumn = CreateColumn(newName, TypeEnum.Number);

            Rows = _scaleService.Discretization(Rows, column.Name, newColumn.Name, ranges);
            _columns.Add(newColumn);
            SetColumns(_columns);
        }

        private async Task OnNormalizationExecute()
        {
            var typeFactory = this.GetTypeFactory();
			var column = await SelectColumn(_columns.Where(s => s.Type != TypeEnum.String).ToList());
			if (column == null)
			{
				return;
			}

			var newName = await CreateColumn(column.Name + "_Normalization");
			if (newName == null)
			{
				return;
			}

			var newColumn = CreateColumn(newName, TypeEnum.Number);

			Rows = _scaleService.Normalization(Rows, column.Name, newColumn.Name);
			_columns.Add(newColumn);
			SetColumns(_columns);
		}

        private async Task OnShowPercentExecute()
        {
            var typeFactory = this.GetTypeFactory();
			var column = await SelectColumn(_columns.Where(s => s.Type != TypeEnum.String).ToList());
			if (column == null)
			{
				return;
			}

			var setPercentWindowViewModel = typeFactory.CreateInstanceWithParametersAndAutoCompletion<SetPercentWindowViewModel>();
			if (!await _uiVisualizerService.ShowDialogAsync(setPercentWindowViewModel) ?? false)
			{
				return;
			}

			var newColumns = new List<ColumnModel> { CreateColumn("TOP", TypeEnum.Number), CreateColumn("BOTTOM", TypeEnum.Number) };

			var percentViewModel = typeFactory.CreateInstanceWithParametersAndAutoCompletion<PercentWindowViewModel>();
			percentViewModel.Rows = _scaleService.ShowProcent(Rows, column.Name, setPercentWindowViewModel.Percent);
			percentViewModel.SetColumns(newColumns);

			if (!await _uiVisualizerService.ShowDialogAsync(percentViewModel) ?? false)
			{
				return;
			}
		}


        private async Task OnScatterExecute()
        {
            var typeFactory = this.GetTypeFactory();

            var columnX = await SelectColumn(_columns.Where(s => s.Type == TypeEnum.Number).ToList(), "X axis");
            if (columnX == null)
            {
                return;
            }
            
            var columnY = await SelectColumn(_columns.Where(s => s.Name != columnX.Name).ToList(), "Y axis");
            if (columnY == null)
            {
                return;
            }

            var scatterWindowViewModel = typeFactory.CreateInstanceWithParametersAndAutoCompletion<ScatterWindowViewModel>();
            if (columnY.Type == TypeEnum.Number)
                scatterWindowViewModel.SetData(this.Rows, columnX.Name, columnY.Name);
            else
                scatterWindowViewModel.SetDataWithClasses(this.Rows, columnX.Name, columnY.Name);
            if (!await _uiVisualizerService.ShowDialogAsync(scatterWindowViewModel) ?? false)
            {
                return;
            }

        }

        public TaskCommand OpenFile { get; private set; }
        public TaskCommand GroupAlphabetically { get; private set; }
        public TaskCommand GroupByOrder { get; private set; }
        public TaskCommand NewRange { get; private set; }
        public TaskCommand Discretization { get; private set; }
        public TaskCommand Normalization { get; private set; }
        public TaskCommand ShowPercent { get; private set; }
        public TaskCommand Scatter { get; private set; }
    }
}
