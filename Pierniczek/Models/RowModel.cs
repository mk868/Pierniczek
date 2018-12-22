using Catel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pierniczek.Models
{
    public class RowModel : ModelBase
    {
        private Dictionary<string, object> _dictionary;

        public RowModel()
        {
            _dictionary = new Dictionary<string, object>();
        }

        public object this[string key]
        {
            get
            {
                return _dictionary[key];
            }
            set
            {
                if (!(value is string || value is decimal))
                    throw new NotSupportedException("not supported data format! ");
                _dictionary[key] = value;
            }
        }

        public T Get<T>(string key) where T : class
        {
            return this[key] as T;
        }
    }
}
