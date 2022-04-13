using System;

namespace ACS_WAPConnectionDetails
{
    public class Connection
    {
        // Name of the system
        private string strName;
        // properties as stored in the Windows Registry
        private uint uiAdminSystemIndicator;
        private uint uiConnectionTimeout;
        private string strDescription;
        private string strIpAddress;
        private uint uiIpAddressCacheTimeout;
        private uint uiIpAddressLookupMode;
        private uint uiPersistenceMode;
        private uint uiPortLookupMode;
        private uint uiSecureSocketsLayer;
        private uint uiSignonMode;
        private string strUserId;
        private uint uiVersionReleaseLevel;

        public uint adminSystemIndicator { get { return uiAdminSystemIndicator; } set { uiAdminSystemIndicator = adminSystemIndicator; } }
        public string name { get { return strName; } set { strName = name; } }
        public uint connectionTimeout { get { return uiConnectionTimeout; } set { uiConnectionTimeout = connectionTimeout; } }
        public string description { get { return strDescription; } set { strDescription = description; } }
        public string ipAddress { get { return strIpAddress; } set { strIpAddress = ipAddress; } }
        public uint ipAddressCacheTimeout { get { return uiIpAddressCacheTimeout; } set { uiIpAddressCacheTimeout = ipAddressCacheTimeout; } }
        public uint ipAddressLookupMode { get { return uiIpAddressLookupMode; } set { uiIpAddressLookupMode = ipAddressLookupMode; } }
        public uint persistenceMode { get { return uiPersistenceMode; } set { uiPersistenceMode = persistenceMode; } }
        public uint portLookupMode { get { return uiPortLookupMode; } set { uiPortLookupMode = portLookupMode; } }
        public uint secureSocketsLayer { get { return uiSecureSocketsLayer; } set { uiSecureSocketsLayer = secureSocketsLayer; } }
        public uint signonMode { get { return uiSignonMode; } set { uiSignonMode = signonMode; } }
        public string userId { get { return strUserId; } set { strUserId = userId; } }
        public uint versionReleaseLevel { get { return uiVersionReleaseLevel; } set { uiVersionReleaseLevel = versionReleaseLevel; } }

        
        // Address lookup types
        public enum IPAddressLookup
        {
            LOOKUP_ALWAYS,          // 0
            LOOKUP_ONE_HOUR,        // 1
            LOOKUP_ONE_DAY,         // 2
            LOOKUP_ONE_WEEK,        // 3
            LOOKUP_NEVER,           // 4
            LOOKUP_AFTER_STARTUP    // 5
        }

        // Persistence Mode
        public enum PersistenceMode
        {
            PERSISTENT,         // 0
            NOT_PERSISTENT      // 1
        }

        // Port Lookup Mode
        public enum PortLookupMode
        {
            USE_PORT_MAPPER,        // 0
            LOCAL_LOOKUP,           // 1
            USE_STANDARD_PORTS      // 2
        }

        // Signon Mode
        public enum SignonMode
        {
            USER_MODE_NOT_SET,      // 0
            USE_DEFAULT_USER,       // 1
            PROMPT_ALWAYS,          // 2
            USE_WINDOWS_LOGON,      // 3
            KERBEROS                // 4
        }

        // Constructor
        public Connection(string strName, uint uiAdminSystemIndicator,
                          uint uiConnectionTimeout, string strDescription,
                          string strIpAddress, uint uiIpAddressCacheTimeout,
                          uint uiIpAddressLookupMode, uint uiPersistenceMode,
                          uint uiPortLookupMode, uint uiSecureSocketsLayer,
                          uint uiSignonMode, string strUserId,
                          uint uiVersionReleaseLevel)
        {
            this.strName = strName;
            this.uiAdminSystemIndicator = uiAdminSystemIndicator;
            this.uiConnectionTimeout = uiConnectionTimeout;
            this.strDescription = strDescription;
            this.strIpAddress = strIpAddress;
            this.uiIpAddressCacheTimeout = uiIpAddressCacheTimeout;
            this.uiIpAddressLookupMode = uiIpAddressLookupMode;
            this.uiPersistenceMode = uiPersistenceMode;
            this.uiPortLookupMode = uiPortLookupMode;
            this.uiSecureSocketsLayer = uiSecureSocketsLayer;
            this.uiSignonMode = uiSignonMode;
            this.strUserId = strUserId;
            this.uiVersionReleaseLevel = uiVersionReleaseLevel;
        }

