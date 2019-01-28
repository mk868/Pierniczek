using Catel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pierniczek.Models
{
    public class RowModel
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
                if (value is string || value is double)
                {
                    _dictionary[key] = value;
                    return;
                }
                if(value is int)
                {
                    _dictionary[key] = (double)(int)value;
                    return;
                }
                if(value is char)
                {
                    _dictionary[key] = ((char)value).ToString();
                }

                throw new NotSupportedException("not supported data format! ");
            }
        }

        public T Get<T>(string key) where T : class
        {
            return this[key] as T;
        }
    }
}
