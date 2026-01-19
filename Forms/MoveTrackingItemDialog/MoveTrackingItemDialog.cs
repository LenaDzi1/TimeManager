#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TimeManager.Forms
{
    public partial class MoveTrackingItemDialog : Form
    {
        public class ContainerDisplayModel
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public override string ToString() => Name;
        }

        private readonly string _itemName;
        private readonly decimal _maxQuantity;
        private readonly string _unit;
        private readonly List<ContainerDisplayModel> _containers;
        private readonly int _currentContainerId;

        public ContainerDisplayModel? TargetContainer => _cmbTargetContainer.SelectedItem as ContainerDisplayModel;
        public decimal AmountToMove => _numAmount.Value;

        public MoveTrackingItemDialog(
            string itemName,
            decimal maxQuantity,
            string unit,
            IEnumerable<ContainerDisplayModel> containers,
            int currentContainerId)
        {
            _itemName = itemName;
            _maxQuantity = maxQuantity;
            _unit = unit;
            _containers = containers.ToList();
            _currentContainerId = currentContainerId;

            InitializeComponent();
            
            _btnOk.Click += BtnOk_Click;
            
            Text = $"Move {_itemName}";
            
            if (_numAmount != null)
            {
                _numAmount.Maximum = _maxQuantity;
                _numAmount.Value = _maxQuantity;
            }
            
            if (_lblUnit != null)
                _lblUnit.Text = _unit;

            LoadContainers();
        }

        private void LoadContainers()
        {
            _cmbTargetContainer.DataSource = _containers;
            _cmbTargetContainer.DisplayMember = "Name";
            _cmbTargetContainer.ValueMember = "Id";

            if (_currentContainerId != 0)
            {
                var current = _containers.FirstOrDefault(c => c.Id == _currentContainerId);
                if (current != null)
                    _cmbTargetContainer.SelectedItem = current;
            }
        }

        private void BtnOk_Click(object? sender, EventArgs e)
        {
            if (AmountToMove <= 0)
            {
                MessageBox.Show("Amount to move must be greater than zero.",
                    "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
                return;
            }

            if (TargetContainer == null)
            {
                MessageBox.Show("Please select target container.",
                    "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
            }
        }
    }
}
