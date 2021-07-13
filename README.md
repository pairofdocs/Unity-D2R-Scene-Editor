## Unity D2R Scene Editor
Use unity to edit position, rotation and scale of D2R models in a level preset


### Create 3D Scene in Unity\
- Launch [Unity](https://store.unity.com/download-nuo) and create a new 3d project and a level
- Add the `saveJson.cs` script to a GameObject of a new 3d project level
- Import model files (`.fbx`, `.dae` format) into Unity Assets. D2R's `.model` files can be converted to `.fbx` and `.dae` format with Noesis and the D2R_Reader plugin (see [Credits and Tools](#credits-and-tools))
- Click `Play` and `Pause` then position models in the scene and add the `SelectionBaseObject.cs` script using the Inspector window on the right. (Identican models can be copy-pasted in the Scene Editor)
- Click `Play` again once done editing and then the `Save` button on the left hand side


### Unity Appends to D2R Preset
- The Unity `Save` script loops through all models on the scene and appends them to a base json D2R preset file
- Use the final json file in your D2R's data preset folder and see the model positions in-game


### Credits and Tools
- Noesis model viewer and converter https://richwhitehouse.com/index.php?content=inc_projects.php
- D2R_reader plugin for Noesis https://forum.xentax.com/viewtopic.php?f=16&t=22277&start=165#p173650
- D2R texture format https://github.com/CucFlavius/Zee-010-Templates/blob/main/DiabloIIResurrected_Texture.bt


### Copyrights
Models and images are copyrighted by Blizzard Entertainment.
