## Unity D2R Scene Editor
Use Unity to edit position, rotation and scale of D2R models in a level preset. Inspired by Shalzuth's [D2RStudio](http://web.archive.org/web/20210508115732/github.com/shalzuth/d2rstudio)


### Create 3D Scene in Unity
- Launch [Unity](https://store.unity.com/download-nuo) and create a new 3d project and a level
- Add the `saveJson.cs` script to a GameObject (like the Main Camera) of the new 3d project level
- Add a Button to the level canvas and give it the text `Save`
- Import model files (`.fbx`, `.dae` format) into Unity Assets. D2R's `.model` files can be converted to `.fbx` and `.dae` format with Noesis and the D2R_Reader plugin (see [Credits and Tools](#credits-and-tools))
- Click `Play` and `Pause` then position models in the scene and add the `SelectionBaseObject.cs` script using the Inspector window on the right. (Identical models can be copy-pasted in the Scene Editor window)
- Each model object should have the `Filepath` variable set in Unity (in the Inspector window) to the path of the `.model` file that D2R uses. E.g. `data/hd/env/model/act3/docktown/act3_docktown_docks/dock01.model` for an act3 dock
- Click `Play` again once done editing and then click the `Save` button in the Scene window
- The scene terrain object is using the act3 image `kurast_minimap_rot_skew2.png`
![Unity docktown scene](./images/act3town_unity_scene.jpg)


### Unity Appends to D2R Preset
- The Unity `Save` script loops through all models on the scene and appends them to a base json D2R preset file (an example `docktown3_base.json` is included in this repo)
- Change the paths to `docktown_base` and `docktown_final` on lines 15 and 16 in the file `saveJson.cs` so they point to file locations on your system
- Launch D2R with the `docktown_final` json file in your D2R's data preset folder and see the model positions in-game
![Docktown in game](./images/act3town_preset_ingame.jpg)


### Credits and Tools
- D2RStudio by Shalzuth http://web.archive.org/web/20210508115732/github.com/shalzuth/d2rstudio
- LSLib by Norbyte https://github.com/Norbyte/lslib
- Noesis model viewer and converter https://richwhitehouse.com/index.php?content=inc_projects.php
- D2R_reader plugin for Noesis https://forum.xentax.com/viewtopic.php?f=16&t=22277&start=165#p173650
- D2R texture format https://github.com/CucFlavius/Zee-010-Templates/blob/main/DiabloIIResurrected_Texture.bt


### Copyrights
Diablo II and Diablo II: Resurrected are [copyrighted](https://www.blizzard.com/en-us/legal/9c9cb70b-d1ed-4e17-998a-16c6df46be7b/copyright-notices) by Blizzard Entertainment, Inc. All rights reserved. Diablo II, Diablo II: Resurrected and Blizzard Entertainment are [trademarks](https://www.blizzard.com/en-us/legal/9c9cb70b-d1ed-4e17-998a-16c6df46be7b/copyright-notices) or registered trademarks of Blizzard Entertainment, Inc. in the U.S. and/or other countries.  
All trademarks referenced here are the properties of their respective owners.

This project and its maintainers are not associated with or endorsed by Blizzard Entertainment, Inc.
