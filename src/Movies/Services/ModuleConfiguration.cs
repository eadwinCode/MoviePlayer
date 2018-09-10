using Microsoft.Practices.Prism.Modularity;
using Movies.MovieServices.ModuleDefinition;
using Movies.PlaylistCollectionView.Services;
using Movies.StatusService.ModuleDefinition;
using SearchComponent.ModuleDefinition;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using VideoPlayerView.ModuleDefinition;
using VirtualizingListView.ModuleDefinition;

namespace Movies.Services
{
    public class MoviesModule
    {
        public string ModuleName { get; set; }
        public Collection<string> DependsOn { get; set; }
        public MoviesModule()
        {
            DependsOn = new Collection<string>();
        }
    }
    public abstract class ModuleConfiguration
    {
        //protected const string VideoPlayerModule = "VideoPlayerView";
        //protected const string StatusServiceModule = "Movies.StatusService";
        //protected const string SearchComponentModule = "SearchComponent";
        protected const string ApplicationServicesModule = "MoviesServiceModuleLoader"; 
        protected const string FileExporerModule = "FileViewModule"; 
        //protected const string LocalVideoFilesModule = "LocalVideoFiles"; 
        //protected const string RemovableStorageFilesModule = "RemovableStorageFiles";
        //protected const string HomePlaylistModule = "RemovableStorageFiles";


        protected const string VideoPlayerProject = "VideoPlayerView";
        protected const string StatusServiceProject = "Movies.StatusService";
        protected const string SearchComponentProject = "SearchComponent";
        protected const string ApplicationServicesProject = "Movies.MovieServices";
        protected const string FileExporerProject = "VirtualizingListView";
        protected const string LocalVideoFilesProject = "LocalVideoFiles";
        protected const string RemovableStorageFilesProject = "RemovableStorageFiles";
        protected const string HomePlaylistProject = "Movies.PlaylistCollectionView";

        public abstract IList<ModuleInfo> GetModuleInfoList();

        public virtual IList<MoviesModule> GetModules()
        {
            return new List<MoviesModule>()
            {
                new MoviesModule{ ModuleName=ApplicationServicesProject},
                new MoviesModule{ ModuleName = VideoPlayerProject},
                new MoviesModule{ ModuleName = StatusServiceProject,DependsOn={ApplicationServicesModule}},
                new MoviesModule{ ModuleName = SearchComponentProject , DependsOn = { FileExporerModule} },
                new MoviesModule{ModuleName = FileExporerProject,DependsOn={ApplicationServicesModule}},
                new MoviesModule{ ModuleName = RemovableStorageFilesProject , DependsOn = { FileExporerModule } },
                new MoviesModule{ModuleName = LocalVideoFilesProject,DependsOn={  FileExporerModule}},
                new MoviesModule{ModuleName = HomePlaylistProject,DependsOn={  FileExporerModule}}
            };
        }
    }


    public class ProjectConfiguration : ModuleConfiguration
    {
        public override IList<ModuleInfo> GetModuleInfoList()
        {
            IList<ModuleInfo> moduleInfos = new List<ModuleInfo>();
            var moduleInterface = typeof(IModule);
            var projects = GetModules();
            foreach (var item in projects)
            {
                string modulePath = AppDomain.CurrentDomain.BaseDirectory + "Modules\\" 
                    + item.ModuleName + ".dll";
                Assembly moduleassembly = Assembly.LoadFile(modulePath);

                foreach (Type exportType in moduleassembly.GetExportedTypes())
                {
                    if (moduleInterface.IsAssignableFrom(exportType))
                    {
                        var moduleInfo = new ModuleInfo
                        {
                            InitializationMode = InitializationMode.WhenAvailable,
                            ModuleName = exportType.Name,
                            ModuleType = exportType.AssemblyQualifiedName,
                            DependsOn = item.DependsOn,
                            Ref = new Uri(modulePath, UriKind.RelativeOrAbsolute).AbsoluteUri
                        };
                        moduleInfos.Add(moduleInfo);
                    }
                }
            }
            return moduleInfos;
        }

        public IList<ModuleInfo> GetModuleInfo()
        {
            //Generating modules with respect to their dependencies.
            IList<ModuleInfo> moduleInfos = new List<ModuleInfo>();


            Type typeA = typeof(MoviesServiceModuleLoader);
            moduleInfos.Add(new ModuleInfo() { ModuleName = typeA.Name, ModuleType = typeA.AssemblyQualifiedName });


            typeA = typeof(VideoPlayerModule);
            moduleInfos.Add(new ModuleInfo() { ModuleName = typeA.Name, ModuleType = typeA.AssemblyQualifiedName });


            typeA = typeof(StatusServiceModule);
            moduleInfos.Add(new ModuleInfo() { ModuleName = typeA.Name, ModuleType = typeA.AssemblyQualifiedName });

            typeA = typeof(FileViewModule);
            moduleInfos.Add(new ModuleInfo() { ModuleName = typeA.Name, ModuleType = typeA.AssemblyQualifiedName });

            typeA = typeof(SearchControlModule);
            moduleInfos.Add(new ModuleInfo() { ModuleName = typeA.Name, ModuleType = typeA.AssemblyQualifiedName });

            typeA = typeof(LocalVideoFileModule);
            moduleInfos.Add(new ModuleInfo() { ModuleName = typeA.Name, ModuleType = typeA.AssemblyQualifiedName });

           typeA = typeof(RemovableStorageModule);
            moduleInfos.Add(new ModuleInfo() { ModuleName = typeA.Name, ModuleType = typeA.AssemblyQualifiedName });


            typeA = typeof(HomePlaylistModule);
            moduleInfos.Add(new ModuleInfo() { ModuleName = typeA.Name, ModuleType = typeA.AssemblyQualifiedName });

            typeA = typeof(InternetRadio.ModuleDefinition.InternetRadioModule);
            moduleInfos.Add(new ModuleInfo() { ModuleName = typeA.Name, ModuleType = typeA.AssemblyQualifiedName });

            return moduleInfos;
        }
    }
}
