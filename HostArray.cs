using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ACS_WAPConnectionDetails
{
    /// <summary>
    /// Extends ArrayList to add a Contains method specifically for objects of type Host
    /// </summary>
    public class HostArray : ArrayList
    {
        /// <summary>
        /// Test to see if this collection contains a host with the same name as the input value item.
        /// </summary>
        /// <param name="item">Input host name to be test</param>
        /// <returns>True if the collection contains a host with the name.</returns>
        public bool Contains(string item)
        {
            foreach (Host host in this)
            {
                if (String.Equals(host.hostName.Trim(), item.Trim(), StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }
            return false;
        }
    }
}
