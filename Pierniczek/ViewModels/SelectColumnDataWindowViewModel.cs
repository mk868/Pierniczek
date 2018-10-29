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
    public class SelectColumnDataWindowViewModel : ViewModelBase
    {
        public SelectColumnDataWindowViewModel()
        {
            this.Title = "Select column";
        }

        public void SetTitle(string title)
        {
            this.Title = title;
        }
        public IList<ColumnModel> Columns { get; set; }
        public ColumnModel SelectedColumn { get; set; }
    }
}
