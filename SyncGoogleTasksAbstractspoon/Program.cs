
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Authentication.OAuth2;
using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;
using Google.Apis.Services;
using Google.Apis.Tasks.v1;
using SyncGoogleTasksAbstractSpoon.Data;

namespace SyncGoogleTasksAbstractSpoon
{
    class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("");
            Console.WriteLine("*** Sync Google Tasks with AbstractSpoon ToDoList ***");
            Console.WriteLine("");

            Console.Write("(Sync all lists or choose one list. If any problem occurs, use the reset-command. If a task are edited on both sides, local task will overwrite the remote version.) ");
            Console.WriteLine("Folder and API settings are compiled into app.config, and must be set.");
            Console.WriteLine("");
          
            if (args.Length == 0)
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("");
                Console.WriteLine("/syncall");
                Console.WriteLine("(Sync all task lists.)");
                Console.WriteLine("");
                Console.WriteLine("/synclist Name");
                Console.WriteLine("(Sync named list to and from the remote Google Cloud.)");
                Console.WriteLine("");
                Console.WriteLine("/resetall");
                Console.WriteLine("(Deletes all local existing task lists and then downloading all the task lists from the remote Google Cloud)");
                Console.WriteLine("");
                Console.WriteLine("/resetlist Name");
                Console.WriteLine("(Deletes the named task list if it exists, and then downloading the task list from the remote Gooogle Cloud)");

                Console.WriteLine("");
            }
            else
            {
                var a = args[0];

                if (a == "/syncall")
                {
                    Sync();
                }
                else if (a == "/synclist")
                {
                    if (args.Length > 1)
                    {
                        Sync(doReset: false, taskListTitle: args[1]);
                    }
                }
                else if (a == "/resetall")
                {
                    Sync(doReset: true);
                }
                else if (a == "/resetlist")
                {
                    if (args.Length > 1)
                    {
                        Sync(doReset: true, taskListTitle: args[1]);
                    }
                }
                else
                {
                    Console.WriteLine("Invalid command");
                }
            }
        }

        private static void Sync(bool doReset = false, string taskListTitle = "")
        {
            if (!FolderPath.ValidateTodoFilePath())
            {
                Console.WriteLine("The setting TodoFilePath in app.config failed to load");
                return;
            }

            FilesHelper.EnsureFoldersExists();

            Console.WriteLine("Logging into Google Task Service ...");
            GoogleTasksManager.LogonToService();

            //Submit local task list changes to remote server
            if (!doReset && taskListTitle == "")
            {
                var listChanges = ChangeListManager.CreateListOfLocalListChangesToSubmitRemotely();
                if (listChanges.Count > 0)
                {
                    Console.WriteLine("Submit local task list changes... ");
                    
                    GoogleTasksManager.SubmitLocalTaskListChanges(listChanges);
                    
                    FilesHelper.DeleteLocalTaskLists(listChanges);
                }
            }

            //Load remote task lists.
            var remoteTaskLists = GoogleTasksManager.LoadLists(taskListTitle);

            if (remoteTaskLists.Count == 0)
            {
                Console.WriteLine("\nNo remoteTaskList found.");
                return;
            }

            //Reset all local tasklists when this option is selected
            if (doReset && taskListTitle == "") FilesHelper.DeleteAllTaskLists();

            //For each remote task list, downloading task list or sync with local task list.
            foreach (var remoteTaskList in remoteTaskLists)
            {
                Console.WriteLine("Syncing TaskList " + remoteTaskList.Title + "...");

                Data.TaskList localTaskList = null;
                
                if (!doReset)
                {
                    localTaskList = AbstractSpoonXmlManager.LoadTaskListFromXmlFile(FolderPath.TodoFilePath, "tdl", remoteTaskList.Title);
                    var localTaskListSync = AbstractSpoonXmlManager.LoadTaskListFromXmlFile(FolderPath.LocalChangesCompareFilePath, "xml", remoteTaskList.Title);
                    var localTaskListChanges = ChangeListManager.CreateListOfLocalTaskChanges(localTaskListSync, localTaskList);
                    // Send all changes in local file to remote server (Google Task Cloud).
                    Console.WriteLine("Local change count:" + localTaskListChanges.Count);
                    GoogleTasksManager.SubmitLocalTaskChanges(remoteTaskList, localTaskListChanges);
                }

                GoogleTasksManager.LoadTasks(remoteTaskList);

                TasksReferenceManager.ConvertTaskIdToAbstractSpoon(remoteTaskList);

                if (!doReset) AbstractSpoonXmlManager.TransferXmlAttributesAndElements(remoteTaskList, localTaskList);

                AbstractSpoonXmlManager.SaveAbstractSpoonTaskListToXmlFile(FolderPath.TodoFilePath, "tdl", remoteTaskList);
                AbstractSpoonXmlManager.SaveAbstractSpoonTaskListToXmlFile(FolderPath.LocalChangesCompareFilePath, "xml", remoteTaskList);

                Console.WriteLine("Done");

            }
            // Delete local tasklists where not existing on remote server.
            if (taskListTitle == "")
            {
                string[] localTodoFiles = FilesHelper.GetTodoFiles();

                foreach (var remotelyDeleted in ChangeListManager.GetChangeListOfRemotelyDeletedLists(localTodoFiles, remoteTaskLists))
                {
                    Console.WriteLine("Delete local verison of \"" + remotelyDeleted.Title + "\"");
                    FilesHelper.DeleteLocalTaskListByTitle(remotelyDeleted.Title);
                }    
            }
            
        }
    }
}
