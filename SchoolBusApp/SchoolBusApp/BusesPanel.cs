using System.Drawing;
using System.Windows.Forms;
using SchoolBusApp.Models;

namespace SchoolBusApp
{
    public class BusesPanel : UserControl
    {
        private TabControl tabs = null!;

        public BusesPanel()
        {
            Dock = DockStyle.Fill;
            BackColor = Color.FromArgb(245, 247, 250);
            RightToLeft = RightToLeft.Yes;

            var lbl = new Label {
                Text = "الحافلات والسائقون  |  Buses & Drivers",
                Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 40, 80),
                AutoSize = true, Location = new Point(0, 5)
            };
            Controls.Add(lbl);

            tabs = new TabControl {
                Location = new Point(0, 45), Font = new Font("Segoe UI", 10f),
                Dock = DockStyle.None
            };

            var busTab = new TabPage("🚌  الحافلات | Buses");
            busTab.Controls.Add(new BusListPanel { Dock = DockStyle.Fill });

            var driverTab = new TabPage("👨‍✈️  السائقون | Drivers");
            driverTab.Controls.Add(new DriverListPanel { Dock = DockStyle.Fill });

            tabs.TabPages.AddRange(new[] { busTab, driverTab });
            Controls.Add(tabs);

            Resize += (s, e) => { tabs.Size = new Size(Width, Height - 50); };
            Load += (s, e) => { tabs.Size = new Size(Width, Height - 50); };
        }
    }

    // ─────────────────────────────────────────────────────────────────────────────
    public class BusListPanel : BaseListPanel
    {
        private List<Bus> _data = new();

        public BusListPanel()
        {
            BuildUI("الحافلات", "Buses");
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "#", FillWeight = 5 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Plate", HeaderText = "رقم اللوحة | Plate", FillWeight = 20 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Model", HeaderText = "الموديل | Model", FillWeight = 20 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Capacity", HeaderText = "السعة | Capacity", FillWeight = 12 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Driver", HeaderText = "السائق | Driver", FillWeight = 25 });
            lblPageTitle.Visible = false;
        }

        protected override void OnRefresh()
        {
            _data = DataAccess.GetBuses();
            grid.Rows.Clear();
            foreach (var b in _data) grid.Rows.Add(b.Id, b.PlateNumber, b.Model, b.Capacity, b.DriverName);
        }

        protected override void OnAdd()
        {
            if (ShowDialog(new Bus()) == DialogResult.OK) OnRefresh();
        }

        protected override void OnEdit()
        {
            if (grid.SelectedRows.Count == 0) { MessageBox.Show("اختر حافلة | Select a bus"); return; }
            var id = (int)grid.SelectedRows[0].Cells["Id"].Value;
            if (ShowDialog(_data.First(x => x.Id == id)) == DialogResult.OK) OnRefresh();
        }

        protected override void OnDelete()
        {
            if (grid.SelectedRows.Count == 0) return;
            var id = (int)grid.SelectedRows[0].Cells["Id"].Value;
            if (MessageBox.Show("حذف الحافلة؟ | Delete bus?", "تأكيد", MessageBoxButtons.YesNo) == DialogResult.Yes)
            { DataAccess.DeleteBus(id); OnRefresh(); }
        }

        private DialogResult ShowDialog(Bus b)
        {
            var drivers = DataAccess.GetDrivers();

            var lblPlate = FormHelper.MakeLbl("رقم اللوحة | Plate *", 360, 20);
            var txtPlate = FormHelper.MakeTxt(80, 18, 180); txtPlate.Text = b.PlateNumber;

            var lblModel = FormHelper.MakeLbl("الموديل | Model", 360, 60);
            var txtModel = FormHelper.MakeTxt(80, 58, 180); txtModel.Text = b.Model;

            var lblCap = FormHelper.MakeLbl("السعة | Capacity", 360, 100);
            var txtCap = FormHelper.MakeTxt(80, 98, 80); txtCap.Text = b.Capacity.ToString();

            var lblDriver = FormHelper.MakeLbl("السائق | Driver", 360, 140);
            var cboDriver = FormHelper.MakeCbo(80, 138);
            cboDriver.Items.Add(new { Id = 0, Name = "-- بدون سائق | No Driver --" });
            foreach (var d in drivers) cboDriver.Items.Add(new { d.Id, Name = d.NameAr });
            cboDriver.DisplayMember = "Name"; cboDriver.ValueMember = "Id";
            cboDriver.SelectedIndex = 0;
            if (b.DriverId.HasValue)
                for (int i = 0; i < cboDriver.Items.Count; i++)
                    if ((int)((dynamic)cboDriver.Items[i]).Id == b.DriverId) { cboDriver.SelectedIndex = i; break; }

            var btnOk = FormHelper.MakeOkBtn(); btnOk.Location = new Point(350, 210);
            var btnCancel = FormHelper.MakeCancelBtn(); btnCancel.Location = new Point(195, 210);

            btnOk.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtPlate.Text)) { MessageBox.Show("رقم اللوحة مطلوب"); return; }
                b.PlateNumber = txtPlate.Text.Trim();
                b.Model = txtModel.Text.Trim();
                b.Capacity = int.TryParse(txtCap.Text, out int cap) ? cap : 0;
                var sel = (dynamic)cboDriver.SelectedItem!;
                b.DriverId = (int)sel.Id == 0 ? null : (int?)sel.Id;
                try { DataAccess.SaveBus(b); } catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); return; }
            };

            var dlg = FormHelper.WrapInDialog(b.Id == 0 ? "إضافة حافلة | Add Bus" : "تعديل حافلة | Edit Bus",
                new Control[] { lblPlate, txtPlate, lblModel, txtModel, lblCap, txtCap, lblDriver, cboDriver, btnOk, btnCancel }, 290);
            return dlg.ShowDialog();
        }
    }

    // ─────────────────────────────────────────────────────────────────────────────
    public class DriverListPanel : BaseListPanel
    {
        private List<Driver> _data = new();

        public DriverListPanel()
        {
            BuildUI("السائقون", "Drivers");
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "#", FillWeight = 5 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "NameAr", HeaderText = "الاسم بالعربي", FillWeight = 22 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "NameEn", HeaderText = "Name (En)", FillWeight = 20 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Phone", HeaderText = "الهاتف | Phone", FillWeight = 15 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "License", HeaderText = "رقم الرخصة | License", FillWeight = 18 });
            lblPageTitle.Visible = false;
        }

        protected override void OnRefresh()
        {
            _data = DataAccess.GetDrivers();
            grid.Rows.Clear();
            foreach (var d in _data) grid.Rows.Add(d.Id, d.NameAr, d.NameEn, d.Phone, d.LicenseNumber);
        }

        protected override void OnAdd()
        {
            if (ShowDialog(new Driver()) == DialogResult.OK) OnRefresh();
        }

        protected override void OnEdit()
        {
            if (grid.SelectedRows.Count == 0) { MessageBox.Show("اختر سائقاً | Select a driver"); return; }
            var id = (int)grid.SelectedRows[0].Cells["Id"].Value;
            if (ShowDialog(_data.First(x => x.Id == id)) == DialogResult.OK) OnRefresh();
        }

        protected override void OnDelete()
        {
            if (grid.SelectedRows.Count == 0) return;
            var id = (int)grid.SelectedRows[0].Cells["Id"].Value;
            if (MessageBox.Show("حذف السائق؟ | Delete driver?", "تأكيد", MessageBoxButtons.YesNo) == DialogResult.Yes)
            { DataAccess.DeleteDriver(id); OnRefresh(); }
        }

        private DialogResult ShowDialog(Driver d)
        {
            var lblAr = FormHelper.MakeLbl("الاسم بالعربي *", 350, 20);
            var txtAr = FormHelper.MakeTxt(80, 18); txtAr.Text = d.NameAr;
            var lblEn = FormHelper.MakeLbl("Name (English)", 350, 60);
            var txtEn = FormHelper.MakeTxt(80, 58); txtEn.Text = d.NameEn;
            var lblPh = FormHelper.MakeLbl("الهاتف | Phone", 350, 100);
            var txtPh = FormHelper.MakeTxt(80, 98, 160); txtPh.Text = d.Phone;
            var lblLic = FormHelper.MakeLbl("رقم الرخصة | License No.", 350, 140);
            var txtLic = FormHelper.MakeTxt(80, 138, 180); txtLic.Text = d.LicenseNumber;

            var btnOk = FormHelper.MakeOkBtn(); btnOk.Location = new Point(350, 200);
            var btnCancel = FormHelper.MakeCancelBtn(); btnCancel.Location = new Point(195, 200);

            btnOk.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtAr.Text)) { MessageBox.Show("الاسم مطلوب"); return; }
                d.NameAr = txtAr.Text.Trim(); d.NameEn = txtEn.Text.Trim();
                d.Phone = txtPh.Text.Trim(); d.LicenseNumber = txtLic.Text.Trim();
                try { DataAccess.SaveDriver(d); } catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); return; }
            };

            return FormHelper.WrapInDialog(d.Id == 0 ? "إضافة سائق | Add Driver" : "تعديل سائق | Edit Driver",
                new Control[] { lblAr, txtAr, lblEn, txtEn, lblPh, txtPh, lblLic, txtLic, btnOk, btnCancel }, 280).ShowDialog();
        }
    }
}
