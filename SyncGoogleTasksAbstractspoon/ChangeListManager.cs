using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyncGoogleTasksAbstractSpoon.Data;

namespace SyncGoogleTasksAbstractSpoon
{
    class ChangeListManager
    {
        public static Data.TaskChanges CreateListOfLocalTaskChanges(Data.TaskList oldlist, Data.TaskList newlist)
        {
            var taskChanges = new Data.TaskChanges();
            Data.TaskChange taskChange;
            bool found;
            foreach (var oldtask in oldlist.AllTasks)
            {
                found = false;

                foreach (var newtask in newlist.AllTasks.Where(newtask => newtask.Id == oldtask.Id))
                {
                    found = true;

                    taskChange = new Data.TaskChange();

                    if (oldtask.ParentId != newtask.ParentId || oldtask.PreviousId != newtask.PreviousId)
                        taskChange.TaskChangeType = TaskChangeType.ChangeAndMove;

                    else if (oldtask.Title != newtask.Title || oldtask.Done != newtask.Done || oldtask.DueDate != newtask.DueDate || oldtask.Notes != newtask.Notes)
                        taskChange.TaskChangeType = TaskChangeType.Change;

                    if (taskChange.TaskChangeType != TaskChangeType.None)
                    {

                        taskChange.NewTask = newtask;
                        taskChange.Id = newtask.Id;
                        taskChanges.AddChangeOverwriteExisting(taskChange);
                    }
                }
                if (!found)
                {
                    taskChange = new TaskChange();
                    taskChange.TaskChangeType = TaskChangeType.Delete;
                    taskChange.Id = oldtask.Id;
                    taskChanges.AddChangeOverwriteExisting(taskChange);
                }

            }

            foreach (var newtask in newlist.AllTasks)
            {
                found = false;

                foreach (var oldtask in oldlist.AllTasks.Where(oldtask => oldtask.Id == newtask.Id))
                    found = true;

                if (!found)
                {
                    taskChange = new TaskChange
                        {
                            TaskChangeType = TaskChangeType.Create,
                            NewTask = newtask,
                            Id = newtask.Id
                        };
                    taskChanges.AddChangeOverwriteExisting(taskChange);
                }
            }

            return taskChanges;
        }

        public static Data.TaskListChanges CreateListOfLocalListChangesToSubmitRemotely()
        {
            var todoFiles = System.IO.Directory.GetFiles(FolderPath.TodoFilePath, "*.tdl");
            var compareFiles = System.IO.Directory.GetFiles(FolderPath.LocalChangesCompareFilePath, "*.xml");
            var taskListChanges = new TaskListChanges();
            var found = false;

            foreach (var todoFile in todoFiles)
            {
                found = false;
                var todoTitle = todoFile.Split('\\').Last().Split('.').First();

                foreach (var syncFile in compareFiles)
                {
                    var compareTitle = syncFile.Split('\\').Last().Split('.').First();

                    if (todoTitle == compareTitle)
                        found = true;

                }
                if (!found)
                {
                    var taskListChange = new TaskListChange();
                    taskListChange.Title = todoTitle;
                    taskListChange.TaskChangeType = TaskListChangeType.Create;
                    taskListChanges.Add(taskListChange);
                }
            }


            foreach (var compareFile in compareFiles)
            {
                found = false;
                var compareTitle = compareFile.Split('\\').Last().Split('.').First();

                foreach (var todoFile in todoFiles)
                {
                    var todoTitle = todoFile.Split('\\').Last().Split('.').First();
                    if (compareTitle == todoTitle)
                        found = true;
                }
                if (!found)
                {
                    var taskListChange = new TaskListChange
                        {
                            Title = compareTitle,
                            TaskChangeType = TaskListChangeType.Delete
                        };
                    taskListChanges.Add(taskListChange);
                }
            }

            return taskListChanges;
        }


        public static Data.TaskListChanges GeChangeListOfRemotelyDeletedLists(string[] localTodoFiles, List<Data.TaskList> remoteTaskLists)
        {

            var found = false;
            var taskListChanges = new Data.TaskListChanges();

            foreach (var todoFile in localTodoFiles)
            {
                var title = todoFile.Split('\\').Last().Split('.').First();
                foreach (var remoteTaskList in remoteTaskLists)
                    if (title == remoteTaskList.Title)
                        found = true;
                if (!found)
                {
                    var taskListChange = new Data.TaskListChange
                    {
                        Title = title,
                        TaskChangeType = TaskListChangeType.Delete
                    };
                    taskListChanges.Add(taskListChange);
                }

            }
            return taskListChanges;
        }
        

    }
}
