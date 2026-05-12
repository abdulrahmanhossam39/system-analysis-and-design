using System.Drawing;
using System.Windows.Forms;
using SchoolBusApp.Models;

namespace SchoolBusApp
{
    public class RoutesPanel : BaseListPanel
    {
        private List<Route> _data = new();

        public RoutesPanel()
        {
            BuildUI("المسارات", "Routes");
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "#", FillWeight = 5 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "NameAr", HeaderText = "اسم المسار", FillWeight = 20 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "NameEn", HeaderText = "Route Name (En)", FillWeight = 18 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Bus", HeaderText = "الحافلة | Bus", FillWeight = 15 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Depart", HeaderText = "وقت الذهاب | Departure", FillWeight = 14 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Return", HeaderText = "وقت العودة | Return", FillWeight = 14 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Notes", HeaderText = "ملاحظات | Notes", FillWeight = 20 });
        }

        protected override void OnRefresh()
        {
            _data = DataAccess.GetRoutes();
            grid.Rows.Clear();
            foreach (var r in _data)
                grid.Rows.Add(r.Id, r.NameAr, r.NameEn, r.BusPlate, r.DepartureTime, r.ReturnTime, r.Notes);
        }

        protected override void OnAdd()
        {
            if (ShowDialog(new Route()) == DialogResult.OK) OnRefresh();
        }

        protected override void OnEdit()
        {
            if (grid.SelectedRows.Count == 0) { MessageBox.Show("اختر مساراً | Select a route"); return; }
            var id = (int)grid.SelectedRows[0].Cells["Id"].Value;
            if (ShowDialog(_data.First(x => x.Id == id)) == DialogResult.OK) OnRefresh();
        }

        protected override void OnDelete()
        {
            if (grid.SelectedRows.Count == 0) return;
            var id = (int)grid.SelectedRows[0].Cells["Id"].Value;
            if (MessageBox.Show("حذف المسار؟ | Delete route?", "تأكيد", MessageBoxButtons.YesNo) == DialogResult.Yes)
            { DataAccess.DeleteRoute(id); OnRefresh(); }
        }

        private DialogResult ShowDialog(Route ro)
        {
            var buses = DataAccess.GetBuses();

            var lblAr = FormHelper.MakeLbl("اسم المسار بالعربي *", 360, 20);
            var txtAr = FormHelper.MakeTxt(80, 18); txtAr.Text = ro.NameAr;

            var lblEn = FormHelper.MakeLbl("Route Name (English)", 360, 60);
            var txtEn = FormHelper.MakeTxt(80, 58); txtEn.Text = ro.NameEn;

            var lblBus = FormHelper.MakeLbl("الحافلة | Bus", 360, 100);
            var cboBus = FormHelper.MakeCbo(80, 98);
            cboBus.Items.Add(new { Id = 0, Name = "-- بدون حافلة | No Bus --" });
            foreach (var b in buses) cboBus.Items.Add(new { b.Id, Name = b.PlateNumber + " - " + b.Model });
            cboBus.DisplayMember = "Name"; cboBus.ValueMember = "Id";
            cboBus.SelectedIndex = 0;
            if (ro.BusId.HasValue)
                for (int i = 0; i < cboBus.Items.Count; i++)
                    if ((int)((dynamic)cboBus.Items[i]).Id == ro.BusId) { cboBus.SelectedIndex = i; break; }

            var lblDep = FormHelper.MakeLbl("وقت الذهاب | Departure (HH:mm)", 360, 140);
            var txtDep = FormHelper.MakeTxt(80, 138, 100); txtDep.Text = ro.DepartureTime;

            var lblRet = FormHelper.MakeLbl("وقت العودة | Return (HH:mm)", 360, 180);
            var txtRet = FormHelper.MakeTxt(80, 178, 100); txtRet.Text = ro.ReturnTime;

            var lblNotes = FormHelper.MakeLbl("ملاحظات | Notes", 360, 220);
            var txtNotes = FormHelper.MakeTxt(80, 218, 440); txtNotes.Text = ro.Notes;

            var btnOk = FormHelper.MakeOkBtn(); btnOk.Location = new Point(350, 270);
            var btnCancel = FormHelper.MakeCancelBtn(); btnCancel.Location = new Point(195, 270);

            btnOk.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtAr.Text)) { MessageBox.Show("اسم المسار مطلوب"); return; }
                ro.NameAr = txtAr.Text.Trim(); ro.NameEn = txtEn.Text.Trim();
                var sel = (dynamic)cboBus.SelectedItem!;
                ro.BusId = (int)sel.Id == 0 ? null : (int?)sel.Id;
                ro.DepartureTime = txtDep.Text.Trim();
                ro.ReturnTime = txtRet.Text.Trim();
                ro.Notes = txtNotes.Text.Trim();
                try { DataAccess.SaveRoute(ro); } catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); return; }
            };

            return FormHelper.WrapInDialog(
                ro.Id == 0 ? "إضافة مسار | Add Route" : "تعديل مسار | Edit Route",
                new Control[] { lblAr, txtAr, lblEn, txtEn, lblBus, cboBus, lblDep, txtDep, lblRet, txtRet, lblNotes, txtNotes, btnOk, btnCancel }, 360
            ).ShowDialog();
        }
    }
}
