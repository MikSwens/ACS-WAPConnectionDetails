using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ACS_WAPConnectionDetails
{
    /// <summary>
    /// Host object encapsulates a host table entry.
    /// It has properties of an IP address, a host name and comment text.
    /// Provides methods to read the system host table and return a HostArray of Host objects.
    /// Provides a method for a Host object to add itself to the system hosts table.
    /// </summary>
    class Host
    {
        private string _hostName;
        private string _hostIpAddress;
        private string _hostComment;

        public string hostName { get { return _hostName; } set { _hostName = value; } }
        public string hostIpAddress { get { return _hostIpAddress; } set { _hostIpAddress = value; } }
        public string hostComment { get { return _hostComment; } set { _hostComment = value; } }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="hostIpAddress">The IP address</param>
        /// <param name="hostName">The Host name</param>
        public Host(string hostIpAddress, string hostName)
        {
            this._hostName = hostName;
            this._hostIpAddress = hostIpAddress;
        }

        /// <summary>
        /// Constructor with comment text.
        /// </summary>
        /// <param name="hostIPAddress">The IP address</param>
        /// <param name="hostName">The Host name</param>
        /// <param name="comment">Comment text</param>
        public Host(string hostIPAddress, string hostName, string comment) : this(hostIPAddress, hostName)
        {
            this._hostComment = comment;
        }

        /// <summary>
        /// Reads the Windows hosts file and returns an ArrayList of Host objects.
        /// </summary>
        /// <returns>An ArrayList of Host objects from the Windows hosts file.</returns>
        public static HostArray ReadHosts()
        {
            HostArray hosts = new HostArray();
            string systemRoot = Environment.GetEnvironmentVariable("SYSTEMROOT");
            if (systemRoot == null)
            {   //Set it to the default
                systemRoot = @"C:\Windows";
            }
            string hostPath = systemRoot + @"\System32\drivers\etc\hosts";
            string[] lines = System.IO.File.ReadAllLines(hostPath);
            //            System.Diagnostics.Debug.WriteLine("\n___________________________\nContents of hosts file:");
            foreach (string line in lines)
            {
                //                System.Diagnostics.Debug.WriteLine("\t" + line);
                if (line.Length > 0)
                {
                    int offset = line.TakeWhile(c => char.IsWhiteSpace(c)).Count();
                    if (line.Substring(offset, 1) != "#")
                    {
                        string[] lineElements = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        if (lineElements.Length >= 3)
                            hosts.Add(new Host(lineElements[0], lineElements[1], lineElements[2]));
                        if (lineElements.Length == 2)
                            hosts.Add(new Host(lineElements[0], lineElements[1]));
                    }
                }
            }
            //            System.Diagnostics.Debug.WriteLine("___________________________\n");
            return hosts;
        }

        /// <summary>
        /// Add this host to the Windows hosts file.
        /// </summary>
        /// <returns>True on a successful update.</returns>
        public bool addToHostsTable()
        {
            try
            {
                string systemRoot = Environment.GetEnvironmentVariable("SYSTEMROOT");
                if (systemRoot == null)
                {   //Set it to the default
                    systemRoot = @"C:\Windows";
                }
                string hostPath = systemRoot + @"\System32\drivers\etc\hosts";
                if (!File.Exists(hostPath))
                {   // Create the hosts file if one doesn't exist
                    string[] strIntro = new string[] {
                        "# This file contains the mappings of IP addresses to host names. Each",
                        "# entry should be kept on an individual line. The IP address should",
                        "# be placed in the first column followed by the corresponding host name.",
                        "# The IP address and the host name should be separated by at least one",
                        "# space.",
                        "#",
                        "# Additionally, comments (such as these) may be inserted on individual",
                        "# lines or following the machine name denoted by a '#' symbol.",
                        "#",
                        "# For example:",
                        "#",
                        "#      102.54.94.97     rhino.acme.com          # source server",
                        "#       38.25.63.10     x.acme.com              # x client host",
                        "",
                        "# localhost name resolution is handled within DNS itself.",
                        "#   127.0.0.1       localhost",
                        "#   ::1             localhost",
                        ""
                    };
                    File.WriteAllLines(hostPath, strIntro, Encoding.UTF8);
                }
                // Verify that a Control z isn't the last character in the file, if so remove it
                using (FileStream stream = new FileStream(hostPath, FileMode.Open, FileAccess.ReadWrite))
                {
                    stream.Seek(-1, SeekOrigin.End);
                    byte b = (byte)stream.ReadByte();
                    if (b == 0b_0001_1010)
                        stream.SetLength(stream.Length - 1);
                }
                // Add the host entry
                string hostEntry = _hostIpAddress + "\t" + _hostName;
                if (!string.IsNullOrEmpty(_hostComment))
                    hostEntry += "\t" + _hostComment;
                File.AppendAllText(hostPath, hostEntry + Environment.NewLine);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error updating system hosts file:\n" + e.Message, "Hosts file error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Remove this host from the Windows hosts file
        /// </summary>
        /// <returns>True on successful removal.</returns>
        public bool removeFromHostsTable()
        {
            try
            {
                string systemRoot = Environment.GetEnvironmentVariable("SYSTEMROOT");
                if (systemRoot == null)
                {   //Set it to the default
                    systemRoot = @"C:\Windows";
                }
                string hostPath = systemRoot + @"\System32\drivers\etc\hosts";

                string[] allFileLines = File.ReadAllLines(hostPath);
                //foreach (string line in allFileLines)
                for(int i=0; i<allFileLines.Length; i++)
                {
                    if (allFileLines[i].Length > 0)
                    {
                        int offset = allFileLines[i].TakeWhile(c => char.IsWhiteSpace(c)).Count();
                        if (allFileLines[i].Substring(offset, 1) != "#")
                        {
                            string[] lineElements = allFileLines[i].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                            if (lineElements.Length >= 2)
                            {
                                if((String.Compare(lineElements[0], _hostIpAddress, StringComparison.InvariantCultureIgnoreCase) == 0) &&
                                    (String.Compare(lineElements[1], _hostName, StringComparison.InvariantCultureIgnoreCase) == 0))
                                {
                                    //Remove this entry
                                    allFileLines[i] = null;
                                }
                            }
                        }
                    }
                }
                File.WriteAllLines(hostPath, allFileLines);
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Error updating system hosts file:\n" + e.Message, "Hosts file error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}
