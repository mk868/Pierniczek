using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pierniczek.Models
{
    [Flags]
    public enum FileDataSeparatorEnum
    {
        WhiteChars = 1,
        Tab = 2,
        Dot = 4,
        Comma = 8,
        Semicolon = 16,
        Space = 32
    }
}
