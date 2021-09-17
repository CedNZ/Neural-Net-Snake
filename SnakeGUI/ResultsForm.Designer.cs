
namespace SnakeGUI
{
    partial class ResultsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.resultsDataGrid = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.resultsDataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // resultsDataGrid
            // 
            this.resultsDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.resultsDataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resultsDataGrid.Location = new System.Drawing.Point(0, 0);
            this.resultsDataGrid.Name = "resultsDataGrid";
            this.resultsDataGrid.RowTemplate.Height = 25;
            this.resultsDataGrid.Size = new System.Drawing.Size(1000, 450);
            this.resultsDataGrid.TabIndex = 0;
            this.resultsDataGrid.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.resultsDataGrid_CellContentClick);
            this.resultsDataGrid.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.resultsDataGrid_CellDoubleClick);
            this.resultsDataGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.resultsDataGrid_CellValueChanged);
            this.resultsDataGrid.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.resultsDataGrid_KeyPress);
            // 
            // ResultsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 450);
            this.Controls.Add(this.resultsDataGrid);
            this.Name = "ResultsForm";
            this.Text = "ResultsForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ResultsForm_FormClosing);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ResultsForm_KeyPress);
            ((System.ComponentModel.ISupportInitialize)(this.resultsDataGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView resultsDataGrid;
    }
}