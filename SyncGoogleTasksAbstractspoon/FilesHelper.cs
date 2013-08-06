using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyncGoogleTasksAbstractSpoon.Data;

namespace SyncGoogleTasksAbstractSpoon
{
    class FilesHelper
    {
        public static void DeleteLocalTaskLists(TaskListChanges taskListChanges)
        {
            foreach (var taskListChange in taskListChanges.Where(change => change.TaskChangeType == TaskListChangeType.Delete))
            {
                DeleteLocalTaskListByTitle(taskListChange.Title);
            }
        }

        public static void DeleteAllTaskLists()
        {
            var files = System.IO.Directory.GetFiles(FolderPath.TodoFilePath, "*.tdl");

            foreach (var file in files)
            {
                var title = file.Split('\\').Last().Split('.').First();
                DeleteLocalTaskListByTitle(title);
            }

        }

        public static void DeleteLocalTaskListByTitle(string taskListTitle)
        {
            var todoFile = FolderPath.TodoFilePath + taskListTitle + ".tdl";

            if (File.Exists(todoFile))
            {

                EnsureTrashFolderExist();
                var trashFile = FolderPath.TrashFilePath + taskListTitle + ".tdl";

                if (File.Exists(trashFile))
                {
                    File.Delete(trashFile);
                }

                File.Copy(todoFile, trashFile); 
                
                File.Delete(todoFile);
            }

            var referenceFile = FolderPath.ReferenceFilePath + taskListTitle + ".xml";

            if (File.Exists(referenceFile))
            {
                File.Delete(referenceFile);
            }

            var compareFile = FolderPath.LocalChangesCompareFilePath + taskListTitle + ".xml";
           
            if (File.Exists(compareFile))
            {
              
                File.Delete(compareFile);
            }
        }

        public static void EnsureFoldersExists()
        {
            if (!Directory.Exists(FolderPath.TodoFilePath))
                Directory.CreateDirectory(FolderPath.TodoFilePath);
            if (!Directory.Exists(FolderPath.LocalChangesCompareFilePath))
                Directory.CreateDirectory(FolderPath.LocalChangesCompareFilePath);
            if (!Directory.Exists(FolderPath.ReferenceFilePath))
                Directory.CreateDirectory(FolderPath.ReferenceFilePath);

        }
        public static void EnsureTrashFolderExist()
        {
            if (!Directory.Exists(FolderPath.TrashFilePath))
                Directory.CreateDirectory(FolderPath.TrashFilePath);
        }
        public static string[] GetTodoFiles()
        {
            var todoFiles = System.IO.Directory.GetFiles(FolderPath.TodoFilePath, "*.tdl");
            return todoFiles;
        }
         

    }
}
