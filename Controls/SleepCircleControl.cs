#nullable enable
using System;
using System.Drawing;
using System.Windows.Forms;

namespace TimeManager.Controls
{
    /// <summary>
    /// Interaktywna kontrolka do ustawiania harmonogramu snu w formie zegara analogowego (12-godzinnego).
    /// 
    /// FUNKCJONALNOŚĆ:
    /// - Wyświetla koło zegara z 12 kreskami (1 kreska = 1 godzina)
    /// - Pokazuje łuk snu (zakres od SleepStart do SleepEnd) jako wypełniony wycinek
    /// - Dwa uchwyty do przeciągania: początek i koniec snu
    /// - Płynne przeciąganie z automatycznym przechodzeniem przez granice AM/PM
    /// 
    /// OBSŁUGA:
    /// - Kliknij i przeciągnij uchwyt aby zmienić godzinę
    /// - Przeciąganie zgodnie z ruchem wskazówek zegara = przesuwanie do przodu w czasie
    /// - Przekroczenie granicy 12/0 automatycznie przełącza AM↔PM
    /// 
    /// ZDARZENIA:
    /// - SleepScheduleChanged: wywoływane przy każdej zmianie harmonogramu
    /// </summary>
    public class SleepCircleControl : Control
    {
        #region Stałe i pola
        
        /// <summary>Promień uchwytu do przeciągania (w pikselach).</summary>
        private const int HandleRadius = 10;
        
        /// <summary>Czy aktualnie przeciągany jest uchwyt początku snu.</summary>
        private bool _draggingStart;
        
        /// <summary>Czy aktualnie przeciągany jest uchwyt końca snu.</summary>
        private bool _draggingEnd;
        
        /// <summary>Ostatni kąt myszki (w radianach) - do obliczania delty przy przeciąganiu.</summary>
        private double _lastAngle;
        
        #endregion

        #region Zdarzenia
        
        /// <summary>Wywoływane gdy użytkownik zmieni harmonogram snu przeciągając uchwyty.</summary>
        public event EventHandler? SleepScheduleChanged;
        
        #endregion

        #region Właściwości
        
        /// <summary>Godzina rozpoczęcia snu (domyślnie 23:00).</summary>
        public TimeSpan SleepStart { get; set; } = TimeSpan.FromHours(23);
        
        /// <summary>Godzina zakończenia snu (domyślnie 7:00).</summary>
        public TimeSpan SleepEnd { get; set; } = TimeSpan.FromHours(7);
        
        #endregion

        #region Konstruktor
        
        /// <summary>
        /// Tworzy nową instancję kontrolki SleepCircleControl.
        /// Ustawia podwójne buforowanie dla płynnego renderowania.
        /// </summary>
        public SleepCircleControl()
        {
            DoubleBuffered = true;
            Size = new Size(260, 260);
            BackColor = Color.White;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }
        
        #endregion

        #region Rysowanie
        
        /// <summary>
        /// Główna metoda renderowania - rysuje koło zegara, kreski, łuk snu i uchwyty.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            var radius = Math.Min(Width, Height) / 2 - 20;
            var center = new PointF(Width / 2f, Height / 2f);
            var circleRect = new RectangleF(center.X - radius, center.Y - radius, radius * 2, radius * 2);

            // Rysuj zewnętrzne koło zegara
            using var pen = new Pen(Color.Gray, 4);
            g.DrawEllipse(pen, circleRect);

            // Rysuj elementy zegara
            DrawTicks(g, center, radius);
            DrawSleepArc(g, center, radius);
            DrawHandle(g, center, radius, SleepStart, Color.DeepSkyBlue);
            DrawHandle(g, center, radius, SleepEnd, Color.DeepSkyBlue);
        }

