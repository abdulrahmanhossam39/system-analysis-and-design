using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace SchoolBusApp
{
    public class SettingsPanel : UserControl
    {
        public SettingsPanel()
        {
            Dock = DockStyle.Fill;
            BackColor = Color.FromArgb(245, 247, 250);
            RightToLeft = RightToLeft.Yes;
            Load += (s, e) => BuildUI();
        }

        private void BuildUI()
        {
            var lbl = new Label {
                Text = "الإعدادات  |  Settings",
                Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 40, 80),
                AutoSize = true, Location = new Point(0, 5)
            };

            // DB connection card
            var card = new Panel {
                Location = new Point(0, 55), Size = new Size(600, 280),
                BackColor = Color.White, BorderStyle = BorderStyle.None
            };
            card.Paint += (s, e) => {
                using var pen = new System.Drawing.Pen(Color.FromArgb(220, 230, 245));
                e.Graphics.DrawRectangle(pen, 0, 0, card.Width-1, card.Height-1);
            };

            var lblCard = new Label {
                Text = "⚙️  إعدادات قاعدة البيانات  |  Database Connection",
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 40, 80),
                AutoSize = true, Location = new Point(15, 15)
            };

            var lblConn = new Label {
                Text = "سلسلة الاتصال | Connection String:",
                AutoSize = true, Location = new Point(15, 55),
                Font = new Font("Segoe UI", 9.5f)
            };

            var txtConn = new TextBox {
                Text = Database.ConnectionString,
                Location = new Point(15, 78), Size = new Size(560, 60),
                Multiline = true, Font = new Font("Consolas", 9f),
                BorderStyle = BorderStyle.FixedSingle,
                ScrollBars = ScrollBars.Vertical
            };

            var lblHint = new Label {
                Text = "مثال | Example:  Server=.;Database=SchoolBusDB;Integrated Security=True;TrustServerCertificate=True;\n" +
                       "أو بكلمة مرور | With password:  Server=.;Database=SchoolBusDB;User Id=sa;Password=YourPass;TrustServerCertificate=True;",
                Font = new Font("Segoe UI", 8.5f), ForeColor = Color.Gray,
                Location = new Point(15, 148), Size = new Size(560, 50)
            };

            var btnTest = new Button {
                Text = "🔌  اختبار الاتصال | Test Connection",
                Location = new Point(15, 210), Size = new Size(240, 34),
                FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(50, 100, 200),
                ForeColor = Color.White, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnTest.FlatAppearance.BorderSize = 0;
            btnTest.Click += (s, e) => TestConnection(txtConn.Text);

            var btnSave = new Button {
                Text = "💾  حفظ | Save",
                Location = new Point(270, 210), Size = new Size(150, 34),
                FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(40, 130, 70),
                ForeColor = Color.White, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += (s, e) => {
                Database.ConnectionString = txtConn.Text.Trim();
                MessageBox.Show("تم حفظ الإعدادات.\nSettings saved. Restart may be required.", "✅",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            card.Controls.AddRange(new Control[] { lblCard, lblConn, txtConn, lblHint, btnTest, btnSave });

            // Info card
            var infoCard = new Panel {
                Location = new Point(0, 350), Size = new Size(600, 130),
                BackColor = Color.FromArgb(235, 242, 255)
            };

            var lblInfo = new Label {
                Text = "ℹ️  معلومات النظام  |  System Info\n\n" +
                       "• البرنامج يتطلب SQL Server (أي إصدار) | Requires SQL Server (any edition)\n" +
                       "• يتم إنشاء جداول البيانات تلقائياً | Tables are created automatically\n" +
                       "• الواجهة ثنائية اللغة (عربي / إنجليزي) | Bilingual UI (Arabic / English)\n" +
                       "• .NET 6.0 Windows Required",
                Font = new Font("Segoe UI", 9f), ForeColor = Color.FromArgb(30, 60, 120),
                Location = new Point(15, 10), Size = new Size(570, 110)
            };
            infoCard.Controls.Add(lblInfo);

            Controls.AddRange(new Control[] { lbl, card, infoCard });
        }

        private void TestConnection(string connStr)
        {
            try
            {
                using var conn = new SqlConnection(connStr);
                conn.Open();
                MessageBox.Show("✅  الاتصال ناجح!\nConnection successful!\n\nServer: " + conn.DataSource,
                    "نجاح | Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌  فشل الاتصال | Connection failed:\n\n" + ex.Message,
                    "خطأ | Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
