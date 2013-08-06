Synchronize Google Tasks with AbstractSpoon ToDoList
=============================================

## Description
This application is a console application which you can synchronize your Google Tasks with AbstractSpoon ToDoList.
It can synchronize all your AbstractSpoon ToDoLists files in a folder with Google Tasks, using Google Tasks API. Each .tdl file synchronizes as a Google task list.
It supports synchronization both ways, deleting, creating, editing lists/files and tasks.

## How to setup and use

1. Download the source code and open the projects in visual studio.
2. Setup settings in app.config
3. Compile
4. Open CMD
5. Write: CD "the bin folder"
6. Write: SyncGoogleTasksAbstractspoon.exe /syncall

You have to set up all the settings in app.config and then compile.
- ClientId - Google API ClientId
- ClientSecret - Google API Secret
- TodoFilePath - where the files are going to be stored.

You can enable Google Tasks API and create you ClientId and ClientSecret here: https://code.google.com/apis/console

The local folder have to be empty the first time you synchronize with Google Tasks.

## Commands:
SyncGoogleTasksAbstractspoon.exe /syncall

 - /syncall
 - /synclist Tasklistname
 - /resetall
 - /resetlist Tasklistname

("/resetall" deletes local task lists with Google Tasks)

## Why this sync-app was made and about the 3.party apps.
For many years, I have used the free and nicely made AbstractSpoon ToDoList for my PC related tasks and the nice and free GoTasks for IPhone. Then I wanted a synchronization between these two favorite apps, so I could use both of the apps in one solution. I did not found any ready-made synchronization solution, so I made one myself.

### AbstractSpoon ToDoList
AbstractSpoon is a open source task outliner application for Windows. It is free, but very powerful and rich on functions. The best task application I have used for Windows. The file format is XML.
http://www.abstractspoon.com/

### Google Tasks and Google Tasks API
Google Tasks is a simple task outliner inside Gmail, with due date and notes. Google has a nicely done and stable API wich makes it a nice place to store your tasks in the Cloud.
There is many apps, which integrates with it.
http://www.gmail.com

### Recommends GoTasks for IPhone
In my opinion, the best task outliner for IPhone is GoTasks. It is very simple to use. It supports dual lines, drag and drop ordering and indents, due dates, notes. It supports many settings such as changing font sizes and a stable synchronization with Google Tasks. Note: I recommends the manual sync option.
https://itunes.apple.com/no/app/gotasks-google-tasks-client/id389113399?mt=8

## About the source code
This console application are written in Visual Studio 2013 and in C#, for Windows.

## Not a developer?
If you are not a developer and want to use this app, you may compile this with the free version of visual studio for windows.
Or you can make a request and let me now if you want a new version of this app.

## License
This sourcecode is free to fork. But please try to contribute to the original application. If you want to copy it into an commercial application, it is okay, but let me know in advance.
