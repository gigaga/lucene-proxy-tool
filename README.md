# Lucene Proxy Tool #

## Overview ##
This tool is needed to allow read Lucene indexes under Windows 10 platform. With this tools, it is possible to get search results although it is not possible in mobile platform. Indeed, in mobile platform (Windows Universal), all .Net library (compiled with .Net framework) are unusable, so, for example, use Lucene.Net library in Windows Universal App is impossible. This tool allows the communication between mobile platform & desktop platform.

## Use ##
Lucene Proxy Tool can be used: 

- From command line: 
	`Lucene Proxy Tool.exe <indexFolder> <field> <token> <maxResult> <fileOutput|stdout>`
	- indexFolder: Lucene index folder used to do search
	- field: field on which is done search
	- token: token used for search
	- maxResult: max result
	- result: can be:
		- fileOutput: in this case, the result of search is written on the file specified
		- stdout: in this case, the result of search is written on console
- From file launcher (lptf):
	- A lptf file used the same arguments of command line and in the same order. Each line is a argument.

## Compilation ##
To generate Lucene Proxy Tool binary, open "lucene_proxy_tool.sln" file with Visual Studio and launch build from "Build / Build Solution" menu.
The binary is generate in "lucene_proxy_tool/bin" folder.

## Installer program ##
After compilation, it is possible to generate the installer program (from Inno Setup Software). Open "installer.iss" file with Inno Setup and launch build from "Build / Compile" menu. The binary is generate in "Output" folder.
