#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TimeManager.Forms
{
    public partial class AddTrackingItemForm : Form
    {
        public enum TrackingType { Food, Medicine }

        // Delegaty do abstrakcji
        private readonly Func<IEnumerable<ProductDisplayModel>> _getProducts;
        private readonly Func<string, string, int> _addProduct;
        private readonly Func<int, bool> _deleteProduct;
        private readonly Action<ProductDisplayModel, decimal, string, DateTime?> _onSubmit;
        private readonly TrackingType _trackingType;

        private readonly string _containerName;

        public class ProductDisplayModel
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string? DefaultUnit { get; set; }

            public override string ToString() => Name;
        }

        public AddTrackingItemForm(
            string containerName,
            Func<IEnumerable<ProductDisplayModel>> getProducts,
            Func<string, string, int> addProduct,
            Func<int, bool> deleteProduct,
            Action<ProductDisplayModel, decimal, string, DateTime?> onSubmit,
            TrackingType trackingType = TrackingType.Medicine)
        {
            _containerName = containerName;
            _getProducts = getProducts;
            _addProduct = addProduct;
            _deleteProduct = deleteProduct;
            _onSubmit = onSubmit;
            _trackingType = trackingType;

            InitializeComponent();

            Text = $"Add item to {_containerName}";

            // Skonfiguruj jednostki na podstawie typu śledzenia
            _cmbUnit.Items.Clear();
            if (_trackingType == TrackingType.Food)
            {
                _cmbUnit.Items.AddRange(new object[] { "pcs", "ml", "g", "kg", "l" });
            }
            else // Medicine
            {
                _cmbUnit.Items.AddRange(new object[] { "pcs", "ml", "g", "tablets", "capsules" });
            }

            if (_dtpExpire != null)
            {
                _dtpExpire.Value = _trackingType == TrackingType.Food
                    ? DateTime.Today.AddDays(7)
                    : DateTime.Today.AddMonths(12);
            }

            if (_cmbUnit != null && _cmbUnit.Items.Count > 0)
                _cmbUnit.SelectedIndex = 0;

            WireUpEvents();
            LoadProducts();
        }

        private void WireUpEvents()
        {
            _btnNewProduct.Click += (s, e) => OnNewProduct();
            _btnDeleteProduct.Click += (s, e) => OnDeleteProduct();
            _btnOk.Click += (s, e) => OnOk();
            _chkHasExpire.CheckedChanged += OnExpireCheckedChanged;
        }

        private void LoadProducts()
        {
            var products = _getProducts()?.ToList() ?? new List<ProductDisplayModel>();
            _cmbProduct.DataSource = products;
            _cmbProduct.DisplayMember = "Name";
            _cmbProduct.ValueMember = "Id";
            _cmbProduct.SelectedIndexChanged -= OnProductChanged;
            _cmbProduct.SelectedIndexChanged += OnProductChanged;
        }

        private void OnProductChanged(object? sender, EventArgs e)
        {
            if (_cmbProduct.SelectedItem is ProductDisplayModel product && !string.IsNullOrEmpty(product.DefaultUnit))
            {
                var unitIndex = _cmbUnit.Items.IndexOf(product.DefaultUnit);
                if (unitIndex >= 0)
                {
                    _cmbUnit.SelectedIndex = unitIndex;
                }
            }
        }

        private void OnNewProduct()
        {
            string name = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter new product name:", "New product", "");

            if (string.IsNullOrWhiteSpace(name))
                return;

            string unit = _cmbUnit.SelectedItem as string ?? "";

            int id = _addProduct(name, unit);

            // Przeładuj listę i wybierz nowy produkt
            LoadProducts();
            var products = _cmbProduct.DataSource as IList;
            if (products != null)
            {
                for (int i = 0; i < products.Count; i++)
                {
                    if (products[i] is ProductDisplayModel mp && mp.Id == id)
                    {
                        _cmbProduct.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        private void OnDeleteProduct()
        {
            if (_cmbProduct.Items.Count == 0)
                return;

            if (_cmbProduct.SelectedItem is not ProductDisplayModel product)
                return;

            bool removed = _deleteProduct(product.Id);

            if (!removed)
            {
                MessageBox.Show(
                    "Cannot delete product because it is used by items in containers.",
                    "Cannot delete",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            LoadProducts();
        }

        private void OnOk()
        {
            if (_cmbProduct.SelectedItem is not ProductDisplayModel product)
            {
                MessageBox.Show("Please select a product.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            if (_numAmount.Value <= 0)
            {
                MessageBox.Show("Amount must be greater than zero.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            string unit = _cmbUnit.SelectedItem as string ?? "";
            DateTime? expireDate = _chkHasExpire.Checked ? _dtpExpire.Value.Date : (DateTime?)null;

            _onSubmit(product, _numAmount.Value, unit, expireDate);
        }

        private void OnExpireCheckedChanged(object? sender, EventArgs e)
        {
            _dtpExpire.Enabled = _chkHasExpire.Checked;
        }

    }
}
