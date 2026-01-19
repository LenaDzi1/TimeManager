// ============================================================================
// StatisticsView.cs
// Widok statystyk użytkownika - wykres radarowy, punkty, nagrody.
// ============================================================================

#region Importy
using System;                   // Podstawowe typy .NET (Math, EventArgs)
using System.Collections.Generic; // Kolekcje generyczne (List, Dictionary)
using System.ComponentModel;    // LicenseManager do wykrycia design mode
using System.Drawing;           // Grafika (Color, Point, Graphics)
using System.Drawing.Drawing2D; // Zaawansowana grafika (SmoothingMode)
using System.Linq;              // LINQ (OrderBy, Max, Where)
using System.Windows.Forms;     // Windows Forms (UserControl, ListView)
using TimeManager.Models;       // Modele (Reward, UserSession)
using TimeManager.Services;     // Serwisy (PointsService, UserService)
#endregion

namespace TimeManager.Forms
{
    /// <summary>
    /// Widok statystyk użytkownika - wyświetla punkty, nagrody i wykres radarowy.
    /// 
    /// GŁÓWNE FUNKCJE:
    /// - Wykres radarowy (radar chart) pokazujący punkty w 8 kategoriach
    /// - Wyświetlanie sumy punktów, najlepszej i najsłabszej kategorii
    /// - System nagród (Rewards) - dodawanie, edycja, wymiana za punkty
    /// - Czyszczenie statystyk z ostatnich 30 dni
    /// 
    /// KATEGORIE (8):
    /// zdrowie, rodzina, mentalność, finanse, praca i kariera,
    /// relaks, rozwój osobisty i edukacja, przyjaciele i ludzie
    /// 
    /// OGRANICZENIA RÓL:
    /// - Kids: mogą wymieniać nagrody, ale nie mogą ich dodawać/edytować
    /// </summary>
    public partial class StatisticsView : UserControl
    {
        #region Pola prywatne - Serwisy

        /// <summary>Serwis zarządzania punktami i nagrodami.</summary>
        private PointsService _pointsService;

        /// <summary>Serwis użytkowników (do funkcji View As).</summary>
        private readonly UserService _userService = new();

        #endregion

        #region Pola prywatne - UI

        /// <summary>Tooltip wyświetlany przy najechaniu na punkt wykresu.</summary>
        private ToolTip _chartToolTip;

        /// <summary>
        /// Lista punktów danych na wykresie (pozycja, kategoria, punkty).
        /// Używana do wykrywania hover i wyświetlania tooltip.
        /// </summary>
        private List<(PointF Point, string Category, int Points)> _chartDataPoints = new();

        /// <summary>Ostatni tekst tooltip (do unikania migotania).</summary>
        private string _lastTooltipText = "";

        #endregion

        #region Pola prywatne - Konfiguracja

