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

        private DataModel DataModel { get; set; }
        public ObservableCollection<BinaryVectorByClassLineModel> Lines { get; set; }

        public BinaryVectorByClassWindowViewModel(DataModel data, IColorService colorService, IMessageService messageService)
        {
            this._colorService = colorService;
            this._messageService = messageService;

            Lines = new ObservableCollection<BinaryVectorByClassLineModel>();

            ColumnX = new SelectColumnModel(data.Columns, TypeEnum.Number, "X axis");
            ColumnY = new SelectColumnModel(data.Columns, TypeEnum.Number, "Y axis");
            ColumnClass = new SelectColumnModel(data.Columns, "Class");
            DataModel = data;

            Generate = new TaskCommand(OnGenerateExecute, GenerateCanExecute);
            NextStep = new TaskCommand(OnNextStepExecute, GenerateCanExecute);
            Finish = new TaskCommand(OnFinishExecute, GenerateCanExecute);
            CalculateVector = new TaskCommand(OnCalculateVectorExecute, GenerateCanExecute);
        }

        public SelectColumnModel ColumnX { get; set; }
        public SelectColumnModel ColumnY { get; set; }
        public SelectColumnModel ColumnClass { get; set; }
        public PlotModel ScatterModel { get; set; }

        public double InputX { get; set; }
        public double InputY { get; set; }
        public string OutputVector { get; set; }
        public string ClickedVector { get; set; }



        private IList<string> _dimensions = new List<string>();
        private Dictionary<string, IList<RowModel>> _sortedRows;
        private string _columnClass;
        private bool _done;
        private bool _nonLinear;
        private string _columnHorizontal;//used in plot lines


        private void InitDimensions()
        {
            this.Lines.Clear();
            _done = false;
            _nonLinear = false;
            var columnX = ColumnX.SelectedColumn.Name;
            var columnY = ColumnY.SelectedColumn.Name;
            _columnHorizontal = columnX;
            _dimensions.Clear();
            _dimensions.Add(columnX);
            _dimensions.Add(columnY);
            _columnClass = ColumnClass.SelectedColumn.Name;

            _sortedRows = new Dictionary<string, IList<RowModel>>();
            foreach (var dimension in _dimensions)
            {
                _sortedRows[dimension] = DataModel.Rows.OrderBy(o => (double)o[dimension]).ToList();
            }
        }

        private void RefreshPlotLines()
        {
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
            var columnX = ColumnX.SelectedColumn.Name;
            var columnY = ColumnY.SelectedColumn.Name;
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

        private int[] GetPointVector(double x, double y)
        {
            var result = new int[this.Lines.Count];
            var i = 0;
            foreach (var line in this.Lines)
            {
                var value = line.Dimension == _columnHorizontal ? x : y;
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

            this.ClickedVector = "[" + string.Join(", ", GetPointVector(position.X, position.Y)) + "]";
        }

        private async Task OnGenerateExecute()
        {
            InitDimensions();
            SetData();
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
            this.OutputVector = "[" + string.Join(", ", GetPointVector(this.InputX, InputY)) + "]";
        }

        private bool GenerateCanExecute()
        {
            return ColumnX.SelectedColumn != null &&
                ColumnY.SelectedColumn != null;
        }

        public TaskCommand Generate { get; private set; }
        public TaskCommand NextStep { get; private set; }
        public TaskCommand Finish { get; private set; }
        public TaskCommand CalculateVector { get; private set; }
    }
}
