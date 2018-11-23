using Pierniczek.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pierniczek.Services
{
    public class ColorService : IColorService
    {
        private Random _random = new Random();
        private List<BaseColor> _predefinedColors = new List<BaseColor>{
            new BaseColor(0xFF0000),
            new BaseColor(0x00FF00),
            new BaseColor(0x0000FF),
            new BaseColor(0x00FFFF),
            new BaseColor(0xFF00FF),
            new BaseColor(0xFFFF00),
            new BaseColor(0x880000),
            new BaseColor(0x008800),
            new BaseColor(0x000088),
            new BaseColor(0x008888),
            new BaseColor(0x880088),
            new BaseColor(0x888800),
        };

        private Dictionary<string, BaseColor> _colors;

        public ColorService()
        {
            _colors = new Dictionary<string, BaseColor>();
        }

        public BaseColor GetUniqueColorByName(string name)
        {
            if (_colors.ContainsKey(name))
                return _colors[name];

            BaseColor color;

            if (_predefinedColors.Count > 0)
            {
                color = _predefinedColors[0];
                _predefinedColors.RemoveAt(0);
            }
            else
            {
                color = new BaseColor(_random.Next());
            }

            _colors[name] = color;
            return color;
        }


    }
}
