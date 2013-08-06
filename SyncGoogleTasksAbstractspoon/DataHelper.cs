using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyncGoogleTasksAbstractSpoon.Data;

namespace SyncGoogleTasksAbstractSpoon
{
    class DataHelper
    {
        public static void AddParentTasks(Data.TaskList datalist, List<Data.Task> allTasks)
        {
            foreach (var datatask in allTasks.Where(datatask => !string.IsNullOrEmpty(datatask.GoogleTaskParentTmp)))
                datatask.ParentTask = GetTaskFromTaskId(allTasks, datatask.GoogleTaskParentTmp);
        }

        public static void AddChildTasks(Data.TaskList datalist, List<Data.Task> allTasks)
        {
            foreach (var datatask in allTasks.Where(datatask => datatask.ParentTask != null))
                datatask.ParentTask.ChildTasks.Add(datatask);
        }

        public static void AddTopTasks(Data.TaskList datalist, List<Data.Task> allTasks)
        {
            foreach (var datatask in allTasks.Where(datatask => datatask.ParentTask == null))
                datalist.TopTask.ChildTasks.Add(datatask);
        }

        public static Data.Task GetTaskFromTaskId(TaskList taskList, string taskId)
        {
            return taskList.AllTasks.FirstOrDefault(datatask => datatask.Id == taskId);
        }
        public static Data.Task GetTaskFromTaskId(List<Data.Task> allTasks, string taskId)
        {
            return allTasks.FirstOrDefault(datatask => datatask.Id == taskId);
        }

       
    }
}
