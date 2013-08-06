using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Linq;
using System.Xml;
using DotNetOpenAuth.OAuth2;

using Google.Apis.Authentication;
using Google.Apis.Authentication.OAuth2;
using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;
using Google.Apis.Samples.Helper;
using Google.Apis.Services;
using Google.Apis.Tasks.v1;
using Google.Apis.Tasks.v1.Data;
using Google.Apis.Util;
using SyncGoogleTasksAbstractSpoon.Data;
using Task = Google.Apis.Tasks.v1.Data.Task;
using TaskList = Google.Apis.Tasks.v1.Data.TaskList;


namespace SyncGoogleTasksAbstractSpoon
{
    class GoogleTasksManager
    {
        private static readonly string Scope = TasksService.Scopes.Tasks.GetStringValue();

        /// <summary>
        /// The remote service on which all the requests are executed.
        /// </summary>
        public static TasksService Service { get; private set; }

        public static void LogonToService()
        {
            // Initialize the service.
            Service = new TasksService(new BaseClientService.Initializer()
            {
                Authenticator = CreateAuthenticator()
            });
        }

        private static IAuthenticator CreateAuthenticator()
        {
            var provider = new NativeApplicationClient(GoogleAuthenticationServer.Description);
            provider.ClientIdentifier = GoogleApiSettings.ClientId;
            provider.ClientSecret = GoogleApiSettings.ClientSecret;
            return new OAuth2Authenticator<NativeApplicationClient>(provider, GetAuthentication);
        }
        /// <summary>
        /// Get the authentication from the user in a web browser.
        /// There is a known issue. When authenticated, the console can not be elevated (admin).
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        private static IAuthorizationState GetAuthentication(NativeApplicationClient client)
        {
            // You should use a more secure way of storing the key here as
            // .NET applications can be disassembled using a reflection tool.
            const string STORAGE = "SyncGoogleTasksAbstractSpoon";
            const string KEY = "y},drdzf11x9;87";
            string scope = TasksService.Scopes.Tasks.GetStringValue();

            // Check if there is a cached refresh token available.
            IAuthorizationState state = AuthorizationMgr.GetCachedRefreshToken(STORAGE, KEY);
            if (state != null)
            {
                try
                {
                    client.RefreshToken(state);
                    return state; // Yes - we are done.
                }
                catch (DotNetOpenAuth.Messaging.ProtocolException ex)
                {
                    CommandLine.WriteError("Using existing refresh token failed: " + ex.Message);
                }
            }

            // Retrieve the authorization from the user.
            state = AuthorizationMgr.RequestNativeAuthorization(client, scope);
            AuthorizationMgr.SetCachedRefreshToken(STORAGE, KEY, state);
            return state;
        }

        public static List<Data.TaskList> LoadLists(string listTitle = "")
        {
            var lists = new List<Data.TaskList>();
            var req = Service.Tasklists.List();

            foreach (TaskList list in req.Fetch().Items)
            {
                if (listTitle.Equals("") || list.Title.Trim().Equals(listTitle, StringComparison.OrdinalIgnoreCase))
                {
                    var allTasks = new List<Data.Task>();
                    var datalist = new Data.TaskList();

                    datalist.Title = list.Title.Trim();
                    datalist.Id = list.Id;
                    datalist.SelfLink = list.SelfLink;
                    datalist.Updated = datalist.Updated;

                    lists.Add(datalist);
                }
            }
            return lists;
        }

        public static void LoadTasks(Data.TaskList taskList)
        {
            var allTasks = new List<Data.Task>();
            var req = Service.Tasks.List(taskList.Id);
            string pageToken = "1";

            do
            {
                req.PageToken = pageToken;
                var res = req.Fetch();
                var tasks = res.Items;

                if (tasks != null)
                {
                    AddTasksToListObject(allTasks, tasks);
                }

                pageToken = res.NextPageToken;

            } while (pageToken != null);

            DataHelper.AddParentTasks(taskList, allTasks);
            DataHelper.AddChildTasks(taskList, allTasks);
            DataHelper.AddTopTasks(taskList, allTasks);
        }

        private static void AddTasksToListObject(List<Data.Task> allTasks, IList<Task> tasks)
        {
            foreach (var task in tasks)
            {
                if (!(task.Deleted.HasValue && task.Deleted.Value))
                {
                    var datatask = new Data.Task();
                    datatask.Id = task.Id;
                    datatask.Title = task.Title;
                    datatask.Notes = task.Notes;
                    if (task.Due != null)
                        datatask.DueDate = System.Xml.XmlConvert.ToDateTime(task.Due, XmlDateTimeSerializationMode.Utc);

                    if (task.Status == "completed")
                        datatask.DoneDate = System.Xml.XmlConvert.ToDateTime(task.Completed, XmlDateTimeSerializationMode.Utc);

                    datatask.GoogleTaskParentTmp = task.Parent;

                    allTasks.Add(datatask);
                }
            }
        }

