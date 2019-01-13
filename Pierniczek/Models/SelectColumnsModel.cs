using Catel.Data;
using Catel.IoC;
using Catel.MVVM;
using Catel.Services;
using Pierniczek.Models;
using Pierniczek.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace Pierniczek.ViewModels
{
    public class SelectColumnsModel : ModelBase
    {
        public SelectColumnsModel()
        {
        }

        public SelectColumnsModel(IList<ColumnModel> columns, string label)
        {
            this.Columns = columns;
            this.Label = label;
        }

        public SelectColumnsModel(IList<ColumnModel> columns, TypeEnum type, string label)
            : this(columns.Where(c => c.Type == type).ToList(), label)
        {
        }

        public IList<ColumnModel> Columns { get; set; }
        public IList<ColumnModel> SelectedColumns { get; set; }
        public string Label { get; set; }
    }
}
