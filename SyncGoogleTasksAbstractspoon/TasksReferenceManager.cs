using System.Collections.Generic;
using System.IO;
using System.Linq;
using SyncGoogleTasksAbstractSpoon.Data;
 
namespace SyncGoogleTasksAbstractSpoon
{
    class TasksReferenceManager
    {
        private static readonly Dictionary<string, List<TaskReference>> Cache = new Dictionary<string, List<TaskReference>>();

        /// <summary>
        /// Converts Google TaskId to AbstractSpoon task ID. New references are stored in Xml.
        /// </summary>
        public static void ConvertTaskIdToAbstractSpoon(TaskList googleTaskTaskList)
        {
            foreach (var task in googleTaskTaskList.AllTasks)
            {
                task.Id = GetAbstractSpoonTaskId(googleTaskTaskList, task.Id);
                task.GoogleTaskParentTmp = ""; // Remove not needed GoogleTaskParentTmp
            }

            //Save ref. information to Xml file.
            string taskListTitle = googleTaskTaskList.Title;
            SaveReferencesToXml(taskListTitle);
        }

        /// <summary>
        /// Get the task id in abstractspoon. When new value, generates new id.
        /// </summary>
        private static string GetAbstractSpoonTaskId(TaskList googleTaskList, string googleTaskId)
        {
            var taskListTitle = googleTaskList.Title;

            var references = GetCachedReferences(taskListTitle);

            foreach (var reference in references.Where(reference => reference.GoogleTaskId == googleTaskId))
                return reference.AbstractSpoonTaskId;

            var abstractSpoonTaskId = GetNextUniqueAbstractSpoonId(references);

            AddNewReference(taskListTitle, abstractSpoonTaskId, googleTaskId);
            
            return abstractSpoonTaskId;
        }

        public static string GetNextUniqueAbstractSpoonId(TaskList taskList)
        {
            var references = GetCachedReferences(taskList.Title);
            return GetNextUniqueAbstractSpoonId(references);
        }
        
        private static string GetNextUniqueAbstractSpoonId(List<TaskReference> references)
        {
            var maxTaskId = references.Select(reference => reference.AbstractSpoonTaskIdAsInteger()).Concat(new[] { 0 }).Max();
            return (++maxTaskId).ToString();
        }

        private static List<TaskReference> GetCachedReferences(string taskListTitle)
        {
            List<TaskReference> references;
            if (!Cache.ContainsKey(taskListTitle))
            {
                references = LoadReferencesFromXml(taskListTitle) ?? new List<TaskReference>();

                Cache.Add(taskListTitle, references);
            }
            else
            {
                references = Cache[taskListTitle];
            }

            return references;
        }


        public static void SaveReferencesToXml(string taskListTitle)
        {
            var references = GetCachedReferences(taskListTitle);

            using (var stream = new FileStream(FolderPath.ReferenceFilePath + taskListTitle + ".xml",
                                           FileMode.Create,
                                           FileAccess.Write, FileShare.None))
            {
                var x = new System.Xml.Serialization.XmlSerializer(references.GetType());
                x.Serialize(stream, references);
            }

        }

        public static List<TaskReference> LoadReferencesFromXml(string taskListTitle)
        {
            string filepath = FolderPath.ReferenceFilePath + taskListTitle + ".xml";

            if (!File.Exists(filepath))
                return null;

            if (File.ReadAllText(filepath).Length == 0)
                return null;

            using (var stream = new FileStream(FolderPath.ReferenceFilePath + taskListTitle + ".xml",
                                               FileMode.Open,
                                               FileAccess.Read, FileShare.Read))
            {
                var x = new System.Xml.Serialization.XmlSerializer(typeof(List<TaskReference>));
                return (List<TaskReference>)x.Deserialize(stream);
            }

        }

        public static string GetGoogleTaskId(string taskListTitle, string taskId)
        {

            var references = GetCachedReferences(taskListTitle);

            foreach (var pair in references.Where(reference => reference.AbstractSpoonTaskId == taskId))
                return pair.GoogleTaskId;

            return "";

        }

        public static void AddNewReference(string listTitle, string abstractspoonId, string googleTaskId)
        {
            var references = GetCachedReferences(listTitle);

            var reference = new TaskReference
            {
                GoogleTaskId = googleTaskId,
                AbstractSpoonTaskId = abstractspoonId
            };

            references.Add(reference);
        }

        public static void RemoveReference(string taskListTitle, string abstractspoonId, string googleTaskId)
        {
            var references = GetCachedReferences(taskListTitle);
            
            TaskReference taskReferenceToRemove = null;
            
            foreach (var reference in references.Where(reference => reference.AbstractSpoonTaskId == abstractspoonId && reference.GoogleTaskId == googleTaskId))
                taskReferenceToRemove = reference;

            if (taskReferenceToRemove != null)
                references.Remove(taskReferenceToRemove);    
            
        }
    }
}
