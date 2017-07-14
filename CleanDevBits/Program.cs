using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CleanDevBits.Lib.Helper;
using CleanDevBits.Lib.Services;

namespace CleanDevBits {
    internal class Program {
        private static readonly List<string> log = new List<string>();

        private static void Main(string[] args) {
            if (args.Length == 0) {
                Console.Out.WriteLine("Please provide the base directory!");
                return;
            }

            // The first argument is the base directory
            string baseDirectory = args.First();

            CleanerService srv = CleanerService.Instance;

            // Hooking up events...
            srv.FileDeleted += srv_FileDeleted;
            srv.DirectoryDeleted += srv_DirectoryDeleted;
            srv.DirectoryDeletingError += srv_DirectoryDeletingError;
            srv.FileDeletingError += srv_FileDeletingError;

            // Let's start by searching for the folders to clean
            List<string> folders = srv.FindFoldersToEmpty(baseDirectory);

            Console.Out.WriteLine("I need to erase the content of the following folders:\n");
            folders.ForEach(folder => { Console.Out.WriteLine("{0}", folder); });

            Console.Out.WriteLine("\nDo you want me to clean these folders? [Y/N]");
            ConsoleKeyInfo keyPressed = Console.ReadKey();

            if (keyPressed.Key == ConsoleKey.Y) {
                long totalErasedBytes = 0;

                folders.ForEach(folder => {
                                    // Erasing folder content...
                                    totalErasedBytes += srv.EraseAllContent(folder, true);
                                });

                // Calculate the erased size in Megabytes
                long totalErasedMb = totalErasedBytes/1048576;

                Console.Out.WriteLine("\nErased a total of {0:n3} Mb!", totalErasedMb);

                DumpLog();
            } else {
                // Quitting the app with nothing done.
                Console.Out.WriteLine("Bye!");
            }

            Console.ReadKey();
        }

        /// <summary>
        /// Creates a dump file containing the complete log of the operations.
        /// </summary>
        private static void DumpLog() {
            string fileName = String.Format("LOG{0:yyyyMMddHHmmss}.txt", DateTime.Now);
            File.WriteAllLines(fileName, log);
        }

        private static void srv_FileDeletingError(object sender, Lib.Event.FileErrorEventArgs e) {
            string msg1 = String.Format("**** Unable to delete file '{0}'", e.File);
            string msg2 = String.Format("\tError: {0}", e.Exception.Message);

            Console.Out.WriteLine(msg1);
            Console.Out.WriteLine(msg2);

            log.Add(msg1);
            log.Add(msg2);
        }

        private static void srv_DirectoryDeletingError(object sender, Lib.Event.DirectoryErrorEventArgs e) {
            string msg1 = String.Format("**** Unable to delete folder '{0}'", e.Directory);
            string msg2 = String.Format("\tError: {0}", e.Exception.Message);

            Console.Out.WriteLine(msg1);
            Console.Out.WriteLine(msg2);

            log.Add(msg1);
            log.Add(msg2);
        }

        private static void srv_DirectoryDeleted(object sender, Lib.Event.DirectoryEventArgs e) {
            string msg1 = String.Format("Deleted folder '{0}'", e.Directory);

            Console.Out.WriteLine(msg1);

            log.Add(msg1);
        }

        private static void srv_FileDeleted(object sender, Lib.Event.FileEventArgs e) {
            string msg1 = String.Format("Deleted file '{0}'", e.File);

            Console.Out.WriteLine(msg1);

            log.Add(msg1);
        }
    }
}