﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace CustomerMaintenance
{
    public partial class frmCustomerMaintenance : Form
    {
        public frmCustomerMaintenance()
        {
            InitializeComponent();
            mmaBooks = new MMABooksEntities();
        }

        private MMABooksEntities mmaBooks;
        private Customer selectedCustomer;

        private void btnGetCustomer_Click(object sender, EventArgs e)
        {
            if (Validator.IsPresent(txtCustomerID) &&
                Validator.IsInt32(txtCustomerID))
            {
                int customerID = Convert.ToInt32(txtCustomerID.Text);
                this.GetCustomer(customerID);
            }
        }

        private void GetCustomer(int CustomerID)
        {
            try
            {
                // Code a query to retrieve the selected customer
                selectedCustomer = (from c in mmaBooks.Customers
                            where c.CustomerID == CustomerID
                            select c).Single();

                if (selectedCustomer == null)
                {
                    MessageBox.Show("No customer found with this ID. " +
                        "Please try again.", "Customer Not Found");
                    this.ClearControls();
                    txtCustomerID.Focus();
                }
                else
                {
                    //  If the customer is found, add code to the GetCustomer method that checks if the State object
                    // has been loaded and that loads if it hasn't.
                  
                    this.DisplayCustomer();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().ToString());
            }
        }

        private void DisplayCustomer()
        {
            txtName.Text = selectedCustomer.Name;
            txtAddress.Text = selectedCustomer.Address;
            txtCity.Text = selectedCustomer.City;
            txtState.Text = selectedCustomer.State.StateName;
            txtZipCode.Text = selectedCustomer.ZipCode;
            btnModify.Enabled = true;
            btnDelete.Enabled = true;
        }

        private void ClearControls()
        {
            txtName.Text = "";
            txtAddress.Text = "";
            txtCity.Text = "";
            txtState.Text = "";
            txtZipCode.Text = "";
            btnModify.Enabled = false;
            btnDelete.Enabled = false;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            frmAddModifyCustomer addModifyCustomerForm = new frmAddModifyCustomer();
            addModifyCustomerForm.addCustomer = true;
            DialogResult result = addModifyCustomerForm.ShowDialog();
            if (result == DialogResult.OK)
            {
                selectedCustomer = addModifyCustomerForm.customer;
                txtCustomerID.Text = selectedCustomer.CustomerID.ToString();
                this.DisplayCustomer();
            }
        }

        private void btnModify_Click(object sender, EventArgs e)
        {
            frmAddModifyCustomer addModifyCustomerForm = new frmAddModifyCustomer();
            addModifyCustomerForm.addCustomer = false;
            addModifyCustomerForm.customer = selectedCustomer;
            DialogResult result = addModifyCustomerForm.ShowDialog();
            if (result == DialogResult.OK || result == DialogResult.Retry)
            {
                selectedCustomer = addModifyCustomerForm.customer;
                this.DisplayCustomer();
            }
            else
            {
                txtCustomerID.Text = "";
                this.ClearControls();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult result =
                MessageBox.Show("Delete " + selectedCustomer.Name + "?",
                "Confirm Delete", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    // Mark the row for deletion.
                    mmaBooks.Customers.Remove((from c in mmaBooks.Customers
                                              where c.CustomerID == selectedCustomer.CustomerID
                                              select c).Single());

                    // Update the database.
                    mmaBooks.SaveChanges();

                    txtCustomerID.Text = "";
                    this.ClearControls();
                }
                // Add concurrency error handling.
                // Place the catch block before the one for a generic exception.

                catch (Exception ex)
                {
                    DbUpdateConcurrencyException(ex as DbUpdateConcurrencyException);
                    MessageBox.Show(ex.Message, ex.GetType().ToString());
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DbUpdateConcurrencyException(DbUpdateConcurrencyException ex)
        {
            if (ex != null)
                ex.Entries.Single().Reload();
        }
    }
}
