# GDC-7

# git setup
- Enable External option in Unity → Preferences → Packages → Repository (only if Unity ver < 4.5)
- Switch to Visible Meta Files in Edit → Project Settings → Editor → Version Control Mode
- Switch to Force Text in Edit → Project Settings → Editor → Asset Serialization Mode
- Save Scene and Project from File menu.

# .gitignore - ignore these folders/files on commit
# =============== #
# Unity generated #
# =============== #
Temp/
Obj/
UnityGenerated/
Library/
Assets/AssetStoreTools*

# ===================================== #
# Visual Studio / MonoDevelop generated #
# ===================================== #
ExportedObj/
*.svd
*.userprefs
*.csproj
*.pidb
*.suo
*.sln
*.user
*.unityproj
*.booproj

# ============ #
# OS generated #
# ============ #
.DS_Store
.DS_Store?
._*
.Spotlight-V100
.Trashes
Icon?
ehthumbs.db
Thumbs.db

