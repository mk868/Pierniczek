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
    public class SelectKGroupDistanceMethodDataWindowViewModel : ViewModelBase
    {
        public SelectKGroupDistanceMethodDataWindowViewModel()
        {
            this.Title = "Select method";

            Methods = new List<DistanceMeasureMethodEnum>
            {
                DistanceMeasureMethodEnum.Euclidean,
                DistanceMeasureMethodEnum.Infinity,
                DistanceMeasureMethodEnum.Mahalanobis,
                DistanceMeasureMethodEnum.l1
            };
        }

        public void SetTitle(string title)
        {
            this.Title = title;
        }
        public IList<DistanceMeasureMethodEnum> Methods { get; set; }
        public DistanceMeasureMethodEnum SelectedMethod { get; set; }
    }
}
