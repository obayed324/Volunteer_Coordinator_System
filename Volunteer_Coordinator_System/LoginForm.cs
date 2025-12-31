using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace Volunteer_Coordinator_System
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            this.Owner?.Hide();
        }

        private void LoginForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Owner?.Show();
        }
        private void clear()
        {
            txtHadleName.Text = string.Empty;
            txtPassword.Text = string.Empty;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string email = txtHadleName.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please fill all fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var parameters = new Dictionary<string, object>
                {
                    {"@Email", email },
                    {"@Password", password }
                };

                string query = @"
                    SELECT 
                        U.UserID, U.Name, U.Email, U.Phone, U.Gender, U.Location, U.BloodGroup,
                        T.UserTypeName
                    FROM [User] U
                    INNER JOIN UserType T ON U.UserTypeID = T.UserTypeID
                    WHERE U.Email = @Email AND U.Password = @Password";

                var result = DbHelper.GetQueryData(query, parameters);

                if (result.Data.Rows.Count > 0)
                {
                    DataRow row = result.Data.Rows[0];
                    string userType = row["UserTypeName"].ToString();

                    Form nextForm = null;

                    switch (userType)
                    {
                        case "Admin":
                            nextForm = new AdminView();
                            break;

                        case "Manager":
                            int managerId = Convert.ToInt32(row["UserID"]);
                            nextForm = new EventManager(managerId);
                            break;

                        case "GeneralUser":
                            User user = new User
                            {
                                UserID = Convert.ToInt32(row["UserID"]),
                                Name = row["Name"].ToString(),
                                Email = row["Email"].ToString(),
                                Phone = row["Phone"].ToString(),
                                Gender = row["Gender"].ToString(),
                                Location = row["Location"].ToString(),
                                BloodGroup = row["BloodGroup"].ToString(),
                                Role = userType
                            };
                            nextForm = new GeneralUser(user);
                            break;
                    }

                    if (nextForm != null)
                    {
                        nextForm.Show(this);
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("No matching view found for this role.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Invalid email or password!", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
