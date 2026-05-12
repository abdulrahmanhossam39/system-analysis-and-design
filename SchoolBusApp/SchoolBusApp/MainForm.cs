using System.Drawing;
using System.Windows.Forms;

namespace SchoolBusApp
{
    public class MainForm : Form
    {
        private Panel pnlSidebar = null!;
        private Panel pnlContent = null!;
        private Panel pnlHeader = null!;
        private Label lblTitle = null!;
        private Label lblClock = null!;
        private System.Windows.Forms.Timer clockTimer = null!;

        // Sidebar buttons
        private Button btnDashboard = null!, btnStudents = null!, btnBuses = null!;
        private Button btnRoutes = null!, btnAttendance = null!, btnPayments = null!, btnSettings = null!;
        private Button? activeBtn = null;

        public MainForm()
        {
            InitializeComponent();
            ShowDashboard();
        }

        private void InitializeComponent()
        {
            Text = "شركة نقل الطلاب | Student Transportation System";
            Size = new Size(1280, 780);
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(1000, 650);
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 9f);
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;

            // ── Header ──────────────────────────────────────────
            pnlHeader = new Panel {
                Dock = DockStyle.Top, Height = 60,
                BackColor = Color.FromArgb(30, 40, 80),
                Padding = new Padding(15, 0, 15, 0)
            };

            var busIcon = new Label {
                Text = "🚌", Font = new Font("Segoe UI Emoji", 22f),
                ForeColor = Color.White, AutoSize = true,
                Location = new Point(15, 10)
            };

            lblTitle = new Label {
                Text = "شركة نقل الطلاب  |   Student Transportation",
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = Color.White, AutoSize = true,
                Location = new Point(80, 17)
            };

            lblClock = new Label {
                Font = new Font("Segoe UI", 10f), ForeColor = Color.FromArgb(180,200,240),
                AutoSize = true, Location = new Point(1130, 22)
            };
            UpdateClock(null, EventArgs.Empty);

            clockTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            clockTimer.Tick += UpdateClock;
            clockTimer.Start();

            pnlHeader.Controls.AddRange(new Control[] { busIcon, lblTitle, lblClock });

            // ── Sidebar ─────────────────────────────────────────
            pnlSidebar = new Panel {
                Dock = DockStyle.Right, Width = 200,
                BackColor = Color.FromArgb(22, 30, 65),
                Padding = new Padding(0, 10, 0, 10)
            };

            var navItems = new[]
            {
                ("🏠  لوحة التحكم | Dashboard", "dashboard"),
                ("👨‍🎓  الطلاب | Students", "students"),
                ("🚌  الحافلات والسائقين | Buses", "buses"),
                ("🗺️  المسارات | Routes", "routes"),
                ("✅  الحضور والغياب | Attendance", "attendance"),
                ("💰  المدفوعات | Payments", "payments"),
                ("⚙️  الإعدادات | Settings", "settings"),
            };

            int yPos = 20;
            foreach (var (text, tag) in navItems)
            {
                var btn = new Button {
                    Text = text, Tag = tag,
                    FlatStyle = FlatStyle.Flat,
                    ForeColor = Color.FromArgb(180, 200, 240),
                    BackColor = Color.Transparent,
                    Font = new Font("Segoe UI", 9f),
                    TextAlign = ContentAlignment.MiddleRight,
                    Padding = new Padding(0, 0, 15, 0),
                    Size = new Size(200, 46),
                    Location = new Point(0, yPos),
                    Cursor = Cursors.Hand
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(40, 55, 110);
                btn.MouseEnter += (s, e) => { if (btn != activeBtn) btn.BackColor = Color.FromArgb(35, 48, 100); };
                btn.MouseLeave += (s, e) => { if (btn != activeBtn) btn.BackColor = Color.Transparent; };
                btn.Click += NavBtn_Click;

                switch (tag)
                {
                    case "dashboard": btnDashboard = btn; break;
                    case "students": btnStudents = btn; break;
                    case "buses": btnBuses = btn; break;
                    case "routes": btnRoutes = btn; break;
                    case "attendance": btnAttendance = btn; break;
                    case "payments": btnPayments = btn; break;
                    case "settings": btnSettings = btn; break;
                }

                pnlSidebar.Controls.Add(btn);
                yPos += 50;
            }

            // Version label at bottom of sidebar
            var lblVer = new Label {
                Text = "v1.0.0", Font = new Font("Segoe UI", 8f),
                ForeColor = Color.FromArgb(80, 100, 150), AutoSize = true,
                Location = new Point(70, 420)
            };
            pnlSidebar.Controls.Add(lblVer);

            // ── Content ─────────────────────────────────────────
            pnlContent = new Panel {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(245, 247, 250),
                Padding = new Padding(20)
            };

            Controls.Add(pnlContent);
            Controls.Add(pnlSidebar);
            Controls.Add(pnlHeader);
        }

        private void UpdateClock(object? s, EventArgs e)
        {
            lblClock.Text = DateTime.Now.ToString("yyyy/MM/dd  HH:mm:ss");
            if (lblClock.Right > pnlHeader.Width - 10)
                lblClock.Left = pnlHeader.Width - lblClock.Width - 10;
        }

        private void NavBtn_Click(object? sender, EventArgs e)
        {
            var btn = (Button)sender!;
            var tag = btn.Tag?.ToString() ?? "";
            SetActiveButton(btn);

            pnlContent.Controls.Clear();
            switch (tag)
            {
                case "dashboard": ShowDashboard(); break;
                case "students": ShowPanel(new StudentsPanel()); break;
                case "buses": ShowPanel(new BusesPanel()); break;
                case "routes": ShowPanel(new RoutesPanel()); break;
                case "attendance": ShowPanel(new AttendancePanel()); break;
                case "payments": ShowPanel(new PaymentsPanel()); break;
                case "settings": ShowPanel(new SettingsPanel()); break;
            }
        }

        private void SetActiveButton(Button btn)
        {
            if (activeBtn != null)
            {
                activeBtn.BackColor = Color.Transparent;
                activeBtn.ForeColor = Color.FromArgb(180, 200, 240);
            }
            activeBtn = btn;
            btn.BackColor = Color.FromArgb(50, 70, 140);
            btn.ForeColor = Color.White;
        }

        private void ShowDashboard()
        {
            SetActiveButton(btnDashboard);
            ShowPanel(new DashboardPanel());
        }

        private void ShowPanel(Control panel)
        {
            panel.Dock = DockStyle.Fill;
            pnlContent.Controls.Clear();
            pnlContent.Controls.Add(panel);
        }
    }
}
