using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pierniczek.Services.Interfaces
{
    public interface IColorService
    {
        BaseColor GetUniqueColorByName(string name);
    }

    public struct BaseColor
    {
        internal BaseColor(int color)
        {
            this.R = (byte)(color >> 16 & 0xFF);
            this.G = (byte)(color >> 8 & 0xFF);
            this.B = (byte)(color >> 0 & 0xFF);
        }

        internal BaseColor(byte r, byte g, byte b)
        {
            this.R = r;
            this.G = g;
            this.B = b;
        }

        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
    }
}
