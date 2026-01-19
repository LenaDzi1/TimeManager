// Import przestrzeni nazw
using System;                   // Podstawowe typy .NET
using System.Collections.Generic; // Kolekcje generyczne
using System.Windows.Forms;     // Windows Forms
using TimeManager.Models;       // Modele aplikacji

namespace TimeManager.Forms
{
    /// <summary>
    /// Formularz wyświetlający szybkie zadania (P1 - niski priorytet)
    /// w danym zakresie dat. Wywoływany z CalendarView.
    /// </summary>
    public partial class QuickTasksForm : Form
    {
        // Lista eventów do wyświetlenia
        private readonly List<Event> _events;
        // Data początkowa zakresu
        private readonly DateTime _from;
        // Data końcowa zakresu
        private readonly DateTime _to;

        /// <summary>
        /// Konstruktor formularza szybkich zadań.
        /// </summary>
        public QuickTasksForm(List<Event> events, DateTime from, DateTime to)
        {
            _events = events ?? new List<Event>();
            _from = from;
            _to = to;

            InitializeComponent();

            SetupListView();
        }

        /// <summary>
        /// Konfiguruje ListView i wypełnia danymi.
        /// </summary>
        private void SetupListView()
        {
            _lblTitle.Text = $"Low priority tasks (P1) in view range: {_from:yyyy-MM-dd} → {_to:yyyy-MM-dd}";

            _lstTasks.Columns.Add("Task", 340);
            _lstTasks.Columns.Add("Duration (min)", 120);

            foreach (var ev in _events)
            {
                var item = new ListViewItem(ev.Title ?? "(no title)");
                item.SubItems.Add(ev.Duration.ToString());
                _lstTasks.Items.Add(item);
            }
        }
    }
}

