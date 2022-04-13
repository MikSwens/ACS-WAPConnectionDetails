using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ACS_WAPConnectionDetails
{
    public partial class HostsTableForm : Form
    {
        private ArrayList hosts;

        public HostsTableForm()
        {
            InitializeComponent();
        }

        public HostsTableForm(HostArray hosts)
        {
            this.hosts = hosts;
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            dataGridHosts.RowHeadersVisible = false;
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("IP Address");
            dataTable.Columns.Add("Host Name");
            dataTable.Columns.Add("Comment");
            foreach (Host host in hosts)
            {
                DataRow row = dataTable.NewRow();
                row["IP Address"] = host.hostIpAddress;
                row["Host Name"] = host.hostName;
                row["Comment"] = host.hostComment;
                dataTable.Rows.Add(row);
            }
            dataGridHosts.DataSource = dataTable;
            dataGridHosts.Refresh();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
