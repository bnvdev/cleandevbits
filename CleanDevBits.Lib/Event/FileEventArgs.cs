using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CleanDevBits.Lib.Event {
    public class FileEventArgs : EventArgs {
        private string file = String.Empty;

        public FileEventArgs() { }

        public string File {
            get { return file; }
            set { file = value; }
        }
    }

    public class FileErrorEventArgs : EventArgs {
        private string file = String.Empty;
        private Exception exception = null;

        public FileErrorEventArgs() { }

        public string File {
            get { return file; }
            set { file = value; }
        }

        public Exception Exception {
            get { return exception; }
            set { exception = value; }
        }
    }

    public class DirectoryEventArgs : EventArgs {
        private string directory = String.Empty;

        public DirectoryEventArgs() { }

        public string Directory {
            get { return directory; }
            set { directory = value; }
        }
    }

    public class DirectoryErrorEventArgs : EventArgs {
        private string directory = String.Empty;
        private Exception exception = null;

        public DirectoryErrorEventArgs() { }

        public string Directory {
            get { return directory; }
            set { directory = value; }
        }

        public Exception Exception {
            get { return exception; }
            set { exception = value; }
        }
    }
}