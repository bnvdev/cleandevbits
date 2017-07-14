using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CleanDevBits.Lib.DomainModel;

namespace CleanDevBits.Lib.Helper {
    public class DirHelper {
        public static void AddToListIfExists(string dir, List<string> list) {
            if (Directory.Exists(dir)) {
                list.Add(dir);
            }
        }

        /// <summary>
        /// This method identifies the dir's type by looking at its
        /// content.
        /// </summary>
        /// <param name="dir">directory to be identified</param>
        /// <returns></returns>
        public static DirType GetDirType(string dir) {
            if (HasFilesOfType(dir, "*.vdproj")) {
                // This is a Setup Project Directory
                return DirType.Setup;
            }

            if (HasFilesOfType(dir, "*.csproj")) {
                // This is a C# Project Directory
                return DirType.CsProject;
            }

            if (HasFilesOfType(dir, "*.vbproj")) {
                // This is a Visual Basic Project Directory
                return DirType.VbProject;
            }

            // This is a Standard Directory
            return DirType.Standard;
        }

        /// <summary>
        /// This method checks if the folder contains at least one file 
        /// that matches the specified search pattern (e.g. *.csprj).
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="searchPattern"></param>
        /// <returns></returns>
        public static bool HasFilesOfType(string dir, string searchPattern) {
            string[] files = Directory.GetFiles(dir, searchPattern);
            return files.Length > 0;
        }
    }
}