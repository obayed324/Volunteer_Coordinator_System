using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace Volunteer_Coordinator_System
{
    public partial class VolunteerAndHelpSeekerManageByAdmin : Form
    {
        public VolunteerAndHelpSeekerManageByAdmin()
        {
            InitializeComponent();
        }

        private void VolunteerAndHelpSeekerManageByAdmin_Load(object sender, EventArgs e)
        {
            if (this.Owner != null)
                this.Owner.Hide();

            LoadHelpSeekers();
            LoadVolunteers();
        }

        private void VolunteerAndHelpSeekerManageByAdmin_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.Owner != null)
                this.Owner.Show();
        }

        // ==========================
        // HELP SEEKER MANAGEMENT (Admin sees all)
        // ==========================
        private void LoadHelpSeekers()
        {
            try
            {
                string query = @"
                SELECT ehs.HelpID AS HelpSeekerID, u.Name, u.Email, e.EventName, e.EventDate, ehs.HelpType, ehs.HelpLocation, ehs.Status
                FROM EventHelpSeeker ehs
                INNER JOIN [User] u ON ehs.UserID = u.UserID
                INNER JOIN Event e ON ehs.EventID = e.EventID";

                var result = DbHelper.GetQueryData(query);
                if (result.HasError)
                {
                    MessageBox.Show(result.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                dgvHelpSeekers.DataSource = result.Data;
                dgvHelpSeekers.ClearSelection();
                txtHelpStatus.Clear();
                txtHelpStatus.Tag = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading help seekers: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvHelpSeekers_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dgvHelpSeekers.Rows[e.RowIndex];
                selectedRow.Selected = true;

                txtHelpStatus.Text = selectedRow.Cells["Status"].Value?.ToString() ?? "";
                txtHelpStatus.Tag = selectedRow.Cells["HelpSeekerID"].Value;
            }
        }

        private void btnHelpRefresh_Click(object sender, EventArgs e)
        {
            LoadHelpSeekers();
        }

        private void btnHelpConfirm_Click(object sender, EventArgs e)
        {
            if (txtHelpStatus.Tag == null)
            {
                MessageBox.Show("Please select a Help Seeker first.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            UpdateHelpSeekerStatus(Convert.ToInt32(txtHelpStatus.Tag), "Confirmed");
        }

        private void btnHelpReject_Click(object sender, EventArgs e)
        {
            if (txtHelpStatus.Tag == null)
            {
                MessageBox.Show("Please select a Help Seeker first.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            UpdateHelpSeekerStatus(Convert.ToInt32(txtHelpStatus.Tag), "Rejected");
        }

        private void UpdateHelpSeekerStatus(int helpId, string newStatus)
        {
            try
            {
                string query = "UPDATE EventHelpSeeker SET Status = @Status WHERE HelpID = @HelpID";
                var parameters = new Dictionary<string, object>
                {
                    { "@Status", newStatus },
                    { "@HelpID", helpId }
                };

                var result = DbHelper.ExecuteNonQuery(query, parameters);
                if (result.HasError)
                {
                    MessageBox.Show(result.Message, "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MessageBox.Show($"Help Seeker status updated to '{newStatus}'.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadHelpSeekers();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating status: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==========================
        // VOLUNTEER MANAGEMENT (Admin sees all)
        // ==========================
        private void LoadVolunteers()
        {
            try
            {
                string query = @"
                SELECT ev.EventVolunteerID, u.Name, u.Email, e.EventName, e.EventDate, ev.AssignedRole, ev.Status
                FROM EventVolunteer ev
                INNER JOIN [User] u ON ev.UserID = u.UserID
                INNER JOIN Event e ON ev.EventID = e.EventID";

                var result = DbHelper.GetQueryData(query);
                if (result.HasError)
                {
                    MessageBox.Show(result.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                dgvVolunteer.DataSource = result.Data;
                dgvVolunteer.ClearSelection();
                txtVolunteerStatus.Clear();
                txtVolunteerStatus.Tag = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading volunteers: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvVolunteer_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                dgvVolunteer.ClearSelection();
                dgvVolunteer.Rows[e.RowIndex].Selected = true;

                DataGridViewRow row = dgvVolunteer.Rows[e.RowIndex];
                txtVolunteerStatus.Text = row.Cells["Status"].Value?.ToString() ?? "";
                txtVolunteerStatus.Tag = row.Cells["EventVolunteerID"].Value;
            }
        }

        private void btnVolunteerRefresh_Click(object sender, EventArgs e)
        {
            LoadVolunteers();
        }

        private void btnVolunteerConfirm_Click(object sender, EventArgs e)
        {
            if (txtVolunteerStatus.Tag == null)
            {
                MessageBox.Show("Please select a Volunteer first.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            UpdateVolunteerStatus(Convert.ToInt32(txtVolunteerStatus.Tag), "Confirmed");
        }

        private void btnVolunteerReject_Click(object sender, EventArgs e)
        {
            if (txtVolunteerStatus.Tag == null)
            {
                MessageBox.Show("Please select a Volunteer first.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            UpdateVolunteerStatus(Convert.ToInt32(txtVolunteerStatus.Tag), "Rejected");
        }

        private void UpdateVolunteerStatus(int volunteerId, string newStatus)
        {
            try
            {
                string query = "UPDATE EventVolunteer SET Status = @Status WHERE EventVolunteerID = @VolunteerID";
                var parameters = new Dictionary<string, object>
                {
                    { "@Status", newStatus },
                    { "@VolunteerID", volunteerId }
                };

                var result = DbHelper.ExecuteNonQuery(query, parameters);
                if (result.HasError)
                {
                    MessageBox.Show(result.Message, "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MessageBox.Show($"Volunteer status updated to '{newStatus}'.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadVolunteers();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating volunteer status: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        
    }
}