        public static void SubmitLocalTaskChanges(Data.TaskList taskList, TaskChanges localchanges)
        {
            foreach (var change in localchanges)
            {
                switch (change.TaskChangeType)
                {
                    case TaskChangeType.None:
                        break;
                    case TaskChangeType.Create:
                        InsertTask(taskList, change.NewTask);
                        break;
                    case TaskChangeType.Change:
                        UpdateTask(taskList, change.NewTask);
                        break;
                    case TaskChangeType.ChangeAndMove:
                        UpdateTask(taskList, change.NewTask);
                        MoveTask(taskList, change.NewTask);
                        break;
                    case TaskChangeType.Delete:
                        DeleteTask(taskList, change.Id);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            if (localchanges.Count > 0)
            {
                TasksReferenceManager.SaveReferencesToXml(taskList.Title);
            }

        }

        private static void InsertTask(Data.TaskList taskList, Data.Task task)
        {
            var gtask = new Task();

            gtask.Title = task.Title;
            gtask.Status = task.DoneDate.HasValue ? "completed" : "needsAction";
            gtask.Completed = task.DoneDate.HasValue ? System.Xml.XmlConvert.ToString(task.DoneDate.Value, XmlDateTimeSerializationMode.Utc) : null;
            gtask.Due = task.DueDate.HasValue ? System.Xml.XmlConvert.ToString(task.DueDate.Value, XmlDateTimeSerializationMode.Utc) : null;
            gtask.Notes = task.Notes;

            var req = Service.Tasks.Insert(gtask, taskList.Id);


            if (task.PreviousId != "")
                req.Previous = TasksReferenceManager.GetGoogleTaskId(taskList.Title, task.PreviousId);
            if (task.ParentId != "")
                req.Parent = TasksReferenceManager.GetGoogleTaskId(taskList.Title, task.ParentId);

            var googleTaskId = req.Fetch().Id;

            TasksReferenceManager.AddNewReference(taskList.Title, task.Id, googleTaskId);
        }

        private static void DeleteTask(Data.TaskList taskList, string taskId)
        {
            var googleTaskId = TasksReferenceManager.GetGoogleTaskId(taskList.Title, taskId);
            if (googleTaskId == "")
            {
                return;
            }

            Service.Tasks.Delete(taskList.Id, googleTaskId).Fetch();
            TasksReferenceManager.RemoveReference(taskList.Title, taskId, googleTaskId);
        }

        private static void UpdateTask(Data.TaskList taskList, Data.Task task)
        {
            var googleTaskId = TasksReferenceManager.GetGoogleTaskId(taskList.Title, task.Id);
            if (googleTaskId == "")
            {
                return;
            }

            Task gtask = Service.Tasks.Get(taskList.Id, googleTaskId).Fetch();

            gtask.Title = task.Title;
            gtask.Status = task.DoneDate.HasValue ? "completed" : "needsAction";
            gtask.Completed = task.DoneDate.HasValue ? System.Xml.XmlConvert.ToString(task.DoneDate.Value, XmlDateTimeSerializationMode.Utc) : null;
            gtask.Due = task.DueDate.HasValue ? System.Xml.XmlConvert.ToString(task.DueDate.Value, XmlDateTimeSerializationMode.Utc) : null;
            gtask.Notes = task.Notes;


            Service.Tasks.Update(gtask, taskList.Id, googleTaskId).Fetch();
        }

        //try
        //{
        //    request.Fetch();
        //    CommandLine.WriteResult("Result", "Success!");
        //} 
        //catch (GoogleApiRequestException ex)
        //{
        //    CommandLine.WriteResult(
        //        "Result", "Failure! (" + ex.RequestError.Code + " - " + ex.RequestError.Message + ")");
        //}

        private static void MoveTask(Data.TaskList taskList, Data.Task task)
        {
            var googleTaskId = TasksReferenceManager.GetGoogleTaskId(taskList.Title, task.Id);
            if (googleTaskId == "")
            {
                return;
            }

            var req = Service.Tasks.Move(taskList.Id, googleTaskId);

            req.Previous = TasksReferenceManager.GetGoogleTaskId(taskList.Title, task.PreviousId);
            req.Parent = TasksReferenceManager.GetGoogleTaskId(taskList.Title, task.ParentId);

            req.Fetch();
        }


        public static void SubmitLocalTaskListChanges(TaskListChanges taskListChanges)
        {
            foreach (var taskListChange in taskListChanges)
            {
                switch (taskListChange.TaskChangeType)
                {
                    case TaskListChangeType.Create:
                        InsertTaskList(taskListChange.Title);

                        break;
                    case TaskListChangeType.Delete:
                        DeleteTaskList(taskListChange.Title);
                        break;
                }
            }
        }

        private static void DeleteTaskList(string taskListTitle)
        {
            foreach (var taskList in LoadLists(taskListTitle))
            {
                Service.Tasklists.Delete(taskList.Id).Fetch();
            }

        }

        private static void InsertTaskList(string taskListTitle)
        {
            Service.Tasklists.Insert(new TaskList()
                {
                    Title = taskListTitle
                }).Fetch();
        }
    }
}
