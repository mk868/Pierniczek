using Catel.Fody;
using Catel.IoC;
using Catel.MVVM;
using Catel.Services;
using Pierniczek.Models;
using Pierniczek.Services.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace Pierniczek.ViewModels
{
    public class SelectColumnsViewModel : ViewModelBase
    {
        [Model]
        [Expose("Columns")]
        // [Expose("SelectedColumns")]
        [Expose("Label")]
        private SelectColumnsModel Model { get; set; }
        

        public SelectColumnsViewModel(SelectColumnsModel model)
        {
            Model = model;
            SelectionChangedCommand = new TaskCommand<ObservableCollection<object>>(OnSelectionChangedExecute);
        }

        private async Task OnSelectionChangedExecute(ObservableCollection<object> obj)
        {
            var selected = Enumerable.ToList<object>(obj).Select(s => s as ColumnModel).ToList();
            Model.SelectedColumns = selected;
        }

        public TaskCommand<ObservableCollection<object>> SelectionChangedCommand { get; set; }
    }
}
