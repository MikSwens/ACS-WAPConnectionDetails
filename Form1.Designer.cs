
using Microsoft.Win32;
using System;
using System.Collections;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ACS_WAPConnectionDetails
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.cbSystem = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbUsers = new System.Windows.Forms.ComboBox();
            this.rbAllUsers = new System.Windows.Forms.RadioButton();
            this.rbDefaultUser = new System.Windows.Forms.RadioButton();
            this.rbCurrentUser = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.cbSignonMode = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.lVersion = new System.Windows.Forms.Label();
            this.btnRemoveFromHosts = new System.Windows.Forms.Button();
            this.tbDefaultUserId = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnAddHost = new System.Windows.Forms.Button();
            this.cbSSL = new System.Windows.Forms.CheckBox();
            this.cbPortLookupMode = new System.Windows.Forms.ComboBox();
            this.cbIpAddressLookupMode = new System.Windows.Forms.ComboBox();
            this.tbIpAddress = new System.Windows.Forms.TextBox();
            this.tbDescription = new System.Windows.Forms.TextBox();
            this.tbConnectionTimeout = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnViewHosts = new System.Windows.Forms.Button();
            this.btnAddPorts = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnAbout = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnNewSystem = new System.Windows.Forms.Button();
            this.btnRemoveSystem = new System.Windows.Forms.Button();
            this.tbSystem = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbSystem
            // 
            this.cbSystem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSystem.FormattingEnabled = true;
            this.cbSystem.Location = new System.Drawing.Point(331, 33);
            this.cbSystem.Name = "cbSystem";
            this.cbSystem.Size = new System.Drawing.Size(234, 23);
            this.cbSystem.TabIndex = 4;
            this.cbSystem.SelectedIndexChanged += new System.EventHandler(this.cbSystem_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbUsers);
            this.groupBox1.Controls.Add(this.rbAllUsers);
            this.groupBox1.Controls.Add(this.rbDefaultUser);
            this.groupBox1.Controls.Add(this.rbCurrentUser);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(299, 153);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Select User";
            // 
            // cbUsers
            // 
            this.cbUsers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbUsers.Enabled = false;
            this.cbUsers.FormattingEnabled = true;
            this.cbUsers.Location = new System.Drawing.Point(21, 115);
            this.cbUsers.Name = "cbUsers";
            this.cbUsers.Size = new System.Drawing.Size(238, 23);
            this.cbUsers.TabIndex = 3;
            this.cbUsers.SelectedIndexChanged += new System.EventHandler(this.cbUsers_SelectedIndexChanged);
            // 
            // rbAllUsers
            // 
            this.rbAllUsers.AutoSize = true;
            this.rbAllUsers.Location = new System.Drawing.Point(15, 90);
            this.rbAllUsers.Name = "rbAllUsers";
            this.rbAllUsers.Size = new System.Drawing.Size(92, 19);
            this.rbAllUsers.TabIndex = 2;
            this.rbAllUsers.Text = "Specific User";
            this.rbAllUsers.UseVisualStyleBackColor = true;
            this.rbAllUsers.CheckedChanged += new System.EventHandler(this.rbAllUsers_CheckedChanged);
            // 
            // rbDefaultUser
            // 
            this.rbDefaultUser.AutoSize = true;
            this.rbDefaultUser.Location = new System.Drawing.Point(15, 56);
            this.rbDefaultUser.Name = "rbDefaultUser";
            this.rbDefaultUser.Size = new System.Drawing.Size(141, 19);
            this.rbDefaultUser.TabIndex = 1;
            this.rbDefaultUser.Text = "Default User (services)";
            this.rbDefaultUser.UseVisualStyleBackColor = true;
            this.rbDefaultUser.CheckedChanged += new System.EventHandler(this.rbDefaultUser_CheckedChanged);
            // 
            // rbCurrentUser
            // 
            this.rbCurrentUser.AutoSize = true;
            this.rbCurrentUser.Checked = true;
            this.rbCurrentUser.Location = new System.Drawing.Point(15, 22);
            this.rbCurrentUser.Name = "rbCurrentUser";
            this.rbCurrentUser.Size = new System.Drawing.Size(91, 19);
            this.rbCurrentUser.TabIndex = 0;
            this.rbCurrentUser.TabStop = true;
            this.rbCurrentUser.Text = "Current User";
            this.rbCurrentUser.UseVisualStyleBackColor = true;
            this.rbCurrentUser.CheckedChanged += new System.EventHandler(this.rbCurrentUser_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.groupBox2.Controls.Add(this.btnUpdate);
            this.groupBox2.Controls.Add(this.cbSignonMode);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.lVersion);
            this.groupBox2.Controls.Add(this.btnRemoveFromHosts);
            this.groupBox2.Controls.Add(this.tbDefaultUserId);
            this.groupBox2.Controls.Add(this.btnCancel);
            this.groupBox2.Controls.Add(this.btnAddHost);
            this.groupBox2.Controls.Add(this.cbSSL);
            this.groupBox2.Controls.Add(this.cbPortLookupMode);
            this.groupBox2.Controls.Add(this.cbIpAddressLookupMode);
            this.groupBox2.Controls.Add(this.tbIpAddress);
            this.groupBox2.Controls.Add(this.tbDescription);
            this.groupBox2.Controls.Add(this.tbConnectionTimeout);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(13, 165);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(553, 366);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Connection Settings";
            // 
            // btnUpdate
            // 
            this.btnUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUpdate.Location = new System.Drawing.Point(440, 317);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(105, 38);
            this.btnUpdate.TabIndex = 24;
            this.btnUpdate.Text = "Update System";
            this.toolTip1.SetToolTip(this.btnUpdate, "Save the changes to the Windows Registry.");
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // cbSignonMode
            // 
            this.cbSignonMode.FormattingEnabled = true;
            this.cbSignonMode.Items.AddRange(new object[] {
            "Not Set",
            "Use Default User (Cache Password)",
            "Prompt Every Time",
            "Use Windows Credentials",
            "Kerberos"});
            this.cbSignonMode.Location = new System.Drawing.Point(171, 218);
            this.cbSignonMode.Name = "cbSignonMode";
            this.cbSignonMode.Size = new System.Drawing.Size(207, 23);
            this.cbSignonMode.TabIndex = 19;
            this.toolTip1.SetToolTip(this.cbSignonMode, resources.GetString("cbSignonMode.ToolTip"));
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(21, 222);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(78, 15);
            this.label13.TabIndex = 22;
            this.label13.Text = "Signon Mode";
            // 
            // lVersion
            // 
            this.lVersion.AutoSize = true;
            this.lVersion.Location = new System.Drawing.Point(171, 286);
            this.lVersion.Name = "lVersion";
            this.lVersion.Size = new System.Drawing.Size(60, 15);
            this.lVersion.TabIndex = 21;
            this.lVersion.Text = "Starting....";
            // 
            // btnRemoveFromHosts
            // 
            this.btnRemoveFromHosts.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveFromHosts.Location = new System.Drawing.Point(154, 317);
            this.btnRemoveFromHosts.Name = "btnRemoveFromHosts";
            this.btnRemoveFromHosts.Size = new System.Drawing.Size(105, 38);
            this.btnRemoveFromHosts.TabIndex = 22;
            this.btnRemoveFromHosts.Text = "Remove From Hosts";
            this.toolTip1.SetToolTip(this.btnRemoveFromHosts, "Remove this IP address, System name and Description to the local hosts file.");
            this.btnRemoveFromHosts.UseVisualStyleBackColor = true;
            this.btnRemoveFromHosts.Click += new System.EventHandler(this.btnRemoveFromHosts_Click);
            // 
            // tbDefaultUserId
            // 
            this.tbDefaultUserId.Location = new System.Drawing.Point(171, 250);
            this.tbDefaultUserId.Name = "tbDefaultUserId";
            this.tbDefaultUserId.Size = new System.Drawing.Size(145, 23);
            this.tbDefaultUserId.TabIndex = 20;
            this.toolTip1.SetToolTip(this.tbDefaultUserId, "The default user ID to be used for connections when the \r\n\'Signon Mode\' is set to" +
        " \'Use Default User (Cache Password)\'.");
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(297, 317);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(105, 38);
            this.btnCancel.TabIndex = 23;
            this.btnCancel.Text = "Cancel Change";
            this.toolTip1.SetToolTip(this.btnCancel, "Remove changes just entered.\r\nNote: this is not an undo operation.");
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnAddHost
            // 
            this.btnAddHost.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddHost.Location = new System.Drawing.Point(11, 317);
            this.btnAddHost.Name = "btnAddHost";
            this.btnAddHost.Size = new System.Drawing.Size(105, 38);
            this.btnAddHost.TabIndex = 21;
            this.btnAddHost.Text = "Add To Hosts";
            this.toolTip1.SetToolTip(this.btnAddHost, "Add this IP address, System name and Description to the local hosts file.");
            this.btnAddHost.UseVisualStyleBackColor = true;
            this.btnAddHost.Click += new System.EventHandler(this.btnAddHost_Click);
            // 
            // cbSSL
            // 
            this.cbSSL.AutoSize = true;
            this.cbSSL.Location = new System.Drawing.Point(171, 190);
            this.cbSSL.Name = "cbSSL";
            this.cbSSL.Size = new System.Drawing.Size(15, 14);
            this.cbSSL.TabIndex = 18;
            this.toolTip1.SetToolTip(this.cbSSL, "System connection will use TLS (fomerly known as secure sockets) for all connecti" +
        "ons.");
            this.cbSSL.UseVisualStyleBackColor = true;
            // 
            // cbPortLookupMode
            // 
            this.cbPortLookupMode.FormattingEnabled = true;
            this.cbPortLookupMode.Items.AddRange(new object[] {
            "Use Port Mapper",
            "Local Lookup (Windows services table)",
            "Use Standard Ports"});
            this.cbPortLookupMode.Location = new System.Drawing.Point(171, 154);
            this.cbPortLookupMode.Name = "cbPortLookupMode";
            this.cbPortLookupMode.Size = new System.Drawing.Size(207, 23);
            this.cbPortLookupMode.TabIndex = 17;
            this.toolTip1.SetToolTip(this.cbPortLookupMode, resources.GetString("cbPortLookupMode.ToolTip"));
            // 
            // cbIpAddressLookupMode
            // 
            this.cbIpAddressLookupMode.FormattingEnabled = true;
            this.cbIpAddressLookupMode.ItemHeight = 15;
            this.cbIpAddressLookupMode.Items.AddRange(new object[] {
            "Lookup Always",
            "Lookup Every Hour",
            "Lookup Once Per Day",
            "Lookup Once Per Week",
            "Never Lookup (Specify IP Address)",
            "Lookup Once Per Windows Startup"});
            this.cbIpAddressLookupMode.Location = new System.Drawing.Point(171, 122);
            this.cbIpAddressLookupMode.Name = "cbIpAddressLookupMode";
            this.cbIpAddressLookupMode.Size = new System.Drawing.Size(207, 23);
            this.cbIpAddressLookupMode.TabIndex = 15;
            this.toolTip1.SetToolTip(this.cbIpAddressLookupMode, resources.GetString("cbIpAddressLookupMode.ToolTip"));
            // 
            // tbIpAddress
            // 
            this.tbIpAddress.Location = new System.Drawing.Point(171, 90);
            this.tbIpAddress.Name = "tbIpAddress";
            this.tbIpAddress.Size = new System.Drawing.Size(207, 23);
            this.tbIpAddress.TabIndex = 13;
            this.toolTip1.SetToolTip(this.tbIpAddress, resources.GetString("tbIpAddress.ToolTip"));
            // 
            // tbDescription
            // 
            this.tbDescription.Location = new System.Drawing.Point(171, 58);
            this.tbDescription.Name = "tbDescription";
            this.tbDescription.Size = new System.Drawing.Size(369, 23);
            this.tbDescription.TabIndex = 12;
            this.toolTip1.SetToolTip(this.tbDescription, "Descriptive text for this system connection.");
            // 
            // tbConnectionTimeout
            // 
            this.tbConnectionTimeout.Location = new System.Drawing.Point(171, 26);
            this.tbConnectionTimeout.MaxLength = 4;
            this.tbConnectionTimeout.Name = "tbConnectionTimeout";
            this.tbConnectionTimeout.Size = new System.Drawing.Size(104, 23);
            this.tbConnectionTimeout.TabIndex = 11;
            this.toolTip1.SetToolTip(this.tbConnectionTimeout, "Connection timeout value in milliseconds.\r\nValid values 0 - 3600\r\nAccepts whole n" +
        "umbers only.");
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(21, 286);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(75, 15);
            this.label11.TabIndex = 11;
            this.label11.Text = "IBM i Version";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(21, 254);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(85, 15);
            this.label10.TabIndex = 10;
            this.label10.Text = "Default User ID";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(21, 190);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(114, 15);
            this.label9.TabIndex = 9;
            this.label9.Text = "Secure Sockets (TLS)";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(21, 158);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(106, 15);
            this.label8.TabIndex = 8;
            this.label8.Text = "Port Lookup Mode";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(21, 126);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(139, 15);
            this.label6.TabIndex = 6;
            this.label6.Text = "IP Address Lookup Mode";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(21, 94);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 15);
            this.label4.TabIndex = 4;
            this.label4.Text = "IP Address";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 62);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 15);
            this.label3.TabIndex = 3;
            this.label3.Text = "Description";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(116, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Connection Timeout";
            // 
            // btnViewHosts
            // 
            this.btnViewHosts.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnViewHosts.Location = new System.Drawing.Point(13, 536);
            this.btnViewHosts.Name = "btnViewHosts";
            this.btnViewHosts.Size = new System.Drawing.Size(167, 37);
            this.btnViewHosts.TabIndex = 102;
            this.btnViewHosts.Text = "View Hosts Entries";
            this.toolTip1.SetToolTip(this.btnViewHosts, "View the contents of the local hosts file.");
            this.btnViewHosts.UseVisualStyleBackColor = true;
            this.btnViewHosts.Click += new System.EventHandler(this.btnViewHosts_Click);
            // 
            // btnAddPorts
            // 
            this.btnAddPorts.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnAddPorts.Location = new System.Drawing.Point(13, 579);
            this.btnAddPorts.Name = "btnAddPorts";
            this.btnAddPorts.Size = new System.Drawing.Size(167, 42);
            this.btnAddPorts.TabIndex = 103;
            this.btnAddPorts.Text = "Add/Edit Host Server Ports In Local Service Table";
            this.toolTip1.SetToolTip(this.btnAddPorts, "Add the host server ports to the local service table.\r\nThese values are used when" +
        " the \'Port Lookup Mode\' is \r\nset to a value of \'Local Lookup (Windows services t" +
        "able)\'");
            this.btnAddPorts.UseVisualStyleBackColor = true;
            this.btnAddPorts.Click += new System.EventHandler(this.btnAddPorts_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(331, 12);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(45, 15);
            this.label12.TabIndex = 5;
            this.label12.Text = "System";
            // 
            // btnAbout
            // 
            this.btnAbout.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnAbout.Image = global::ACS_WAPConnectionDetails.Properties.Resources.Setting_Icon_32;
            this.btnAbout.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAbout.Location = new System.Drawing.Point(401, 579);
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new System.Drawing.Size(167, 42);
            this.btnAbout.TabIndex = 110;
            this.btnAbout.Text = "About...";
            this.btnAbout.UseVisualStyleBackColor = true;
            this.btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 632);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(576, 22);
            this.statusStrip.TabIndex = 105;
            this.statusStrip.Text = "statusStrip1";
            // 
            // StatusLabel
            // 
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // btnNewSystem
            // 
            this.btnNewSystem.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnNewSystem.Location = new System.Drawing.Point(207, 536);
            this.btnNewSystem.Name = "btnNewSystem";
            this.btnNewSystem.Size = new System.Drawing.Size(167, 37);
            this.btnNewSystem.TabIndex = 106;
            this.btnNewSystem.Text = "Add New System";
            this.btnNewSystem.UseVisualStyleBackColor = true;
            this.btnNewSystem.Click += new System.EventHandler(this.btnNewSystem_Click);
            // 
            // btnRemoveSystem
            // 
            this.btnRemoveSystem.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnRemoveSystem.Location = new System.Drawing.Point(207, 579);
            this.btnRemoveSystem.Name = "btnRemoveSystem";
            this.btnRemoveSystem.Size = new System.Drawing.Size(167, 42);
            this.btnRemoveSystem.TabIndex = 107;
            this.btnRemoveSystem.Text = "Remove System From Current Context";
            this.btnRemoveSystem.UseVisualStyleBackColor = true;
            this.btnRemoveSystem.Click += new System.EventHandler(this.btnRemoveSystem_Click);
            // 
            // tbSystem
            // 
            this.tbSystem.Enabled = false;
            this.tbSystem.Location = new System.Drawing.Point(331, 34);
            this.tbSystem.Name = "tbSystem";
            this.tbSystem.Size = new System.Drawing.Size(234, 23);
            this.tbSystem.TabIndex = 111;
            this.tbSystem.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(576, 654);
            this.Controls.Add(this.tbSystem);
            this.Controls.Add(this.btnRemoveSystem);
            this.Controls.Add(this.btnNewSystem);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.btnAbout);
            this.Controls.Add(this.btnViewHosts);
            this.Controls.Add(this.btnAddPorts);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cbSystem);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "IBM ACS Windows Connection Details";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GroupBox groupBox1;
        private RadioButton rbAllUsers;
        private RadioButton rbDefaultUser;
        private RadioButton rbCurrentUser;
        private ComboBox cbUsers;
        private ComboBox cbSystem;
        private GroupBox groupBox2;
        private Label label4;
        private Label label3;
        private Label label2;
        private Label label11;
        private Label label10;
        private Label label9;
        private Label label8;
        private Label label6;
        private Label label12;
        private Label label13;
        private TextBox tbConnectionTimeout;
        private TextBox tbDescription;
        private TextBox tbIpAddress;
        private ComboBox cbIpAddressLookupMode;
        private ComboBox cbPortLookupMode;
        private CheckBox cbSSL;
        private ComboBox cbSignonMode;
        private TextBox tbDefaultUserId;
        private Label lVersion;
        private Button btnUpdate;
        private Button btnCancel;
        private Button btnAddPorts;
        private Button btnAddHost;
        private Button btnRemoveFromHosts;
        private Button btnViewHosts;
        private ToolTip toolTip1;
        private Button btnAbout;
        private StatusStrip statusStrip;
        private Button btnNewSystem;
        private Button btnRemoveSystem;
        private TextBox tbSystem;
        private ToolStripStatusLabel StatusLabel;
    }
}

