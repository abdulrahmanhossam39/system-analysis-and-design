using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SchoolBusApp
{
    /// <summary>Base class providing a standard toolbar + DataGridView layout.</summary>
    public class BaseListPanel : UserControl
    {
        protected DataGridView grid = null!;
        protected TextBox txtSearch = null!;
        protected Panel pnlToolbar = null!;
        protected Button btnAdd = null!, btnEdit = null!, btnDelete = null!, btnRefresh = null!;
        protected Label lblPageTitle = null!;

        protected void BuildUI(string titleAr, string titleEn)
        {
            BackColor = Color.FromArgb(245, 247, 250);
            RightToLeft = RightToLeft.Yes;
            Dock = DockStyle.Fill;

            // Title
            lblPageTitle = new Label {
                Text = $"{titleAr}  |  {titleEn}",
                Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 40, 80),
                AutoSize = true, Location = new Point(0, 5)
            };
            Controls.Add(lblPageTitle);

            // Toolbar
            pnlToolbar = new Panel {
                Location = new Point(0, 45), Height = 48,
                BackColor = Color.White
            };
            pnlToolbar.Paint += (s, e) =>
            {
                using var pen = new Pen(Color.FromArgb(220, 230, 245));
                e.Graphics.DrawRectangle(pen, 0, 0, pnlToolbar.Width - 1, pnlToolbar.Height - 1);
            };

            btnAdd = MakeBtn("➕  إضافة | Add", Color.FromArgb(40, 140, 80), 10);
            btnEdit = MakeBtn("✏️  تعديل | Edit", Color.FromArgb(50, 100, 200), 155);
            btnDelete = MakeBtn("🗑️  حذف | Delete", Color.FromArgb(200, 60, 60), 300);
            btnRefresh = MakeBtn("🔄  تحديث | Refresh", Color.FromArgb(80, 80, 80), 445);

            txtSearch = new TextBox {
                PlaceholderText = "🔍  بحث | Search...",
                Location = new Point(600, 10), Size = new Size(220, 28),
                Font = new Font("Segoe UI", 10f), BorderStyle = BorderStyle.FixedSingle
            };
            txtSearch.TextChanged += (s, e) => OnSearch(txtSearch.Text);

            pnlToolbar.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete, btnRefresh, txtSearch });
            Controls.Add(pnlToolbar);

            // Grid
            grid = new DataGridView {
                Location = new Point(0, 103),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing,
                ColumnHeadersHeight = 38,
                RowTemplate = { Height = 34 },
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                GridColor = Color.FromArgb(230, 235, 245),
                EnableHeadersVisualStyles = false
            };

            // Header style
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 40, 80);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(30, 40, 80);
            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            // Row style
            grid.DefaultCellStyle.BackColor = Color.White;
            grid.DefaultCellStyle.ForeColor = Color.FromArgb(40, 40, 60);
            grid.DefaultCellStyle.Font = new Font("Segoe UI", 9f);
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 220, 245);
            grid.DefaultCellStyle.SelectionForeColor = Color.FromArgb(20, 30, 70);
            grid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            // Alternate row
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 255);

            grid.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0) OnEdit(); };

            Controls.Add(grid);

            btnAdd.Click += (s, e) => OnAdd();
            btnEdit.Click += (s, e) => OnEdit();
            btnDelete.Click += (s, e) => OnDelete();
            btnRefresh.Click += (s, e) => OnRefresh();

            Resize += (s, e) => ResizeControls();
            Load += (s, e) => { ResizeControls(); OnRefresh(); };
        }

        protected void ResizeControls()
        {
            pnlToolbar.Width = Width;
            grid.Width = Width;
            grid.Height = Height - 108;
            txtSearch.Left = Width - 240;
        }

        private Button MakeBtn(string text, Color color, int x)
        {
            var b = new Button {
                Text = text, FlatStyle = FlatStyle.Flat,
                BackColor = color, ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Size = new Size(135, 30), Location = new Point(x, 9),
                Cursor = Cursors.Hand
            };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }

        protected virtual void OnAdd() { }
        protected virtual void OnEdit() { }
        protected virtual void OnDelete() { }
        protected virtual void OnRefresh() { }
        protected virtual void OnSearch(string text) { }
    }

    // ─── Standard field dialog helper ───────────────────────────────────────────
    public static class FormHelper
    {
        public static Label MakeLbl(string text, int x, int y) =>
            new Label { Text = text, Location = new Point(x, y), AutoSize = true,
                Font = new Font("Segoe UI", 9.5f), ForeColor = Color.FromArgb(40,40,80) };

        public static TextBox MakeTxt(int x, int y, int w = 260) =>
            new TextBox { Location = new Point(x, y), Size = new Size(w, 28),
                Font = new Font("Segoe UI", 10f), BorderStyle = BorderStyle.FixedSingle };

        public static ComboBox MakeCbo(int x, int y, int w = 260) =>
            new ComboBox { Location = new Point(x, y), Size = new Size(w, 28),
                Font = new Font("Segoe UI", 10f), DropDownStyle = ComboBoxStyle.DropDownList };

        public static Button MakeOkBtn(string text = "حفظ  |  Save") =>
            new Button { Text = text, DialogResult = DialogResult.OK,
                BackColor = Color.FromArgb(40, 130, 80), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Size = new Size(140, 34),
                Font = new Font("Segoe UI", 10f, FontStyle.Bold) };

        public static Button MakeCancelBtn() =>
            new Button { Text = "إلغاء  |  Cancel", DialogResult = DialogResult.Cancel,
                BackColor = Color.FromArgb(180, 60, 60), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Size = new Size(140, 34),
                Font = new Font("Segoe UI", 10f) };

        public static Form WrapInDialog(string title, Control[] controls, int height = 450)
        {
            var dlg = new Form {
                Text = title, StartPosition = FormStartPosition.CenterParent,
                Size = new Size(620, height), FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false, MinimizeBox = false,
                BackColor = Color.White, RightToLeft = RightToLeft.Yes, RightToLeftLayout = true
            };
            foreach (var c in controls) dlg.Controls.Add(c);
            return dlg;
        }
    }
}
