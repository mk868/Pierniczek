using Catel.IoC;
using Catel.MVVM;
using Catel.Services;
using OxyPlot;
using OxyPlot.Series;
using Pierniczek.Models;
using Pierniczek.Services.Interfaces;
using Pierniczek.Sphere;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Pierniczek.ViewModels
{
    public class Plot3dWindowViewModel : ViewModelBase
    {
        private readonly IColorService _colorService;
        public const int Offset = 20;
        public Point3D CameraPosition { get; set; }
        public Model3DCollection Points { get; set; }

        public SelectColumnModel ColumnX { get; set; }
        public SelectColumnModel ColumnY { get; set; }
        public SelectColumnModel ColumnZ { get; set; }
        public SelectColumnModel ColumnClass { get; set; }

        private DataModel DataModel { get; set; }

        public Plot3dWindowViewModel(DataModel data, IColorService colorService)
        {
            this._colorService = colorService;
            CameraPosition = new Point3D(0, 0, -100);

            ColumnX = new SelectColumnModel(data.Columns.Where(c => c.Type == TypeEnum.Number).ToList(), "X axis");
            ColumnY = new SelectColumnModel(data.Columns.Where(c => c.Type == TypeEnum.Number).ToList(), "Y axis");
            ColumnZ = new SelectColumnModel(data.Columns.Where(c => c.Type == TypeEnum.Number).ToList(), "Z axis");
            ColumnClass = new SelectColumnModel(data.Columns, "Class");

            CameraUp = new TaskCommand(OnCameraUpExecute);
            CameraDown = new TaskCommand(OnCameraDownExecute);
            CameraLeft = new TaskCommand(OnCameraLeftExecute);
            CameraRight = new TaskCommand(OnCameraRightExecute);
            ZoomIn = new TaskCommand(OnZoomInExecute);
            ZoomOut = new TaskCommand(OnZoomOutExecute);

            Points = new Model3DCollection();

            Generate = new TaskCommand(OnGenerateExecute, GenerateCanExecute);
            DataModel = data;
        }


        private async Task OnCameraUpExecute()
        {
            var point = CameraPosition;
            point.Y += Offset;
            CameraPosition = point;
        }

        private async Task OnCameraDownExecute()
        {
            var point = CameraPosition;
            point.Y -= Offset;
            CameraPosition = point;
        }

        private async Task OnCameraLeftExecute()
        {
            var point = CameraPosition;
            point.X += Offset;
            CameraPosition = point;
        }

        private async Task OnCameraRightExecute()
        {
            var point = CameraPosition;
            point.X -= Offset;
            CameraPosition = point;
        }

        private async Task OnZoomInExecute()
        {
            var point = CameraPosition;
            point.Z += Offset;
            CameraPosition = point;
        }

        private async Task OnZoomOutExecute()
        {
            var point = CameraPosition;
            point.Z -= Offset;
            CameraPosition = point;
        }


        public TaskCommand CameraUp { get; private set; }
        public TaskCommand CameraDown { get; private set; }
        public TaskCommand CameraLeft { get; private set; }
        public TaskCommand CameraRight { get; private set; }
        public TaskCommand ZoomOut { get; private set; }
        public TaskCommand ZoomIn { get; private set; }
        public TaskCommand Generate { get; private set; }

        private async Task OnGenerateExecute()
        {
            var columnX = ColumnX.SelectedColumn.Name;
            var columnY = ColumnY.SelectedColumn.Name;
            var columnZ = ColumnZ.SelectedColumn.Name;
            var columnClass = ColumnClass.SelectedColumn.Name;

            this.Points.Clear();
            foreach (var row in DataModel.Rows)
            {
                var x = (decimal)row[columnX];
                var y = (decimal)row[columnY];
                var z = (decimal)row[columnZ];
                var className = ColumnClass.SelectedColumn.Type == TypeEnum.String ? (string)row[columnClass] : ((decimal)row[columnClass]).ToString();
                var classColor = _colorService.GetUniqueColorByName(className);

                var colorPoint3D = new ColorPoint3D();
                colorPoint3D.X = (double)x;
                colorPoint3D.Y = (double)y;
                colorPoint3D.Z = (double)z;
                colorPoint3D.Color = Color.FromRgb(classColor.R, classColor.G, classColor.B);

                this.Points.Add(colorPoint3D.GetGeometryModel3D());
            }
        }


        private bool GenerateCanExecute()
        {
            return ColumnX.SelectedColumn != null &&
                ColumnY.SelectedColumn != null &&
                ColumnZ.SelectedColumn != null &&
                ColumnClass.SelectedColumn != null;
        }
    }
}
