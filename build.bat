echo "BUILDING START :)"

echo "Building for Mac (arm)"
dotnet publish CellularSwarm.Visualizer/CellularSwarm.Visualizer.csproj -c Release -r osx-arm64 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true /p:DebugType=None /p:EnableCompressionInSingleFile=true
echo "Building for Mac (Intel)"
dotnet publish CellularSwarm.Visualizer/CellularSwarm.Visualizer.csproj -c Release -r osx-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true /p:DebugType=None /p:EnableCompressionInSingleFile=true

echo "Building for Windows (x64)"
dotnet publish CellularSwarm.Visualizer/CellularSwarm.Visualizer.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true /p:DebugType=None /p:EnableCompressionInSingleFile=true
echo "Building for Windows (arm)"
dotnet publish CellularSwarm.Visualizer/CellularSwarm.Visualizer.csproj -c Release -r win-arm64 --self-contained true /p:PublishSingleFile=true  /p:IncludeNativeLibrariesForSelfExtract=true /p:DebugType=None /p:EnableCompressionInSingleFile=true

echo "BUILDING END :)"