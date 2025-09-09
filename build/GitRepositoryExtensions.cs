using System.Linq;
using NuGet.Versioning;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.Utilities.Collections;

public static class GitRepositoryExtensions
{
    public static bool CurrentCommitHasVersionTag(this GitRepository gitRepository)
    {
        var versionTagsOnCurrentCommit = gitRepository.GetSemanticVersionsOnCurrentCommit();

        return versionTagsOnCurrentCommit.Length > 0;
    }

    public static SemanticVersion GetLatestVersionTagOnCurrentCommit(this GitRepository gitRepository)
    {
        var versionTagsOnCurrentCommit = gitRepository.GetSemanticVersionsOnCurrentCommit();

        Assert.NotEmpty(versionTagsOnCurrentCommit, $"The current commit '{gitRepository.Commit}' has no valid tag!");

        return versionTagsOnCurrentCommit.First();
    }

    public static bool TryGetLatestVersionTagOnCurrentCommit(this GitRepository gitRepository, out SemanticVersion version)
    {
        var versionTagsOnCurrentCommit = gitRepository.GetSemanticVersionsOnCurrentCommit();

        if (versionTagsOnCurrentCommit.Length == 0)
        {
            version = null;
            return false;
        }

        version = versionTagsOnCurrentCommit.First();
        return true;
    }

    public static SemanticVersion[] GetSemanticVersionsOnCurrentCommit(this GitRepository gitRepository)
    {
        return gitRepository.Tags.Select(t => SemanticVersion.TryParse(t.TrimStart('v'), out SemanticVersion v) ? v : null).WhereNotNull().OrderByDescending(t => t).ToArray();
    }
}