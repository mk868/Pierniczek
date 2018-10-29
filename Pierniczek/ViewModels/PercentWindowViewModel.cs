using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;

using Catel.MVVM;

using Pierniczek.Models;

namespace Pierniczek.ViewModels
{
	public class PercentWindowViewModel : ViewModelBase
	{
		public IList<RowModel> Rows { get; set; }
		public ObservableCollection<DataGridColumn> Columns { get; set; }

		public void SetColumns(List<ColumnModel> fields)
		{
			var columns = new List<DataGridColumn>();

			foreach (var field in fields)
			{
				var column = new DataGridTextColumn
				{
					Header = field.Name,
					Width = new DataGridLength(1, DataGridLengthUnitType.Star),
					Binding = new Binding($"[{field.Name}]")
				};
				columns.Add(column);
			}

			Columns = new ObservableCollection<DataGridColumn>(columns);
		}
	}
}
