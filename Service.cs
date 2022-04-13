using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;

namespace ACS_WAPConnectionDetails
{
    /// <summary>
    /// This class represents a service as a service name, port, protocol, aliases and comment text.
    /// </summary>
    public class Service
    {
        private string _name;
        private string _portAndProtocol;
        private int _port;
        private string _protocol;
        private List<string> _aliases;
        private string _comment;

        public string name { get { return _name; } set { _name = value; } }
        public string portAndProtocol { get { return _portAndProtocol; } set { _portAndProtocol = value; } }
        public int port { get { return _port; } set { _port = value; } }
        public string protocol { get { return _protocol; } set { _protocol = value; } }
        public List<string> aliases { get { return _aliases; } set { _aliases = value; } }
        public string comment { get { return _comment; } set { _comment = value; } }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serviceName">The name of the service</param>
        /// <param name="servicePort">The port and protocol pair</param>
        public Service(string serviceName, string servicePort)
        {
            this._name = serviceName;
            this._portAndProtocol = servicePort;
            string[] strPortAndProtocol = servicePort.Split('/');
            _port = int.Parse(strPortAndProtocol[0]);
            _protocol = strPortAndProtocol[1];
            this._aliases = new List<string>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serviceName">The name of the service</param>
        /// <param name="servicePort">The port and protocol pair</param>
        /// <param name="comment">Comment text</param>
        public Service(string serviceName, string servicePort, string comment) : this(serviceName, servicePort)
        {
            this._comment = comment;
        }

        /// <summary>
        /// Add an alias to the service.
        /// </summary>
        /// <param name="alias">the service name alias</param>
        public void addAlias(string alias) { this._aliases.Add(alias); }

        /// <summary>
        /// Formats the service information in the same format as the Windows services file.
        /// </summary>
        /// <returns>
        /// string - the service information in the same format as the Windows services file.
        /// </returns>
        public string toString()
        {   // According to RFC 6335, the maximum length for a service name is 15 characters
            string strAliases = "";
            foreach(string aliasName in _aliases)
            {
                strAliases += aliasName + " ";
            }
            return _name.PadRight(18) + _portAndProtocol.PadRight(11) + strAliases.PadRight(23) + _comment;
        }

        /// <summary>
        /// Determines if the service is equal to the one provided as input.
        /// </summary>
        /// <param name="compareService">Service to be compared</param>
        /// <returns>True if the services are strictly equal.</returns>
        public bool Equals(Service compareService)
        {
            if (string.Compare(this._portAndProtocol, compareService.portAndProtocol, StringComparison.InvariantCultureIgnoreCase) != 0)
                return false;
            if (string.Compare(this._name, compareService.name, StringComparison.InvariantCultureIgnoreCase) != 0)
                return false;
            foreach(string alias in this._aliases)
                if (!compareService._aliases.Contains(alias))
                    return false;
            foreach(string compareAlias in compareService._aliases)
                if (!_aliases.Contains(compareAlias))
                    return false;
            return true;
        }

        /// <summary>
        /// Determines if the service has the same values other than the port. 
        /// </summary>
        /// <param name="compareService">Service to be compared</param>
        /// <returns>True if the service name, protocol and aliases are the same.</returns>
        public bool EqualsExceptPort(Service compareService)
        {
            if (string.Compare(this._protocol, compareService._protocol, StringComparison.InvariantCultureIgnoreCase) != 0)
                return false;
            if (string.Compare(this._name, compareService.name, StringComparison.InvariantCultureIgnoreCase) != 0)
                return false;
            foreach (string alias in this._aliases)
                if (!compareService._aliases.Contains(alias))
                    return false;
            foreach (string compareAlias in compareService._aliases)
                if (!_aliases.Contains(compareAlias))
                    return false;
            return true;
        }

        /// <summary>
        /// Determines if this collection contains the host server services.
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns>True if this collection contains the host server service names.</returns>
        public static bool ContainsHostServer(string serviceName)
        {
            return new[] {"as-central", "as-database", "as-dtaq", "as-file", "as-netprt",
                          "as-rmtcmd", "as-signon", "as-central-s", "as-database-s",
                          "as-dtaq-s", "as-file-s", "as-netprt-s", "as-rmtcmd-s",
                          "as-signon-s"}.Contains(serviceName);
        }

        /// <summary>
        /// Reads the Windows services file and returns a ServiceList of Service objects.
        /// </summary>
        /// <returns>ServiceList built from Windows services file.</returns>
        public static ServiceList ReadServices()
        {
            ServiceList services = new ServiceList();
            Service service = null;
            string systemRoot = Environment.GetEnvironmentVariable("SYSTEMROOT");
            if (systemRoot == null)
            {   //Set it to the default
                systemRoot = @"C:\Windows";
            }
            string servicesPath = systemRoot + @"\System32\drivers\etc\services";
            string[] lines = System.IO.File.ReadAllLines(servicesPath);
            //            System.Diagnostics.Debug.WriteLine("\n___________________________\nContents of services file:");
            foreach (string line in lines)
            {
                //                System.Diagnostics.Debug.WriteLine("\t" + line);
                if (line.Length > 0)
                {
                    int offset = line.TakeWhile(c => char.IsWhiteSpace(c)).Count();
                    if (line.Substring(offset, 1) != "#")
                    {
                        string[] lineElements = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        if (lineElements.Length >= 2)
                        {
                            service = new Service(lineElements[0], lineElements[1]);
                            services.Add(service);
                        }
                        if (lineElements.Length > 2)
                        {
                            for (int i=2; i<lineElements.Length; i++)
                            {
                                if (lineElements[i].StartsWith('#'))
                                {   // append the rest of the elements together into one comment string
                                    string newComment = lineElements[i];
                                    while (++i < lineElements.Length)
                                    {
                                        newComment += " " + lineElements[i];
                                    }
                                    service.comment = newComment;
                                }
                                else
                                    service.addAlias(lineElements[i]);
                            }
                        }
                    }
                }
            }
            return services;
            //            System.Diagnostics.Debug.WriteLine("___________________________\n");
        }

        /// <summary>
        /// Writes the services contained in newHostServices to the Windows services file.
        /// </summary>
        /// <param name="newHostServices">ServiceList of services to be add/updated.</param>
        public static void WriteServices(ServiceList newHostServices) 
        {
            // Define the path for the Windows services file
            string systemRoot = Environment.GetEnvironmentVariable("SYSTEMROOT");
            if (systemRoot == null)
            {   //Set it to the default
                systemRoot = @"C:\Windows";
            }
            string servicesPath = systemRoot + @"\System32\drivers\etc\services";

            // Create the file if it does not exist
            if (!File.Exists(servicesPath))
            {   // Create the hosts file if one doesn't exist
                string[] strIntro = new string[] {
                    "# This file contains port numbers for well-known services defined by IANA",
                    "#",
                    "# Format::",
                    "#",
                    "# <service name>  <port number>/<protocol>  [aliases...]   [#<comment>]",
                    "#",
                    ""
                };
                File.WriteAllLines(servicesPath, strIntro, Encoding.UTF8);
            }

            // Remove the host server services from the existing file
            ArrayList allFileLines = new ArrayList(File.ReadAllLines(servicesPath));
            for(int i=allFileLines.Count-1; i>=0; i--)
            {
                if(allFileLines[i].ToString().Length > 0)
                {
                    int offset = allFileLines[i].ToString().TakeWhile(c => char.IsWhiteSpace(c)).Count();
                    if (allFileLines[i].ToString().Substring(offset, 1) != "#")
                    {
                        string[] lineElements = allFileLines[i].ToString().Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        if (lineElements.Length >= 2)
                        {
                            if (Service.ContainsHostServer(lineElements[0]))
                            {   //Remove this entry
                                allFileLines.RemoveAt(i);
                            }
                        }
                    }
                }
            }
            // Remove blank lines from the end of the file
            bool foundValidData = false;
            int arrayElement = allFileLines.Count;
            while (!foundValidData)
            {
                if (string.IsNullOrWhiteSpace(allFileLines[--arrayElement].ToString()))
                    allFileLines.RemoveAt(arrayElement);
                else
                    foundValidData = true;
            }
            try
            {
                File.WriteAllLines(servicesPath, (string[])allFileLines.ToArray(typeof(string)));
            }
            catch (Exception e)
            {
                Type etype = e.GetType();
                if (e.GetType().Name == "UnauthorizedAccessException")
                {
                    MessageBox.Show("Saving the services file requires elevated authority.", "Access Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // Append the host servers to the end of the file
            // Verify that a Control z isn't the last character in the file, if so remove it
            using (FileStream stream = new FileStream(servicesPath, FileMode.Open, FileAccess.ReadWrite))
            {
                stream.Seek(-1, SeekOrigin.End);
                byte b = (byte)stream.ReadByte();
                if (b == 0b_0001_1010)
                    stream.SetLength(stream.Length - 1);
            }
            // Add the host server service entries
            string serviceEntries = null;
            foreach(Service hostService in newHostServices)
            {
                serviceEntries += hostService.toString() + "\n";
            }
            File.AppendAllText(servicesPath, serviceEntries);
        }

    }
}
