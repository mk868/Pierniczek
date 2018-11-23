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

        public Plot3dWindowViewModel(IColorService colorService)
        {
            this._colorService = colorService;
            CameraPosition = new Point3D(0, 0, -100);

            CameraUp = new TaskCommand(OnCameraUpExecute);
            CameraDown = new TaskCommand(OnCameraDownExecute);
            CameraLeft = new TaskCommand(OnCameraLeftExecute);
            CameraRight = new TaskCommand(OnCameraRightExecute);
            ZoomIn = new TaskCommand(OnZoomInExecute);
            ZoomOut = new TaskCommand(OnZoomOutExecute);

            Points = new Model3DCollection();
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




        public string ColumnX { get; internal set; }
        public string ColumnY { get; internal set; }
        public string ColumnZ { get; internal set; }
        public string ColumnClass { get; internal set; }
        public IList<RowModel> Rows { get; internal set; }

        internal void Init()
        {
            foreach (var row in Rows)
            {
                var x = (decimal)row[ColumnX];
                var y = (decimal)row[ColumnY];
                var z = (decimal)row[ColumnZ];
                var className = (string)row[ColumnClass];
                var classColor = _colorService.GetUniqueColorByName(className);

                var colorPoint3D = new ColorPoint3D();
                colorPoint3D.X = (double)x;
                colorPoint3D.Y = (double)y;
                colorPoint3D.Z = (double)z;
                colorPoint3D.Color = Color.FromRgb(classColor.R, classColor.G, classColor.B);

                this.Points.Add(colorPoint3D.GetGeometryModel3D());
            }
        }
    }
}
