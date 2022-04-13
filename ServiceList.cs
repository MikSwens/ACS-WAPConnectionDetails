using System;
using System.Collections.Generic;
using System.Text;

namespace ACS_WAPConnectionDetails
{
    /// <summary>
    /// Extends List to provide functionality for a collection of Service objects.
    /// </summary>
    public class ServiceList : List<Service>
    {
        /// <summary>
        /// List of host server service names.
        /// </summary>
        List<string> hostServerServiceNames = new List<string>()
        {
            "as-central", "as-database", "as-dtaq", "as-file", "as-netprt", "as-rmtcmd", "as-signon",
            "as-central-s", "as-database-s", "as-dtaq-s", "as-file-s", "as-netprt-s", "as-rmtcmd-s", 
            "as-signon-s", "drda", "ddm-ssl", "telnet-ssl"
        };

        /// <summary>
        /// Get the port number for a given service name.
        /// </summary>
        /// <param name="serviceName">Name of the service</param>
        /// <returns>Service port number as an int</returns>
        public int getServicePort(string serviceName)
        {
            Service temp = Find(x => x.name == serviceName);
            if (temp != null)
                return temp.port;
            else
                return -1;
        }

        /// <summary>
        /// Strict comparitor services must have the same service name, port, protocol and aliases
        /// </summary>
        /// <param name="compareService">Service to check for</param>
        /// <returns>True if this list contains the input Service</returns>
        public new bool Contains(Service compareService)
        {
            foreach (Service service in this)
            {
                if (service.Equals(compareService))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Compare the service name of the input service to the service names contained in this collection.
        /// </summary>
        /// <param name="compareService">Input Service</param>
        /// <returns>
        /// true if the service name is contained in this collection.
        /// </returns>
        public bool ContainsServiceName(Service compareService)
        {
            foreach(Service service in this)
            {
                if (string.Compare(compareService.name, service.name, StringComparison.InvariantCultureIgnoreCase) == 0)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Compare input service name to the service names in this collection.
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns>
        /// true if the collection contains the service name.
        /// </returns>
        public bool ContainsServiceName(string serviceName)
        {
            foreach(Service service in this)
            {
                if (string.Compare(serviceName, service.name, StringComparison.InvariantCultureIgnoreCase) == 0)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Checks to see if all of the host server service names are contained in this collection.
        /// </summary>
        /// <returns>
        /// true if this collection contains an entry for every host server service.
        /// </returns>
        public bool ContainsHostServers()
        {
            foreach(string serviceName in hostServerServiceNames)
            {
                if (!this.ContainsServiceName(serviceName))
                    return false;
            }
            return true;
        }
    }
}
