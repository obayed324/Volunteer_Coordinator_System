using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace Volunteer_Coordinator_System
{
    public partial class Donationform : Form
    {
        public Donationform()
        {
            InitializeComponent();
        }

        private void Donationform_Load(object sender, EventArgs e)
        {
            if (this.Owner != null)
                this.Owner.Hide();

            LoadDonations();
        }

        private void Donationform_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.Owner != null)
                this.Owner.Show();
        }

        // ==========================
        // LOAD ALL DONATIONS
        // ==========================
        private void LoadDonations()
        {
            try
            {
                string query = @"
                SELECT 
                    d.DonationID,
                    u.Name AS DonorName,
                    u.Email AS DonorEmail,
                    u.Phone,
                    d.DonationType,
                    d.Amount,
                    d.DonationDate
                FROM EventDonation d
                INNER JOIN [User] u ON d.UserID = u.UserID";

                var result = DbHelper.GetQueryData(query);

                if (result.HasError)
                {
                    MessageBox.Show(result.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                dgvDonationDetails.DataSource = result.Data;
                dgvDonationDetails.ClearSelection();

                CalculateTotalDonation(result.Data);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading donations: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==========================
        // CALCULATE TOTAL AMOUNT
        // ==========================
        private void CalculateTotalDonation(DataTable donationTable)
        {
            try
            {
                decimal totalAmount = 0;

                foreach (DataRow row in donationTable.Rows)
                {
                    if (decimal.TryParse(row["Amount"].ToString(), out decimal amount))
                    {
                        totalAmount += amount;
                    }
                }

                lblTotalAmount.Text = $"Total Amount: {totalAmount:C}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error calculating total amount: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==========================
        // REFRESH BUTTON
        // ==========================
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadDonations();
        }

       
    }
}
