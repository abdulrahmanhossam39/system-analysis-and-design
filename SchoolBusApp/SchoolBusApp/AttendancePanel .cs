using System.Drawing;
using System.Windows.Forms;
using SchoolBusApp.Models;

namespace SchoolBusApp
{
    public class AttendancePanel : BaseListPanel
    {
        private List<AttendanceRecord> _data = new();
        private DateTimePicker dtpDate = null!;
        private Label lblTotal = null!;
        private Button btnSave = null!;        // 5th button — shown only in edit mode
        private bool _isEditMode = false;
        private bool _isLoading  = false;

        public AttendancePanel()
        {
            BuildUI("الحضور والغياب", "Attendance");

            // ── Relabel the 4 standard toolbar buttons ────────────────────
            btnAdd.Text      = "⚡  توليد | Generate";
            btnAdd.BackColor = Color.FromArgb(50, 120, 200);

            btnEdit.Text      = "✏️  تعديل | Edit";
            btnEdit.BackColor = Color.FromArgb(50, 100, 200);

            btnDelete.Text      = "🗑️  حذف | Delete";
            btnDelete.BackColor = Color.FromArgb(200, 60, 60);
            btnDelete.Enabled   = false;

            btnRefresh.Text      = "🔄  تحديث | Refresh";
            btnRefresh.BackColor = Color.FromArgb(80, 80, 80);

            // ── Save button — sits right after Refresh, hidden until edit mode ──
            btnSave = new Button
            {
                Text      = "💾  حفظ | Save",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(30, 140, 80),
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 9f, FontStyle.Bold),
                Size      = new Size(135, 30),
                Location  = new Point(590, 9),
                Cursor    = Cursors.Hand,
                Visible   = false                // hidden until Edit is clicked
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += (s, e) => SaveAll();
            pnlToolbar.Controls.Add(btnSave);

            // ── Date picker (right side of toolbar) ───────────────────────
            var lblDate = new Label
            {
                Text     = "التاريخ | Date:",
                AutoSize = true,
                Font     = new Font("Segoe UI", 9.5f)
            };
            dtpDate = new DateTimePicker
            {
                Size   = new Size(180, 28),
                Format = DateTimePickerFormat.Short,
                Value  = DateTime.Today
            };
            dtpDate.ValueChanged += (s, e) => OnRefresh();

            // ── Total label (green) ───────────────────────────────────────
            lblTotal = new Label
            {
                Text      = "إجمالي: 0  |  Present: 0",
                AutoSize  = true,
                Font      = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 140, 80)
            };

            pnlToolbar.Controls.AddRange(new Control[] { dtpDate, lblDate, lblTotal });

            // ── Legend strip ──────────────────────────────────────────────
            var pnlLegend = new Panel { Height = 26, BackColor = Color.FromArgb(245, 247, 250) };
            AddLegendDot(pnlLegend, "حاضر | Present", Color.FromArgb(40, 167, 69),   0);
            AddLegendDot(pnlLegend, "غائب | Absent",  Color.FromArgb(220, 53,  69), 160);
            AddLegendDot(pnlLegend, "متأخر | Late",   Color.FromArgb(255, 193,  7), 320);
            Controls.Add(pnlLegend);

            // ── Grid columns ──────────────────────────────────────────────
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Id",   HeaderText = "#",                          FillWeight = 5  });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Name", HeaderText = "اسم الطالب | Student Name", FillWeight = 28 });

            var cboMorning = new DataGridViewComboBoxColumn { Name = "Morning", HeaderText = "الذهاب | Morning", FillWeight = 18 };
            cboMorning.Items.AddRange("Present", "Absent", "Late");
            grid.Columns.Add(cboMorning);

            var cboEvening = new DataGridViewComboBoxColumn { Name = "Evening", HeaderText = "العودة | Evening", FillWeight = 18 };
            cboEvening.Items.AddRange("Present", "Absent", "Late");
            grid.Columns.Add(cboEvening);

            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Notes", HeaderText = "ملاحظات | Notes", FillWeight = 31 });