        /// <summary>
        /// Rysuje 12 kresek godzinowych na obwodzie zegara (1 kreska = 1 godzina).
        /// Góra zegara (12:00) to godzina 0.
        /// </summary>
        private void DrawTicks(Graphics g, PointF center, float radius)
        {
            using var pen = new Pen(Color.Gray, 2);
            for (int hour = 0; hour < 12; hour++)
            {
                var angle = HourToAngle12h(hour);
                var outer = PointOnCircle(center, radius, angle);
                var inner = PointOnCircle(center, radius - 10, angle);
                g.DrawLine(pen, inner, outer);
            }
        }

        /// <summary>
        /// Rysuje wypełniony wycinek koła reprezentujący czas snu.
        /// Łuk zaczyna się od SleepStart i rozciąga do SleepEnd (zgodnie z ruchem wskazówek).
        /// </summary>
        private void DrawSleepArc(Graphics g, PointF center, float radius)
        {
            using var brush = new SolidBrush(Color.DeepSkyBlue);
            var rect = new RectangleF(center.X - radius, center.Y - radius, radius * 2, radius * 2);

            // Oblicz czas trwania snu (uwzględniając przejście przez północ)
            var durationHours = SleepEnd - SleepStart;
            if (durationHours < TimeSpan.Zero)
            {
                durationHours += TimeSpan.FromHours(24);
            }

            // Kąt łuku: pełne 360° = 12 godzin na zegarze
            var sweep = (float)(durationHours.TotalHours / 12.0 * 360.0);
            var startAngle = TimeToAngleDegrees(SleepStart);

            g.FillPie(brush, rect, startAngle, sweep);
        }

        /// <summary>
        /// Rysuje okrągły uchwyt do przeciągania na obwodzie zegara.
        /// </summary>
        private void DrawHandle(Graphics g, PointF center, float radius, TimeSpan time, Color color)
        {
            var angle = TimeToAngle(time);
            var point = PointOnCircle(center, radius, angle);
            var handleRect = new RectangleF(point.X - HandleRadius, point.Y - HandleRadius, HandleRadius * 2, HandleRadius * 2);
            using var brush = new SolidBrush(color);
            g.FillEllipse(brush, handleRect);
            g.DrawEllipse(Pens.White, handleRect);
        }
        
        #endregion

        #region Interakcja z myszą
        
        /// <summary>
        /// Obsługuje naciśnięcie myszy - sprawdza czy kliknięto na uchwyt i rozpoczyna przeciąganie.
        /// Zapisuje początkowy kąt myszki dla podejścia delta-based.
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            var radius = Math.Min(Width, Height) / 2 - 20;
            var center = new PointF(Width / 2f, Height / 2f);

            var startPoint = PointOnCircle(center, radius, TimeToAngle(SleepStart));
            var endPoint = PointOnCircle(center, radius, TimeToAngle(SleepEnd));

