using System.Drawing;
using System.Windows.Forms;

namespace TimeManager.Forms
{
    partial class HomeBrowserDesignView
    {
        private Panel _canvasPanel;
        private Label _lblSize;
        private NumericUpDown _numWidth;
        private NumericUpDown _numHeight;
        private Button _btnFloorUp;
        private Button _btnFloorDown;
        private Button _btnAddFloor;
        private Button _btnDeleteFloor;
        private Button _btnAddWall;
        private Button _btnAddContainer;
        private Button _btnSave;
        private TextBox _txtFloorName;
        private Panel _rightPanel;

        private ListBox _lstContainers;
        private Label _lblContainerSettings;
        private TextBox _txtContainerName;
        private TextBox _txtRoomTag;
        private TextBox _txtItemName;
        private NumericUpDown _numItemQty;
        private ComboBox _cmbItemUnit;
        private Button _btnAddItem;
        private Button _btnDeleteItems;
        private ListView _lvItems;
        private Label _lblAddingItem;
        private Label _lblPoints;
        private Label _lblPointA;
        private NumericUpDown _numPointAX;
        private NumericUpDown _numPointAY;
        private Label _lblPointB;
        private NumericUpDown _numPointBX;
        private NumericUpDown _numPointBY;
        private Button _btnAddSubcontainer;
        private Button _btnDeleteContainer;
        private Label _lblSubsections;


