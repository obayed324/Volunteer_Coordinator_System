using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace Volunteer_Coordinator_System
{
    public partial class DonorView : Form
    {
        private int _eventId;
        private User currentUser;

        public DonorView(User user, int eventId)
        {
            InitializeComponent();
            currentUser = user;
            _eventId = eventId;
        }

        private void DonorView_Load(object sender, EventArgs e)
        {
            if (this.Owner != null)
                this.Owner.Hide();
        }

        private void DonorView_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.Owner != null)
                this.Owner.Show();
        }

        private void btnDonation_Click(object sender, EventArgs e)
        {
            string donationType = cmbDonationType.SelectedItem?.ToString() ?? "";
            string amountText = txtAmount.Text.Trim();
            DateTime donationDate = DonationDate.Value.Date;

            
            if (string.IsNullOrEmpty(donationType))
            {
                MessageBox.Show("Please select a donation type.");
                return;
            }

            if (!decimal.TryParse(amountText, out decimal amount))
            {
                MessageBox.Show("Please enter a valid numeric amount.");
                return;
            }

            try
            {
                string query = @"INSERT INTO EventDonation 
                                (EventID, UserID, DonationType, Amount, DonationDate)
                                VALUES (@EventID, @UserID, @DonationType, @Amount, @DonationDate)";

                var parameters = new Dictionary<string, object>
                {
                    {"@EventID", _eventId},
                    {"@UserID", currentUser.UserID},
                    {"@DonationType", donationType},
                    {"@Amount", amount},
                    {"@DonationDate", donationDate}
                };

                var result = DbHelper.ExecuteNonQuery(query, parameters);

                if (!result.HasError)
                {
                    MessageBox.Show("Donation recorded successfully. Thank you for your contribution!",
                                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm();
                }
                else
                {
                    MessageBox.Show("Failed to record donation. Please try again.",
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while recording donation: " + ex.Message);
            }
        }

        // ✅ Clear all fields after successful save
        private void ClearForm()
        {
            cmbDonationType.SelectedIndex = -1;
            txtAmount.Clear();
            DonationDate.Value = DateTime.Now;
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void txtAmount_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void DonationDate_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void cmbDonationType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
