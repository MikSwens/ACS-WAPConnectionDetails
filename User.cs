using System.Collections;

namespace ACS_WAPConnectionDetails
{
    /// <summary>
    /// Object used to store Windows users from the HKeyUsers hive
    /// </summary>
    class User
    {
        private string _sID;
        private string _userName;
        private ArrayList _connections = new ArrayList(); // of Connection
        public string sID { get { return _sID; } set{ _sID = sID;} }
        public string userName { get { return _userName; } set { _userName = userName; } }
        public ArrayList connections { get { return _connections; } set { _connections = connections; } }

        /// <summary>
        /// Constructor without a connection array
        /// </summary>
        /// <param name="userName">Windows User Name</param>
        /// <param name="sID">Windows User SID</param>
        public User(string userName, string sID)
        {
            this._sID = sID;
            this._userName = userName;
        }

        /// <summary>
        /// Complete constructor
        /// </summary>
        /// <param name="userName">Windows User Name</param>
        /// <param name="sID">Windows User SID</param>
        /// <param name="connections">ArrayList of Connection objects</param>
        public User(string userName, string sID, ArrayList connections)
        {
            this._userName = userName;
            this._sID = sID;
            this._connections = connections; // of Connection;
        }

        /// <summary>
        /// Get a connection at a specific index.
        /// </summary>
        /// <param name="index">Index of the connection in the connection ArrayList</param>
        /// <returns>Connection at index</returns>
        public Connection getConnection(int index) { return (Connection)connections[index]; }
        public void setConnections(ArrayList connectionList)
        {
            _connections = connectionList;
        }
    }

}