        private void InitializeComponent()
        {
            _canvasPanel = new Panel();
            _lblSize = new Label();
            _numWidth = new NumericUpDown();
            _numHeight = new NumericUpDown();
            _btnFloorUp = new Button();
            _btnFloorDown = new Button();
            _btnAddFloor = new Button();
            _btnDeleteFloor = new Button();
            _btnAddWall = new Button();
            _btnAddContainer = new Button();
            _btnSave = new Button();
            _txtFloorName = new TextBox();
            _rightPanel = new Panel();
            _lblAddingItem = new Label();
            _txtItemName = new TextBox();
            _numItemQty = new NumericUpDown();
            _cmbItemUnit = new ComboBox();
            _btnAddItem = new Button();
            _btnDeleteItems = new Button();
            _lvItems = new ListView();
            _lblContainerSettings = new Label();
            _txtContainerName = new TextBox();
            _txtRoomTag = new TextBox();
            _lblPoints = new Label();
            _lblPointA = new Label();
            _numPointAX = new NumericUpDown();
            _numPointAY = new NumericUpDown();
            _lblPointB = new Label();
            _numPointBX = new NumericUpDown();
            _numPointBY = new NumericUpDown();
            _btnAddSubcontainer = new Button();
            _btnDeleteContainer = new Button();
            _lblSubsections = new Label();
            _lstContainers = new ListBox();
            ((System.ComponentModel.ISupportInitialize)_numWidth).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_numHeight).BeginInit();
            _rightPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_numItemQty).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_numPointAX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_numPointAY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_numPointBX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_numPointBY).BeginInit();
            SuspendLayout();
            // 
            // _canvasPanel
            // 
            _canvasPanel.BackColor = Color.FromArgb(252, 253, 255);
            _canvasPanel.BorderStyle = BorderStyle.FixedSingle;
            _canvasPanel.Location = new Point(19, 38);
            _canvasPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            _canvasPanel.Name = "_canvasPanel";
            _canvasPanel.Size = new Size(775, 445);
            _canvasPanel.TabIndex = 0;
            // 
            // _lblSize
            // 
            _lblSize.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            _lblSize.AutoSize = true;
            _lblSize.Location = new Point(216, 490);
            _lblSize.Name = "_lblSize";
            _lblSize.Size = new Size(88, 15);
            _lblSize.TabIndex = 1;
            _lblSize.Text = "House size (m):";
            // 
            // _numWidth
            // 
            _numWidth.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            _numWidth.Location = new Point(331, 490);
            _numWidth.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
            _numWidth.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            _numWidth.Name = "_numWidth";
            _numWidth.Size = new Size(80, 23);
            _numWidth.TabIndex = 2;
            _numWidth.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // _numHeight
            // 
            _numHeight.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _numHeight.Location = new Point(810, 223);
            _numHeight.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
            _numHeight.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            _numHeight.Name = "_numHeight";
            _numHeight.Size = new Size(80, 23);
            _numHeight.TabIndex = 3;
            _numHeight.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // _btnFloorUp
            // 
            _btnFloorUp.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _btnFloorUp.BackColor = Color.FromArgb(235, 245, 255);
            _btnFloorUp.Cursor = Cursors.Hand;
            _btnFloorUp.FlatAppearance.BorderColor = Color.FromArgb(52, 152, 219);
            _btnFloorUp.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 225, 250);
            _btnFloorUp.FlatStyle = FlatStyle.Flat;
            _btnFloorUp.Font = new Font("Segoe UI", 9F);
            _btnFloorUp.ForeColor = Color.FromArgb(52, 152, 219);
            _btnFloorUp.Location = new Point(810, 96);
            _btnFloorUp.Name = "_btnFloorUp";
            _btnFloorUp.Size = new Size(80, 32);
            _btnFloorUp.TabIndex = 4;
            _btnFloorUp.Text = "▲ Up";
            _btnFloorUp.UseVisualStyleBackColor = false;
            // 
            // _btnFloorDown
            // 
            _btnFloorDown.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _btnFloorDown.BackColor = Color.FromArgb(235, 245, 255);
            _btnFloorDown.Cursor = Cursors.Hand;
            _btnFloorDown.FlatAppearance.BorderColor = Color.FromArgb(52, 152, 219);
            _btnFloorDown.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 225, 250);
            _btnFloorDown.FlatStyle = FlatStyle.Flat;
            _btnFloorDown.Font = new Font("Segoe UI", 9F);
            _btnFloorDown.ForeColor = Color.FromArgb(52, 152, 219);
            _btnFloorDown.Location = new Point(810, 134);
            _btnFloorDown.Name = "_btnFloorDown";
            _btnFloorDown.Size = new Size(80, 32);
            _btnFloorDown.TabIndex = 5;
            _btnFloorDown.Text = "▼ Down";
            _btnFloorDown.UseVisualStyleBackColor = false;
            // 
            // _btnAddFloor
            // 
            _btnAddFloor.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            _btnAddFloor.BackColor = Color.FromArgb(235, 245, 255);
            _btnAddFloor.Cursor = Cursors.Hand;
            _btnAddFloor.FlatAppearance.BorderColor = Color.FromArgb(52, 152, 219);
            _btnAddFloor.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 225, 250);
            _btnAddFloor.FlatStyle = FlatStyle.Flat;
            _btnAddFloor.Font = new Font("Segoe UI", 9F);
            _btnAddFloor.ForeColor = Color.FromArgb(52, 152, 219);
            _btnAddFloor.Location = new Point(306, 529);
            _btnAddFloor.Name = "_btnAddFloor";
            _btnAddFloor.Size = new Size(120, 36);
            _btnAddFloor.TabIndex = 6;
            _btnAddFloor.Text = "+ Add floor";
            _btnAddFloor.UseVisualStyleBackColor = false;
            // 
            // _btnDeleteFloor
            // 
            _btnDeleteFloor.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            _btnDeleteFloor.BackColor = Color.FromArgb(255, 240, 240);
            _btnDeleteFloor.Cursor = Cursors.Hand;
            _btnDeleteFloor.FlatAppearance.BorderColor = Color.FromArgb(231, 76, 60);
            _btnDeleteFloor.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 220, 220);
            _btnDeleteFloor.FlatStyle = FlatStyle.Flat;
            _btnDeleteFloor.Font = new Font("Segoe UI", 9F);
            _btnDeleteFloor.ForeColor = Color.FromArgb(231, 76, 60);
            _btnDeleteFloor.Location = new Point(447, 529);
            _btnDeleteFloor.Name = "_btnDeleteFloor";
            _btnDeleteFloor.Size = new Size(120, 36);
            _btnDeleteFloor.TabIndex = 7;
            _btnDeleteFloor.Text = "Delete floor";
            _btnDeleteFloor.UseVisualStyleBackColor = false;
            // 
            // _btnAddWall
            // 
            _btnAddWall.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            _btnAddWall.BackColor = Color.FromArgb(235, 245, 255);
            _btnAddWall.Cursor = Cursors.Hand;
            _btnAddWall.FlatAppearance.BorderColor = Color.FromArgb(52, 152, 219);
            _btnAddWall.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 225, 250);
            _btnAddWall.FlatStyle = FlatStyle.Flat;
            _btnAddWall.Font = new Font("Segoe UI", 9F);
            _btnAddWall.ForeColor = Color.FromArgb(52, 152, 219);
            _btnAddWall.Location = new Point(36, 529);
            _btnAddWall.Name = "_btnAddWall";
            _btnAddWall.Size = new Size(100, 36);
            _btnAddWall.TabIndex = 8;
            _btnAddWall.Text = "Add wall";
            _btnAddWall.UseVisualStyleBackColor = false;
            // 
            // _btnAddContainer
            // 
            _btnAddContainer.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            _btnAddContainer.BackColor = Color.FromArgb(235, 245, 255);
            _btnAddContainer.Cursor = Cursors.Hand;
            _btnAddContainer.FlatAppearance.BorderColor = Color.FromArgb(52, 152, 219);
            _btnAddContainer.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 225, 250);
            _btnAddContainer.FlatStyle = FlatStyle.Flat;
            _btnAddContainer.Font = new Font("Segoe UI", 9F);
            _btnAddContainer.ForeColor = Color.FromArgb(52, 152, 219);
            _btnAddContainer.Location = new Point(156, 529);
            _btnAddContainer.Name = "_btnAddContainer";
            _btnAddContainer.Size = new Size(130, 36);
            _btnAddContainer.TabIndex = 9;
            _btnAddContainer.Text = "Add container";
            _btnAddContainer.UseVisualStyleBackColor = false;
            // 
            // _btnSave
            // 
            _btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            _btnSave.BackColor = Color.FromArgb(232, 245, 233);
            _btnSave.Cursor = Cursors.Hand;
            _btnSave.FlatAppearance.BorderColor = Color.FromArgb(39, 174, 96);
            _btnSave.FlatAppearance.BorderSize = 2;
            _btnSave.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 235, 210);
            _btnSave.FlatStyle = FlatStyle.Flat;
            _btnSave.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            _btnSave.ForeColor = Color.FromArgb(39, 174, 96);
            _btnSave.Location = new Point(703, 528);
            _btnSave.Name = "_btnSave";
            _btnSave.Size = new Size(110, 42);
            _btnSave.TabIndex = 11;
            _btnSave.Text = "Save";
            _btnSave.UseVisualStyleBackColor = false;
            // 
            // _txtFloorName
            // 
            _txtFloorName.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _txtFloorName.Location = new Point(805, 38);
            _txtFloorName.Name = "_txtFloorName";
            _txtFloorName.PlaceholderText = "Floor name";
            _txtFloorName.Size = new Size(99, 23);
            _txtFloorName.TabIndex = 13;
            // 
            // _rightPanel
            // 
            _rightPanel.AutoScroll = true;
            _rightPanel.BackColor = Color.White;
            _rightPanel.BorderStyle = BorderStyle.FixedSingle;
            _rightPanel.Controls.Add(_lblAddingItem);
            _rightPanel.Controls.Add(_txtItemName);
            _rightPanel.Controls.Add(_numItemQty);
            _rightPanel.Controls.Add(_cmbItemUnit);
            _rightPanel.Controls.Add(_btnAddItem);
            _rightPanel.Controls.Add(_btnDeleteItems);
            _rightPanel.Controls.Add(_lvItems);
            _rightPanel.Controls.Add(_lblContainerSettings);
            _rightPanel.Controls.Add(_txtContainerName);
            _rightPanel.Controls.Add(_txtRoomTag);
            _rightPanel.Controls.Add(_lblPoints);
            _rightPanel.Controls.Add(_lblPointA);
            _rightPanel.Controls.Add(_numPointAX);
            _rightPanel.Controls.Add(_numPointAY);
            _rightPanel.Controls.Add(_lblPointB);
            _rightPanel.Controls.Add(_numPointBX);
            _rightPanel.Controls.Add(_numPointBY);
            _rightPanel.Controls.Add(_btnAddSubcontainer);
            _rightPanel.Controls.Add(_btnDeleteContainer);
            _rightPanel.Controls.Add(_lblSubsections);
            _rightPanel.Controls.Add(_lstContainers);
            _rightPanel.Dock = DockStyle.Right;
            _rightPanel.Location = new Point(955, 10);
            _rightPanel.Name = "_rightPanel";
            _rightPanel.Size = new Size(315, 590);
            _rightPanel.TabIndex = 13;
            // 
            // _lblAddingItem
            // 
            _lblAddingItem.AutoSize = true;
            _lblAddingItem.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            _lblAddingItem.Location = new Point(12, 449);
            _lblAddingItem.Name = "_lblAddingItem";
            _lblAddingItem.Size = new Size(92, 19);
            _lblAddingItem.TabIndex = 0;
            _lblAddingItem.Text = "Adding item";
            // 
            // _txtItemName
            // 
            _txtItemName.Location = new Point(12, 474);
            _txtItemName.Name = "_txtItemName";
            _txtItemName.PlaceholderText = "Item name";
            _txtItemName.Size = new Size(273, 23);
            _txtItemName.TabIndex = 1;
            // 
            // _numItemQty
            // 
            _numItemQty.DecimalPlaces = 1;
            _numItemQty.Location = new Point(12, 514);
            _numItemQty.Maximum = new decimal(new int[] { 999, 0, 0, 0 });
            _numItemQty.Name = "_numItemQty";
            _numItemQty.Size = new Size(273, 23);
            _numItemQty.TabIndex = 2;
            _numItemQty.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // _cmbItemUnit
            // 
            _cmbItemUnit.Items.AddRange(new object[] { "pcs", "kg", "l" });
            _cmbItemUnit.Location = new Point(12, 552);
            _cmbItemUnit.Name = "_cmbItemUnit";
            _cmbItemUnit.Size = new Size(273, 23);
            _cmbItemUnit.TabIndex = 3;
            // 
            // _btnAddItem
            // 
            _btnAddItem.Location = new Point(12, 590);
            _btnAddItem.Name = "_btnAddItem";
            _btnAddItem.Size = new Size(273, 32);
            _btnAddItem.TabIndex = 4;
            _btnAddItem.Text = "Add item";
            // 
            // _btnDeleteItems
            // 
            _btnDeleteItems.BackColor = Color.White;
            _btnDeleteItems.FlatStyle = FlatStyle.Flat;
            _btnDeleteItems.Location = new Point(12, 768);
            _btnDeleteItems.Name = "_btnDeleteItems";
            _btnDeleteItems.Size = new Size(273, 32);
            _btnDeleteItems.TabIndex = 5;
            _btnDeleteItems.Text = "Delete items";
            _btnDeleteItems.UseVisualStyleBackColor = false;
            // 
            // _lvItems
            // 
            _lvItems.FullRowSelect = true;
            _lvItems.GridLines = true;
            _lvItems.Location = new Point(12, 642);
            _lvItems.Name = "_lvItems";
            _lvItems.Size = new Size(273, 120);
            _lvItems.TabIndex = 6;
            _lvItems.UseCompatibleStateImageBehavior = false;
            _lvItems.View = View.Details;
            // 
            // _lblContainerSettings
            // 
            _lblContainerSettings.AutoSize = true;
            _lblContainerSettings.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            _lblContainerSettings.Location = new Point(12, 12);
            _lblContainerSettings.Name = "_lblContainerSettings";
            _lblContainerSettings.Size = new Size(129, 19);
            _lblContainerSettings.TabIndex = 7;
            _lblContainerSettings.Text = "Container settings";
            // 
            // _txtContainerName
            // 
            _txtContainerName.Location = new Point(12, 37);
            _txtContainerName.Name = "_txtContainerName";
            _txtContainerName.PlaceholderText = "Name";
            _txtContainerName.Size = new Size(273, 23);
            _txtContainerName.TabIndex = 8;
            // 
            // _txtRoomTag
            // 
            _txtRoomTag.Location = new Point(12, 75);
            _txtRoomTag.Name = "_txtRoomTag";
            _txtRoomTag.PlaceholderText = "Room";
            _txtRoomTag.Size = new Size(273, 23);
            _txtRoomTag.TabIndex = 9;
            // 
            // _lblPoints
            // 
            _lblPoints.AutoSize = true;
            _lblPoints.Location = new Point(12, 111);
            _lblPoints.Name = "_lblPoints";
            _lblPoints.Size = new Size(43, 15);
            _lblPoints.TabIndex = 12;
            _lblPoints.Text = "Points:";
            // 
            // _lblPointA
            // 
            _lblPointA.AutoSize = true;
            _lblPointA.Location = new Point(12, 136);
            _lblPointA.Name = "_lblPointA";
            _lblPointA.Size = new Size(18, 15);
            _lblPointA.TabIndex = 13;
            _lblPointA.Text = "A:";
            // 
            // _numPointAX
            // 
            _numPointAX.DecimalPlaces = 3;
            _numPointAX.Location = new Point(40, 134);
            _numPointAX.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            _numPointAX.Name = "_numPointAX";
            _numPointAX.Size = new Size(100, 23);
            _numPointAX.TabIndex = 14;
            // 
            // _numPointAY
            // 
            _numPointAY.DecimalPlaces = 3;
            _numPointAY.Location = new Point(150, 134);
            _numPointAY.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            _numPointAY.Name = "_numPointAY";
            _numPointAY.Size = new Size(100, 23);
            _numPointAY.TabIndex = 15;
            // 
            // _lblPointB
            // 
            _lblPointB.AutoSize = true;
            _lblPointB.Location = new Point(12, 171);
            _lblPointB.Name = "_lblPointB";
            _lblPointB.Size = new Size(17, 15);
            _lblPointB.TabIndex = 16;
            _lblPointB.Text = "B:";
            // 
            // _numPointBX
            // 
            _numPointBX.DecimalPlaces = 3;
            _numPointBX.Location = new Point(40, 169);
            _numPointBX.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            _numPointBX.Name = "_numPointBX";
            _numPointBX.Size = new Size(100, 23);
            _numPointBX.TabIndex = 17;
            // 
            // _numPointBY
            // 
            _numPointBY.DecimalPlaces = 3;
            _numPointBY.Location = new Point(150, 169);
            _numPointBY.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            _numPointBY.Name = "_numPointBY";
            _numPointBY.Size = new Size(100, 23);
            _numPointBY.TabIndex = 18;
            // 
            // _btnAddSubcontainer
            // 
            _btnAddSubcontainer.Location = new Point(12, 206);
            _btnAddSubcontainer.Name = "_btnAddSubcontainer";
            _btnAddSubcontainer.Size = new Size(273, 32);
            _btnAddSubcontainer.TabIndex = 23;
            _btnAddSubcontainer.Text = "Add subcontainer";
            // 
            // _btnDeleteContainer
            // 
            _btnDeleteContainer.BackColor = Color.FromArgb(220, 53, 69);
            _btnDeleteContainer.FlatStyle = FlatStyle.Flat;
            _btnDeleteContainer.ForeColor = Color.White;
            _btnDeleteContainer.Location = new Point(12, 246);
            _btnDeleteContainer.Name = "_btnDeleteContainer";
            _btnDeleteContainer.Size = new Size(273, 32);
            _btnDeleteContainer.TabIndex = 24;
            _btnDeleteContainer.Text = "Delete container";
            _btnDeleteContainer.UseVisualStyleBackColor = false;
            // 
            // _lblSubsections
            // 
            _lblSubsections.AutoSize = true;
            _lblSubsections.Location = new Point(12, 290);
            _lblSubsections.Name = "_lblSubsections";
            _lblSubsections.Size = new Size(85, 15);
            _lblSubsections.TabIndex = 25;
            _lblSubsections.Text = "Subcontainers:";
            // 
            // _lstContainers
            // 
            _lstContainers.ItemHeight = 15;
            _lstContainers.Location = new Point(12, 315);
            _lstContainers.Name = "_lstContainers";
            _lstContainers.Size = new Size(273, 94);
            _lstContainers.TabIndex = 27;
            // 
            // HomeBrowserDesignView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(_rightPanel);
            Controls.Add(_txtFloorName);
            Controls.Add(_btnSave);
            Controls.Add(_btnAddContainer);
            Controls.Add(_btnAddWall);
            Controls.Add(_btnDeleteFloor);
            Controls.Add(_btnAddFloor);
            Controls.Add(_btnFloorUp);
            Controls.Add(_btnFloorDown);
            Controls.Add(_numHeight);
            Controls.Add(_numWidth);
            Controls.Add(_lblSize);
            Controls.Add(_canvasPanel);
            Name = "HomeBrowserDesignView";
            Padding = new Padding(10);
            Size = new Size(1280, 610);
            ((System.ComponentModel.ISupportInitialize)_numWidth).EndInit();
            ((System.ComponentModel.ISupportInitialize)_numHeight).EndInit();
            _rightPanel.ResumeLayout(false);
            _rightPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)_numItemQty).EndInit();
            ((System.ComponentModel.ISupportInitialize)_numPointAX).EndInit();
            ((System.ComponentModel.ISupportInitialize)_numPointAY).EndInit();
            ((System.ComponentModel.ISupportInitialize)_numPointBX).EndInit();
            ((System.ComponentModel.ISupportInitialize)_numPointBY).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}

