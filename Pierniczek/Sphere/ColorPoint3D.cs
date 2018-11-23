using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Composition;
using System.Windows.Media.Media3D;

namespace Pierniczek.Sphere
{
    public class ColorPoint3D
    {
        public Color Color { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        
        public ColorPoint3D()
        {
        }
        
        public GeometryModel3D GetGeometryModel3D()
        {
            var spherePoint = new SphereGeometry3D()
            {
                Radius = 1,
                Separators = 10
            };

            var geo = new GeometryModel3D();
            geo.Geometry = new MeshGeometry3D()
            {
                Positions = spherePoint.Points,
                TriangleIndices = spherePoint.TriangleIndices
            };

            geo.Material = new DiffuseMaterial()
            {
                Brush = new SolidColorBrush()
                {
                    Opacity = 1,
                    Color = Color
                }
            };

            geo.Transform = new ScaleTransform3D()
            {
                ScaleX = 0.5,
                ScaleY = 0.5,
                ScaleZ = 0.5,
                CenterX = -this.X * 10,//relative
                CenterY = this.Y * 10,
                CenterZ = this.Z * 10
            };

            return geo;
        }
    }
}
