using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ACS_WAPConnectionDetails
{
    /// <summary>
    /// This form allows a user to set the port values for the host servers so that they can
    /// be added or updated in the Windows services file.
    /// </summary>
    public partial class HostServerPortsForm : Form
    {
        /// <summary>
        /// Initialize the form with data provided as input in the form of a ServiceList of
        /// Service objects.
        /// </summary>
        /// <param name="services">ServiceList of the services.</param>
        public HostServerPortsForm(ServiceList services)
        {
            InitializeComponent();
            // Initialize the values in the form with either the values from the input
            // or default values if those services are not in the input ServiceList.
            if (services.ContainsServiceName("as-central"))
                tbCentral.Text = services.getServicePort("as-central").ToString();
            else
                tbCentral.Text = "8470";

            if (services.ContainsServiceName("as-central-s"))
                tbCentralS.Text = services.getServicePort("as-central-s").ToString();
            else
                tbCentralS.Text = "9470";

            if (services.ContainsServiceName("as-database"))
                tbDatabase.Text = services.getServicePort("as-database").ToString();
            else
                tbDatabase.Text = "8471";

            if (services.ContainsServiceName("as-database-s"))
                tbDatabaseS.Text = services.getServicePort("as-database-s").ToString();
            else
                tbDatabaseS.Text = "9471";

            if (services.ContainsServiceName("as-dtaq"))
                tbDtaq.Text = services.getServicePort("as-dtaq").ToString();
            else
                tbDtaq.Text = "8472";

            if (services.ContainsServiceName("as-dtaq-s"))
                tbDtaqS.Text = services.getServicePort("as-dtaq-s").ToString();
            else
                tbDtaqS.Text = "9472";

            if (services.ContainsServiceName("as-file"))
                tbFile.Text = services.getServicePort("as-file").ToString();
            else
                tbFile.Text = "8473";

            if (services.ContainsServiceName("as-file-s"))
                tbFileS.Text = services.getServicePort("as-file-s").ToString();
            else
                tbFileS.Text = "9473";

            if (services.ContainsServiceName("as-netprt"))
                tbNetprt.Text = services.getServicePort("as-netprt").ToString();
            else
                tbNetprt.Text = "8474";

            if (services.ContainsServiceName("as-netprt-s"))
                tbNetprtS.Text = services.getServicePort("as-netprt-s").ToString();
            else
                tbNetprtS.Text = "9474";

            if (services.ContainsServiceName("as-rmtcmd"))
                tbRmtcmd.Text = services.getServicePort("as-rmtcmd").ToString();
            else
                tbRmtcmd.Text = "8475";

            if (services.ContainsServiceName("as-rmtcmd-s"))
                tbRmtcmdS.Text = services.getServicePort("as-rmtcmd-s").ToString();
            else
                tbRmtcmdS.Text = "9475";

            if (services.ContainsServiceName("as-signon"))
                tbSignon.Text = services.getServicePort("as-signon").ToString();
            else
                tbSignon.Text = "8476";

            if (services.ContainsServiceName("as-signon-s"))
                tbSignonS.Text = services.getServicePort("as-signon-s").ToString();
            else
                tbSignonS.Text = "9476";

            if (services.ContainsServiceName("drda"))
                tbDrda.Text = services.getServicePort("drda").ToString();
            else
                tbDrda.Text = "446";

            if (services.ContainsServiceName("ddm-s"))
                tbDdmS.Text = services.getServicePort("ddm-s").ToString();
            else
                tbDdmS.Text = "448";

            if (services.ContainsServiceName("telnet-ssl"))
                tbTelnetS.Text = services.getServicePort("telnet-ssl").ToString();
            else
                tbTelnetS.Text = "992";
        }

        /// <summary>
        /// Set the input text boxes to default host server ports.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSetToDefaults_Click(object sender, EventArgs e)
        {
            tbCentral.Text = "8470";
            tbDatabase.Text = "8471";
            tbDtaq.Text = "8472";
            tbFile.Text = "8473";
            tbNetprt.Text = "8474";
            tbRmtcmd.Text = "8475";
            tbSignon.Text = "8476";
            tbCentralS.Text = "9470";
            tbDatabaseS.Text = "9471";
            tbDtaqS.Text = "9472";
            tbFileS.Text = "9473";
            tbNetprtS.Text = "9474";
            tbRmtcmdS.Text = "9475";
            tbSignonS.Text = "9476";
            tbDrda.Text = "446";
            tbDdmS.Text = "448";
            tbTelnetS.Text = "992";
        }

        /// <summary>
        /// Tests if a port is in the range of user defined ports (1024-65535).
        /// </summary>
        /// <param name="value">Port number as a string.</param>
        /// <returns>True if the input value is between 1024 and 65535</returns>
        public bool isValidPortValue(string value)
        {
            try
            {
                if (int.Parse(value) >= 1024 && int.Parse(value) <= 65535)
                    return true;
                else
                    return false;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// TextBox validation routine assigned to all of the text boxes.  Tests that the port values
        /// are in the user defined port range.  Does not test the values for the drda, ddm-ssl or 
        /// telnet-ssl ports which are typically below that range.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void portTextBoxInput_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            List<string> exemptPorts = new List<string> { "tbDrda", "tbDdmS", "tbTelnetS" };
            TextBox tb = (TextBox)sender;
            if (!exemptPorts.Contains(tb.Name, StringComparer.InvariantCultureIgnoreCase))
            {
                if (!isValidPortValue(tb.Text))
                {
                    e.Cancel = true;
                    tb.SelectAll();
                    tb.Focus();
                    MessageBox.Show("Port number must be an integer between 1024 and 65535.", "Invalid Data",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Handles the OK button click to update the Windows services file with the values from this 
        /// form.  Again, tests for valid values before updating the service table and closing the form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            List<string> exemptPorts = new List<string> { "tbDrda", "tbDdmS", "tbTelnetS" };
            // Validate port values are all integers between 1024 and 65535
            foreach (Control control in this.Controls.OfType<System.Windows.Forms.TextBox>())
            {
                try
                {
                    TextBox tb = (TextBox)control;
                    int portNumber = int.Parse(tb.Text);
                    if (!(exemptPorts.Contains(control.Name, StringComparer.InvariantCultureIgnoreCase)) &&
                        !((portNumber >= 1024) && (portNumber <+65535)))
                    {
                        tb.SelectAll();
                        tb.Focus();
                        return;
                    }
                }
                catch(Exception ex)
                {
                    TextBox tb = (TextBox)control;
                    tb.SelectAll();
                    tb.Focus();
                    return;
                }
            }

            // Create a ServiceList of the host server ports:
            ServiceList hostServersList = new ServiceList();
            hostServersList.Add(new Service("as-central",    tbCentral.Text + "/tcp",   labelCentral.Text));
            hostServersList.Add(new Service("as-database",   tbDatabase.Text + "/tcp",  labelDatabase.Text));
            hostServersList.Add(new Service("as-dtaq",       tbDtaq.Text + "/tcp",      labelDtaq.Text));
            hostServersList.Add(new Service("as-file",       tbFile.Text + "/tcp",      labelFile.Text));
            hostServersList.Add(new Service("as-netprt",     tbNetprt.Text + "/tcp",    labelNetprt.Text));
            hostServersList.Add(new Service("as-rmtcmd",     tbRmtcmd.Text + "/tcp",    labelRmtcmd.Text));
            hostServersList.Add(new Service("as-signon",     tbSignon.Text + "/tcp",    labelSignon.Text));
            hostServersList.Add(new Service("as-central-s",  tbCentralS.Text + "/tcp",  labelCentralS.Text));
            hostServersList.Add(new Service("as-database-s", tbDatabaseS.Text + "/tcp", labelDatabaseS.Text));
            hostServersList.Add(new Service("as-dtaq-s",     tbDtaqS.Text + "/tcp",     labelDtaqS.Text));
            hostServersList.Add(new Service("as-file-s",     tbFileS.Text + "/tcp",     labelFileS.Text));
            hostServersList.Add(new Service("as-netprt-s",   tbNetprtS.Text + "/tcp",   labelNetprtS.Text));
            hostServersList.Add(new Service("as-rmtcmd-s",   tbRmtcmdS.Text + "/tcp",   labelRmtcmdS.Text));
            hostServersList.Add(new Service("as-signon-s",   tbSignonS.Text + "/tcp",   labelSignonS.Text));
            hostServersList.Add(new Service("drda",          tbDrda.Text + "/tcp",      labelDrda.Text));
            hostServersList.Add(new Service("ddm-ssl",       tbDdmS.Text + "/tcp",     labelDrdaS.Text));
            hostServersList.Add(new Service("telnet-ssl",    tbTelnetS.Text + "/tcp",   labelTelnetS.Text));

            // Write host server ports out to the service file.
            Service.WriteServices(hostServersList);
            this.Dispose();
        }

        /// <summary>
        /// Closes this form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