        /// <summary>
        /// Sprawdzenie czy jesteśmy w trybie designera Visual Studio.
        /// Blokuje inicjalizację serwisów która wymaga bazy danych.
        /// </summary>
        private bool IsDesignMode =>
            LicenseManager.UsageMode == LicenseUsageMode.Designtime ||
            (Site?.DesignMode ?? false) ||
            DesignMode ||
            string.Equals(
                System.Diagnostics.Process.GetCurrentProcess().ProcessName,
                "devenv",
                StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Mapowanie kategorii na kolory (spójne z Event.GetDefaultColorForCategory).
        /// Każda kategoria ma przypisany unikalny kolor.
        /// </summary>
        private readonly Dictionary<string, Color> _categoryColors = new Dictionary<string, Color>
        {
            { "health", ColorTranslator.FromHtml("#9DE284") },                      // Zielony
            { "family", ColorTranslator.FromHtml("#ECAA54") },                       // Pomarańczowy
            { "mentality", ColorTranslator.FromHtml("#94E5F3") },                    // Jasnoniebieski
            { "finance", ColorTranslator.FromHtml("#839EF6") },                      // Niebieski
            { "work and career", ColorTranslator.FromHtml("#FF7676") },              // Czerwony
            { "relax", ColorTranslator.FromHtml("#FFE374") },                        // Żółty
            { "self development and education", ColorTranslator.FromHtml("#C898DB") }, // Fioletowy
            { "friends and people", ColorTranslator.FromHtml("#EA6591") }            // Różowy
        };

        /// <summary>Lista nazw wszystkich 8 kategorii.</summary>
        private readonly string[] _categories = new[]
        {
            "health", "family", "mentality", "finance",
            "work and career", "relax", "self development and education", "friends and people"
        };

        #endregion

        #region Konstruktor

        /// <summary>
        /// Konstruktor widoku statystyk.
        /// Inicjalizuje serwisy, ładuje dane i stosuje ograniczenia ról.
        /// </summary>
        public StatisticsView()
        {
            // Inicjalizuj kontrolki z Designer.cs
            InitializeComponent();

            // W trybie designera nie inicjalizuj serwisów (wymagają bazy)
            if (IsDesignMode) return;

            // Inicjalizuj serwis punktów
            _pointsService = new PointsService();

            // Podepnij handlery zdarzeń
            WireUpEvents();

            // Załaduj dane
            LoadStatistics();
            LoadRewards();

            // Zastosuj ograniczenia oparte na roli użytkownika
            ApplyRoleRestrictions();
        }

        #endregion

        #region Inicjalizacja

        /// <summary>
        /// Stosuje ograniczenia UI oparte na roli użytkownika.
        /// Kids mogą wymieniać nagrody, ale nie mogą ich dodawać.
        /// </summary>
        private void ApplyRoleRestrictions()
        {
            // Kids nie mogą dodawać nagród
            if (UserSession.IsKid && _btnAddReward != null)
            {
                _btnAddReward.Visible = false;
            }
        }

        /// <summary>
        /// Podpina handlery zdarzeń do kontrolek.
        /// </summary>
        private void WireUpEvents()
        {
            // === PRZYCISKI AKCJI ===
            _btnAddReward.Click += (_, __) => OnAddReward();
            _btnRedeemReward.Click += (_, __) => OnRedeemReward();
            _btnHistory.Click += (_, __) => OnHistoryClick();
            _btnClearStatistics.Click += (_, __) => OnClearStatistics();

            // === RYSOWANIE KOLOROWYCH RAMEK ETYKIET ===

            // Ramka dla "Total Points" - pomarańczowa
            _lblTotalPoints.Paint += (s, e) => {
                using var pen = new Pen(Color.FromArgb(243, 156, 18), 2);
                e.Graphics.DrawRectangle(pen, 0, 0, _lblTotalPoints.Width - 1, _lblTotalPoints.Height - 1);
            };

            // Ramka dla "Best Category" - kolor zależny od kategorii
            _lblBestCategory.Paint += (s, e) => {
                var borderColor = _lblBestCategory.Tag is Color c ? c : Color.FromArgb(39, 174, 96);
                using var pen = new Pen(borderColor, 2);
                e.Graphics.DrawRectangle(pen, 0, 0, _lblBestCategory.Width - 1, _lblBestCategory.Height - 1);
            };

            // Ramka dla "Worst Category" - kolor zależny od kategorii
            _lblWorstCategory.Paint += (s, e) => {
                var borderColor = _lblWorstCategory.Tag is Color c ? c : Color.FromArgb(52, 152, 219);
                using var pen = new Pen(borderColor, 2);
                e.Graphics.DrawRectangle(pen, 0, 0, _lblWorstCategory.Width - 1, _lblWorstCategory.Height - 1);
            };

            // === LISTA NAGRÓD ===
            // Pojedyncze kliknięcie otwiera edycję
            _rewardsListView.Activation = ItemActivation.OneClick;
            _rewardsListView.ItemActivate += (_, __) => OnRewardDoubleClick();
            _rewardsListView.DoubleClick += (_, __) => OnRewardDoubleClick();

            // === WYKRES RADAROWY ===
            _radarChartPanel.MouseMove += RadarChartPanel_MouseMove;

            // Inicjalizuj tooltip dla wykresu
            _chartToolTip = new ToolTip();
            _chartToolTip.InitialDelay = 100;
            _chartToolTip.ReshowDelay = 100;

            // Responsywny layout dla etykiet punktów
            pointsPanel.Resize += (_, __) => RelayoutPointsLabels();
        }

        /// <summary>
        /// Przelicza rozmiary i pozycje etykiet punktów przy zmianie rozmiaru panelu.
        /// Zapewnia równomierny rozkład 3 etykiet w jednym wierszu.
        /// </summary>
        private void RelayoutPointsLabels()
        {
            if (pointsPanel == null) return;

            const int margin = 15;
            const int spacing = 10;
            const int labelHeight = 55;

            // Oblicz szerokość etykiety (1/3 dostępnej szerokości)
            var availableWidth = pointsPanel.ClientSize.Width - (margin * 2);
            var labelWidth = Math.Max(160, (availableWidth - (spacing * 2)) / 3);

            // Ustaw rozmiary
            _lblTotalPoints.Size = new Size(labelWidth, labelHeight);
            _lblBestCategory.Size = new Size(labelWidth, labelHeight);
            _lblWorstCategory.Size = new Size(labelWidth, labelHeight);

            // Ustaw pozycje (równomiernie rozłożone)
            _lblTotalPoints.Location = new Point(margin, margin);
            _lblBestCategory.Location = new Point(margin + labelWidth + spacing, margin);
            _lblWorstCategory.Location = new Point(margin + (labelWidth + spacing) * 2, margin);
        }

        #endregion

        #region Klasy pomocnicze

        /// <summary>
        /// Klasa pomocnicza dla ComboBox wyboru użytkownika.
        /// Przechowuje ID użytkownika i tekst do wyświetlenia.
        /// </summary>
        private class UserOption
        {
            public int UserId { get; }
            public string Text { get; }
            public UserOption(int id, string text) { UserId = id; Text = text; }
            public override string ToString() => Text;
        }

        #endregion

        #region Wczytywanie danych

        /// <summary>
        /// Ładuje statystyki punktów z bazy i aktualizuje UI.
        /// 
        /// AKTUALIZUJE:
        /// - Etykietę "Total Points" (suma punktów)
        /// - Etykietę "Best Category" (kategoria z największą liczbą punktów)
        /// - Etykietę "Worst Category" (kategoria z najmniejszą liczbą lub 0)
        /// - Wykres radarowy
        /// </summary>
        private void LoadStatistics()
        {
            if (_pointsService == null) return;

            // Pobierz punkty z bieżącego miesiąca
            var categoryPoints = _pointsService.GetCategoryPointsForCurrentMonth();
            var totalPoints = _pointsService.GetTotalPoints();

            // === SUMA PUNKTÓW ===
            _lblTotalPoints.Text = $"🏆  You have {totalPoints} points";

            // === NAJLEPSZA KATEGORIA ===
            var sortedCategories = categoryPoints.OrderByDescending(kvp => kvp.Value).ToList();
            if (sortedCategories.Count > 0 && sortedCategories[0].Value > 0)
            {
                var bestCategory = sortedCategories[0].Key;
                _lblBestCategory.Text = $"You are great at: {bestCategory}";

                // Ustaw kolor tła i ramki na podstawie kategorii
                if (_categoryColors.ContainsKey(bestCategory))
                {
                    var catColor = _categoryColors[bestCategory];
                    _lblBestCategory.BackColor = LightenColor(catColor, 0.85); // Jaśniejsze tło
                    _lblBestCategory.ForeColor = DarkenColor(catColor, 0.3);   // Ciemniejszy tekst
                    _lblBestCategory.Tag = catColor; // Kolor dla Paint
                }
            }
            else
            {
                _lblBestCategory.Text = "You are great at: -";
                _lblBestCategory.BackColor = Color.FromArgb(245, 250, 245);
                _lblBestCategory.ForeColor = Color.FromArgb(39, 174, 96);
                _lblBestCategory.Tag = Color.FromArgb(39, 174, 96);
            }

            // === NAJSŁABSZA KATEGORIA ===
            if (sortedCategories.Count > 0)
            {
                // Szukaj kategorii z 0 punktów
                var worst = sortedCategories.LastOrDefault(kvp => kvp.Value == 0);
                string worstCategory = null;

                if (worst.Key != null)
                {
                    worstCategory = worst.Key;
                }
                else if (sortedCategories.Count > 1)
                {
                    // Jeśli nie ma kategorii z 0, weź ostatnią (najmniej punktów)
                    worstCategory = sortedCategories.Last().Key;
                }

                if (worstCategory != null)
                {
                    _lblWorstCategory.Text = $"You should work on: {worstCategory}";

                    // Ustaw kolor tła i ramki na podstawie kategorii
                    if (_categoryColors.ContainsKey(worstCategory))
                    {
                        var catColor = _categoryColors[worstCategory];
                        _lblWorstCategory.BackColor = LightenColor(catColor, 0.85);
                        _lblWorstCategory.ForeColor = DarkenColor(catColor, 0.3);
                        _lblWorstCategory.Tag = catColor;
                    }
                }
                else
                {
                    _lblWorstCategory.Text = "You should work on: -";
                    _lblWorstCategory.BackColor = Color.FromArgb(245, 248, 255);
                    _lblWorstCategory.ForeColor = Color.FromArgb(52, 152, 219);
                    _lblWorstCategory.Tag = Color.FromArgb(52, 152, 219);
                }
            }
            else
            {
                _lblWorstCategory.Text = "You should work on: -";
                _lblWorstCategory.BackColor = Color.FromArgb(245, 248, 255);
                _lblWorstCategory.ForeColor = Color.FromArgb(52, 152, 219);
                _lblWorstCategory.Tag = Color.FromArgb(52, 152, 219);
            }

            // Wymuś odświeżenie ramek
            _lblBestCategory.Invalidate();
            _lblWorstCategory.Invalidate();

            // Odśwież wykres radarowy
            _radarChartPanel.Invalidate();
        }

        /// <summary>
        /// Ładuje listę nagród z bazy i wyświetla w ListView.
        /// Pokazuje tylko nagrody które nie zostały jeszcze wymienione.
        /// </summary>
        private void LoadRewards()
        {
            if (_pointsService == null) return;

            _rewardsListView.Items.Clear();
            var rewards = _pointsService.GetRewards();

            // Dodaj tylko niewymienione nagrody
            foreach (var reward in rewards.Where(r => !r.IsRedeemed))
            {
                var item = new ListViewItem(reward.Name);
                item.SubItems.Add(reward.PointsCost + " p");
                item.Tag = reward; // Przechowaj obiekt nagrody
                _rewardsListView.Items.Add(item);
            }
        }

        #endregion

        #region Pomocnicze metody kolorów

        /// <summary>
        /// Tworzy jaśniejszą wersję koloru.
        /// </summary>
        /// <param name="color">Kolor bazowy</param>
        /// <param name="factor">Współczynnik rozjaśnienia (0-1)</param>
        /// <returns>Rozjaśniony kolor</returns>
        private Color LightenColor(Color color, double factor)
        {
            return Color.FromArgb(
                color.A,
                (int)(color.R + (255 - color.R) * factor),
                (int)(color.G + (255 - color.G) * factor),
                (int)(color.B + (255 - color.B) * factor));
        }

        /// <summary>
        /// Tworzy ciemniejszą wersję koloru.
        /// </summary>
        /// <param name="color">Kolor bazowy</param>
        /// <param name="factor">Współczynnik przyciemnienia (0-1)</param>
        /// <returns>Przyciemniony kolor</returns>
        private Color DarkenColor(Color color, double factor)
        {
            return Color.FromArgb(
                color.A,
                (int)(color.R * (1 - factor)),
                (int)(color.G * (1 - factor)),
                (int)(color.B * (1 - factor)));
        }

        #endregion

        #region Obsługa nagród

        /// <summary>
        /// Handler "Add Reward" - otwiera formularz dodawania nowej nagrody.
        /// </summary>
        private void OnAddReward()
        {
            using (var form = new AddRewardForm())
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    _pointsService.AddReward(form.RewardName, form.Description, form.PointsCost);
                    LoadRewards();
                }
            }
        }

