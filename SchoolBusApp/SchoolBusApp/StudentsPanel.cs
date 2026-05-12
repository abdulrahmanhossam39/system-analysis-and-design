using System.Drawing;
using System.Windows.Forms;
using SchoolBusApp.Models;

namespace SchoolBusApp
{
    public class StudentsPanel : BaseListPanel
    {
        private List<Student> _data = new();

        public StudentsPanel()
        {
            BuildUI("الطلاب", "Students");
            SetupColumns();
        }

        private void SetupColumns()
        {
            grid.Columns.Clear();
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "#", Width = 50, FillWeight = 5 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "NameAr", HeaderText = "الاسم بالعربي", FillWeight = 20 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "NameEn", HeaderText = "Name (En)", FillWeight = 18 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "School", HeaderText = "المدرسة | School", FillWeight = 15 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Grade", HeaderText = "الصف | Grade", FillWeight = 10 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Parent", HeaderText = "ولي الأمر | Parent", FillWeight = 15 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Phone", HeaderText = "الهاتف | Phone", FillWeight = 12 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Route", HeaderText = "المسار | Route", FillWeight = 12 });
        }

        protected override void OnRefresh() => LoadData("");
        protected override void OnSearch(string t) => LoadData(t);

        private void LoadData(string search)
        {
            try
            {
                _data = DataAccess.GetStudents(search);
                grid.Rows.Clear();
                foreach (var s in _data)
                    grid.Rows.Add(s.Id, s.NameAr, s.NameEn, s.SchoolName, s.Grade, s.ParentName, s.ParentPhone, s.RouteName);
            }
            catch (Exception ex) { MessageBox.Show("خطأ في تحميل البيانات:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        protected override void OnAdd()
        {
            var dlg = BuildStudentDialog(new Student());
            if (dlg.ShowDialog() == DialogResult.OK) OnRefresh();
        }

        protected override void OnEdit()
        {
            if (grid.SelectedRows.Count == 0) { MessageBox.Show("اختر طالباً أولاً | Select a student first"); return; }
            var id = (int)grid.SelectedRows[0].Cells["Id"].Value;
            var s = _data.First(x => x.Id == id);
            var dlg = BuildStudentDialog(s);
            if (dlg.ShowDialog() == DialogResult.OK) OnRefresh();
        }

        protected override void OnDelete()
        {
            if (grid.SelectedRows.Count == 0) { MessageBox.Show("اختر طالباً | Select a student"); return; }
            var id = (int)grid.SelectedRows[0].Cells["Id"].Value;
            var name = grid.SelectedRows[0].Cells["NameAr"].Value?.ToString();
            if (MessageBox.Show($"حذف الطالب: {name}?\nDelete student: {name}?", "تأكيد | Confirm",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                DataAccess.DeleteStudent(id);
                OnRefresh();
            }
        }

        private Form BuildStudentDialog(Student s)
        {
            var routes = DataAccess.GetRoutes();

            var lblNameAr = FormHelper.MakeLbl("الاسم بالعربي *", 350, 20);
            var txtNameAr = FormHelper.MakeTxt(80, 18); txtNameAr.Text = s.NameAr;
            txtNameAr.RightToLeft = RightToLeft.Yes;

            var lblNameEn = FormHelper.MakeLbl("Name (English)", 350, 60);
            var txtNameEn = FormHelper.MakeTxt(80, 58); txtNameEn.Text = s.NameEn;

            var lblSchool = FormHelper.MakeLbl("المدرسة | School *", 350, 100);
            var txtSchool = FormHelper.MakeTxt(80, 98); txtSchool.Text = s.SchoolName;

            var lblGrade = FormHelper.MakeLbl("الصف | Grade", 350, 140);
            var txtGrade = FormHelper.MakeTxt(80, 138, 120); txtGrade.Text = s.Grade;

            var lblParent = FormHelper.MakeLbl("ولي الأمر | Parent Name", 350, 180);
            var txtParent = FormHelper.MakeTxt(80, 178); txtParent.Text = s.ParentName;

            var lblParentPhone = FormHelper.MakeLbl("هاتف ولي الأمر | Parent Phone", 350, 220);
            var txtParentPhone = FormHelper.MakeTxt(80, 218, 160); txtParentPhone.Text = s.ParentPhone;

            var lblPhone = FormHelper.MakeLbl("هاتف الطالب | Student Phone", 350, 260);
            var txtPhone = FormHelper.MakeTxt(80, 258, 160); txtPhone.Text = s.Phone;

            var lblAddress = FormHelper.MakeLbl("العنوان | Address", 350, 300);
            var txtAddress = FormHelper.MakeTxt(80, 298); txtAddress.Text = s.Address;

            var lblRoute = FormHelper.MakeLbl("المسار | Route", 350, 340);
            var cboRoute = FormHelper.MakeCbo(80, 338);
            cboRoute.Items.Add(new ComboItem(0, "-- بدون مسار | No Route --"));
            foreach (var r in routes) cboRoute.Items.Add(new ComboItem(r.Id, r.NameAr));
            cboRoute.SelectedIndex = 0;
            if (s.RouteId.HasValue)
                for (int i = 0; i < cboRoute.Items.Count; i++)
                    if (((ComboItem)cboRoute.Items[i]).Id == s.RouteId) { cboRoute.SelectedIndex = i; break; }

            var btnOk = FormHelper.MakeOkBtn();
            btnOk.Location = new Point(350, 390);
            var btnCancel = FormHelper.MakeCancelBtn();
            btnCancel.Location = new Point(195, 390);

            btnOk.Click += (sender, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtNameAr.Text)) { MessageBox.Show("الاسم بالعربي مطلوب | Arabic name required"); return; }
                s.NameAr = txtNameAr.Text.Trim();
                s.NameEn = txtNameEn.Text.Trim();
                s.SchoolName = txtSchool.Text.Trim();
                s.Grade = txtGrade.Text.Trim();
                s.ParentName = txtParent.Text.Trim();
                s.ParentPhone = txtParentPhone.Text.Trim();
                s.Phone = txtPhone.Text.Trim();
                s.Address = txtAddress.Text.Trim();
                var item = (ComboItem)cboRoute.SelectedItem!;
                s.RouteId = item.Id == 0 ? null : item.Id;
                try { DataAccess.SaveStudent(s); }
                catch (Exception ex) { MessageBox.Show("خطأ | Error: " + ex.Message); return; }
            };

            return FormHelper.WrapInDialog(
                s.Id == 0 ? "إضافة طالب | Add Student" : "تعديل طالب | Edit Student",
                new Control[] { lblNameAr, txtNameAr, lblNameEn, txtNameEn,
                    lblSchool, txtSchool, lblGrade, txtGrade,
                    lblParent, txtParent, lblParentPhone, txtParentPhone,
                    lblPhone, txtPhone, lblAddress, txtAddress,
                    lblRoute, cboRoute, btnOk, btnCancel }, 460);
        }

        private class ComboItem { public int Id; public string Name;
            public ComboItem(int id, string n) { Id = id; Name = n; }
            public override string ToString() => Name; }
    }
}
