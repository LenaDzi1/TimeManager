using System.Windows.Forms;

namespace TimeManager.Forms
{
    public partial class AddContainerForm : Form
    {
        public string InputValue => _inputBox.Text;

        public AddContainerForm(string title, string prompt, string defaultValue = "")
        {
            InitializeComponent();
            this.Text = title; 
            _lblPrompt.Text = prompt;
            _inputBox.Text = defaultValue;
            
            this.Load += (s, e) => 
            {
                _inputBox.SelectAll();
                _inputBox.Focus();
            };
        }

        public static string Show(string title, string prompt, string defaultValue = "")
        {
            using var form = new AddContainerForm(title, prompt, defaultValue);
            var result = form.ShowDialog();
            return result == DialogResult.OK ? form.InputValue : null;
        }
    }
}
