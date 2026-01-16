echo "Building for Mac (arm)"
dotnet publish CellularSwarm.Visualizer/CellularSwarm.Visualizer.csproj  -c Release -r osx-arm64 --self-contained true /p:PublishSingleFile=true