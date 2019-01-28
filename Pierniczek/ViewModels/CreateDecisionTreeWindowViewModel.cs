using Catel.Fody;
using Catel.IoC;
using Catel.MVVM;
using Catel.Services;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Series;
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
    public class CreateDecisionTreeWindowViewModel : ViewModelBase
    {
        private IMessageService _messageService;

        //[Model]
        //[Expose("Columns")]
        public DataModel DataModel { get; set; }
        public ObservableCollection<DecisionTreeBaseModel> Nodes { get; set; }

        public SelectColumnsModel Columns { get; set; }
        public SelectColumnModel ColumnClass { get; set; }
        public PlotModel ScatterModel { get; set; }

        public IList<PairModel<string, double>> InputDimensions { get; set; }
        public string OutputVector { get; set; }
        public string ClickedVector { get; set; }

        public CreateDecisionTreeWindowViewModel(DataModel data, IMessageService messageService)
        {
            _messageService = messageService;

            Nodes = new ObservableCollection<DecisionTreeBaseModel>();

            Columns = new SelectColumnsModel(data.Columns, TypeEnum.Number, "Attributes");
            ColumnClass = new SelectColumnModel(data.Columns, TypeEnum.String, "Class");
            DataModel = data;

            Generate = new TaskCommand(OnGenerateExecute);
        }

        private DecisionTreeBaseModel GetTreeNode(IList<string> attributeColumns, IList<RowModel> rows)
        {
            var classColumnName = ColumnClass.SelectedColumn.Name;
            var classes = rows.Select(r => r[classColumnName].ToString()).Distinct().ToList();

            if (classes.Count == 0) // ERROR
            {
                return new DecisionTreeLeafModel()
                {
                    Name = "Leaf" + (Nodes.Count + 1),
                    Value = null
                };
            }
            if (attributeColumns.Count == 0) // ERROR
            {
                return new DecisionTreeLeafModel()
                {
                    Name = "Leaf" + (Nodes.Count + 1),
                    Value = null
                };
            }
            if (classes.Count == 1)
            {
                return new DecisionTreeLeafModel()
                {
                    Name = "Leaf" + (Nodes.Count + 1),
                    Value = classes.FirstOrDefault()
                };
            }


            var minEntropy = double.MaxValue;
            string bestAttrColumn = null;

            foreach (var attrColumn in attributeColumns)
            {
                double entropy = 0;

                var values = rows.Select(r => (double)r[attrColumn]).ToList();
                var uniqueValues = values.Distinct().ToList();

                foreach (var uValue in uniqueValues)
                {
                    double uValueCount = values.Count(w => w == uValue);
                    double tmp = 0;

                    foreach (var uClass in classes)
                    {
                        double uClassValueCount = rows.Count(r =>
                            (double)r[attrColumn] == uValue &&
                            r[classColumnName].ToString() == uClass
                        );

                        if (uClassValueCount == 0)
                            continue;

                        tmp += (uClassValueCount / uValueCount) * Math.Log(uClassValueCount / uValueCount, 2);
                    }

                    entropy -= (uValueCount / rows.Count) * tmp;
                }
                if (entropy < minEntropy)
                {
                    minEntropy = entropy;
                    bestAttrColumn = attrColumn;
                }
            }
            DecisionTreeNodeModel result = new DecisionTreeNodeModel()
            {
                Name = "Node" + (Nodes.Count + 1),
                Attribute = bestAttrColumn
            };

            return result;
        }

        private IList<string> CloneAndRemove(IList<string> list, string item)
        {
            var result = new List<string>(list);
            result.Remove(item);
            return result;
        }

        private async Task OnGenerateExecute()
        {
            Nodes.Clear();

            if (ColumnClass.SelectedColumn == null)
            {
                await _messageService.ShowWarningAsync("Class column not set");
                return;
            }

            var classes = DataModel.Rows.Select(r => r[ColumnClass.SelectedColumn.Name].ToString()).Distinct().ToList();
            var allAttributeColumns = Columns.SelectedColumns.Select(s => s.Name).ToList();

            var parents = new List<Tuple<DecisionTreeNodeModel, IList<RowModel>, IList<string>>>();

            var rootNode = GetTreeNode(allAttributeColumns, DataModel.Rows);

            Nodes.Add(rootNode);
            if (rootNode is DecisionTreeNodeModel)
            {
                parents.Add(new Tuple<DecisionTreeNodeModel, IList<RowModel>, IList<string>>(rootNode as DecisionTreeNodeModel, DataModel.Rows, allAttributeColumns));
            }

            while (parents.Count > 0)
            {
                var parent = parents[0];
                parents.RemoveAt(0);
                var node = parent.Item1;
                var rows = parent.Item2;
                var attributeColumns = parent.Item3;

                var uniqueValues = rows.Select(r => (double)r[node.Attribute]).Distinct().ToList();
                var childrenAttributeColumns = CloneAndRemove(attributeColumns, node.Attribute);
                //check all leafs
                foreach (var uValue in uniqueValues)
                {
                    var childRows = rows.Where(r => (double)r[node.Attribute] == uValue).ToList();
                    var childNode = GetTreeNode(childrenAttributeColumns, childRows);
                    Nodes.Add(childNode);
                    node.Paths.Add(new Tuple<double, DecisionTreeBaseModel>(uValue, childNode));
                    if (childNode is DecisionTreeNodeModel)
                    {
                        parents.Add(new Tuple<DecisionTreeNodeModel, IList<RowModel>, IList<string>>(childNode as DecisionTreeNodeModel, childRows, childrenAttributeColumns));
                    }
                }


            }
        }

        public TaskCommand Generate { get; private set; }
    }
}
