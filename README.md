Synchronize Google Tasks with AbstractSpoon ToDoList
=============================================

## Description
This application is a console application which you can synchronize your Google Tasks with AbstractSpoon ToDoList.
It can synchronize all your AbstractSpoon ToDoLists files in a folder with Google Tasks, using Google Tasks API. Each .tdl file is synchronized as a Google Tasklist.
It supports synchronization both ways, deleting, creating, editing lists/files and tasks.

## First time use and compilation
This source verison have to be setup and compiled before use. The configurations are set in app.config file, which are compiled into the exe file. You have to edit the app.config before compilation.
- ClientId - Google API ClientId
- ClientSecret - Google API Secret
- TodoFilePath - where the file are going to be stored.

You can enable Google Tasks API and create you ClientId and ClientSecret here: https://code.google.com/apis/console

The local folder have to be empty the first time you synchronize with Goolge Tasks.

## How to use
1. Setup settings and compile
2. Open CMD
3. Write: CD "the bin folder"
4. Write: SyncGoogleTasksAbstractspoon.exe /syncall

### Commands:
 - /syncall
 - /synclist Tasklistname
 - /resetall
 - /resetlist Tasklistname

(/resetall deletes local tasklists with Goolge Tasks)

## Why it was made
For many years I have used the free and nicely made AbstractSpoon ToDoList for my PC related tasks and the nice and free GoTasks for Iphone. Then I wanted an synchronization between theese two apps, so i could have one solution for all. I did not found any ready made synchronization solution, so I made one my self.

## About the apps

### AbstractSpoon
AbstractSpoon is a open source task outliner application for windows. It is free, but very very powerfull and rich on functions. The best task application I know for windows.

### Google Tasks and Google Tasks API
Google Tasks is a simple task outliner inside Gmail, with due date and notes. Google has a nicely done and stable API wich makes it a nice place to store your tasks in the Cloud.
There is a lot of apps which integrates with it.

### Recommends GoTasks for Iphone
In my opinion, the best task outliner for Iphone is GoTasks. It is very simple to use. It supports dual lines, drag and drop ordering and indents, due dates, notes. It supports a lot of settings such as changing font sizes and a fairly stable synchronization with Google Tasks. But I must recomend the manual sync option.

## About the sourcecode
This console application is written in Visual Studio 2013 and in C#, for Windows.

## Non developer?
If you are not a developer and want to use this app, you may compile this with the free verison of visual studio for windows.
Or you can make a request for a new verison of this app.

## Licence
This sourcecode is free to fork. But please try to contribute to the original application. If you want to copy it into an commercial application, it is okay, but let me know in advance.