            // ── Row colour-coding ─────────────────────────────────────────
            grid.CellFormatting += (s, e) =>
            {
                if (e.RowIndex < 0 || grid.Rows[e.RowIndex].IsNewRow) return;
                if (grid.Rows[e.RowIndex].Selected) return;
                var m  = grid.Rows[e.RowIndex].Cells["Morning"].Value?.ToString();
                var ev = grid.Rows[e.RowIndex].Cells["Evening"].Value?.ToString();
                grid.Rows[e.RowIndex].DefaultCellStyle.BackColor =
                    (m == "Absent" || ev == "Absent") ? Color.FromArgb(255, 235, 235) :
                    (m == "Late"   || ev == "Late")   ? Color.FromArgb(255, 248, 215) :
                    Color.White;
            };

            // ── Delete enabled only when row selected ─────────────────────
            grid.SelectionChanged += (s, e) =>
            {
                if (!_isLoading)
                    btnDelete.Enabled = grid.SelectedRows.Count > 0;
            };

            // ── Layout ────────────────────────────────────────────────────
            Resize += (s, e) =>
            {
                PositionExtras();
                pnlLegend.Location = new Point(0, pnlToolbar.Bottom);
                pnlLegend.Width    = Width;
                grid.Location      = new Point(0, pnlToolbar.Bottom + pnlLegend.Height);
                grid.Height        = Height - grid.Top;
            };
            Load += (s, e) =>
            {
                PositionExtras();
                pnlLegend.Location = new Point(0, pnlToolbar.Bottom);
                pnlLegend.Width    = Width;
                grid.Location      = new Point(0, pnlToolbar.Bottom + pnlLegend.Height);
                grid.Height        = Height - grid.Top;
            };
        }

        // ── Toolbar right-side positioning ────────────────────────────────
        private void PositionExtras()
        {
            // date label + picker anchored to right
            dtpDate.Location = new Point(Width - 410, 10);
            var lblDate = pnlToolbar.Controls.OfType<Label>()
                              .FirstOrDefault(l => l.Text.StartsWith("التاريخ"));
            if (lblDate != null) lblDate.Location = new Point(Width - 225, 14);
            lblTotal.Location = new Point(Width - 640, 14);
        }

        private static void AddLegendDot(Panel parent, string text, Color color, int x)
        {
            parent.Controls.Add(new Panel { BackColor = color, Size = new Size(13, 13), Location = new Point(x + 4, 6) });
            parent.Controls.Add(new Label { Text = text, AutoSize = true, Font = new Font("Segoe UI", 9f), Location = new Point(x + 22, 4) });
        }

        // ── BaseListPanel overrides ───────────────────────────────────────
        protected override void OnAdd()
        {
            DataAccess.GenerateDailyAttendance(dtpDate.Value.Date);
            OnRefresh();
        }

        protected override void OnEdit() => ToggleEditMode();