            if (IsNearHandle(e.Location, startPoint))
            {
                _draggingStart = true;
                _lastAngle = PointToAngle(e.Location);
            }
            else if (IsNearHandle(e.Location, endPoint))
            {
                _draggingEnd = true;
                _lastAngle = PointToAngle(e.Location);
            }
        }

        /// <summary>
        /// Obsługuje ruch myszy podczas przeciągania.
        /// Używa podejścia delta-based: oblicza o ile godzin przesunąć czas na podstawie delty kąta.
        /// To zapewnia płynne przechodzenie przez granice AM/PM bez skoków.
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (!_draggingStart && !_draggingEnd)
            {
                return;
            }

            var currentAngle = PointToAngle(e.Location);
            
            // Oblicz deltę kąta (o ile przesunęła się myszka)
            var deltaAngle = currentAngle - _lastAngle;
            
            // Normalizuj deltę przy przekraczaniu granicy -PI/+PI
            if (deltaAngle > Math.PI) deltaAngle -= 2 * Math.PI;
            if (deltaAngle < -Math.PI) deltaAngle += 2 * Math.PI;
            
            // Konwertuj deltę kąta na deltę godzin (pełne koło = 12 godzin)
            var deltaHours = deltaAngle / (2 * Math.PI) * 12.0;
            
            _lastAngle = currentAngle;

            if (_draggingStart)
            {
                var newHours = SleepStart.TotalHours + deltaHours;
                // Normalizuj do zakresu 0-24 godzin
                newHours = ((newHours % 24) + 24) % 24;
                SleepStart = TimeSpan.FromHours(newHours);
                SleepScheduleChanged?.Invoke(this, EventArgs.Empty);
                Invalidate();
            }
            else if (_draggingEnd)
            {
                var newHours = SleepEnd.TotalHours + deltaHours;
                // Normalizuj do zakresu 0-24 godzin
                newHours = ((newHours % 24) + 24) % 24;
                SleepEnd = TimeSpan.FromHours(newHours);
                SleepScheduleChanged?.Invoke(this, EventArgs.Empty);
                Invalidate();
            }
        }

        /// <summary>
        /// Obsługuje zwolnienie myszy - kończy przeciąganie.
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _draggingStart = false;
            _draggingEnd = false;
        }

        /// <summary>
        /// Sprawdza czy punkt (kliknięcie) jest w pobliżu uchwytu.
        /// </summary>
        private bool IsNearHandle(Point location, PointF handle)
        {
            var dx = location.X - handle.X;
            var dy = location.Y - handle.Y;
            var distance = Math.Sqrt(dx * dx + dy * dy);
            return distance <= HandleRadius * 1.8;
        }
        
        #endregion

        #region Pomocnicze funkcje geometrii
        
        /// <summary>
        /// Oblicza punkt na obwodzie koła dla danego kąta.
        /// </summary>
        private static PointF PointOnCircle(PointF center, float radius, double angleRadians)
        {
            var x = center.X + radius * Math.Cos(angleRadians);
            var y = center.Y + radius * Math.Sin(angleRadians);
            return new PointF((float)x, (float)y);
        }

        /// <summary>
        /// Konwertuje pozycję myszki na kąt względem środka kontrolki (w radianach).
        /// </summary>
        private double PointToAngle(Point p)
        {
            var center = new PointF(Width / 2f, Height / 2f);
            var dx = p.X - center.X;
            var dy = p.Y - center.Y;
            return Math.Atan2(dy, dx);
        }
        
        #endregion

        #region Konwersja czas-kąt
        
        /// <summary>
        /// Konwertuje godzinę zegara (0-11) na kąt w radianach.
        /// Godzina 12/0 jest na górze (-90° w GDI+), ruch zgodny ze wskazówkami zegara.
        /// </summary>
        private static double HourToAngle12h(int hour)
        {
            var fraction = (hour % 12) / 12.0;
            var degrees = fraction * 360.0 - 90.0;
            return degrees * Math.PI / 180.0;
        }

        /// <summary>
        /// Konwertuje TimeSpan na kąt na zegarze 12-godzinnym (w radianach).
        /// Zegar przechodzi dwa pełne obroty na dobę (AM i PM na tej samej pozycji).
        /// </summary>
        private static double TimeToAngle(TimeSpan time)
        {
            var hour12 = time.TotalHours % 12.0;
            var fraction = hour12 / 12.0;
            var degrees = fraction * 360.0 - 90.0;
            return degrees * Math.PI / 180.0;
        }

        /// <summary>
        /// Konwertuje TimeSpan na kąt w stopniach (dla GDI+ FillPie).
        /// </summary>
        private static float TimeToAngleDegrees(TimeSpan time)
        {
            var hour12 = (float)(time.TotalHours % 12.0);
            var fraction = hour12 / 12f;
            return fraction * 360f - 90f;
        }
        
        #endregion

        #region Metody publiczne
        
        /// <summary>
        /// Zwraca czas trwania snu (uwzględniając przejście przez północ).
        /// </summary>
        public TimeSpan GetDuration()
        {
            var duration = SleepEnd - SleepStart;
            if (duration < TimeSpan.Zero)
            {
                duration += TimeSpan.FromHours(24);
            }
            return duration;
        }
        
        #endregion
    }
}
