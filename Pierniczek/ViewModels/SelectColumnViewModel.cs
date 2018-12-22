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
    public class SelectColumnViewModel : ViewModelBase
    {
        [Model]
        [Expose("Columns")]
        [Expose("SelectedColumn")]
        [Expose("Label")]
        private SelectColumnModel Model { get; set; }

        public SelectColumnViewModel(SelectColumnModel model)
        {
            Model = model;
        }
    }
}
