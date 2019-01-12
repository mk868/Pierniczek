using Catel.IoC;
using Catel.MVVM;
using Catel.Services;
using OxyPlot;
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
    public class ScatterWindowViewModel : ViewModelBase
    {
        private readonly IColorService _colorService;
        private DataModel DataModel { get; set; }

        public ScatterWindowViewModel(DataModel data, IColorService colorService)
        {
            this._colorService = colorService;

            ColumnX = new SelectColumnModel(data.Columns, TypeEnum.Number, "X axis");
            ColumnY = new SelectColumnModel(data.Columns, "Y axis");
            ColumnClass = new SelectColumnModel(data.Columns, TypeEnum.String, "Class");
            DataModel = data;

            Generate = new TaskCommand(OnGenerateExecute, GenerateCanExecute);
        }


        public SelectColumnModel ColumnX { get; set; }
        public SelectColumnModel ColumnY { get; set; }
        public SelectColumnModel ColumnClass { get; set; }
        public PlotModel ScatterModel { get; set; }


        public void SetData()
        {
            bool isColumnYClass = false;
            var columnX = ColumnX.SelectedColumn.Name;
            var columnY = ColumnY.SelectedColumn.Name;
            var columnClass = ColumnClass.SelectedColumn?.Name; //TODO can be set to null!
            if (ColumnY.SelectedColumn.Type == TypeEnum.String)
            {
                isColumnYClass = true;
                columnClass = columnY;
            }


            var tmp = new PlotModel();
            tmp.Title = $"Scatter plot X: {columnX}, Y: {columnY}, Class: {columnClass}";

            //create line per class
            var lines = DataModel.Rows
                .Select(s => s[columnClass] as string)
                .Distinct()
                .ToDictionary(x => x, x =>
                {
                    var color = _colorService.GetUniqueColorByName(x);
                    return new LineSeries
                    {
                        StrokeThickness = 0,
                        MarkerSize = 3,
                        MarkerStroke = OxyColor.FromRgb(color.R, color.G, color.B),
                        MarkerType = MarkerType.Diamond,
                        Title = x
                    };
                });
            
            Dictionary<(double, string), int> classCount = null;
            if (isColumnYClass)
            {
                classCount = DataModel.Rows
                    .GroupBy(x =>
                        new { xVal = (double)x[columnX], yClass = x[columnY] as string }
                ) //hardcore ;)
                    .ToDictionary(x => new ValueTuple<double, string>(x.Key.xVal, x.Key.yClass), x => x.Count());
            }

            foreach (var row in DataModel.Rows)
            {
                var rowX = (double)row[columnX];
                var rowClass = row[columnClass] as string;
                double val;
                if (isColumnYClass)
                {
                    var key = new ValueTuple<double, string>(rowX, rowClass);
                    if (!classCount.ContainsKey(key))
                        continue;

                    val = classCount[key];
                    classCount.Remove(key);
                }
                else
                {
                    val = (double)row[columnY];
                }
                
                var line = lines[rowClass];
                line.Points.Add(new DataPoint(rowX, val));
            }

            foreach (var line in lines)
            {
                tmp.Series.Add(line.Value);
            }
            this.ScatterModel = tmp;
        }

        private async Task OnGenerateExecute()
        {
            SetData();
        }


        private bool GenerateCanExecute()
        {
            return ColumnX.SelectedColumn != null &&
                ColumnY.SelectedColumn != null;
        }

        public TaskCommand Generate { get; private set; }
    }
}