        /// <summary>
        /// Handler "Redeem Reward" - wymienia wybraną nagrodę za punkty.
        /// 
        /// WALIDACJA:
        /// - Nagroda musi być wybrana
        /// - Nagroda nie może być już wymieniona
        /// - Użytkownik musi mieć wystarczająco punktów
        /// </summary>
        private void OnRedeemReward()
        {
            // Sprawdź czy wybrano nagrodę
            if (_rewardsListView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a reward to redeem.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var selectedItem = _rewardsListView.SelectedItems[0];
            var reward = selectedItem.Tag as Reward;
            if (reward == null) return;

            // Sprawdź czy nagroda nie jest już wymieniona
            if (reward.IsRedeemed)
            {
                MessageBox.Show("This reward has already been redeemed.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Sprawdź czy użytkownik ma wystarczająco punktów
            int totalPoints = _pointsService.GetTotalPoints();
            if (totalPoints < reward.PointsCost)
            {
                MessageBox.Show("You don't have enough points to collect this reward.", "Insufficient Points",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Poproś o potwierdzenie
            var result = MessageBox.Show(
                $"Redeem '{reward.Name}' for {reward.PointsCost} points?",
                "Confirm Redemption",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                if (_pointsService.RedeemReward(reward.RewardID))
                {
                    MessageBox.Show("Congrats, you've just gained a reward. Keep going!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadRewards();
                    LoadStatistics(); // Odśwież punkty
                }
                else
                {
                    MessageBox.Show("Failed to redeem reward. Please try again.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void OnHistoryClick()
        {
            if (_pointsService == null) return;
            var history = _pointsService.GetRedeemedRewards(UserSession.ContextUserId);
            using var form = new RedeemedRewardsForm(history);
            form.ShowDialog(this);
        }

        /// <summary>
        /// Handler "Clear Statistics" - czyści statystyki z ostatnich 30 dni.
        /// Wymaga potwierdzenia przed usunięciem.
        /// </summary>
        private void OnClearStatistics()
        {
            var result = MessageBox.Show(
                "Are you sure you want to delete your statistics from last 30 days?",
                "Confirm",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                _pointsService.ClearCategoryPointsLast30Days();
                _pointsService.ClearRedeemedHistory();
                LoadStatistics();
            }
        }

        /// <summary>
        /// Handler kliknięcia nagrody - otwiera formularz edycji/usunięcia.
        /// Kids nie mogą edytować nagród (tylko wymieniać).
        /// 
        /// WYNIK DIALOGU:
        /// - OK: zapisz zmiany
        /// - Abort: usuń nagrodę (z potwierdzeniem)
        /// </summary>
        private void OnRewardDoubleClick()
        {
            if (_rewardsListView.SelectedItems.Count == 0)
                return;

            var selectedItem = _rewardsListView.SelectedItems[0];
            var reward = selectedItem.Tag as Reward;
            if (reward == null) return;

            // Kids nie mogą edytować nagród
            if (UserSession.IsKid)
            {
                MessageBox.Show("You cannot edit rewards.", "Access Denied",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Otwórz dialog edycji
            using var form = new EditRewardForm(reward);
            var result = form.ShowDialog(this);

            if (result == DialogResult.OK)
            {
                // Zapisz zmiany
                _pointsService.UpdateReward(reward.RewardID, form.RewardName, form.Description, form.PointsCost);
                LoadRewards();
            }
            else if (result == DialogResult.Abort) // Abort == "Delete"
            {
                // Poproś o potwierdzenie usunięcia
                var confirmDelete = MessageBox.Show(
                    $"Are you sure you want to delete the reward '{reward.Name}'?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (confirmDelete == DialogResult.Yes)
                {
                    _pointsService.DeleteReward(reward.RewardID);
                    LoadRewards();
                }
            }
        }

        #endregion

        #region Wykres radarowy

        /// <summary>
        /// Handler Paint dla panelu wykresu radarowego.
        /// 
        /// RYSUJE:
        /// - Okręgi siatki (5 poziomów)
        /// - 8 osi (po jednej dla każdej kategorii)
        /// - Etykiety kategorii
        /// - Punkty danych (kolorowe kółka)
        /// - Wielokąt łączący punkty (wypełniony, półprzezroczysty)
        /// </summary>
        private void RadarChartPanel_Paint(object sender, PaintEventArgs e)
        {
            if (_pointsService == null) return;

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias; // Wygładzone krawędzie
            g.Clear(Color.White);

            // Pobierz dane punktów
            var categoryPoints = _pointsService.GetCategoryPointsForCurrentMonth();
            var panel = sender as Panel;
            if (panel == null) return;

            // Oblicz centrum i promień wykresu
            int centerX = panel.Width / 2;
            int centerY = panel.Height / 2;
            int radius = Math.Min(centerX, centerY) - 40;

            // Znajdź max punktów (do skalowania)
            int maxPoints = categoryPoints.Values.DefaultIfEmpty(0).Max();
            if (maxPoints == 0) maxPoints = 100; // Domyślna skala

            // === SIATKA (5 koncentrycznych okręgów) ===
            using (var gridPen = new Pen(Color.FromArgb(230, 230, 230), 1))
            {
                for (int i = 1; i <= 5; i++)
                {
                    int r = radius * i / 5;
                    g.DrawEllipse(gridPen, centerX - r, centerY - r, r * 2, r * 2);
                }
            }

            // === OSIE (8 linii od centrum) ===
            using (var axisPen = new Pen(Color.FromArgb(200, 200, 200), 1))
            {
                for (int i = 0; i < 8; i++)
                {
                    double angle = (i * 2 * Math.PI / 8) - (Math.PI / 2); // Start od góry
                    int x = centerX + (int)(radius * Math.Cos(angle));
                    int y = centerY + (int)(radius * Math.Sin(angle));
                    g.DrawLine(axisPen, centerX, centerY, x, y);
                }
            }

            // === ETYKIETY KATEGORII ===
            using (var labelFont = new Font("Segoe UI", 9, FontStyle.Regular))
            using (var labelBrush = new SolidBrush(Color.Black))
            using (var format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.None,
                FormatFlags = StringFormatFlags.LineLimit
            })
            {
                for (int i = 0; i < _categories.Length; i++)
                {
                    double angle = (i * 2 * Math.PI / 8) - (Math.PI / 2);
                    int labelX = centerX + (int)((radius + 30) * Math.Cos(angle));
                    int labelY = centerY + (int)((radius + 30) * Math.Sin(angle));

                    string categoryName = _categories[i];
                    var layoutRect = new RectangleF(labelX - 70, labelY - 30, 140, 60);
                    g.DrawString(categoryName, labelFont, labelBrush, layoutRect, format);
                }
            }

            // === PUNKTY DANYCH I WIELOKĄT ===
            var points = new List<PointF>();
            _chartDataPoints.Clear(); // Wyczyść dla tooltip

            for (int i = 0; i < _categories.Length; i++)
            {
                string category = _categories[i];
                int pointsValue = categoryPoints.ContainsKey(category) ? categoryPoints[category] : 0;

                // Normalizuj wartość do 0-1
                double normalizedValue = (double)pointsValue / maxPoints;
                if (normalizedValue > 1.0) normalizedValue = 1.0;

                // Oblicz pozycję punktu
                double angle = (i * 2 * Math.PI / 8) - (Math.PI / 2);
                float x = centerX + (float)(radius * normalizedValue * Math.Cos(angle));
                float y = centerY + (float)(radius * normalizedValue * Math.Sin(angle));
                points.Add(new PointF(x, y));

                // Zapisz dane punktu dla tooltip
                _chartDataPoints.Add((new PointF(x, y), category, pointsValue));

                // Rysuj punkt (kolorowe kółko)
                Color pointColor = _categoryColors.ContainsKey(category)
                    ? _categoryColors[category]
                    : Color.Gray;
                using (var brush = new SolidBrush(pointColor))
                {
                    g.FillEllipse(brush, x - 5, y - 5, 10, 10);
                }
            }

            // Rysuj wielokąt łączący punkty
            if (points.Count > 2)
            {
                // Fioletowy wielokąt - półprzezroczysty
                using (var polygonBrush = new SolidBrush(Color.FromArgb(100, 155, 89, 182)))
                using (var polygonPen = new Pen(Color.FromArgb(155, 89, 182), 2))
                {
                    g.FillPolygon(polygonBrush, points.ToArray());
                    g.DrawPolygon(polygonPen, points.ToArray());
                }
            }
        }

        /// <summary>
        /// Handler MouseMove dla panelu wykresu - wyświetla tooltip przy najechaniu na punkt.
        /// </summary>
        private void RadarChartPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (_chartDataPoints == null || _chartDataPoints.Count == 0 || _chartToolTip == null)
                return;

            const float hitRadius = 12f; // Promień wykrycia hover
            string tooltipText = "";

            // Sprawdź czy mysz jest blisko któregoś punktu
            foreach (var dataPoint in _chartDataPoints)
            {
                float dx = e.X - dataPoint.Point.X;
                float dy = e.Y - dataPoint.Point.Y;
                float distance = (float)Math.Sqrt(dx * dx + dy * dy);

                if (distance <= hitRadius)
                {
                    tooltipText = $"{dataPoint.Category}: {dataPoint.Points} points";
                    break;
                }
            }

            // Aktualizuj tooltip tylko jeśli tekst się zmienił (unikaj migotania)
            if (tooltipText != _lastTooltipText)
            {
                _lastTooltipText = tooltipText;
                if (string.IsNullOrEmpty(tooltipText))
                {
                    _chartToolTip.Hide(_radarChartPanel);
                }
                else
                {
                    _chartToolTip.Show(tooltipText, _radarChartPanel, e.X + 15, e.Y + 15);
                }
            }
        }

        #endregion
    }
}
