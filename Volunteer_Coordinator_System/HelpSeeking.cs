
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Volunteer_Coordinator_System
{
    public partial class HelpSeeking : Form
    {
        private int currentUserId;
        private int currentEventId; 

        public HelpSeeking(int userId, int eventId = 0) 
        {
            InitializeComponent();
            currentUserId = userId;
            currentEventId = eventId;
        }

        private void HelpSeeking_Load(object sender, EventArgs e)
        {
            
            cmbHelpType.Items.Add("Medicine");
            cmbHelpType.Items.Add("Food");
            cmbHelpType.Items.Add("Clothes");
            cmbHelpType.Items.Add("Shelter");
        }
        private void HelpSeeking_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            string helpType = cmbHelpType.SelectedItem?.ToString() ?? "";
            string location = txtLocation.Text.Trim();
            string phone = txtPhone.Text.Trim();

           
            if (string.IsNullOrEmpty(helpType))
            {
                MessageBox.Show("Please select a help type.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(location))
            {
                MessageBox.Show("Please enter your location.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(phone))
            {
                MessageBox.Show("Please enter your phone number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string query = "INSERT INTO EventHelpSeeker (EventID, UserID, HelpType, HelpLocation, Phone) " +
                               "VALUES (@EventID, @UserID, @HelpType, @HelpLocation, @Phone)";

                var parameters = new Dictionary<string, object>()
                {
                    {"@EventID", currentEventId },
                    {"@UserID", currentUserId },
                    {"@HelpType", helpType },
                    {"@HelpLocation", location },
                    {"@Phone", phone }
                };

                var result = DbHelper.ExecuteNonQuery(query, parameters);

                if (!result.HasError)
                {
                    MessageBox.Show("Help request submitted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm();
                }
                else
                {
                    MessageBox.Show("Failed to submit request.\n" + result.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected error: " + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearForm()
        {
            cmbHelpType.SelectedIndex = -1;
            txtLocation.Clear();
            txtPhone.Clear();
        }

       
    }
}

