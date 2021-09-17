using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnakeGUI
{
    public class DataGridCellClasses
    {
        public class DataGridViewCitizenCell : DataGridViewTextBoxCell
        {
            protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
            {
                base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);

                if (value is int citizen && rowIndex >= 0)
                {
                    var percent = citizen / 100.0;

                    var percentFillRect = new Rectangle(cellBounds.X, cellBounds.Y, (int)(cellBounds.Width * percent), cellBounds.Height);

                    var brush = new SolidBrush(Color.FromName(DataGridView.Rows[rowIndex].Cells[nameof(Results.Name)].Value as string)) ?? Brushes.Purple;
                    graphics.FillRectangle(brush, percentFillRect);
                    graphics.DrawString(citizen.ToString(), Control.DefaultFont, Brushes.White, cellBounds.X + 2, cellBounds.Y + 5);
                    graphics.DrawString(citizen.ToString(), Control.DefaultFont, Brushes.Black, cellBounds.X + 1, cellBounds.Y + 4);
                }
            }
        }

        public class DataGridViewCitizenColumn : DataGridViewColumn
        {
            public DataGridViewCitizenColumn(string name)
            {
                CellTemplate = new DataGridViewCitizenCell();
                Name = name;
                DataPropertyName = name;
            }
        }

        public class HighlightableDataGridViewCell : DataGridViewTextBoxCell
        {
            protected override void OnMouseEnter(int rowIndex)
            {
                try
                {
                    foreach (DataGridViewRow row in DataGridView.Rows)
                    {
                        if (row.Cells[ColumnIndex].Value != null && row.Cells[ColumnIndex].Value.Equals(Value))
                        {
                            row.DefaultCellStyle.BackColor = Color.FromName(row.Cells[nameof(Results.Name)].Value as string);
                        }
                    }
                }
                catch (ArgumentOutOfRangeException) { }

                base.OnMouseEnter(rowIndex);
            }

            protected override void OnMouseLeave(int rowIndex)
            {
                try
                {
                    foreach (DataGridViewRow row in DataGridView.Rows)
                    {
                        if (row.Cells[ColumnIndex].Value != null && row.Cells[ColumnIndex].Value.Equals(Value))
                        {
                            row.DefaultCellStyle.BackColor = Color.White;
                        }
                    }
                }
                catch (ArgumentOutOfRangeException) { }
                base.OnMouseEnter(rowIndex);
            }
        }

        public class HighlightableDataGridViewColumn : DataGridViewColumn
        {
            public HighlightableDataGridViewColumn(string name)
            {
                Name = name;
                DataPropertyName = name;
                CellTemplate = new HighlightableDataGridViewCell();
            }
        }


        public class DataGridViewDefaultColumn : DataGridViewColumn
        {
            public DataGridViewDefaultColumn(string name)
            {
                Name = name;
                DataPropertyName = name;
                CellTemplate = new DataGridViewTextBoxCell();
            }
        }
    }
}
