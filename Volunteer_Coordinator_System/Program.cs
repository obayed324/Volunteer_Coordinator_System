using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Volunteer_Coordinator_System
{
    internal static class Program
    {
       
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new MainForm());
            Application.Run(new MainForm());
            //Application.Run(new VolunteerAndHelpSeekerManageByAdmin());

        }
    }
}
