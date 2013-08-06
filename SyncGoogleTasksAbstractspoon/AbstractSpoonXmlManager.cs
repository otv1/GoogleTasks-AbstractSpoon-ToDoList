using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;


namespace SyncGoogleTasksAbstractSpoon
{
    class AbstractSpoonXmlManager
    {
        public static void SaveAbstractSpoonTaskListToXmlFile(string folderpath, string extension, Data.TaskList taskList)
        {
            var settings = new XmlWriterSettings { Indent = true, Encoding = System.Text.Encoding.Unicode };

            var writer = XmlWriter.Create(folderpath + taskList.Title + "." + extension, settings);
            writer.WriteStartDocument();

            writer.WriteStartElement("TODOLIST");
            writer.WriteAttributeString("NEXTUNIQUEID", TasksReferenceManager.GetNextUniqueAbstractSpoonId(taskList));
            WriteTasksToXml(writer, taskList.TopTask.ChildTasks);
            writer.WriteEndDocument();

            writer.Flush();
            writer.Close();
        }

        private static void WriteTasksToXml(XmlWriter writer, List<Data.Task> tasks)
        {
            foreach (var task in tasks)
            {
                writer.WriteStartElement("TASK");

                writer.WriteAttributeString("TITLE", task.Title);

                writer.WriteAttributeString("ID", task.Id);

                if (task.DueDate.HasValue)
                    writer.WriteAttributeString("DUEDATE", task.DueDate.Value.ToOADate().ToString());

                if (task.DoneDate.HasValue)
                    writer.WriteAttributeString("DONEDATE", task.DoneDate.Value.ToOADate().ToString());

                if (task.AbstractSpoonAttributes != null)
                    foreach (var attr in task.AbstractSpoonAttributes.Where(attr =>
                                                                            attr.Name != "TITLE" &&
                                                                            attr.Name != "ID" &&
                                                                            attr.Name != "DONEDATE" &&
                                                                            attr.Name != "DUEDATE"))
                    {
                        writer.WriteAttributeString(attr.Name.ToString(), attr.Value);
                    }
                if (!string.IsNullOrEmpty(task.Notes))
                {
                    writer.WriteStartElement("COMMENTS");
                    writer.WriteValue(task.Notes);
                    writer.WriteEndElement();
                }
                if (task.AbstractSpoonElements != null)
                    foreach (var element in task.AbstractSpoonElements.Where(element => element.Name != "TASK" && element.Name != "COMMENTS"))
                    {
                        writer.WriteElementString(element.Name.ToString(), element.Value);
                    }

                WriteTasksToXml(writer, task.ChildTasks);

                writer.WriteEndElement();
            }
        }

        public static Data.TaskList LoadTaskListFromXmlFile(string folderpath, string extension, string title)
        {
            var taskList = new Data.TaskList { Title = title, Id = "" };

            if (System.IO.File.Exists(folderpath + title + "." + extension))
                ProcessReadElement(XDocument.Load(folderpath + title + "." + extension).Root, taskList.TopTask);

            return taskList;
        }

        private static void ProcessReadElement(XElement element, Data.Task task)
        {
            if (element.Name == "TASK")
            {
                task.Id = element.Attribute("ID").Value;
                task.Title = element.Attribute("TITLE").Value;
                task.DueDate = FromAttrOADate(element.Attribute("DUEDATE"));
                task.DoneDate = FromAttrOADate(element.Attribute("DONEDATE"));
                task.AbstractSpoonAttributes = element.Attributes();
                task.AbstractSpoonElements = element.Elements();
            }

            if (element.HasElements)
            {
                foreach (var child in element.Elements())
                {
                    if (child.Name == "TASK")
                    {
                        var childtask = new Data.Task();
                        task.ChildTasks.Add(childtask);
                        childtask.ParentTask = task;
                        ProcessReadElement(child, childtask);
                    }
                    if (child.Name == "COMMENTS")
                    {
                        task.Notes = child.Value;
                    }
                }
            }
        }
        private static DateTime? FromAttrOADate(XAttribute attribute)
        {
            if (attribute != null)
            {
                string s = attribute.Value.Replace(".", ",");
                double d = Convert.ToDouble(s);
                return DateTime.FromOADate(d);
            }
            return null;
        }

        public static void TransferXmlAttributesAndElements(Data.TaskList toTaskList, Data.TaskList fromTaskList)
        {
            foreach (var taskTo in toTaskList.AllTasks)
                foreach (var taskFrom in fromTaskList.AllTasks.Where(taskFrom => taskTo.Id == taskFrom.Id))
                {
                    taskTo.AbstractSpoonAttributes = taskFrom.AbstractSpoonAttributes;

                    taskTo.AbstractSpoonElements = taskFrom.AbstractSpoonElements;
                }
        }

    }
}
