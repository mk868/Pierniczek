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
        private List<Color> _predefinedColors = new List<Color>{
            new Color(0xFF0000),
            new Color(0x00FF00),
            new Color(0x0000FF),
            new Color(0x00FFFF),
            new Color(0xFF00FF),
            new Color(0xFFFF00),
            new Color(0x880000),
            new Color(0x008800),
            new Color(0x000088),
            new Color(0x008888),
            new Color(0x880088),
            new Color(0x888800),
        };

        private Dictionary<string, Color> _colors;

        public ColorService()
        {
            _colors = new Dictionary<string, Color>();
        }

        public Color GetUniqueColorByName(string name)
        {
            if (_colors.ContainsKey(name))
                return _colors[name];

            Color color;

            if (_predefinedColors.Count > 0)
            {
                color = _predefinedColors[0];
                _predefinedColors.RemoveAt(0);
            }
            else
            {
                color = new Color(_random.Next());
            }

            _colors[name] = color;
            return color;
        }


    }
}
