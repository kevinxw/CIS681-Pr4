/*
 * File manager is used to manage files (look up / save as files)
 */

//#define DEBUG_ON

using System.IO;
using Microsoft.Win32;

namespace CIS681.Fall2012.VDS.Data.IO {
    public sealed class FileManager {
        private const string filter = "XML Data File|*.xml|All Files|*.*";
        private const string initialDirectory = "./";
        // return a read-only stream
        public static Stream Open() {
            OpenFileDialog fd = new OpenFileDialog();
            // initial directory
            fd.InitialDirectory = Path.GetFullPath(initialDirectory);
            // allowed file types
            fd.Filter = filter;
            fd.RestoreDirectory = false;
            // select .xml as default
            fd.FilterIndex = 1;
            fd.Title = "Select Project Data File..";
            if (fd.ShowDialog().GetValueOrDefault())
                return fd.OpenFile();
            else
                return null;
        }

        // save file to, return a read / write stream
        public static Stream Save() {
            SaveFileDialog fd = new SaveFileDialog();
            fd.InitialDirectory = Path.GetFullPath(initialDirectory);
            fd.Filter = filter;
            fd.FilterIndex = 1;
            fd.RestoreDirectory = false;
            if (fd.ShowDialog().GetValueOrDefault())
                return fd.OpenFile();
            else
                return null;
        }
    }
}

#if DEBUG_ON
namespace CIS681.Fall2012.VDS.Data.IO {
    class TEST_CLASS {
        public static void Main() {
            // read file
            using (Stream sr = FileManager.Open()) {
                System.Console.WriteLine(sr.ReadByte());
            }
            // write file
            using (Stream sw = FileManager.Save()) {
                sw.WriteByte((byte)12);
            }
        }
    }
}
#endif