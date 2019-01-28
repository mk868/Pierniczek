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
    public class DataWindowViewModel : ViewModelBase
    {
        private readonly IUIVisualizerService _uiVisualizerService;
        private readonly IFileService _fileService;
        private readonly IMessageService _messageService;
        private readonly IClassService _classService;
        private readonly IScaleService _scaleService;
        private readonly IClassificationService _classificationService;


        public DataWindowViewModel(IUIVisualizerService uiVisualizerService, IFileService fileService, IMessageService messageService, IClassService classService, IScaleService scaleService,
            IClassificationService classificationService)
        {
            this._uiVisualizerService = uiVisualizerService;
            this._fileService = fileService;
            this._messageService = messageService;
            this._classService = classService;
            this._scaleService = scaleService;
            this._classificationService = classificationService;

            OpenFile = new TaskCommand(OnOpenFileExecute);
            SaveFile = new TaskCommand(OnSaveFileExecute, DataOperationsCanExecute);
            //GroupAlphabetically = new TaskCommand(OnGroupAlphabeticallyExecute, DataOperationsCanExecute);
            //GroupByOrder = new TaskCommand(OnGroupByOrderExecute, DataOperationsCanExecute);
            //NewRange = new TaskCommand(OnNewRangeExecute, DataOperationsCanExecute);
            Discretization = new TaskCommand(OnDiscretizationExecute, DataOperationsCanExecute);
            CreateDecisionTree = new TaskCommand(OnCreateDecisionTreeExecute, DataOperationsCanExecute);
            //Normalization = new TaskCommand(OnNormalizationExecute, DataOperationsCanExecute);
            //ShowPercent = new TaskCommand(OnShowPercentExecute, DataOperationsCanExecute);
            Scatter = new TaskCommand(OnScatterExecute, DataOperationsCanExecute);
            BinaryVectorByClass = new TaskCommand(OnBinaryVectorByClassExecute, DataOperationsCanExecute);
            Plot3D = new TaskCommand(OnPlot3DExecute, DataOperationsCanExecute);
            //Knn = new TaskCommand(OnKnnExecute, DataOperationsCanExecute);
            //KnnLOO = new TaskCommand(OnKnnLOOExecute, DataOperationsCanExecute);
            //KGroup = new TaskCommand(OnKGroupExecute, DataOperationsCanExecute);
        }

        public DataModel Data { get; set; }

        private async Task OnOpenFileExecute()
        {
            var typeFactory = this.GetTypeFactory();
            var openFileWindowViewModel = typeFactory.CreateInstanceWithParametersAndAutoCompletion<OpenFileWindowViewModel>();

            if (await _uiVisualizerService.ShowDialogAsync(openFileWindowViewModel) ?? false)
            {
                Data = openFileWindowViewModel.LoadData();
            }
        }

        private async Task OnSaveFileExecute()
        {
            var dependencyResolver = this.GetDependencyResolver();
            var saveFileService = dependencyResolver.Resolve<ISaveFileService>();
            if (await saveFileService.DetermineFileAsync())
            {
                var path = saveFileService.FileName;
                _fileService.Save(path, Data, path.EndsWith("csv") ? FileDataSeparatorEnum.Comma : FileDataSeparatorEnum.WhiteChars);//FIXME
            }
        }

        private async Task OnDiscretizationExecute()
        {
            var typeFactory = this.GetTypeFactory();
            var discretizationWindowViewModel = typeFactory.CreateInstanceWithParametersAndAutoCompletion<DiscretizationWindowViewModel>(Data);
            await _uiVisualizerService.ShowDialogAsync(discretizationWindowViewModel);
        }

        private async Task OnCreateDecisionTreeExecute()
        {
            var typeFactory = this.GetTypeFactory();
            var createDecisionTreeWindowViewModel = typeFactory.CreateInstanceWithParametersAndAutoCompletion<CreateDecisionTreeWindowViewModel>(Data);
            await _uiVisualizerService.ShowDialogAsync(createDecisionTreeWindowViewModel);
        }

        //private async Task OnNewRangeExecute()
        //{
        //    var typeFactory = this.GetTypeFactory();

        //    var column = await SelectColumn(_columns.Where(w => w.Use).Where(s => s.Type != TypeEnum.String).ToList());
        //    if (column == null)
        //    {
        //        return;
        //    }

        //    var newRangeDataWindowViewModel = typeFactory.CreateInstanceWithParametersAndAutoCompletion<NewRangeDataWindowViewModel>();
        //    if (!await _uiVisualizerService.ShowDialogAsync(newRangeDataWindowViewModel) ?? false)
        //    {
        //        return;
        //    }

        //    var min = newRangeDataWindowViewModel.Min;
        //    var max = newRangeDataWindowViewModel.Max;

        //    if (max <= min)
        //    {
        //        await _messageService.ShowErrorAsync("Max <= Min!");
        //        return;
        //    }


        //    var newName = await CreateColumn(column.Name + "_NewRange");
        //    if (newName == null)
        //    {
        //        return;
        //    }

        //    var newColumn = CreateColumn(newName, TypeEnum.Number);

        //    Rows = _scaleService.ChangeRange(Rows, column.Name, newColumn.Name, min, max);
        //    _columns.Add(newColumn);
        //    SetColumns(_columns);

        //}

        //private async Task OnGroupAlphabeticallyExecute()
        //{
        //    var typeFactory = this.GetTypeFactory();

        //    var column = await SelectColumn(_columns.Where(w => w.Use).Where(s => s.Type == TypeEnum.String).ToList());
        //    if (column == null)
        //    {
        //        return;
        //    }

        //    var newName = await CreateColumn(column.Name + "_GroupAlp");
        //    if (newName == null)
        //    {
        //        return;
        //    }

        //    var newColumn = CreateColumn(newName, TypeEnum.Number);

        //    Rows = _classService.GroupAlphabetically(column.Name, newColumn.Name, Rows);
        //    _columns.Add(newColumn);
        //    SetColumns(_columns);
        //}


        //private async Task OnGroupByOrderExecute()
        //{
        //    var typeFactory = this.GetTypeFactory();

        //    var column = await SelectColumn(_columns.Where(w => w.Use).Where(s => s.Type == TypeEnum.String).ToList());
        //    if (column == null)
        //    {
        //        return;
        //    }

        //    var newName = await CreateColumn(column.Name + "_GroupByOrder");
        //    if (newName == null)
        //    {
        //        return;
        //    }

        //    var newColumn = CreateColumn(newName, TypeEnum.Number);

        //    Rows = _classService.GroupByOrder(column.Name, newColumn.Name, Rows);
        //    _columns.Add(newColumn);
        //    SetColumns(_columns);
        //}


        //private async Task OnNormalizationExecute()
        //{
        //    var typeFactory = this.GetTypeFactory();
        //    var column = await SelectColumn(_columns.Where(w => w.Use).Where(s => s.Type == TypeEnum.Number).ToList());
        //    if (column == null)
        //    {
        //        return;
        //    }

        //    var newName = await CreateColumn(column.Name + "_Normalization");
        //    if (newName == null)
        //    {
        //        return;
        //    }

        //    var newColumn = CreateColumn(newName, TypeEnum.Number);

        //    Rows = _scaleService.Normalization(Rows, column.Name, newColumn.Name);
        //    _columns.Add(newColumn);
        //    SetColumns(_columns);
        //}

        //private async Task OnShowPercentExecute()
        //{
        //    var typeFactory = this.GetTypeFactory();
        //    var column = await SelectColumn(_columns.Where(w => w.Use).Where(s => s.Type == TypeEnum.Number).ToList());
        //    if (column == null)
        //    {
        //        return;
        //    }

        //    var setPercentWindowViewModel = typeFactory.CreateInstanceWithParametersAndAutoCompletion<SetPercentWindowViewModel>();
        //    if (!await _uiVisualizerService.ShowDialogAsync(setPercentWindowViewModel) ?? false)
        //    {
        //        return;
        //    }

        //    var newColumns = new List<ColumnModel> { CreateColumn("TOP", TypeEnum.Number), CreateColumn("BOTTOM", TypeEnum.Number) };

        //    var percentViewModel = typeFactory.CreateInstanceWithParametersAndAutoCompletion<PercentWindowViewModel>();
        //    percentViewModel.Rows = _scaleService.ShowProcent(Rows, column.Name, setPercentWindowViewModel.Percent);
        //    percentViewModel.SetColumns(newColumns);

        //    if (!await _uiVisualizerService.ShowDialogAsync(percentViewModel) ?? false)
        //    {
        //        return;
        //    }
        //}


        private async Task OnScatterExecute()
        {
            var typeFactory = this.GetTypeFactory();
            var scatterWindowViewModel = typeFactory.CreateInstanceWithParametersAndAutoCompletion<ScatterWindowViewModel>(Data);
            await _uiVisualizerService.ShowDialogAsync(scatterWindowViewModel);
        }

        private async Task OnBinaryVectorByClassExecute()
        {
            var typeFactory = this.GetTypeFactory();
            var scatterWindowViewModel = typeFactory.CreateInstanceWithParametersAndAutoCompletion<BinaryVectorByClassWindowViewModel>(Data);
            await _uiVisualizerService.ShowDialogAsync(scatterWindowViewModel);
        }

        private async Task OnPlot3DExecute()
        {
            var typeFactory = this.GetTypeFactory();
            var plot3dWindowViewModel = typeFactory.CreateInstanceWithParametersAndAutoCompletion<Plot3dWindowViewModel>(Data);
            await _uiVisualizerService.ShowDialogAsync(plot3dWindowViewModel);
        }


        //private async Task OnKnnExecute()
        //{
        //    var typeFactory = this.GetTypeFactory();

        //    var columnX = await SelectColumn(_columns.Where(w => w.Use).Where(s => s.Type == TypeEnum.Number).ToList(), "X axis");
        //    if (columnX == null)
        //    {
        //        return;
        //    }

        //    var columnY = await SelectColumn(_columns.Where(w => w.Use).Where(s => s.Type == TypeEnum.Number).Where(s => s.Name != columnX.Name).ToList(), "Y axis");
        //    if (columnY == null)
        //    {
        //        return;
        //    }

        //    var columnClass = await SelectColumn(_columns.Where(w => w.Use).Where(s => s.Type == TypeEnum.String).ToList(), "decision class");
        //    if (columnClass == null)
        //    {
        //        return;
        //    }

        //    var knnWindowViewModel = typeFactory.CreateInstanceWithParametersAndAutoCompletion<KnnWindowViewModel>();
        //    knnWindowViewModel.Rows = this.Rows;
        //    knnWindowViewModel.ColumnX = columnX.Name;
        //    knnWindowViewModel.ColumnY = columnY.Name;
        //    knnWindowViewModel.ColumnClass = columnClass.Name;
        //    knnWindowViewModel.Init();
        //    if (!await _uiVisualizerService.ShowDialogAsync(knnWindowViewModel) ?? false)
        //    {
        //        return;
        //    }

        //}

        //private async Task OnKnnLOOExecute()
        //{
        //    var typeFactory = this.GetTypeFactory();

        //    var columnX = await SelectColumn(_columns.Where(w => w.Use).Where(s => s.Type == TypeEnum.Number).ToList(), "X axis");
        //    if (columnX == null)
        //    {
        //        return;
        //    }

        //    var columnY = await SelectColumn(_columns.Where(w => w.Use).Where(s => s.Type == TypeEnum.Number).Where(s => s.Name != columnX.Name).ToList(), "Y axis");
        //    if (columnY == null)
        //    {
        //        return;
        //    }

        //    var columnClass = await SelectColumn(_columns.Where(w => w.Use).Where(s => s.Type == TypeEnum.String).ToList(), "decision class");
        //    if (columnClass == null)
        //    {
        //        return;
        //    }

        //    var knnWindowViewModel = typeFactory.CreateInstanceWithParametersAndAutoCompletion<KnnLeaveOneOutWindowViewModel>();
        //    knnWindowViewModel.Rows = this.Rows;
        //    knnWindowViewModel.ColumnX = columnX.Name;
        //    knnWindowViewModel.ColumnY = columnY.Name;
        //    knnWindowViewModel.ColumnClass = columnClass.Name;
        //    if (!await _uiVisualizerService.ShowDialogAsync(knnWindowViewModel) ?? false)
        //    {
        //        return;
        //    }
        //}

        //private async Task OnKGroupExecute()
        //{
        //    var typeFactory = this.GetTypeFactory();

        //    var columnX = await SelectColumn(_columns.Where(w => w.Use).Where(s => s.Type == TypeEnum.Number).ToList(), "X axis");
        //    if (columnX == null)
        //    {
        //        return;
        //    }

        //    var columnY = await SelectColumn(_columns.Where(w => w.Use).Where(s => s.Type == TypeEnum.Number).Where(s => s.Name != columnX.Name).ToList(), "Y axis");
        //    if (columnY == null)
        //    {
        //        return;
        //    }

        //    var newName = await CreateColumn("New_KDecision");
        //    if (newName == null)
        //    {
        //        return;
        //    }

        //    var selectMethodViewModel = typeFactory.CreateInstanceWithParametersAndAutoCompletion<SelectKGroupDistanceMethodDataWindowViewModel>();
        //    if (!await _uiVisualizerService.ShowDialogAsync(selectMethodViewModel) ?? false)
        //    {
        //        return;
        //    }

        //    var kInputDataWindowViewModel = typeFactory.CreateInstanceWithParametersAndAutoCompletion<KInputDataWindowViewModel>();
        //    if (!await _uiVisualizerService.ShowDialogAsync(kInputDataWindowViewModel) ?? false)
        //    {
        //        return;
        //    }

        //    var newColumn = CreateColumn(newName, TypeEnum.Number);

        //    _classificationService.KGroup(columnX.Name, columnY.Name, newName, selectMethodViewModel.SelectedMethod, kInputDataWindowViewModel.KValue, Rows);
        //    _columns.Add(newColumn);
        //    SetColumns(_columns);
        //}


        private bool DataOperationsCanExecute()
        {
            return this.Data?.Rows.Count > 0;
        }

        public TaskCommand OpenFile { get; private set; }
        public TaskCommand SaveFile { get; private set; }
        public TaskCommand GroupAlphabetically { get; private set; }
        public TaskCommand GroupByOrder { get; private set; }
        public TaskCommand NewRange { get; private set; }
        public TaskCommand Discretization { get; private set; }
        public TaskCommand CreateDecisionTree { get; private set; }
        public TaskCommand Normalization { get; private set; }
        public TaskCommand ShowPercent { get; private set; }
        public TaskCommand Scatter { get; private set; }
        public TaskCommand Plot3D { get; private set; }
        public TaskCommand Knn { get; private set; }
        public TaskCommand KnnLOO { get; private set; }
        public TaskCommand KGroup { get; private set; }
        public TaskCommand BinaryVectorByClass { get; private set; }

    }
}
