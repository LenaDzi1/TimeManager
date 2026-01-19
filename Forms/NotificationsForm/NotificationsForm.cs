// Import przestrzeni nazw
using System;                   // Podstawowe typy .NET
using System.Drawing;           // Grafika i kolory
using System.Windows.Forms;     // Windows Forms
using TimeManager.Models;       // Modele aplikacji
using TimeManager.Services;     // Serwisy aplikacji

namespace TimeManager.Forms.Notifications
{
    /// <summary>
    /// Formularz listy powiadomień.
    /// Wyświetla karty powiadomień z możliwością usunięcia i nawigacji.
    /// </summary>
    public partial class NotificationsForm : Form
    {
        // Serwis powiadomień
        private readonly NotificationService _notificationService;
        // Callback do nawigacji po kliknięciu powiadomienia
        private readonly Action<Notification> _onNavigate;

        /// <summary>
        /// Konstruktor formularza powiadomień.
        /// </summary>
        public NotificationsForm(NotificationService notificationService, Action<Notification> onNavigate)
        {
            _notificationService = notificationService ?? new NotificationService();
            _onNavigate = onNavigate;
            InitializeComponent();
        }

        /// <summary>
        /// Load - sprawdza i tworzy powiadomienia trackingowe.
        /// </summary>
        private void NotificationsForm_Load(object sender, EventArgs e)
        {
            _notificationService.CheckAndCreateTrackingNotifications();
            LoadNotifications();
        }

        private void LoadNotifications()
        {
            _notificationsFlow.SuspendLayout();
            _notificationsFlow.Controls.Clear();

            var notifications = _notificationService.GetAllNotifications();
            foreach (var notification in notifications)
            {
                _notificationsFlow.Controls.Add(CreateNotificationCard(notification));
            }

            _notificationsFlow.ResumeLayout();
            ResizeCards();
        }

        private Panel CreateNotificationCard(Notification notification)
        {
            var (header, fallbackMessage) = GetDisplayContent(notification);

            var card = new Panel
            {
                Width = CalculateCardWidth(),
                Height = 65,    /// tutaj zmieniliśmy wysokość
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(6),
                Tag = notification
            };

            card.Cursor = Cursors.Hand;
            card.Click += (_, __) => HandleNavigate(notification);

            var headerLabel = new Label
            {
                Text = $"{header}:",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point),
                AutoSize = true,
                Location = new Point(12, 12)
            };
            headerLabel.Click += (_, __) => HandleNavigate(notification);

            var messageLabel = new Label
            {
                Text = string.IsNullOrWhiteSpace(notification.Message) ? fallbackMessage : notification.Message,
                Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point),
                AutoSize = false,
                Location = new Point(12, 32),
                Size = new Size(430, 35)
            };
            messageLabel.Click += (_, __) => HandleNavigate(notification);

            var dateLabel = new Label
            {
                Text = notification.ScheduledDateTime.ToString("yyyy-MM-dd HH:mm"),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point),
                AutoSize = true
            };
            dateLabel.Click += (_, __) => HandleNavigate(notification);

            var deleteButton = new Button
            {
                Text = "X",
                BackColor = Color.Transparent,
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point),
                Size = new Size(32, 32),
                TabStop = false,
                Cursor = Cursors.Hand
            };
            deleteButton.FlatAppearance.BorderSize = 0;
            deleteButton.Click += (_, __) => DeleteNotification(notification, card);

            card.Controls.Add(headerLabel);
            card.Controls.Add(messageLabel);
            card.Controls.Add(deleteButton);
            card.Controls.Add(dateLabel);

            LayoutCard(card, messageLabel, deleteButton, dateLabel);
            card.Resize += (_, __) => LayoutCard(card, messageLabel, deleteButton, dateLabel);

            return card;
        }

        private void LayoutCard(Control card, Label messageLabel, Button deleteButton, Label dateLabel)
        {
            var width = card.ClientSize.Width;

            deleteButton.Location = new Point(width - deleteButton.Width - 10, 8);

            var dateX = width - dateLabel.PreferredWidth - deleteButton.Width - 20;
            dateLabel.Location = new Point(Math.Max(12, dateX), 14);
            
            var availableTextWidth = width - 58;
            messageLabel.Size = new Size(Math.Max(220, availableTextWidth), 35);
        }

        private void DeleteNotification(Notification notification, Control card)
        {
            _notificationService.DeleteNotification(notification.NotificationID);
            _notificationsFlow.Controls.Remove(card);
            card.Dispose();
        }

        private void HandleNavigate(Notification notification)
        {
            _notificationService.MarkAsRead(notification.NotificationID);
            _onNavigate?.Invoke(notification);
            Close();
        }

        private (string header, string fallbackMessage) GetDisplayContent(Notification notification)
        {
            var type = (notification.NotificationType ?? string.Empty).ToLowerInvariant();
            return type switch
            {
                "foodtracking" or "food" or "fridge" => ("Food tracker", "There are missing items in your kitchen."),
                "foodexpiry" => ("Food tracker", "A product is expiring soon."),
                "medicinetracking" or "medicine" or "medication" => ("Medicine tracker", $"The medicine \"{notification.Title}\" is ending soon."),
                "planttracker" or "plant" => ("Plant tracker", $"{notification.Title} needs watering."),
                _ => ("Notification", "You have a new notification.")
            };
        }

        private void NotificationsFlow_SizeChanged(object sender, EventArgs e)
        {
            ResizeCards();
        }

        private void ResizeCards()
        {
            var width = CalculateCardWidth();
            foreach (Control control in _notificationsFlow.Controls)
            {
                control.Width = width;
            }
        }

        private int CalculateCardWidth()
        {
            return Math.Max(320, _notificationsFlow.ClientSize.Width - 16);
        }
    }
}

