using Catel.Fody;
using Catel.IoC;
using Catel.MVVM;
using Catel.Services;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Series;
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
    public class BinaryVectorByClassWindowViewModel : ViewModelBase
    {
        private readonly IColorService _colorService;
        private readonly IMessageService _messageService;
        private readonly IUIVisualizerService _uiVisualizerService;

        //[Model]
        //[Expose("Columns")]
        public DataModel DataModel { get; set; }
        public ObservableCollection<BinaryVectorByClassLineModel> Lines { get; set; }

        public SelectColumnsModel DimensionColumns { get; set; }
        public SelectColumnModel ColumnClass { get; set; }
        public PlotModel ScatterModel { get; set; }

        public IList<PairModel<string, double>> InputDimensions { get; set; }
        public string OutputVector { get; set; }
        public string ClickedVector { get; set; }



        private IList<string> _dimensions = new List<string>();
        private Dictionary<string, IList<RowModel>> _sortedRows;
        private string _columnClass;
        private bool _done;
        private bool _nonLinear;
        private string _columnHorizontal;//used in plot lines


        public BinaryVectorByClassWindowViewModel(DataModel data, IColorService colorService, IMessageService messageService, IUIVisualizerService uiVisualizerService)
        {
            this._colorService = colorService;
            this._messageService = messageService;
            this._uiVisualizerService = uiVisualizerService;

            Lines = new ObservableCollection<BinaryVectorByClassLineModel>();
            InputDimensions = new ObservableCollection<PairModel<string, double>>();

            DimensionColumns = new SelectColumnsModel(data.Columns, TypeEnum.Number, "Dimensions");
            ColumnClass = new SelectColumnModel(data.Columns, "Class");
            DataModel = data;

            Generate = new TaskCommand(OnGenerateExecute, GenerateCanExecute);
            NextStep = new TaskCommand(OnNextStepExecute, GenerateCanExecute);
            Finish = new TaskCommand(OnFinishExecute, GenerateCanExecute);
            CalculateVector = new TaskCommand(OnCalculateVectorExecute, GenerateCanExecute);
            AddVectorColumn = new TaskCommand(OnAddVectorColumnExecute, GenerateCanExecute);
        }


        private void InitDimensions()
        {
            this.Lines.Clear();
            _done = false;
            _nonLinear = false;
            _dimensions.Clear();
            foreach (var column in DimensionColumns.SelectedColumns)
            {
                _dimensions.Add(column.Name);
            }
            _columnHorizontal = _dimensions.Count == 2 ? _dimensions[0] : null;
            _columnClass = ColumnClass.SelectedColumn.Name;

            _sortedRows = new Dictionary<string, IList<RowModel>>();
            foreach (var dimension in _dimensions)
            {
                _sortedRows[dimension] = DataModel.Rows.OrderBy(o => (double)o[dimension]).ToList();
            }
        }

        private void RefreshPlotLines()
        {
            if (_dimensions.Count != 2)
            {
                return;
            }

            this.ScatterModel.Annotations.Clear();

            int i = 1;
            foreach (var line in this.Lines)
            {
                LineAnnotationType type;
                double x = 0;
                double y = 0;
                LineStyle style = line.Direction ? LineStyle.Dash : LineStyle.Dot;

                if (line.Dimension == _columnHorizontal)
                {
                    type = LineAnnotationType.Vertical;
                    x = line.Value;
                }
                else
                {
                    type = LineAnnotationType.Horizontal;
                    y = line.Value;
                }

                var color = _colorService.GetUniqueColorByName(line.Dimension);
                LineAnnotation plotLine = new LineAnnotation()
                {
                    StrokeThickness = 2,
                    Color = OxyColor.FromRgb(color.R, color.G, color.B),
                    Type = type,
                    Text = $"Line {i++}",
                    TextColor = OxyColors.Black,
                    X = x,
                    Y = y,
                    LineStyle = style
                };

                this.ScatterModel.Annotations.Add(plotLine);
                this.ScatterModel.InvalidatePlot(true);
            }
        }

        private void PutNextDivision()
        {
            //max extreme elements
            string meeDimension = null;
            int meeCount = 0;
            IList<RowModel> meePoints = null;
            bool meeDirection = false;
            double meeValue = 0;
            bool meeFound = false;

            foreach (var dimension in _dimensions)
            {
                var fclass = _sortedRows[dimension].FirstOrDefault()[_columnClass].ToString();

                var firstOtherPoint = _sortedRows[dimension].SkipWhile(r => fclass == r[_columnClass].ToString()).FirstOrDefault();
                var valueLimit = (double)firstOtherPoint[dimension];
                var peripheralPoints = _sortedRows[dimension]
                    .TakeWhile(
                        r =>
                            fclass == r[_columnClass].ToString() &&
                            ((double)r[dimension]) < valueLimit
                        ).ToList();

                if (peripheralPoints.Count > meeCount)
                {
                    meeDimension = dimension;
                    meeCount = peripheralPoints.Count;
                    meePoints = peripheralPoints;
                    meeDirection = false;
                    meeValue = (double)peripheralPoints.LastOrDefault()[dimension];
                    meeFound = true;
                }
            }

            foreach (var dimension in _dimensions)
            {
                var fclass = _sortedRows[dimension].LastOrDefault()[_columnClass].ToString();

                var firstOtherPoint = _sortedRows[dimension].Reverse().SkipWhile(r => fclass == r[_columnClass].ToString()).FirstOrDefault();
                var valueLimit = (double)firstOtherPoint[dimension];
                var peripheralPoints = _sortedRows[dimension].Reverse()
                    .TakeWhile(
                        r =>
                            fclass == r[_columnClass].ToString() &&
                            ((double)r[dimension]) > valueLimit
                        ).ToList();

                if (peripheralPoints.Count > meeCount)
                {
                    meeDimension = dimension;
                    meeCount = peripheralPoints.Count;
                    meePoints = peripheralPoints;
                    meeDirection = true;
                    meeValue = (double)peripheralPoints.LastOrDefault()[dimension];
                    meeFound = true;
                }
            }

            if (!meeFound)
            {
                _nonLinear = true;
                return;
            }

            var line = new BinaryVectorByClassLineModel();
            line.Dimension = meeDimension;
            line.Direction = meeDirection;
            line.Value = meeValue;
            this.Lines.Add(line);

            foreach (var dimension in _dimensions)
            {
                var rows = _sortedRows[dimension];
                foreach (var point in meePoints)
                {
                    rows.Remove(point);
                }
            }

            //if data has same class -> work done
            var firstClass = _sortedRows[meeDimension].First()[_columnClass].ToString();
            if (_sortedRows[meeDimension].All(w => w[_columnClass].ToString() == firstClass))
            {
                _done = true;
            }
        }

        public void SetData()//TODO share with Scatter
        {
            if (_dimensions.Count != 2)
            {
                this.ScatterModel = null;
                return;
            }
            var columnX = _dimensions[0];
            var columnY = _dimensions[1];
            var columnClass = ColumnClass.SelectedColumn.Name;

            var tmp = new PlotModel();
            tmp.Title = $"Scatter plot X: {columnX}, Y: {columnY}, Class: {columnClass}";

            //create line per class
            var lines = DataModel.Rows
                .Select(s => s[columnClass].ToString())
                .Distinct()
                .ToDictionary(x => x, x =>
                {
                    var color = _colorService.GetUniqueColorByName(x);
                    var lineSeries = new LineSeries
                    {
                        StrokeThickness = 0,
                        MarkerSize = 3,
                        MarkerStroke = OxyColor.FromRgb(color.R, color.G, color.B),
                        MarkerType = MarkerType.Diamond,
                        Title = x
                    };
                    lineSeries.MouseDown += LineSeries_MouseDown;
                    return lineSeries;
                });

            foreach (var row in DataModel.Rows)
            {
                var rowX = (double)row[columnX];
                var rowY = (double)row[columnY];
                var rowClass = row[columnClass].ToString();
                var val = rowY;
                var line = lines[rowClass];
                line.Points.Add(new DataPoint(rowX, val));
            }

            foreach (var line in lines)
            {
                tmp.Series.Add(line.Value);
            }
            this.ScatterModel = tmp;
        }

        private int[] GetPointVector(IList<double> values)
        {
            if (values.Count != _dimensions.Count)
            {
                throw new Exception("values.Count != _dimensions.Count");
            }

            var result = new int[this.Lines.Count];
            var i = 0;
            foreach (var line in this.Lines)
            {
                var value = values[_dimensions.IndexOf(line.Dimension)];
                var item = 0;

                if (line.Direction && value >= line.Value || !line.Direction && value <= line.Value)
                {
                    item = 1;
                }

                result[i++] = item;
            }

            return result;
        }

        private void LineSeries_MouseDown(object sender, OxyMouseDownEventArgs e)
        {

            var position = (sender as LineSeries).InverseTransform(e.Position);

            this.ClickedVector = "[" + string.Join(", ", GetPointVector(new List<double> { position.X, position.Y })) + "]";
        }

        private async Task OnGenerateExecute()
        {
            InitDimensions();
            SetData();
            UpdateInputDimensionsList();
        }

        private void UpdateInputDimensionsList()
        {
            //delete not used
            this.InputDimensions
                .Where(w =>
                    !this.DimensionColumns.SelectedColumns.Any(a => a.Name == w.Key)
                )
                .ToList()
                .ForEach(e => this.InputDimensions.Remove(e));

            //add new
            for (var i = 0; i < this.DimensionColumns.SelectedColumns.Count; i++)
            {
                var column = this.DimensionColumns.SelectedColumns[i];
                var columnName = column.Name;
                if (this.InputDimensions.Any(c => c.Key == columnName))
                {
                    continue;
                }

                var pair = new PairModel<string, double>();
                pair.Key = columnName;
                pair.Value = 0;
                this.InputDimensions.Insert(i, pair);
            }
        }

        private bool ShowFinishMessage()
        {
            if (_done)
            {
                _messageService.ShowInformationAsync("done!");
                return true;
            }
            if (_nonLinear)
            {
                _messageService.ShowWarningAsync("Data non linear!");
                return true;
            }
            return false;
        }

        private async Task OnNextStepExecute()
        {
            if (ShowFinishMessage())
            {
                return;
            }
            PutNextDivision();
            RefreshPlotLines();
            ShowFinishMessage();
        }

        private async Task OnFinishExecute()
        {
            if (ShowFinishMessage())
            {
                return;
            }

            while (!_done && !_nonLinear)
            {
                PutNextDivision();
            }
            RefreshPlotLines();

            ShowFinishMessage();
        }

        private async Task OnCalculateVectorExecute()
        {
            var values = new List<double>();
            foreach (var dimension in _dimensions)
            {
                var value = InputDimensions.Where(w => w.Key == dimension).First().Value;

                values.Add(value);
            }

            this.OutputVector = "[" + string.Join(", ", GetPointVector(values)) + "]";
        }



        private async Task OnAddVectorColumnExecute()
        {

            var typeFactory = this.GetTypeFactory();

            var newColumnDataWindowViewModel = typeFactory.CreateInstanceWithParametersAndAutoCompletion<NewColumnDataWindowViewModel>();
            newColumnDataWindowViewModel.ColumnName = "Vector";
            if (!await _uiVisualizerService.ShowDialogAsync(newColumnDataWindowViewModel) ?? false)
            {
                return;
            }
            var newName = newColumnDataWindowViewModel.ColumnName;
            if (DataModel.Columns.Any(s => s.Name == newName))
            {
                await _messageService.ShowErrorAsync("Column name already exist!");
                return;
            }

            var column = new ColumnModel()
            {
                Name = newName,
                Type = TypeEnum.String,
                Use = true
            };
            this.DataModel.Columns.Add(column);

            foreach (var row in DataModel.Rows)
            {
                var values = new List<double>();
                foreach (var dimension in _dimensions)
                {
                    var value = (double)row[dimension];

                    values.Add(value);
                }
                var vector = "[" + string.Join(",", GetPointVector(values)) + "]";

                row[newName] = vector;
            }

            await _messageService.ShowInformationAsync("Column added!");
        }

        private bool GenerateCanExecute()
        {
            return true;
            //DimensionColumns.SelectedColumns != null &&
            //    DimensionColumns.SelectedColumns.Count > 0 &&
            //    ColumnClass.SelectedColumn != null;
        }

        public TaskCommand Generate { get; private set; }
        public TaskCommand NextStep { get; private set; }
        public TaskCommand Finish { get; private set; }
        public TaskCommand CalculateVector { get; private set; }
        public TaskCommand AddVectorColumn { get; private set; }
    }
}
