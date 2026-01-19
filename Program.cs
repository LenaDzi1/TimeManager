using System;
using System.Diagnostics;
using System.Windows.Forms;
using TimeManager.Forms;
using TimeManager.Models;

namespace TimeManager
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            AppDomain.CurrentDomain.FirstChanceException += (s, e) =>
            {
                if (e.Exception is InvalidOperationException)
                {
                    Debug.WriteLine("===== FIRST CHANCE InvalidOperationException =====");
                    Debug.WriteLine(e.Exception.ToString());
                    Debug.WriteLine("==================================================");
                }
            };

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Always clear remembered credentials when app exits
            Application.ApplicationExit += (s, e) => LoginForm.ClearSavedCredentials();
            
            // Show login form first
            using (var loginForm = new LoginForm())
            {
                if (loginForm.ShowDialog() == DialogResult.OK && UserSession.IsAuthenticated)
                {
                    // Administrator goes straight to AdminPanelForm
                    if (UserSession.IsAdministrator)
                        Application.Run(new AdminPanelForm());
                    else
                        Application.Run(new MainForm());
                }
                // If login cancelled or failed, application exits
            }
        }
    }
}





