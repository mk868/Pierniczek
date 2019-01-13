using Catel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pierniczek.Models
{
    public class PairModel<TKey, TValue> : ModelBase
    {
        public TKey Key { get; set; }
        public TValue Value { get; set; }

        public PairModel()
        {
        }

        public PairModel(TKey key, TValue value)
        {
            this.Key = key;
            this.Value = value;
        }
    }
}