        protected override void OnDelete()
        {
            if (grid.SelectedRows.Count == 0)
            {
                MessageBox.Show("يرجى تحديد صف أولاً | Please select a row first",
                    "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnDelete.Enabled = false;
                return;
            }
            HandleDelete(grid.SelectedRows[0].Index);
        }

        protected override void OnRefresh() => LoadData(txtSearch.Text);
        protected override void OnSearch(string text) => LoadData(text);

        // ── Edit mode toggle ──────────────────────────────────────────────
        private void ToggleEditMode()
        {
            _isEditMode = !_isEditMode;

            // Toggle grid editability (keep Id & Name always read-only)
            grid.ReadOnly = !_isEditMode;
            if (grid.Columns["Id"]   != null) grid.Columns["Id"]!.ReadOnly   = true;
            if (grid.Columns["Name"] != null) grid.Columns["Name"]!.ReadOnly = true;

            if (_isEditMode)
            {
                // Switch Edit → Cancel, show Save
                btnEdit.Text      = "🚫  إلغاء | Cancel";
                btnEdit.BackColor = Color.FromArgb(120, 120, 120);
                btnSave.Visible   = true;

                // Disable Generate/Delete/Refresh while editing
                btnAdd.Enabled     = false;
                btnDelete.Enabled  = false;
                btnRefresh.Enabled = false;
            }
            else
            {
                // Switch Cancel → Edit, hide Save
                btnEdit.Text      = "✏️  تعديل | Edit";
                btnEdit.BackColor = Color.FromArgb(50, 100, 200);
                btnSave.Visible   = false;

                // Re-enable other buttons
                btnAdd.Enabled     = true;
                btnRefresh.Enabled = true;

                OnRefresh();   // reload fresh data (discards unsaved changes on cancel)
            }
        }

        // ── Delete ────────────────────────────────────────────────────────
        private void HandleDelete(int rowIndex)
        {
            var id   = Convert.ToInt32(grid.Rows[rowIndex].Cells["Id"].Value);
            var name = grid.Rows[rowIndex].Cells["Name"].Value?.ToString();

            if (MessageBox.Show(
                    $"هل أنت متأكد من حذف سجل '{name}'؟\nDelete attendance record for '{name}'?",
                    "تأكيد الحذف | Confirm Delete",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    DataAccess.DeleteAttendance(id);
                    _isLoading = true;
                    grid.Rows.RemoveAt(rowIndex);
                    _isLoading = false;
                    _data.RemoveAll(a => a.Id == id);
                    UpdateTotal();
                    btnDelete.Enabled = false;
                }
                catch (Exception ex)
                {
                    _isLoading = false;
                    MessageBox.Show("خطأ في الحذف: " + ex.Message, "خطأ",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ── Save all inline edits ─────────────────────────────────────────
        private void SaveAll()
        {
            // Commit any cell that is currently being edited
            grid.EndEdit();

            try
            {
                foreach (DataGridViewRow row in grid.Rows)
                {
                    if (row.IsNewRow || row.Cells["Id"].Value == null) continue;
                    DataAccess.UpdateAttendance(
                        Convert.ToInt32(row.Cells["Id"].Value),
                        row.Cells["Morning"].Value?.ToString() ?? "Present",
                        row.Cells["Evening"].Value?.ToString() ?? "Present",
                        row.Cells["Notes"].Value?.ToString()   ?? ""
                    );
                }
                MessageBox.Show("تم حفظ جميع البيانات بنجاح ✅", "نجاح",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Exit edit mode (reloads fresh data from DB)
                _isEditMode = true;   // ToggleEditMode will flip it to false
                ToggleEditMode();
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ أثناء الحفظ: " + ex.Message, "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── Load & filter ─────────────────────────────────────────────────
        private void LoadData(string search = "")
        {
            try
            {
                _isLoading = true;
                _data = DataAccess.GetAttendance(dtpDate.Value.Date);
                grid.Rows.Clear();

                var filtered = string.IsNullOrWhiteSpace(search)
                    ? _data
                    : _data.Where(a => a.StudentName
                          .Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

                foreach (var a in filtered)
                    grid.Rows.Add(a.Id, a.StudentName, a.MorningStatus, a.EveningStatus, a.Notes);

                UpdateTotal();
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ في تحميل البيانات: " + ex.Message);
            }
            finally
            {
                _isLoading = false;
                btnDelete.Enabled = grid.SelectedRows.Count > 0;
            }
        }

        private void UpdateTotal()
        {
            int total   = grid.Rows.Count;
            int present = grid.Rows.Cast<DataGridViewRow>()
                .Count(r => r.Cells["Morning"].Value?.ToString() == "Present");
            if (lblTotal != null)
                lblTotal.Text = $"إجمالي: {total}  |  Present: {present}";
        }
    }
}
