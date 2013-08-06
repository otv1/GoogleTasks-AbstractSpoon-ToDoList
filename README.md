Sync Google Tasks with AbstractSpoon ToDoList
=============================================

## Description
This application is a console application where you can synchronize your Google Tasks with AbstractSpoon ToDoList.
It can synchronize all your AbstractSpoon ToDoLists files in a folder with Google Tasks, using Google Tasks API. Each file is synchronized as a Google Tasklist.
It supports syncronization both ways, deleting, creating, editing lists/files and tasks.

## About the sourcecode
This console application is written in Visual Studio 2013 and in C#, for windows.

## First time use and compilation
The configurations are set in app.config file, which are compiled into the exe file. You have to edit the app.config before compilation.
- ClientId - Google API ClientId
- ClientSecret - Google API Secret
- TodoFilePath - where the file are going to be stored.

You can enable Google Tasks API and create you ClientId and ClientSecret here: https://code.google.com/apis/console

The local folder have to be empty the first time you synchronize with Goolge Tasks.

## Non developer?
If you are not a developer and want to use this app, you may compile this with the free verison of visual studio for windows or mono on other platform.
Or you can me a request.

## Licence
This sourcecode is free to use and fork on github. If you want to copy it into an commercial application, it is okay, but please let me know.