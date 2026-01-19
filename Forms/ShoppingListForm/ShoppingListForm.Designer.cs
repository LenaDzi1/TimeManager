using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TimeManager.Forms
{
    partial class ShoppingListForm
    {
        private IContainer components = null;
        private ListView _listView;
        private Button _btnRefresh;
        private Button _btnClose;
        private Panel headerPanel;
        private Label lblTitle;
        private Panel listContainer;
        private Panel buttonsPanel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            _listView = new ListView();
            _btnRefresh = new Button();
            _btnClose = new Button();
            headerPanel = new Panel();
            lblTitle = new Label();
            listContainer = new Panel();
            buttonsPanel = new Panel();
            headerPanel.SuspendLayout();
            listContainer.SuspendLayout();
            buttonsPanel.SuspendLayout();
            SuspendLayout();
            // 
            // _listView
            // 
            _listView.Columns.AddRange(new ColumnHeader[] {
            new ColumnHeader { Text = "Name", Width = 150 },
            new ColumnHeader { Text = "Amount", Width = 80 },
            new ColumnHeader { Text = "Unit", Width = 80 },
            new ColumnHeader { Text = "Type", Width = 100 },
            new ColumnHeader { Text = "Source", Width = 100 }});
            _listView.Dock = DockStyle.Top;
            _listView.Font = new Font("Segoe UI", 10F);
            _listView.FullRowSelect = true;
            _listView.GridLines = true;
            _listView.Location = new Point(20, 80);
            _listView.Name = "_listView";
            _listView.Size = new Size(535, 350);
            _listView.TabIndex = 0;
            _listView.UseCompatibleStateImageBehavior = false;
            _listView.View = View.Details;
            // 
            // _btnRefresh
            // 
            _btnRefresh.BackColor = Color.FromArgb(52, 152, 219);
            _btnRefresh.FlatAppearance.BorderSize = 0;
            _btnRefresh.FlatStyle = FlatStyle.Flat;
            _btnRefresh.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            _btnRefresh.ForeColor = Color.White;
            _btnRefresh.Location = new Point(20, 10);
            _btnRefresh.Name = "_btnRefresh";
            _btnRefresh.Size = new Size(100, 30);
            _btnRefresh.TabIndex = 0;
            _btnRefresh.Text = "Refresh";
            _btnRefresh.UseVisualStyleBackColor = false;
            // 
            // _btnClose
            // 
            _btnClose.BackColor = Color.FromArgb(149, 165, 166);
            _btnClose.FlatAppearance.BorderSize = 0;
            _btnClose.FlatStyle = FlatStyle.Flat;
            _btnClose.Font = new Font("Segoe UI", 9F);
            _btnClose.ForeColor = Color.White;
            _btnClose.Location = new Point(455, 10);
            _btnClose.Name = "_btnClose";
            _btnClose.Size = new Size(100, 30);
            _btnClose.TabIndex = 1;
            _btnClose.Text = "Close";
            _btnClose.UseVisualStyleBackColor = false;
            // 
            // headerPanel
            // 
            headerPanel.BackColor = Color.FromArgb(52, 152, 219);
            headerPanel.Controls.Add(lblTitle);
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Location = new Point(0, 0);
            headerPanel.Name = "headerPanel";
            headerPanel.Padding = new Padding(20, 0, 20, 0);
            headerPanel.Size = new Size(575, 60);
            headerPanel.TabIndex = 0;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(20, 15);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(154, 30);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Shopping List";
            // 
            // listContainer
            // 
            listContainer.BackColor = Color.White;
            listContainer.Controls.Add(_listView);
            listContainer.Dock = DockStyle.Fill;
            listContainer.Location = new Point(0, 0);
            listContainer.Name = "listContainer";
            listContainer.Padding = new Padding(20, 80, 20, 60);
            listContainer.Size = new Size(575, 440);
            listContainer.TabIndex = 1;
            // 
            // buttonsPanel
            // 
            buttonsPanel.BackColor = Color.White;
            buttonsPanel.Controls.Add(_btnRefresh);
            buttonsPanel.Controls.Add(_btnClose);
            buttonsPanel.Dock = DockStyle.Bottom;
            buttonsPanel.Location = new Point(0, 440);
            buttonsPanel.Name = "buttonsPanel";
            buttonsPanel.Padding = new Padding(20, 10, 20, 10);
            buttonsPanel.Size = new Size(575, 50);
            buttonsPanel.TabIndex = 2;
            // 
            // ShoppingListForm
            // 
            BackColor = Color.White;
            ClientSize = new Size(575, 490);
            Controls.Add(headerPanel);
            Controls.Add(listContainer);
            Controls.Add(buttonsPanel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ShoppingListForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Shopping List";
            headerPanel.ResumeLayout(false);
            headerPanel.PerformLayout();
            listContainer.ResumeLayout(false);
            buttonsPanel.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}









