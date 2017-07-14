using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CleanDevBits.Lib.DomainModel;
using CleanDevBits.Lib.Event;
using CleanDevBits.Lib.Helper;

namespace CleanDevBits.Lib.Services {
    public class CleanerService {
        private static readonly CleanerService instance = new CleanerService();

        #region Events and their handlers

        public event EventHandler<FileEventArgs> FileDeleted;
        public event EventHandler<FileErrorEventArgs> FileDeletingError;
        public event EventHandler<DirectoryEventArgs> DirectoryDeleted;
        public event EventHandler<DirectoryErrorEventArgs> DirectoryDeletingError;

        protected void OnDirectoryDeletingError(DirectoryErrorEventArgs e) {
            EventHandler<DirectoryErrorEventArgs> handler = DirectoryDeletingError;
            if (handler != null) handler(this, e);
        }

        protected void OnDirectoryDeleted(DirectoryEventArgs e) {
            EventHandler<DirectoryEventArgs> handler = DirectoryDeleted;
            if (handler != null) handler(this, e);
        }

        protected void OnFileDeleted(FileEventArgs e) {
            EventHandler<FileEventArgs> handler = FileDeleted;
            if (handler != null) handler(this, e);
        }

        protected void OnFileDeletingError(FileErrorEventArgs e) {
            EventHandler<FileErrorEventArgs> handler = FileDeletingError;
            if (handler != null) handler(this, e);
        }

        #endregion

        public static CleanerService Instance {
            get { return instance; }
        }

        public List<string> FindFoldersToEmpty(string dir) {
            List<string> list = new List<string>();

            FindFoldersToEmpty(dir, list);

            return list;
        }

        private void FindFoldersToEmpty(string dir, List<string> list) {
            // First of all I need to check if the dir really exists
            if (!Directory.Exists(dir)) {
                throw new DirectoryNotFoundException(
                    String.Format("La directory '{0}' non esiste!", dir));
            }

            DirType dirType = DirHelper.GetDirType(dir);
            switch (dirType) {
                case DirType.Standard:
                    // I need to investigate all subfolders...
                    string[] subDirs = Directory.GetDirectories(dir);
                    foreach (string subDir in subDirs) {
                        FindFoldersToEmpty(subDir, list);
                    }
                    break;
                case DirType.CsProject:
                    string csBinDir = Path.Combine(dir, "bin");
                    DirHelper.AddToListIfExists(csBinDir, list);
                    string csObjDir = Path.Combine(dir, "obj");
                    DirHelper.AddToListIfExists(csObjDir, list);
                    break;
                case DirType.VbProject:
                    break;
                case DirType.Setup:
                    string setupDebugDir = Path.Combine(dir, "Debug");
                    DirHelper.AddToListIfExists(setupDebugDir, list);
                    string setupReleaseDir = Path.Combine(dir, "Release");
                    DirHelper.AddToListIfExists(setupReleaseDir, list);
                    break;
            }
        }

        /// <summary>
        /// Erases the folder's content and all of its subdirectories.
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="includeSubdirs"></param>
        public long EraseAllContent(string dir, bool includeSubdirs) {
            // Total amount of erased bytes
            long totalErasedBytes = 0;

            // First of all I need to check if the dir really exists
            if (!Directory.Exists(dir)) {
                throw new DirectoryNotFoundException(
                    String.Format("La directory '{0}' non esiste!", dir));
            }

            DirectoryInfo dirInfo = new DirectoryInfo(dir);

            if (includeSubdirs) {
                // Let's remove all subdirs before processing files...
                DirectoryInfo[] subDirs = dirInfo.GetDirectories();
                foreach (DirectoryInfo subdir in subDirs) {
                    totalErasedBytes += EraseAllContent(subdir.FullName, true);

                    try {
                        // I can now remove the directory itself...
                        subdir.Delete();
                        OnDirectoryDeleted(new DirectoryEventArgs {
                            Directory = subdir.FullName
                        });
                    } catch (Exception excp) {
                        OnDirectoryDeletingError(new DirectoryErrorEventArgs {
                            Directory = subdir.FullName,
                            Exception = excp
                        });
                    }
                }
            }

            // Now let's erase all single files...
            FileInfo[] files = dirInfo.GetFiles();
            foreach (FileInfo file in files) {
                try {
                    File.Delete(file.FullName);
                    totalErasedBytes += file.Length;

                    OnFileDeleted(new FileEventArgs {
                        File = file.FullName
                    });
                } catch (Exception excp) {
                    OnFileDeletingError(new FileErrorEventArgs {
                        File = file.FullName,
                        Exception = excp
                    });
                }
            }

            return totalErasedBytes;
        }
    }
}