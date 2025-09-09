using System;
using NuGet.Versioning;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Serilog;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.PathConstruction;

class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Test);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution Solution;

    [GitRepository] readonly GitRepository GitRepository;

    Project KeepAChangelogProject => Solution.GetProject("KeepAChangelog.IO");

    readonly AbsolutePath PublishDirectory = RootDirectory / "publish";

    SemanticVersion Version = new(0, 0, 1, "prerelease");

    Target Clean => _ => _
        .Executes(() =>
        {
            DotNetTasks.DotNetClean(s => s
                .SetProject(Solution)
                .SetConfiguration(Configuration)
            );

            PublishDirectory.CreateOrCleanDirectory();
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetTasks.DotNetRestore(s => s
                .SetProjectFile(Solution)
            );
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetTasks.DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore()
            );
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTasks.DotNetTest(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoBuild()
            );
        });

    Target Pack => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            if (GitRepository.CurrentCommitHasVersionTag())
                Version = GitRepository.GetLatestVersionTagOnCurrentCommit();

            Log.Information("Version {Version}", Version);

            string fileVersion = $"{Version.Major}.{Version.Minor}.{Version.Patch}.0";
            Log.Information("File Version {FileVersion}", fileVersion);

            string assemblyVersion = $"{Version.Major}.0.0.0";
            Log.Information("Assembly Version {AssemblyVersion}", assemblyVersion);

            string informationalVersion = Version.ToFullString();
            Log.Information("Informational Version {InformationalVersion}", informationalVersion);

            DotNetTasks.DotNetPack(s => s
                .SetProject(KeepAChangelogProject)
                .SetConfiguration(Configuration)
                .SetOutputDirectory(PublishDirectory)
                .SetVersion(Version.ToString())
                .SetFileVersion(fileVersion)
                .SetAssemblyVersion(assemblyVersion)
                .SetInformationalVersion(informationalVersion)
                .EnableDeterministic()
                .EnableContinuousIntegrationBuild()
                .SetCopyright($"Copyright {DateTime.UtcNow.Year} (c) Sandro Figo")
                .EnableNoRestore()
            );
        });
}