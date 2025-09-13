using System;
using System.IO;
using System.Text;
using KeepAChangelog.IO;
using Microsoft.AspNetCore.StaticFiles;
using NuGet.Versioning;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitHub;
using Octokit;
using Serilog;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.PathConstruction;
using Project = Nuke.Common.ProjectModel.Project;
using Release = Octokit.Release;

class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Test);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution Solution;

    [GitRepository] readonly GitRepository GitRepository;

    [Parameter("NuGet API Key"), Secret] readonly string NuGetApiKey;

    Project KeepAChangelogProject => Solution.GetProject("KeepAChangelog.IO");

    readonly AbsolutePath PublishDirectory = RootDirectory / "publish";
    readonly AbsolutePath ChangelogFile = RootDirectory / "CHANGELOG.md";

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

    Target ValidateChangelog => _ => _
        .Executes(() =>
        {
            var changelog = Changelog.FromFile(ChangelogFile);

            Assert.True(ChangelogFile.ReadAllText() == changelog.ToString());
            
            Log.Information("CHANGELOG.md is valid");
        });
    
    Target Test => _ => _
        .DependsOn(Compile, ValidateChangelog)
        .Executes(() =>
        {
            DotNetTasks.DotNetTest(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoBuild()
            );
        });

    Target Pack => _ => _
        .DependsOn(Compile, Test)
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

    Target PublishPackageToNuGet => _ => _
        .DependsOn(Pack)
        .Executes(() =>
        {
            foreach (AbsolutePath file in PublishDirectory.GlobFiles("*.nupkg"))
            {
                DotNetTasks.DotNetNuGetPush(s => s
                    .SetTargetPath(file)
                    .SetSource("https://api.nuget.org/v3/index.json")
                    .SetApiKey(NuGetApiKey)
                    .EnableSkipDuplicate()
                );
            }
        });

    Target PublishPackageToGithub => _ => _
        .DependsOn(Pack)
        .After(PublishPackageToNuGet)
        .Executes(() =>
        {
            foreach (AbsolutePath file in PublishDirectory.GlobFiles("*.nupkg"))
            {
                DotNetTasks.DotNetNuGetPush(s => s
                    .SetTargetPath(file)
                    .SetSource($"https://nuget.pkg.github.com/{GitHubActions.Instance.RepositoryOwner}/index.json")
                    .SetApiKey(GitHubActions.Instance.Token)
                    .EnableSkipDuplicate()
                );
            }
        });

    Target FormatChangelog => _ => _
        .Executes(() =>
        {
            Changelog.FromFile(ChangelogFile).ToFile(ChangelogFile); // TODO-SFIGO: make it easier to format a changelog file
            Log.Information("Successfully formatted CHANGELOG.md");
        });

    Target PublishGitHubRelease => _ => _
        .DependsOn(Pack, FormatChangelog)
        .After(PublishPackageToNuGet)
        .Executes(async () =>
        {
            Changelog changelog = Changelog.FromFile(ChangelogFile);
            var releaseBody = new StringBuilder();

            // TODO-SFIGO: add a way to get the unreleased release and the first released release
            releaseBody.AppendJoin(Environment.NewLine + Environment.NewLine, changelog.Releases[0].Categories); // TODO-SFIGO: make it easier to get sorted categories as a single string; CategoriesCollection class?

            GitHubTasks.GitHubClient = new GitHubClient(new ProductHeaderValue("KeepAChangelog.IO"))
            {
                Credentials = new Credentials(GitHubActions.Instance.Token)
            };

            string owner = GitRepository.GetGitHubOwner();
            string name = GitRepository.GetGitHubName();

            SemanticVersion version = GitRepository.GetLatestVersionTagOnCurrentCommit();

            var newRelease = new NewRelease($"v{version}")
            {
                Draft = true,
                Name = $"v{version}",
                Prerelease = version.IsPrerelease,
                Body = releaseBody.ToString()
            };

            Release createdRelease = await GitHubTasks.GitHubClient.Repository.Release.Create(owner, name, newRelease);

            foreach (AbsolutePath file in PublishDirectory.GlobFiles("*.nupkg"))
            {
                await using FileStream fileStream = File.OpenRead(file);

                if (!new FileExtensionContentTypeProvider().TryGetContentType(file, out string contentType))
                {
                    contentType = "application/octet-stream";
                }

                var assetUpload = new ReleaseAssetUpload
                {
                    FileName = file.Name,
                    ContentType = contentType,
                    RawData = fileStream
                };

                await GitHubTasks.GitHubClient.Repository.Release.UploadAsset(createdRelease, assetUpload);
            }

            await GitHubTasks.GitHubClient.Repository.Release.Edit(owner, name, createdRelease.Id, new ReleaseUpdate { Draft = false });
        });

    Target Publish => _ => _
        .Requires(() => GitRepository.CurrentCommitHasVersionTag())
        .DependsOn(PublishPackageToNuGet, PublishPackageToGithub, PublishGitHubRelease);
}