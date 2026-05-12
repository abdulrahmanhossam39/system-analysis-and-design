using System.Drawing;
using System.Windows.Forms;
using SchoolBusApp.Models;

namespace SchoolBusApp
{
    public class PaymentsPanel : BaseListPanel
    {
        private List<Payment> _data = new();
        private ComboBox cboYear = null!;
        private Label lblTotal = null!;

        public PaymentsPanel()
        {
            BuildUI("المدفوعات", "Payments");

            // Add year filter to toolbar
            cboYear = new ComboBox { Size = new Size(90, 28), Font = new Font("Segoe UI", 10f),
                DropDownStyle = ComboBoxStyle.DropDownList };
            cboYear.Items.Add("كل السنوات | All");
            for (int y = DateTime.Now.Year; y >= DateTime.Now.Year - 5; y--) cboYear.Items.Add(y.ToString());
            cboYear.SelectedIndex = 0;
            cboYear.SelectedIndexChanged += (s, e) => OnRefresh();
            pnlToolbar.Controls.Add(cboYear);

            lblTotal = new Label { AutoSize = true, Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 140, 80) };
            pnlToolbar.Controls.Add(lblTotal);

            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "#", FillWeight = 5 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Student", HeaderText = "الطالب | Student", FillWeight = 22 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Amount", HeaderText = "المبلغ | Amount", FillWeight = 12 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Month", HeaderText = "الشهر | Month", FillWeight = 12 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Year", HeaderText = "السنة | Year", FillWeight = 8 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Date", HeaderText = "تاريخ الدفع | Date", FillWeight = 14 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "الحالة | Status", FillWeight = 12 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Notes", HeaderText = "ملاحظات | Notes", FillWeight = 20 });

            Resize += (s, e) => PositionExtras();
            Load += (s, e) => PositionExtras();
        }

        private void PositionExtras()
        {
            cboYear.Location = new Point(Width - 360, 10);
            lblTotal.Location = new Point(Width - 440, 14);
        }

        protected override void OnRefresh() => LoadData(txtSearch.Text);
        protected override void OnSearch(string t) => LoadData(t);

        private void LoadData(string search)
        {
            try
            {
                int year = cboYear.SelectedIndex > 0 ? int.Parse(cboYear.SelectedItem!.ToString()!) : 0;
                _data = DataAccess.GetPayments(search, year);
                grid.Rows.Clear();
                foreach (var p in _data)
                {
                    int ri = grid.Rows.Add(p.Id, p.StudentName, p.Amount.ToString("N2") + " ر.س",
                        p.Month, p.Year, p.PaymentDate.ToShortDateString(), p.Status, p.Notes);
                    if (p.Status == "Pending")
                        grid.Rows[ri].DefaultCellStyle.BackColor = Color.FromArgb(255, 245, 220);
                }
                var total = _data.Sum(x => x.Amount);
                lblTotal.Text = $"الإجمالي | Total: {total:N0} ر.س";
            }
            catch (Exception ex) { MessageBox.Show("خطأ | Error: " + ex.Message); }
        }

        protected override void OnAdd()
        {
            if (ShowDialog(new Payment()) == DialogResult.OK) OnRefresh();
        }

        protected override void OnEdit()
        {
            if (grid.SelectedRows.Count == 0) { MessageBox.Show("اختر سجلاً | Select a record"); return; }
            var id = (int)grid.SelectedRows[0].Cells["Id"].Value;
            if (ShowDialog(_data.First(x => x.Id == id)) == DialogResult.OK) OnRefresh();
        }

        protected override void OnDelete()
        {
            if (grid.SelectedRows.Count == 0) return;
            var id = (int)grid.SelectedRows[0].Cells["Id"].Value;
            if (MessageBox.Show("حذف السجل؟ | Delete record?", "تأكيد", MessageBoxButtons.YesNo) == DialogResult.Yes)
            { DataAccess.DeletePayment(id); OnRefresh(); }
        }

        private DialogResult ShowDialog(Payment p)
        {
            var students = DataAccess.GetStudents();
            var months = new[] { "يناير", "فبراير", "مارس", "أبريل", "مايو", "يونيو",
                                  "يوليو", "أغسطس", "سبتمبر", "أكتوبر", "نوفمبر", "ديسمبر" };

            var lblStudent = FormHelper.MakeLbl("الطالب | Student *", 370, 20);
            var cboStudent = FormHelper.MakeCbo(80, 18, 280);
            foreach (var s in students) cboStudent.Items.Add(new { s.Id, Name = s.NameAr });
            cboStudent.DisplayMember = "Name"; cboStudent.ValueMember = "Id";
            if (p.StudentId > 0)
                for (int i = 0; i < cboStudent.Items.Count; i++)
                    if ((int)((dynamic)cboStudent.Items[i]).Id == p.StudentId) { cboStudent.SelectedIndex = i; break; }

            var lblAmt = FormHelper.MakeLbl("المبلغ | Amount (ر.س) *", 370, 60);
            var txtAmt = FormHelper.MakeTxt(80, 58, 120);
            txtAmt.Text = p.Amount > 0 ? p.Amount.ToString() : "";

            var lblMonth = FormHelper.MakeLbl("الشهر | Month", 370, 100);
            var cboMonth = FormHelper.MakeCbo(80, 98, 160);
            foreach (var m in months) cboMonth.Items.Add(m);
            if (!string.IsNullOrEmpty(p.Month)) cboMonth.SelectedItem = p.Month;
            else cboMonth.SelectedIndex = DateTime.Today.Month - 1;

            var lblYear = FormHelper.MakeLbl("السنة | Year", 370, 140);
            var txtYear = FormHelper.MakeTxt(80, 138, 80);
            txtYear.Text = p.Year > 0 ? p.Year.ToString() : DateTime.Today.Year.ToString();

            var lblDate = FormHelper.MakeLbl("تاريخ الدفع | Payment Date", 370, 180);
            var dtp = new DateTimePicker { Location = new Point(80, 178), Size = new Size(160, 28),
                Format = DateTimePickerFormat.Short, Value = p.Id == 0 ? DateTime.Today : p.PaymentDate };

            var lblStatus = FormHelper.MakeLbl("الحالة | Status", 370, 220);
            var cboStatus = FormHelper.MakeCbo(80, 218, 140);
            cboStatus.Items.AddRange(new object[] { "Paid", "Pending", "Partial" });
            cboStatus.SelectedItem = p.Status != "" ? p.Status : "Paid";

            var lblNotes = FormHelper.MakeLbl("ملاحظات | Notes", 370, 260);
            var txtNotes = FormHelper.MakeTxt(80, 258, 440); txtNotes.Text = p.Notes;

            var btnOk = FormHelper.MakeOkBtn(); btnOk.Location = new Point(360, 310);
            var btnCancel = FormHelper.MakeCancelBtn(); btnCancel.Location = new Point(205, 310);

            btnOk.Click += (s, e) =>
            {
                if (cboStudent.SelectedItem == null) { MessageBox.Show("اختر الطالب"); return; }
                if (!decimal.TryParse(txtAmt.Text, out decimal amt) || amt <= 0)
                { MessageBox.Show("أدخل مبلغاً صحيحاً | Enter valid amount"); return; }
                p.StudentId = (int)((dynamic)cboStudent.SelectedItem).Id;
                p.Amount = amt;
                p.Month = cboMonth.SelectedItem?.ToString() ?? "";
                p.Year = int.TryParse(txtYear.Text, out int yr) ? yr : DateTime.Today.Year;
                p.PaymentDate = dtp.Value;
                p.Status = cboStatus.SelectedItem?.ToString() ?? "Paid";
                p.Notes = txtNotes.Text.Trim();
                try { DataAccess.SavePayment(p); } catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); return; }
            };

            return FormHelper.WrapInDialog(
                p.Id == 0 ? "إضافة دفعة | Add Payment" : "تعديل دفعة | Edit Payment",
                new Control[] { lblStudent, cboStudent, lblAmt, txtAmt, lblMonth, cboMonth,
                    lblYear, txtYear, lblDate, dtp, lblStatus, cboStatus, lblNotes, txtNotes, btnOk, btnCancel }, 390
            ).ShowDialog();
        }
    }
}
