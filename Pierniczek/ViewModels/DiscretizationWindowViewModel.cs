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
    public class DiscretizationWindowViewModel : ViewModelBase
    {
        private readonly IMessageService _messageService;
        private readonly IScaleService _scaleService;

        public DataModel Model { get; set; }
        public SelectColumnModel Column { get; set; }
        public NewColumnModel NewColumn { get; set; }
        public TaskCommand AddCommand { get; private set; }

        public DiscretizationWindowViewModel(DataModel data, IMessageService messageService, IScaleService scaleService)
        {
            _messageService = messageService;
            _scaleService = scaleService;

            Column = new SelectColumnModel(data.Columns, TypeEnum.Number, "Class");
            Ranges = new ObservableCollection<RangeModel>();
            Ranges.Add(new RangeModel(null, 1, true, false));

            Model = data;

            NewColumn = new NewColumnModel();
            AddCommand = new TaskCommand(OnAddCommandExecute);
        }

        private async Task OnAddCommandExecute()
        {
            if(Column.SelectedColumn == null)
            {
                await _messageService.ShowWarningAsync("Source column not set");
                return;
            }

            if (string.IsNullOrEmpty(NewColumn.ColumnName))
            {
                await _messageService.ShowWarningAsync("New column name not set");
                return;
            }

            if (Model.Columns.Any(a=>a.Name == NewColumn.ColumnName))
            {
                await _messageService.ShowWarningAsync("Column already exists!");
                return;
            }

            _scaleService.Discretization(Model, Column.SelectedColumn.Name, NewColumn.ColumnName, Ranges);

            await _messageService.ShowInformationAsync("Added!");
        }

        public ObservableCollection<RangeModel> Ranges { get; set; }
    }
}
