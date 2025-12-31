using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Volunteer_Coordinator_System
{
    public partial class VolunteerView : Form
    {
        private int currentUserId;
        private int currentEventId;

        public VolunteerView(int userId, int eventId)
        {
            InitializeComponent();
            currentUserId = userId;
            currentEventId = eventId;
        }

        private void VolunteerView_Load(object sender, EventArgs e)
        {
            // Set default values
            txtRole.Text = "Volunteer";
            txtStatus.Text = "Pending";
        }

        private void btnVolunteer_Click(object sender, EventArgs e)
        {
            // Confirm user wants to apply
            DialogResult dialogResult = MessageBox.Show(
                "Do you want to apply as a Volunteer for this event?",
                "Confirm Application",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    string query = "INSERT INTO EventVolunteer (EventID, UserID, AssignedRole, Status) " +
                                   "VALUES (@EventID, @UserID, @AssignedRole, @Status)";

                    var parameters = new Dictionary<string, object>()
                    {
                        {"@EventID", currentEventId },
                        {"@UserID", currentUserId },
                        {"@AssignedRole", txtRole.Text },
                        {"@Status", txtStatus.Text }
                    };

                    var result = DbHelper.ExecuteNonQuery(query, parameters);

                    if (!result.HasError)
                    {
                        MessageBox.Show("You have successfully applied as a Volunteer!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        btnVolunteer.Enabled = false; // Disable after applying
                    }
                    else
                    {
                        MessageBox.Show("Failed to apply.\n" + result.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unexpected error: " + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
