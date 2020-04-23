UpdateMe - Torch Plugin for Space Engineers DS
----------------------------------------------

v1.0

This is a free to use UNOFFICIAL Torch plugin for Space Engineers DS.
Feel freely to use, modify, publish and republish this plugin as long as you include a public visible mention of their authors and collaborators.
You can suggest and/or implement new features or fixes and you will be added as collaborator or author.


What it does?
------------

Nothing fancy. 
This plugin force Torch to check every certain time if a new version of Torch.Server or a Space Engineers DS version has been released using the same implementation as Torch uses to do it.
When a new version is detected it ask Torch for a restart showing log and chat messages about it and the time before restart.


Manual installation:
--------------------

1- Compile or download a compiled version of this source code. (files must be zipped in a file)
2- Copy the <Guid> tag included in the manifest to the Torch.cfg file of your server under <Plugins> tag. (remember to do it as <guid> and not as <Guid>)
3- Copy the zip file to Plugins directory of your Torch server.
4- That's it, like any other plugin. You can configure it with the UpdateMe.cfg file.

UpdateMe.cfg example:

<?xml version="1.0" encoding="utf-8"?>
<UpdateMePluginConfig xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <Enabled>true</Enabled>
  <LogEnabled>true</LogEnabled>
  <RestartForNewTorchVersion>true</RestartForNewTorchVersion>
  <MessageForNewTorchVersion>A new Torch version has been found.</MessageForNewTorchVersion>
  <RestartForNewDSVersion>true</RestartForNewDSVersion>
  <MessageForNewDSVersion>A new Space Engineers DS version has been found.</MessageForNewDSVersion>
  <RestartInMinutes>2</RestartInMinutes> <!--How much time the server wait before restart. -->
  <CheckFrequencyInMinutes>5</CheckFrequencyInMinutes> <!--How much time the server wait before check again for updates. -->
</UpdateMePluginConfig>


TODO:
-----

- Handle case when a new DS version brakes Torch so a manual installation of Torch.Server will be required.
- I would like to see a feature like this included by default in Torch or Torch 2 in benefit of all SE community :)



Authors and collaborators:
-------------------------

titan3217

Long live Lord Clang.