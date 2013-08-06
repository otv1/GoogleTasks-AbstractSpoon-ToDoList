using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SyncGoogleTasksAbstractSpoon.Data
{
    public class TaskList
    {
        public TaskList()
        {
            TopTask = new Data.Task();
        }

        public string Id { get; set; }
        public string Title { get; set; }
        public Task TopTask { get; set; }
        public string SelfLink { get; set; }
        public string Updated { get; set; }

        public List<Task> AllTasks
        {
            get
            {
                var list = new List<Task>();
                AddChildrenRecursive(list, TopTask);
                return list;
            }
        }

        private void AddChildrenRecursive(List<Task> list, Data.Task task)
        {
            foreach (var childtask in task.ChildTasks)
            {
                list.Add(childtask);
                AddChildrenRecursive(list, childtask);
            }
        }
    }

    public class Task
    {
        public Task()
        {
            ChildTasks = new List<Task>();
        }

        public string Id { get; set; }
        public string Title { get; set; }
        public string Notes { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? DoneDate { get; set; }
        public bool Done
        {
            get { return DoneDate.HasValue; }
        }
        public List<Task> ChildTasks { get; set; }
        public Task ParentTask { get; set; }
        public string GoogleTaskParentTmp { get; set; }
        public string ParentId
        {
            get
            {
                if (ParentTask != null && !string.IsNullOrEmpty(ParentTask.Id))
                    return ParentTask.Id;
                return "";
            }
        }
        public string PreviousId
        {
            get
            {
                string prevtask = "";
                foreach (var task in ParentTask.ChildTasks)
                {
                    if (task.Id == Id)
                        return prevtask;

                    prevtask = task.Id;
                }
                return "";
            }
        }

        public IEnumerable<XAttribute> AbstractSpoonAttributes { get; set; }
        public IEnumerable<XElement> AbstractSpoonElements { get; set; }

    }

    [Serializable]
    public class TaskReference
    {
        public string GoogleTaskId { get; set; }
        public string AbstractSpoonTaskId { get; set; }
        public int AbstractSpoonTaskIdAsInteger()
        {
            if (string.IsNullOrEmpty(AbstractSpoonTaskId))
                return 0;

            return Convert.ToInt32(AbstractSpoonTaskId);
        }
    }

    public class TaskChanges : List<TaskChange>
    {
        public void AddChangeOverwriteExisting(TaskChange taskChange)
        {
            TaskChange foundTaskChange = null;
            foreach (var c in this)
            {
                if (c.Id == taskChange.Id)
                {
                    foundTaskChange = c;
                }
            }

            if (foundTaskChange != null)
            {
                Remove(foundTaskChange);
            }

            this.Add(taskChange);

        }
    }

    public class TaskChange
    {
        public string Id { get; set; }
        public TaskChangeType TaskChangeType { get; set; }
        public Task NewTask { get; set; }

    }
    public enum TaskChangeType
    {
        None, Create, Change, ChangeAndMove, Delete
    }

    public class TaskListChanges : List<TaskListChange>
    {
    }

    public class TaskListChange
    {
        public string Title { get; set; }
        public TaskListChangeType TaskChangeType { get; set; }
    }

    public enum TaskListChangeType
    {
        None, Create, Delete
    }
}