        /// <summary>
        /// Converts the DWord registry value to human readable string.
        /// </summary>
        /// <param name="uiVRM">uint version of VRM as stored in the registry.</param>
        /// <returns>String value of the VRM.</returns>
        public static string VRMDWord2String(uint uiVRM)
        {
            string v = ((uiVRM & 0b_0000_0000_1111_1111_0000_0000_0000_0000) >> 16).ToString();
            string r = ((uiVRM & 0b_0000_0000_0000_0000_1111_1111_0000_0000) >> 8).ToString();
            string m = (uiVRM & 0b_0000_0000_0000_0000_0000_0000_1111_1111).ToString();
            if (v == "0" && r == "0" && m == "0")
                return "Unknown";
            else
                return "V" + v + "R" + r + "M" + m;
        }

        /// <summary>
        /// Converts Version information from string in format VnRnMn to a DWord format used in the registry
        /// Input:  string in the format VnRnMn
        /// Output:  uint in the format 0x00vvrrmm
        /// </summary>
        /// <param name="strVRM">String VRM</param>
        /// <returns>uint version of VRM</returns>
        public static uint VRMString2DWord(string strVRM)
        {
            if (string.Compare(strVRM, "UNKNOWN", System.StringComparison.InvariantCultureIgnoreCase) == 0)
                return 0;
            else
            {
                /*
                string strV = strVRM.Substring(1, 1);
                string strR = strVRM.Substring(3, 1);
                string strM = strVRM.Substring(5, 1);
                uint uiV = uint.Parse(strV);
                uint uiR = uint.Parse(strR);
                uint uiM = uint.Parse(strM);
                uint x = (uiV << 16) | (uiR << 8) | uiM;
                */
                //System.Diagnostics.Debug.WriteLine(Convert.ToString(x, 2));
                return (uint.Parse(strVRM.Substring(1, 1)) << 16) | 
                    (uint.Parse(strVRM.Substring(3, 1)) << 8) |
                    uint.Parse(strVRM.Substring(5, 1));
            }
        }

        // Getters and Setters
        
        public bool setAdminSystemIndicator(uint inputValue)
        {
            if (inputValue == 0 || inputValue == 1)
            {
                uiAdminSystemIndicator = inputValue;
                return true;
            }
            return false;
        }
        public bool setAdminSystemIndicator(bool inputValue)
        {
            if (!inputValue)
                uiAdminSystemIndicator = 0;
            else
                uiAdminSystemIndicator = 1;
            return true;
        }

        public bool isAdminSystem() { return uiAdminSystemIndicator == 1; }

        public bool setConnectionTimeout(uint inputValue)
        {
            if (inputValue >= 0 && inputValue <= 3600)
            {
                uiConnectionTimeout = inputValue;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool setIpAddressLookupMode(uint inputValue)
        {
            if (inputValue >= 0 && inputValue <= 5)
            {
                uiIpAddressLookupMode = inputValue;
                return true;
            }
            return false;
        }

        public bool setPersistenceMode(uint inputValue)
        {
            if (inputValue >= 0 && inputValue <= 1)
            {
                uiPersistenceMode = inputValue;
                return true;
            }
            return false;
        }

        public bool setPersistenceMode(bool inputValue)
        {
            if (inputValue)
                uiPersistenceMode = 0;
            else
                uiPersistenceMode = 1;
            return true;
        }

        public bool setPortLookupMode(uint inputValue)
        {
            if (inputValue >= 0 && inputValue <= 2)
            {
                uiPortLookupMode = inputValue;
                return true;
            }
            return false;
        }

        public bool isSecureSocketsLayer() { return uiSecureSocketsLayer == 1; }
 
        public string getVersionReleaseLevelString()
        {
            string v = ((uiVersionReleaseLevel & 0b_0000_0000_1111_1111_0000_0000_0000_0000) >> 16).ToString();
            string r = ((uiVersionReleaseLevel & 0b_0000_0000_0000_0000_1111_1111_0000_0000) >> 8).ToString();
            string m = (uiVersionReleaseLevel & 0b_0000_0000_0000_0000_0000_0000_1111_1111).ToString();
            if (v == "0" && r == "0" && m == "0")
                return "Unknown";
            else
                return "V" + v + "R" + r + "M" + m;
        }


    }
}
