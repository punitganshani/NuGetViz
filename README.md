# NuGetViz

**Available at [NuGetViz.com](http://www.nugetviz.com)**

NuGet Visualizer allows searching of NuGet packages from any repository, enables visualizing dependencies of NuGet packages using graphs, allows downloads of NuGet packages (.nupkg)

Compatible with NuGet API v3.0 and built on .NET Framework 4.5.2, this utility was created to allow searching of NuGet packages within enterprise environment.

Configuration is maintained in nugetviz.json file which is expected at the root of the application. You can add multiple NuGet repository URI in the configuration file like,

```
{
  "Repositories": [
    {
      "name": "NuGet",
      "key": "nuget",
      "uri": "https://www.nuget.org/api/v2/"
    }
  ]
}
```



# Screenshots

View the screenshots at [NuGet Visualizer](http://www.ganshani.com/open-source/nuget-visualizer/)

# Prerequisities

## Development
- Microsoft Visual Studio 2015
- Windows 8.1 or 10

## Deployment 
- Microsoft .NET Framework 4.5.2
- IIS 8


# More repositories

If you want to have add more **public** repositories to NuGet Visualizer, please send a pull request to edit `nugetviz.json` file.  Once accepted, the repository will be available on NuGet Visualizer 
