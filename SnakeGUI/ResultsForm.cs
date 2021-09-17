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
        private DataTable _resultsTable;
        private List<GameWrapper> _games;

        public ResultsForm(List<GameWrapper> games)
        {
            InitializeComponent();

            _games = games;
            _resultsTable = new DataTable();
            _resultsTable.Columns.Add("#", typeof(int));
            _resultsTable.Columns.Add(nameof(Results.Name), typeof(string));
            _resultsTable.Columns.Add(nameof(Results.Score), typeof(string));
            _resultsTable.Columns.Add(nameof(Results.Length), typeof(int));
            _resultsTable.Columns.Add(nameof(Results.Layers), typeof(string));
            _resultsTable.Columns.Add(nameof(Results.ActivationFunction), typeof(string));
            _resultsTable.Columns.Add(nameof(Results.Generation), typeof(int));
            _resultsTable.Columns.Add(nameof(Results.Citizen), typeof(int));
            _resultsTable.Columns.Add(nameof(Results.Draw), typeof(bool));
            _resultsTable.Columns.Add(nameof(Results.Guid), typeof(Guid));

            RegenerateDataTable();

            resultsDataGrid.AutoGenerateColumns = false;
            BindingSource bindingSource = new BindingSource
            {
                DataSource = _resultsTable,
            };

            resultsDataGrid.Columns.Add(new DataGridCellClasses.DataGridViewDefaultColumn("#"));
            resultsDataGrid.Columns.Add(new DataGridCellClasses.HighlightableDataGridViewColumn(nameof(Results.Name)));
            resultsDataGrid.Columns.Add(new DataGridCellClasses.DataGridViewDefaultColumn(nameof(Results.Score)));
            resultsDataGrid.Columns.Add(new DataGridCellClasses.DataGridViewDefaultColumn(nameof(Results.Length)));
            resultsDataGrid.Columns.Add(new DataGridCellClasses.HighlightableDataGridViewColumn(nameof(Results.Layers)));
            resultsDataGrid.Columns.Add(new DataGridCellClasses.HighlightableDataGridViewColumn(nameof(Results.ActivationFunction)));
            resultsDataGrid.Columns.Add(new DataGridCellClasses.DataGridViewDefaultColumn(nameof(Results.Generation)));
            resultsDataGrid.Columns.Add(new DataGridCellClasses.DataGridViewCitizenColumn(nameof(Results.Citizen)));
            resultsDataGrid.Columns.Add(new DataGridCellClasses.CheckboxDataGridViewColumn(nameof(Results.Draw)));
            resultsDataGrid.Columns.Add(new DataGridCellClasses.HiddenDataGridViewColumn(nameof(Results.Guid)));

            resultsDataGrid.DataSource = bindingSource;

            Timer timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;

            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            RegenerateDataTable();
        }

        public void RegenerateDataTable()
        {
            _resultsTable.Clear();

            int i = 0;

            var results = _games.Select(g => new
            {
                brushName = ((SolidBrush)g.Colour).Color.Name,
                score = g.Manager.BestFitness,
                length = g.Snake.SnakeBody.Count(),
                layers = g.Manager.Layers,
                activationFunc = g.Manager.ActivationFunc,
                generation = g.Manager.Generation,
                citizen = g.Manager.Current,
                draw = g.DrawGame,
                guid = g.Manager.Guid,
            }).OrderByDescending(x => x.score).ToList();

            foreach (var game in results)
            {
                var row = _resultsTable.NewRow();

                row["#"] = ++i;
                row[nameof(Results.Name)] = game.brushName;
                row[nameof(Results.Score)] = game.score.ToString("N");
                row[nameof(Results.Length)] = game.length;
                row[nameof(Results.Layers)] = game.layers;
                row[nameof(Results.ActivationFunction)] = game.activationFunc;
                row[nameof(Results.Generation)] = game.generation;
                row[nameof(Results.Citizen)] = game.citizen;
                row[nameof(Results.Draw)] = game.draw;
                row[nameof(Results.Guid)] = game.guid;

                _resultsTable.Rows.Add(row);
            }
        }

        private void Results_ListChanged(object sender, ListChangedEventArgs e)
        {
            Refresh();
        }

        private void resultsDataGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (resultsDataGrid.Columns[e.ColumnIndex].Name == nameof(Results.Draw))
            {
                var checkbox = (bool)(resultsDataGrid.Rows[e.RowIndex].Cells[nameof(Results.Draw)].Value);
                var guid = resultsDataGrid.Rows[e.RowIndex].Cells[nameof(Results.Guid)].Value as Guid?;
                if (guid.HasValue)
                {
                    _games.First(x => x.Manager.Guid == guid).DrawGame = checkbox;
                }
            }
        }

        private void resultsDataGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            resultsDataGrid.CommitEdit(DataGridViewDataErrorContexts.Commit);
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
        public bool Draw;
        public Guid Guid;
    }
}
