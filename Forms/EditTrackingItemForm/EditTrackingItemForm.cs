#nullable enable
using System;
using System.Windows.Forms;

namespace TimeManager.Forms
{
    public partial class EditTrackingItemForm : Form
    {
        public enum TrackingType { Food, Medicine }

        private readonly Action<decimal, string, DateTime?> _onSave;
        private readonly TrackingType _trackingType;
        
        private readonly string _itemName;
        private readonly decimal _initialQuantity;
        private readonly string _initialUnit;
        private readonly DateTime? _initialExpireDate;

        public EditTrackingItemForm(
            string itemName,
            decimal quantity,
            string unit,
            DateTime? expireDate,
            Action<decimal, string, DateTime?> onSave,
            TrackingType trackingType = TrackingType.Medicine)
        {
            _itemName = itemName;
            _initialQuantity = quantity;
            _initialUnit = unit;
            _initialExpireDate = expireDate;
            _onSave = onSave;
            _trackingType = trackingType;

            InitializeComponent();

            Text = $"Edit {_itemName}";
            if (_lblProductName != null)
                _lblProductName.Text = $"Product: {_itemName}";

            _cmbUnit.Items.Clear();
            if (_trackingType == TrackingType.Food)
            {
                _cmbUnit.Items.AddRange(new object[] { "pcs", "ml", "g", "kg", "l" });
            }
            else
            {
                _cmbUnit.Items.AddRange(new object[] { "pcs", "ml", "g", "tablets", "capsules" });
            }
            
            WireUpEvents();
            LoadItemData();
        }

        private void WireUpEvents()
        {
            _btnOk.Click += (s, e) => OnOk();
            _chkHasExpire.CheckedChanged += OnExpireCheckedChanged;
        }

        private void LoadItemData()
        {
            _numAmount.Value = _initialQuantity;

            _chkHasExpire.CheckedChanged -= OnExpireCheckedChanged;
            _chkHasExpire.Checked = _initialExpireDate.HasValue;
            _chkHasExpire.CheckedChanged += OnExpireCheckedChanged;
            
            _dtpExpire.Enabled = _chkHasExpire.Checked;

            if (_cmbUnit.Items.Count > 0)
            {
                int unitIndex = _cmbUnit.Items.IndexOf(_initialUnit);
                if (unitIndex >= 0)
                {
                    _cmbUnit.SelectedIndex = unitIndex;
                }
                else
                {
                    _cmbUnit.SelectedIndex = 0;
                }
            }

            if (_initialExpireDate.HasValue)
            {
                _dtpExpire.Value = _initialExpireDate.Value;
            }
            else
            {
                _dtpExpire.Value = DateTime.Today.AddMonths(12);
            }
        }

        private void OnOk()
        {
            DateTime? expiration = _chkHasExpire.Checked ? _dtpExpire.Value.Date : (DateTime?)null;
            string unit = _cmbUnit.SelectedItem as string ?? _initialUnit;
            
            _onSave(_numAmount.Value, unit, expiration);
        }

        private void OnExpireCheckedChanged(object? sender, EventArgs e)
        {
            _dtpExpire.Enabled = _chkHasExpire.Checked;
        }
    }
}
