using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SchoolBusApp
{
    public class DashboardPanel : UserControl
    {
        public DashboardPanel()
        {
            BackColor = Color.FromArgb(245, 247, 250);
            RightToLeft = RightToLeft.Yes;
            Load += (s, e) => LoadData();
        }

        private void LoadData()
        {
            Controls.Clear();
            try
            {
                var (students, buses, drivers, routes, todayPay, presentToday) = DataAccess.GetDashboardStats();

                // Page title
                var lblPage = new Label {
                    Text = "لوحة التحكم  |  Dashboard",
                    Font = new Font("Segoe UI", 18f, FontStyle.Bold),
                    ForeColor = Color.FromArgb(30, 40, 80),
                    AutoSize = true, Location = new Point(0, 0)
                };
                Controls.Add(lblPage);

                var lblSub = new Label {
                    Text = DateTime.Now.ToString("dddd, dd MMMM yyyy"),
                    Font = new Font("Segoe UI", 10f),
                    ForeColor = Color.Gray, AutoSize = true,
                    Location = new Point(0, 38)
                };
                Controls.Add(lblSub);

                // Stat cards
                var cards = new[]
                {
                    ("👨‍🎓", $"{students}", "إجمالي الطلاب\nTotal Students", Color.FromArgb(63,114,175)),
                    ("🚌", $"{buses}", "الحافلات\nBuses", Color.FromArgb(40,167,100)),
                    ("👨‍✈️", $"{drivers}", "السائقون\nDrivers", Color.FromArgb(220,100,40)),
                    ("🗺️", $"{routes}", "المسارات\nRoutes", Color.FromArgb(140,80,200)),
                    ("✅", $"{presentToday}", "حاضرون اليوم\nPresent Today", Color.FromArgb(30,160,160)),
                    ("💰", $"{todayPay:N0} ر.س", "مدفوعات اليوم\nToday's Payments", Color.FromArgb(180,130,30)),
                };

                int x = 0, y = 80, i = 0;
                int cardW = 220, cardH = 120, gap = 20;

                foreach (var (icon, value, label, color) in cards)
                {
                    if (i > 0 && i % 3 == 0) { x = 0; y += cardH + gap; }
                    var card = CreateStatCard(icon, value, label, color, x, y, cardW, cardH);
                    Controls.Add(card);
                    x += cardW + gap;
                    i++;
                }

                // Quick info panel
                var infoPanel = new Panel {
                    Location = new Point(0, y + cardH + 30),
                    Size = new Size(700, 120),
                    BackColor = Color.White
                };
                infoPanel.Paint += (s, e) => {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    using var pen = new Pen(Color.FromArgb(220, 230, 245), 1);
                    e.Graphics.DrawRectangle(pen, 0, 0, infoPanel.Width-1, infoPanel.Height-1);
                };

                var lblInfo = new Label {
                    Text = "⚡  نصائح سريعة  |  Quick Tips\n\n" +
                           "• استخدم قائمة الجانب للتنقل بين الأقسام\n" +
                           "• Use the sidebar to navigate between sections\n" +
                           "• سجّل الحضور يومياً من قسم الحضور والغياب",
                    Font = new Font("Segoe UI", 9f),
                    ForeColor = Color.FromArgb(60, 80, 130),
                    Dock = DockStyle.Fill,
                    Padding = new Padding(15)
                };
                infoPanel.Controls.Add(lblInfo);
                Controls.Add(infoPanel);
            }
            catch (Exception ex)
            {
                var lbl = new Label { Text = "⚠️ " + ex.Message, ForeColor = Color.Red,
                    AutoSize = true, Location = new Point(10, 10) };
                Controls.Add(lbl);
            }
        }

        private Panel CreateStatCard(string icon, string value, string label, Color color, int x, int y, int w, int h)
        {
            var card = new Panel {
                Location = new Point(x, y), Size = new Size(w, h),
                BackColor = Color.White, Cursor = Cursors.Default
            };

            card.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                // Left accent bar
                using var brush = new SolidBrush(color);
                e.Graphics.FillRectangle(brush, w - 6, 0, 6, h);
                // Shadow simulation
                using var pen = new Pen(Color.FromArgb(220, 230, 245));
                e.Graphics.DrawRectangle(pen, 0, 0, w-1, h-1);
            };

            var lblIcon = new Label { Text = icon, Font = new Font("Segoe UI Emoji", 22f),
                AutoSize = true, Location = new Point(w - 55, 15) };
            var lblValue = new Label { Text = value,
                Font = new Font("Segoe UI", 20f, FontStyle.Bold),
                ForeColor = color, AutoSize = true, Location = new Point(12, 15) };
            var lblLabel = new Label { Text = label,
                Font = new Font("Segoe UI", 8f), ForeColor = Color.Gray,
                AutoSize = true, Location = new Point(12, 62) };

            card.Controls.AddRange(new Control[] { lblIcon, lblValue, lblLabel });
            return card;
        }
    }
}
