using Catel.Fody;
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
    public class DataViewModel : ViewModelBase
    {
        [Model]
        [Expose("Rows")]
        private DataModel Model { get; set; }

        public DataViewModel(DataModel model)
            : this()
        {
            Model = model;

            foreach (var column in model.Columns)
            {
                if (!column.Use)
                    continue;

                var dataGridColumn = new DataGridTextColumn();
                dataGridColumn.Header = column.Name;
                dataGridColumn.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                dataGridColumn.Binding = new Binding($"[{column.Name}]");
                Columns.Add(dataGridColumn);
            }
        }

        public DataViewModel()
        {
            Columns = new ObservableCollection<DataGridColumn>();
        }

        public ObservableCollection<DataGridColumn> Columns { get; private set; }

        public string Count => Model?.Rows.Count.ToString();
        public string ColumnsCount => Model?.Columns.Count.ToString();
        public string VisibleColumns => Model?.Columns.Where(c => c.Use).Count().ToString();
        public bool DataLoaded => Model != null;
    }
}
