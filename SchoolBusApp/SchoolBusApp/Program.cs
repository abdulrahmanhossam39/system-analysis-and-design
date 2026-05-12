using System.Windows.Forms;

namespace SchoolBusApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetHighDpiMode(HighDpiMode.SystemAware);

            // Try to initialize database
            try
            {
                Database.InitializeDatabase();
            }
            catch (Exception ex)
            {
                var result = MessageBox.Show(
                    "⚠️  تعذّر الاتصال بقاعدة البيانات.\nCould not connect to the database.\n\n" +
                    ex.Message + "\n\n" +
                    "هل تريد المتابعة وتعديل الإعدادات؟\nContinue to Settings to configure the connection?",
                    "خطأ في الاتصال | Connection Error",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.No) return;
            }

            Application.Run(new MainForm());
        }
    }
}
