using System;
using System.Collections.Generic;
using System.Linq;

using Catel.MVVM;

namespace Pierniczek.ViewModels
{
	public class SetPercentWindowViewModel : ViewModelBase
	{
		public SetPercentWindowViewModel()
		{

		}

		private int _percent = 0;

		public int Percent
		{
			get => this._percent;
			set
			{
				if (value > 100)
				{
					this._percent = 100;
				}
				else if (value < 0)
				{
					this._percent = 0;
				}
				else
				{
					this._percent = value;
				}
			}
		}
	}
}
