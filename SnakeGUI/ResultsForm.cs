using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnakeGUI
{
    public partial class ResultsForm : Form
    {
        public ResultsForm(DataTable results)
        {
            InitializeComponent();

            resultsDataGrid.AutoGenerateColumns = false;
            BindingSource bindingSource = new BindingSource
            {
                DataSource = results,
            };

            resultsDataGrid.Columns.Add(new DataGridCellClasses.DataGridViewDefaultColumn("#"));
            resultsDataGrid.Columns.Add(new DataGridCellClasses.DataGridViewDefaultColumn(nameof(Results.Name)));
            resultsDataGrid.Columns.Add(new DataGridCellClasses.DataGridViewDefaultColumn(nameof(Results.Score)));
            resultsDataGrid.Columns.Add(new DataGridCellClasses.DataGridViewDefaultColumn(nameof(Results.Length)));
            resultsDataGrid.Columns.Add(new DataGridCellClasses.HighlightableDataGridViewColumn(nameof(Results.Layers)));
            resultsDataGrid.Columns.Add(new DataGridCellClasses.HighlightableDataGridViewColumn(nameof(Results.ActivationFunction)));
            resultsDataGrid.Columns.Add(new DataGridCellClasses.DataGridViewDefaultColumn(nameof(Results.Generation)));
            resultsDataGrid.Columns.Add(new DataGridCellClasses.DataGridViewCitizenColumn(nameof(Results.Citizen)));

            resultsDataGrid.DataSource = bindingSource;
        }

        private void Results_ListChanged(object sender, ListChangedEventArgs e)
        {
            Refresh();
        }
    }

    public class Results
    {
        public string Name;
        public string ActivationFunction;
        public string Layers;
        public double Score;
        public int Generation;
        public int Citizen;
        public int Length;
    }
}
