# GDC-7

git setup

- Enable External option in Unity → Preferences → Packages → Repository (only if Unity ver < 4.5)
- Switch to Visible Meta Files in Edit → Project Settings → Editor → Version Control Mode
- Switch to Force Text in Edit → Project Settings → Editor → Asset Serialization Mode
- Save Scene and Project from File menu.

<br>
#gitignore [ignore on commit]
--- Unity Generated ---
+ Temp/<br>
+ Obj/<br>
+ UnityGenerated/<br>
+ Library/<br>
+ Assets/AssetStoreTools*<br>

<br>
--- Visual Studio / MonoDevelop generated ---
+ ExportedObj/
+ *.svd<br>
+ *.userprefs<br>
+ *.csproj<br>
+ *.pidb<br>
+ *.suo<br>
+ *.sln<br>
+ *.user<br>
+ *.unityproj<br>
+ *.booproj<br>

<br>
--- OS generated ---
+ .DS_Store<br>
+ .DS_Store?<br>
+ ._*<br>
+ .Spotlight-V100<br>
+ .Trashes<br>
+ Icon?<br>
+ ehthumbs.db<br>
+ Thumbs.db<br>
