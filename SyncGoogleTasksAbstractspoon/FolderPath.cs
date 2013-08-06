using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncGoogleTasksAbstractSpoon
{
    class FolderPath
    {
        private static string _todoFilePath;

        /// <summary>
        /// Where to save the todo files..
        /// </summary>
        public static string TodoFilePath
        {
            get
            {
                if (_todoFilePath != "")
                {
                    _todoFilePath = ConfigurationManager.AppSettings["TodoFilePath"].TrimEnd('\\') + '\\';
                }
                return _todoFilePath;
            }
        }
        private static string SyncFilePath
        {
            get { return TodoFilePath + @"Sync\"; }
        }
        public static string LocalChangesCompareFilePath
        {
            get { return SyncFilePath + @"Compare\"; }
        }
        public static string ReferenceFilePath
        {
            get { return SyncFilePath + @"Reference\"; }
        }

        public static string TrashFilePath
        {
            get { return SyncFilePath + @"Trash\"; }
        }

        public static bool ValidateTodoFilePath()
        {
            string todofilepath;
            try
            {
                todofilepath = TodoFilePath;
            }
            catch (Exception)
            {
                return false;
            }
            if (todofilepath != null && todofilepath.Length > 5)
            {
                return true;
            }
            return false;

        }
    }
}
