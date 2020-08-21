using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace ACE.Common
{
    public static class UpdateManager
    {
        public static bool Enabled = false;
        public static string UpdateConfigFile;

        // The Key in these is the Iteration
        public static Dictionary<int, UpdateData> CellUpdates;
        public static Dictionary<int, UpdateData> PortalUpdates;
        public static Dictionary<int, UpdateData> LanguageUpdates;

        public static void Initialize()
        {
            Enabled = ConfigManager.Config.Server.EnableClientUpdates;
            UpdateConfigFile = ConfigManager.Config.Server.UpdateConfigFile;

            // If the UpdateConfigFile does not exist, disable updates!
            if (Enabled && (!System.IO.File.Exists(UpdateConfigFile) || UpdateConfigFile == ""))
                Enabled = false;

        }

        /// <summary>
        /// Loads the Data from the UpdateConfigFile
        /// </summary>
        public static void Load()
        {

        }

        //public static Cell

    }
}
