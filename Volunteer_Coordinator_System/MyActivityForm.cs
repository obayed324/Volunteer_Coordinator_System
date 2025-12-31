using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace Volunteer_Coordinator_System
{
    public partial class MyActivityForm : Form
    {
        private User currentUser;

        public MyActivityForm(User user)
        {
            InitializeComponent();
            currentUser = user;
        }

        private void MyActivityForm_Load(object sender, EventArgs e)
        {
            if (this.Owner != null)
                this.Owner.Hide();

            LoadMyActivities();
        }

        private void MyActivityForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.Owner != null)
                this.Owner.Show();
        }

        private void LoadMyActivities()
        {
            try
            {
                string query = @"
                SELECT 
                    e.EventName,
                    ISNULL(v.Status, 'No') AS Volunteering,
                    CASE 
                        WHEN d.Amount IS NOT NULL THEN CAST(d.Amount AS NVARCHAR(20))
                        ELSE 'No'
                    END AS Donation,
                    ISNULL(h.Status, 'No') AS HelpSeeking
                FROM Event e
                LEFT JOIN EventVolunteer v ON e.EventID = v.EventID AND v.UserID = @UserID
                LEFT JOIN EventHelpSeeker h ON e.EventID = h.EventID AND h.UserID = @UserID
                LEFT JOIN EventDonation d ON e.EventID = d.EventID AND d.UserID = @UserID
                WHERE v.UserID = @UserID OR h.UserID = @UserID OR d.UserID = @UserID";

                var parameters = new Dictionary<string, object>
                {
                    { "@UserID", currentUser.UserID }
                };

                var result = DbHelper.GetQueryData(query, parameters);

                if (result.HasError)
                {
                    MessageBox.Show(result.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Create a new DataTable with Status column
                DataTable dt = result.Data;
                if (!dt.Columns.Contains("Status"))
                    dt.Columns.Add("Status", typeof(string));

                // Set Status column based on other values
                foreach (DataRow row in dt.Rows)
                {
                    string status = "Pending";

                    if (row["Volunteering"].ToString().Trim().Equals("Yes", StringComparison.OrdinalIgnoreCase) ||
                        row["HelpSeeking"].ToString().Trim().Equals("Yes", StringComparison.OrdinalIgnoreCase) ||
                        row["Donation"].ToString() != "No")
                    {
                        status = "Completed";
                    }

                    row["Status"] = status;
                }

                dgvMyActivity.DataSource = dt;

                // --- DataGridView setup ---
                dgvMyActivity.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvMyActivity.ReadOnly = true;
                dgvMyActivity.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvMyActivity.MultiSelect = false;
                dgvMyActivity.AllowUserToAddRows = false;

                dgvMyActivity.Columns["EventName"].HeaderText = "Event Name";
                dgvMyActivity.Columns["Volunteering"].HeaderText = "Volunteering";
                dgvMyActivity.Columns["Donation"].HeaderText = "Donation";
                dgvMyActivity.Columns["HelpSeeking"].HeaderText = "Help Seeking";
                dgvMyActivity.Columns["Status"].HeaderText = "Status";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading activities: " + ex.Message);
            }
        }
    }
}
