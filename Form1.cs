using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ACS_WAPConnectionDetails
{
    public partial class Form1 : Form
    {
        // Used as to hold a value that is not displayed nor changed but
        // should be written back to the registry.
        private uint systemIpAddressCacheTimeout = 0;

        /// <summary>
        /// Defines the distinct registry hives.
        /// </summary>
        public enum RegHive
        {
            CURRENT_USER,
            ALL_USERS,
            LOCAL_MACHINE
        }

        private System.ComponentModel.IContainer components = null;

        // an arraylist of elements containing a user and their connections
        private ArrayList allUsersConnections = new ArrayList();

        // an arraylist of connections for the current user
        private ArrayList currentUserConnections = new ArrayList();

        // an arraylist of the connections for the default user
        private ArrayList defaultUserConnections = new ArrayList();
        
        // ArrayList of Hosts objects
        private HostArray hosts = new HostArray();

        // List of Service objects
        private ServiceList services;

        // Temporary class property used to hold ArrayLists of connections
        private ArrayList enabledUserConnectionList;


        /// <summary>
        /// Initialize the main form for the application.  Reads the registry to create a collection of
        /// User objects which contain ArrayLists of the connections, ArrayLists of Connection objects
        /// for default and current user.  Reads the Windows hosts and service files and stores their data 
        /// in a HostArray object and ServiceList object.
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            ReadRegistry();
            this.hosts = Host.ReadHosts();
            btnAddPorts.Enabled = IsAdministrator();
            services = Service.ReadServices();
            foreach (Service service in services)
                System.Diagnostics.Debug.WriteLine(service.toString());
            //Initialize for current user selected
            cbSystem.Items.Clear();
            if (rbCurrentUser.Enabled)
                enabledUserConnectionList = currentUserConnections;
            else
                enabledUserConnectionList = defaultUserConnections;
            foreach (Connection connection in enabledUserConnectionList)
            {
                cbSystem.Items.Add(connection.name);
            }
            if (cbSystem.Items.Count > 0)
                cbSystem.SelectedIndex = 0;
            // Add the users to the 'All users' list
            foreach (User user in allUsersConnections)
            {
                cbUsers.Items.Add(user.userName);
            }
            if (cbUsers.Items.Count > 0)
                cbUsers.SelectedIndex = 0;
            // Without these calls the system list is truncated initiallyHello Ser
            rbCurrentUser_CheckedChanged(null, null);
            rbDefaultUser_CheckedChanged(null, null);
        }

        /// <summary>  Read the local machine registry and create 3 ArrayList objects:
        /// currentUserConnections, an ArrayList of Connection objects encapsulating each defined connections for the current user
        /// defaultUserConnection, an ArrayList of Connection objects encapsulating each defined connections for the .DEFAULT user
        /// allUsersConnections,  an ArrayList of User objects encapsulating each user, their SID, Name, and defined connections.
        /// Input:  None
        /// Output:  None, modifies class properties
        protected void ReadRegistry()
        {
            if (!IsAdministrator())
            {
                // Create the current user connections
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                currentUserConnections = ReadConnections(RegHive.CURRENT_USER, "software\\ibm\\client access express\\currentversion\\environments\\my connections");
            }
            else
            {   // if the application is running as Administrator the current user hive isn't really the desktop user.
                rbCurrentUser.Enabled = false;
                rbCurrentUser.Checked = false;
                rbDefaultUser.Checked = true;
                this.Text += "    *Running as Administrator";
                rbCurrentUser.Text += "\n   * not available when running as administrator.";
            }
            // Create the default user connections
            defaultUserConnections = ReadConnections(RegHive.ALL_USERS, ".DEFAULT\\Software\\IBM\\Client Access Express\\CurrentVersion\\Environments\\My Connections");
            // Create a collection of all of the local users defined on the system with connections
            allUsersConnections = ReadAllUsers();
        }

        /// <summary>
        /// Find all the local users on this machine.  Filter out those without connections or that the user isn't authorized to.
        /// </summary>
        /// <returns>An ArrayList of User objects</returns>
        private ArrayList ReadAllUsers()
        {
            ArrayList localHive = new ArrayList();
            try
            {
                // Build a list of all the local accounts in the local registry
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\hivelist"))
                {
                    if (key != null)
                    {
                        string[] hiveStrings = key.GetValueNames();
                        foreach (string hive in hiveStrings)
                        {
                            if (hive.StartsWith("\\REGISTRY\\USER\\S", StringComparison.InvariantCultureIgnoreCase))
                            {
                                string strUserName = (string)key.GetValue(hive);
                                if (strUserName.EndsWith("\\NTUSER.DAT\0", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    strUserName = strUserName.Substring(0, strUserName.IndexOf("\\NTUSER.DAT"));
                                    strUserName = strUserName.Substring(strUserName.LastIndexOf("\\") + 1);
                                    System.Diagnostics.Debug.WriteLine("User: " + strUserName + "\t SID: " + hive.Substring(15));
                                    localHive.Add(new User(strUserName, hive.Substring(15)));
                                }
                            }
                        }
                    }
                }
                System.Diagnostics.Debug.WriteLine("____________________________________________");
                for (int i = localHive.Count - 1; i >= 0; i--)
                {
                    User hive = (User)localHive[i];
                    try
                    {
                        using (RegistryKey key = Registry.Users.OpenSubKey(hive.sID + "\\SOFTWARE\\IBM\\Client Access Express\\CurrentVersion\\Environments\\My Connections"))
                        {
                            if (key != null)
                            {
                                hive.setConnections(ReadConnections(RegHive.ALL_USERS,
                                    hive.sID + "\\SOFTWARE\\IBM\\Client Access Express\\CurrentVersion\\Environments\\My Connections"));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.HResult == -2146233078)
                        {
                            System.Diagnostics.Debug.WriteLine("Access Not allowed to " + hive.userName);
                        }
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                        localHive.Remove(hive);
                    }
                }
                return localHive;
            }
            catch (Exception e)
            {
                MessageBox.Show("Error reading the registry:\n" + e.Message, "Error reading registry", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        /// <summary>
        /// Reads the connections defined in the registry hive defined by regHive and the
        /// user defined by strRegPath.
        /// </summary>
        /// <param name="regHive">The section of the Windows registry.</param>
        /// <param name="strRegPath">The registry path for the specific Windows user.</param>
        /// <returns></returns>
        protected ArrayList ReadConnections(RegHive regHive, string strRegPath)
        {
            try
            {
                RegistryKey key = null;
                switch (regHive)
                {
                    case RegHive.CURRENT_USER:
                        key = Registry.CurrentUser.OpenSubKey(strRegPath);
                        break;
                    case RegHive.ALL_USERS:
                        key = Registry.Users.OpenSubKey(strRegPath);
                        break;
                }
                if (key != null)
                {
                    ArrayList connections = new ArrayList();
                    uint uiAdminSystemIndicator = 0;
                    uint uiConnectionTimeout = 0;
                    string strDescription = null;
                    string strIpAddress = null;
                    uint uiIpAddressCacheTimeout = 0;
                    uint uiIpAddressLookupMode = 0;
                    uint uiPersistenceMode = 0;
                    uint uiPortLookupMode = 0;
                    uint uiSecureSocketsLayer = 0;
                    uint uiSignonMode = 0;
                    string strUserId = null;
                    uint uiVersionReleaseLevel = 0;
                    string strSystem = null;

                    string[] systems = key.GetSubKeyNames();
                    foreach (string system in systems)
                    {
                        strSystem = system;
                        RegistryKey syskey = key.OpenSubKey(system);
                        syskey = syskey.OpenSubKey("Communication");
                        Object oKey = syskey.GetValue("Admin System Indicator");
                        if (oKey != null)
                        {
                            uiAdminSystemIndicator = (uint)Convert.ToUInt32(oKey);
                            //MessageBox.Show("Admin System Indicator: " + o.ToString(), system);
                        }
                        oKey = syskey.GetValue("Connect Timeout");
                        if (oKey != null)
                            uiConnectionTimeout = (uint)Convert.ToUInt32(oKey);
                        oKey = syskey.GetValue("Description");
                        if (oKey != null)
                            strDescription = oKey.ToString();
                        oKey = syskey.GetValue("IP Address");
                        if (oKey != null)
                            strIpAddress = oKey.ToString();
                        oKey = syskey.GetValue("IP Address cache timeout");
                        if (oKey != null)
                            uiIpAddressCacheTimeout = (uint)Convert.ToUInt32(oKey);
                        oKey = syskey.GetValue("IP address lookup mode");
                        if (oKey != null)
                            uiIpAddressLookupMode = (uint)Convert.ToUInt32(oKey);
                        oKey = syskey.GetValue("Persistence Mode");
                        if (oKey != null)
                            uiPersistenceMode = (uint)Convert.ToUInt32(oKey);
                        oKey = syskey.GetValue("Port lookup mode");
                        if (oKey != null)
                            uiPortLookupMode = (uint)Convert.ToUInt32(oKey);
                        oKey = syskey.GetValue("Secure Sockets Layer");
                        if (oKey != null)
                            uiSecureSocketsLayer = (uint)Convert.ToUInt32(oKey);
                        oKey = syskey.GetValue("Signon Mode");
                        if (oKey != null)
                            uiSignonMode = (uint)Convert.ToUInt32(oKey);
                        oKey = syskey.GetValue("User ID");
                        if (oKey != null)
                            strUserId = oKey.ToString();
                        oKey = syskey.GetValue("Version Release Level");
                        if (oKey != null)
                            uiVersionReleaseLevel = (uint)Convert.ToUInt32(oKey);

                        connections.Add(new Connection(strSystem, uiAdminSystemIndicator, uiConnectionTimeout,
                            strDescription, strIpAddress, uiIpAddressCacheTimeout, uiIpAddressLookupMode,
                            uiPersistenceMode, uiPortLookupMode, uiSecureSocketsLayer, uiSignonMode,
                            strUserId, uiVersionReleaseLevel));
                    }
                    return connections;
                }
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error reading registry:\n" + ex.Message, "Error reading registry", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        /// <summary>
        /// If the All Users radio button is selected, enable the list of users defined
        /// on this system.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbAllUsers_CheckedChanged(object sender, EventArgs e)
        {
            if (rbAllUsers.Checked)
            {
                cbUsers.Enabled = true;
                cbUsers_SelectedIndexChanged(null, null);
            }
            else
            {
                cbUsers.Enabled = false;
            }
        }

        /// <summary>
        /// If the Current User is selected, update the system list with those defined
        /// under HKeyCurrentUser.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbCurrentUser_CheckedChanged(object sender, EventArgs e)
        {
            if (rbCurrentUser.Checked)
            {
                cbSystem.Items.Clear();
                foreach (Connection connection in currentUserConnections)
                {
                    cbSystem.Items.Add(connection.name);
                }
                if (cbSystem.Items.Count > 0)
                    cbSystem.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// If the Default User has been selected, update the system list with those defined
        /// under the .DEFAULT user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbDefaultUser_CheckedChanged(object sender, EventArgs e)
        {
            if (rbDefaultUser.Checked)
            {
                cbSystem.Items.Clear();
                foreach (Connection connection in defaultUserConnections)
                {
                    cbSystem.Items.Add(connection.name);
                }
                try { cbSystem.SelectedIndex = 0; } catch { }
            }
        }

        /// <summary>
        /// Update the system list based on the user just selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbSystem.Items.Clear();
            User user = (User)allUsersConnections[cbUsers.SelectedIndex];
            foreach (Connection connection in user.connections)
            {
                cbSystem.Items.Add(connection.name);
            }
            if (cbSystem.Items.Count > 0)
                cbSystem.SelectedIndex = 0;
        }

        /// <summary>
        /// Updates the interface with connection values from the selected system
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbSystem_SelectedIndexChanged(object sender, EventArgs e)
        {
            Connection connection = null;
            if (rbCurrentUser.Enabled && rbCurrentUser.Checked)
                connection = (Connection)currentUserConnections[cbSystem.SelectedIndex];
            if (rbDefaultUser.Checked)
                connection = (Connection)defaultUserConnections[cbSystem.SelectedIndex];
            if (rbAllUsers.Checked)
            {
                User user = (User)allUsersConnections[cbUsers.SelectedIndex];
                connection = (Connection)user.getConnection(cbSystem.SelectedIndex);
            }
            systemIpAddressCacheTimeout = connection.ipAddressCacheTimeout;
            tbConnectionTimeout.Text = connection.connectionTimeout.ToString();
            tbDescription.Text = connection.description;
            tbIpAddress.Text = connection.ipAddress;
            cbIpAddressLookupMode.SelectedIndex = (int)connection.ipAddressLookupMode;
            cbPortLookupMode.SelectedIndex = (int)connection.portLookupMode;
            cbSSL.Checked = connection.isSecureSocketsLayer();
            cbSignonMode.SelectedIndex = (int)connection.signonMode;
            tbDefaultUserId.Text = connection.userId;
            lVersion.Text = connection.getVersionReleaseLevelString();
            btnAddHost.Enabled = !hosts.Contains(connection.name);
            btnRemoveFromHosts.Enabled = hosts.Contains(connection.name);
        }

        /// <summary>
        /// Resets the interface to viewing the already configured systems.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            btnRemoveSystem.Enabled = true;
            cbSystem.Enabled = true;
            cbSystem.Visible = true;
            tbSystem.Enabled = false;
            tbSystem.Visible = false;
            btnCancel.Text = "Cancel Change";
            btnUpdate.Text = "Update System";
            cbSystem_SelectedIndexChanged(null, null);
            StatusLabel.Text = "";
        }

        /// <summary>
        /// Performs double-duty by updating an exising system or adding a new system.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!validateValues())
            {
                return;
            }
            string connectionName = null;
            if (btnUpdate.Text == "Update System")
                connectionName = cbSystem.SelectedItem.ToString();
            else  // This is a new system to be added...perform some validation
            {
                ArrayList existingConnections = null;
                connectionName = tbSystem.Text;
                if (string.IsNullOrWhiteSpace(connectionName))
                {
                    MessageBox.Show("A system name must be provided.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tbSystem.Focus();
                    return;
                }
                if (rbCurrentUser.Checked)
                {
                    existingConnections = currentUserConnections;
                }
                if (rbDefaultUser.Checked)
                {
                    existingConnections = defaultUserConnections;
                }
                if (rbAllUsers.Checked)
                {
                    User user = (User)allUsersConnections[cbUsers.SelectedIndex];
                    existingConnections = user.connections;
                }
                foreach (Connection cnn in existingConnections)
                {
                    if (string.Compare(cnn.name, connectionName, StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        MessageBox.Show("The system is already defined for this user.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
            Connection connection = new Connection(
                connectionName, 0,
                (uint)Convert.ToUInt32(tbConnectionTimeout.Text),
                tbDescription.Text,
                tbIpAddress.Text,
                systemIpAddressCacheTimeout,
                (uint)cbIpAddressLookupMode.SelectedIndex, 0,
                (uint)cbPortLookupMode.SelectedIndex,
                (uint)(cbSSL.Checked ? 1 : 0),
                (uint)cbSignonMode.SelectedIndex,
                tbDefaultUserId.Text,
                Connection.VRMString2DWord(lVersion.Text)
                );
            if (rbCurrentUser.Checked)
            {
                // convert the collection to an array, replace this element, convert back to array list, then update the registry
                Connection[] arrayConnection = (Connection[])currentUserConnections.ToArray(typeof(Connection));
                arrayConnection[cbSystem.SelectedIndex] = connection;
                currentUserConnections.Clear();
                currentUserConnections.AddRange(arrayConnection);
                updateConnection(RegHive.CURRENT_USER,
                    "Software\\IBM\\Client Access Express\\CurrentVersion\\Environments\\My Connections\\"
                    + connection.name + "\\Communication", connection);
            }
            if (rbDefaultUser.Checked)
            {
                Connection[] arrayConnection = (Connection[])defaultUserConnections.ToArray(typeof(Connection));
                arrayConnection[cbSystem.SelectedIndex] = connection;
                defaultUserConnections.Clear();
                defaultUserConnections.AddRange(arrayConnection);
                updateConnection(RegHive.ALL_USERS,
                    ".DEFAULT\\Software\\IBM\\Client Access Express\\CurrentVersion\\Environments\\My Connections\\"
                    + connection.name + "\\Communication", connection);
            }
            if (rbAllUsers.Checked)
            {
                User user = (User)allUsersConnections[cbUsers.SelectedIndex];
                ArrayList connections = user.connections;
                Connection[] arrayConnections = (Connection[])connections.ToArray(typeof(Connection));
                arrayConnections[cbSystem.SelectedIndex] = connection;
                connections.Clear();
                connections.AddRange(arrayConnections);
                user.connections = connections;
                updateConnection(RegHive.ALL_USERS, user.sID +
                    "\\SOFTWARE\\IBM\\Client Access Express\\CurrentVersion\\Environments\\My Connections\\"
                    + connection.name + "\\Communication", connection);
            }
            // Refresh the collections to show any new systems.
            ReadRegistry();
            rbAllUsers_CheckedChanged(null, null);
            rbCurrentUser_CheckedChanged(null, null);
            rbDefaultUser_CheckedChanged(null, null);
            btnRemoveSystem.Enabled = true;
        }

        /// <summary>
        /// Performs the Windows registry updates to update an existing system or add
        /// a new one based on the current text (name) of the btnUpdate button.
        /// </summary>
        /// <param name="regHive">The registry hive to be updated.</param>
        /// <param name="strRegPath">The path in the registry hive for the system.</param>
        /// <param name="connection">The connection to be written or overwritten.</param>
        /// <returns></returns>
        private bool updateConnection(RegHive regHive, string strRegPath, Connection connection)
        {
            try
            {
                RegistryKey key = null;
                if (btnUpdate.Text == "Update System")  // Updating an existing system
                {
                    switch (regHive)
                    {
                        case RegHive.CURRENT_USER:
                            key = Registry.CurrentUser.OpenSubKey(strRegPath, true);
                            break;
                        case RegHive.ALL_USERS:
                            key = Registry.Users.OpenSubKey(strRegPath, true);
                            break;
                    }
                    if (key != null)
                    {
                        key.SetValue("Admin System Indicator", connection.adminSystemIndicator, RegistryValueKind.DWord);
                        key.SetValue("Connect Timeout", connection.connectionTimeout, RegistryValueKind.DWord);
                        key.SetValue("Description", connection.description);
                        key.SetValue("IP Address", connection.ipAddress);
                        key.SetValue("IP address lookup mode", connection.ipAddressLookupMode, RegistryValueKind.DWord);
                        key.SetValue("Port lookup mode", connection.portLookupMode, RegistryValueKind.DWord);
                        key.SetValue("Secure Sockets Layer", connection.secureSocketsLayer, RegistryValueKind.DWord);
                        key.SetValue("Signon Mode", connection.signonMode, RegistryValueKind.DWord);
                        key.SetValue("User ID", connection.userId);
                        key.Close();
                    }
                    StatusLabel.Text = "System updated.";
                    return true;
                }
                else // Adding a new system
                {
                    switch (regHive)
                    {
                        case RegHive.CURRENT_USER:
                            key = Registry.CurrentUser.CreateSubKey(strRegPath, true);
                            break;
                        case RegHive.ALL_USERS:
                            key = Registry.Users.CreateSubKey(strRegPath, true);
                            break;
                    }
                    if (key != null)
                    {
                        key.SetValue("Admin System Indicator", connection.adminSystemIndicator, RegistryValueKind.DWord);
                        key.SetValue("Connect Timeout", connection.connectionTimeout, RegistryValueKind.DWord);
                        key.SetValue("Description", connection.description);
                        key.SetValue("IP Address", connection.ipAddress);
                        key.SetValue("IP address lookup mode", connection.ipAddressLookupMode, RegistryValueKind.DWord);
                        key.SetValue("Persistence Mode", 0, RegistryValueKind.DWord);
                        key.SetValue("Port lookup mode", connection.portLookupMode, RegistryValueKind.DWord);
                        key.SetValue("Secure Sockets Layer", connection.secureSocketsLayer, RegistryValueKind.DWord);
                        key.SetValue("Signon Mode", connection.signonMode, RegistryValueKind.DWord);
                        key.SetValue("User ID", connection.userId);
                        key.Close();
                    }
                    StatusLabel.Text = "System Added.";
                    tbSystem.Enabled = false;
                    tbSystem.Text = "";
                    tbSystem.Visible = false;
                    cbSystem.Enabled = true;
                    cbSystem.Visible = true;
                    btnUpdate.Text = "Update System";
                    btnCancel.Text = "Cancel Change";
                    return true;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error updating the registry:\n" + e.Message, "Registry Update Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                StatusLabel.Text = "Add/Update failed.";
                return false;
            }
        }

        /// <summary>
        /// Validate the input values for the connection timeout, IP Address, and Default User ID
        /// </summary>
        /// <returns>
        /// True if all values are in a valid format
        /// </returns>
        private bool validateValues()
        {
            // Validate connection timeout is number between 0 and 3600
            try
            {
                int timeout = Convert.ToInt32(tbConnectionTimeout.Text);
                if (timeout < 0 || timeout > 3600)
                {
                    MessageBox.Show("The connection timeout value must be between 0 and 3600 milliseconds.",
                        "Invalid Input Value", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tbConnectionTimeout.SelectAll();
                    tbConnectionTimeout.Focus();
                    return false;
                }
            }
            catch (Exception e)
            {
                if (e.HResult == -2146233033)
                {
                    MessageBox.Show("The connection timeout value must be an integer value between 0 and 3600 milliseconds.",
                        "Invalid Input Value", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tbConnectionTimeout.SelectAll();
                    tbConnectionTimeout.Focus();
                    return false;
                }
            }

            // Validate the IP address only if lookup mode is set to use saved IP address
            if (string.Compare(cbIpAddressLookupMode.SelectedItem.ToString(), "Never Lookup (Specify IP Address)",
                StringComparison.InvariantCultureIgnoreCase) == 0 &&
                !validateIpAddress(tbIpAddress.Text))
                return false;

            // Validate that the user ID is valid format for IBM i user profile:
            //   The user profile name can be a maximum of 10 characters.The characters can be:
            //    Any letter(A through Z)
            //    Any number(0 through 9)
            //    These special characters: pound(#), dollar ($), underline (_), at (@).
            //    The user profile name cannot begin with a number.
            Regex regex = new Regex("^[A-Z#$_@][0-9A-Z#$_@]+$");
            if ((!regex.IsMatch(tbDefaultUserId.Text.ToUpper()) || tbDefaultUserId.Text.Length > 10)
                && cbSignonMode.SelectedIndex == (int)Connection.SignonMode.USE_DEFAULT_USER)
            {
                MessageBox.Show("The user ID can be a maximum of 10 characters.\nThe characters can be:\n" +
                    "\n - Any letter(A through Z)\n - Any number(0 through 9)\n - These special characters:\n" +
                    "     pound(#)\n     dollar ($)\n     underline (_)\n     at (@)\n - The user profile name cannot begin with a number.",
                                "Invalid Input Value", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbDefaultUserId.SelectAll();
                tbDefaultUserId.Focus();
                return false;
            }
            // Display a warning if the default user is not empty and contains an invalid id name
            if ((!regex.IsMatch(tbDefaultUserId.Text.ToUpper()) || tbDefaultUserId.Text.Length > 10) &&
                (tbDefaultUserId.Text.Length > 0))
            {
                MessageBox.Show("Warning:  The update was saved however; the user ID is invalid.  A user profile can be a maximum of " +
                    "10 characters.\n\nThe characters can be:\n - Any letter(A through Z)\n - Any number(0 through 9)\n - These special characters:\n" +
                    "     pound(#)\n     dollar ($)\n     underline (_)\n     at (@)\n - The user profile name cannot begin with a number.",
                    "Invalid Input Value", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tbDefaultUserId.SelectAll();
                tbDefaultUserId.Focus();
            }
            return true;
        }

        // Validate IP Address is valid format in the form:
        // nnn.nnn.nnn.nnn where 255>=nnn>=000
        // first make sure it contains only digits and periods
        private bool validateIpAddress(string ipAddress)
        {
            Regex regex = new Regex("^[0-9.]+$");
            if (!regex.IsMatch(ipAddress))
            {
                MessageBox.Show("The IP address must be in a valid IPv4 format: nnn.nnn.nnn.nnn where 255 >= nnn >= 0.  It can only contain digits and periods.",
                                "Invalid Input Value", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbIpAddress.SelectAll();
                tbIpAddress.Focus();
                return false;
            }
            // check the octets to be sure that they are 4 valid numbers between 0 and 255 separated by periods
            string[] octets = ipAddress.Split(".");
            if (octets.Length != 4)
            {
                MessageBox.Show("The IP address must be in a valid IPv4 format: nnn.nnn.nnn.nnn where 255 >= nnn >= 0.",
                                "Invalid Input Value", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbIpAddress.SelectAll();
                tbIpAddress.Focus();
                return false;
            }
            try
            {
                for (int i = 0; i < octets.Length; i++)
                {
                    int iOctet = Convert.ToInt32(octets[i]);
                    if (iOctet < 0 || iOctet > 255 || octets[i].Length > 3)
                    {
                        MessageBox.Show("The IP address must be in a valid IPv4 format: nnn.nnn.nnn.nnn where 255 >= nnn >= 0.",
                                        "Invalid Input Value", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        tbIpAddress.SelectAll();
                        tbIpAddress.Focus();
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                if (e.HResult == -2146233033)
                {
                    MessageBox.Show("The IP address must be in a valid IPv4 format: nnn.nnn.nnn.nnn where 255 >= nnn >= 0.",
                                    "Invalid Input Value", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tbIpAddress.SelectAll();
                    tbIpAddress.Focus();
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Adds the selected system to the Windows hosts file using the system name as the
        /// host name and the defined IP address.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddHost_Click(object sender, EventArgs e)
        {
            if (validateIpAddress(tbIpAddress.Text))
            {
                string systemHostName = null;
                if (tbIpAddress.Text.Length < 7)
                {
                    MessageBox.Show("The IP address must be in a valid IPv4 format: nnn.nnn.nnn.nnn where 255 >= nnn >= 0.",
                                    "Invalid Input Value", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tbIpAddress.SelectAll();
                    tbIpAddress.Focus();
                    StatusLabel.Text = "Invalid IP address.";
                    return;
                }
                if (tbSystem.Enabled)
                {
                    if (string.IsNullOrWhiteSpace(tbSystem.Text))
                    {
                        MessageBox.Show("The system name should be a valid host name.",
                                        "Invalid Input Value", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        tbSystem.Focus();
                        StatusLabel.Text = "Invalid IP address.";
                        return;
                    }
                    else
                    {
                        systemHostName = tbSystem.Text;
                    }
                }
                else // cbSystem is enabled
                {
                    systemHostName = cbSystem.Text;
                }
                Host host = new Host(tbIpAddress.Text, systemHostName, tbDescription.Text);
                if (host.addToHostsTable())
                {   // if the update to the hosts table is successful, upate the internal list
                    // and disable the button.
                    hosts.Add(host);
                    if (tbSystem.Enabled)
                    {   // Disable both buttons until the system is added to the registry.
                        // otherwise someone could change the IP or system name can cause
                        // a failing remove function.
                        btnAddHost.Enabled = false;
                        btnRemoveFromHosts.Enabled = false;
                    }
                    else
                        cbSystem_SelectedIndexChanged(null, null);
                }
                StatusLabel.Text = "System added to hosts file.";
            }
        }

        /// <summary>
        /// Invokes the HostsTableForm dialog to display the current contents of the
        /// Windows hosts file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnViewHosts_Click(object sender, EventArgs e)
        {
            HostsTableForm hostsForm = new HostsTableForm(hosts);
            hostsForm.Show();
            StatusLabel.Text = "";
        }

        /// <summary>
        /// Removes the selected system from the Windows hosts file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRemoveFromHosts_Click(object sender, EventArgs e)
        {
            Host host = new Host(tbIpAddress.Text.Trim(), cbSystem.Text, tbDescription.Text.Trim());
            if (host.removeFromHostsTable())
            {
                hosts.Clear();
                hosts = Host.ReadHosts();
                cbSystem_SelectedIndexChanged(null, null);
                StatusLabel.Text = "System removed from hosts file.";
            }
            else
                StatusLabel.Text = "Failed to remove system from hosts file.";
        }

        /// <summary>
        /// Invokes the AboutBox dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAbout_Click(object sender, EventArgs e)
        {
            AboutBox about = new AboutBox();
            about.ShowDialog();
        }

        /// <summary>
        /// Invokes the HostServerPortsForm which can be used to define the ports 
        /// used by the IBM i Access Client Solutions Windows Application Package 
        /// clients and save them to the Windows services file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddPorts_Click(object sender, EventArgs e)
        {
            services = Service.ReadServices();
            HostServerPortsForm portsForm = new HostServerPortsForm(services);
            StatusLabel.Text = "";
            portsForm.ShowDialog();
            portsForm.Dispose();
        }

        /// <summary>
        /// Initialized the interface with default connection values, hides the
        /// system connection combo box and shows the new system text box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNewSystem_Click(object sender, EventArgs e)
        {
            btnRemoveSystem.Enabled = false;
            cbSystem.Enabled = false;
            cbSystem.Visible = false;
            tbSystem.Enabled = true;
            tbSystem.Visible = true;
            btnCancel.Text = "Cancel Add";
            btnUpdate.Text = "Add System";
            btnRemoveFromHosts.Enabled = false;
            btnAddHost.Enabled = true;
            tbConnectionTimeout.Text = "30";
            tbDescription.Text = "";
            tbIpAddress.Text = "";
            cbIpAddressLookupMode.SelectedIndex = 0;
            cbPortLookupMode.SelectedIndex = 0;
            cbSSL.Checked = false;
            cbSignonMode.SelectedIndex = 0;
            tbDefaultUserId.Text = "";
            lVersion.Text = "Unknown";
            StatusLabel.Text = "";
        }

        /// <summary>
        /// Processes the remove system button click.  Removes the selected system 
        /// from the Windows registry.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRemoveSystem_Click(object sender, EventArgs e)
        {
            string connection = cbSystem.SelectedItem.ToString();
            bool removed = false;
            if (rbCurrentUser.Checked)
            {
                removed = removeConnection(RegHive.CURRENT_USER,
                    "Software\\IBM\\Client Access Express\\CurrentVersion\\Environments\\My Connections\\"
                    + connection);
            }
            if (rbDefaultUser.Checked)
            {
                removed = removeConnection(RegHive.ALL_USERS,
                    ".DEFAULT\\Software\\IBM\\Client Access Express\\CurrentVersion\\Environments\\My Connections\\"
                    + connection);
            }
            if (rbAllUsers.Checked)
            {
                User user = (User)allUsersConnections[cbUsers.SelectedIndex];
                removed = removeConnection(RegHive.ALL_USERS, user.sID +
                    "\\SOFTWARE\\IBM\\Client Access Express\\CurrentVersion\\Environments\\My Connections\\"
                    + connection);
            }
            if(removed)
                StatusLabel.Text = "System removed.";
            ReadRegistry();
            rbAllUsers_CheckedChanged(null, null);
            rbCurrentUser_CheckedChanged(null, null);
            rbDefaultUser_CheckedChanged(null, null);
        }

        /// <summary>
        /// Removes the connection defined by the input values from the Windows registry.
        /// </summary>
        /// <param name="regHive">the part of the registry.</param>
        /// <param name="strRegPath">the path to be removed.</param>
        /// <returns></returns>
        private bool removeConnection(RegHive regHive, string strRegPath)
        {
            try
            {
                switch (regHive)
                {
                    case RegHive.CURRENT_USER:
                        Registry.CurrentUser.DeleteSubKeyTree(strRegPath,true);
                        break;
                    case RegHive.ALL_USERS:
                        Registry.Users.DeleteSubKeyTree(strRegPath, true);
                        break;
                }
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Failure updating registry:\n" + e.Message, "Update Failure",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                StatusLabel.Text = "Failed to remove system.";
                return false;
            }
        }

        /// <summary>
        /// Determines if the application is running with elevated authority.
        /// </summary>
        /// <returns>True if application is running elevated.</returns>
        public static bool IsAdministrator()
        {
            return (new WindowsPrincipal(WindowsIdentity.GetCurrent()))
                      .IsInRole(WindowsBuiltInRole.Administrator);
        }

    }
}
