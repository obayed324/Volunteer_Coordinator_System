using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Volunteer_Coordinator_System
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {

            LoginForm login = new LoginForm();
            login.Show(this); 

        }

        private void btnSignUp_Click(object sender, EventArgs e)
        {
            signUp signUp = new signUp();
            signUp.Show(this);
        }

       
    }
}
