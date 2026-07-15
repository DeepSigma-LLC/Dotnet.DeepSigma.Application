
namespace DeepSigma.Application;

/// <summary>
/// Provides utility methods for the application.
/// </summary>
public class AppUtilities
{
    /// <summary>
    /// Gets the current version of the app from the assembly information.
    /// </summary>
    /// <returns>The current version of the app, or null if it cannot be determined.</returns>
    public static Version? GetAppVersionFromAssembly()
    {
        return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;        
    }

    /// <summary>
    /// Gets the current version of the app as a string in the format "vMajor.Minor.Build.Revision".
    /// </summary>
    /// <returns></returns>
    public static string? GetAppVersionString()
    {
        var version = GetAppVersionFromAssembly();
        return version == null ? null : $"v{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
    }

    /// <summary>
    /// Gets the username of the user currently logged into the operating system.
    /// </summary>
    /// <returns>The username of the current user.</returns>
    public static string UserName()
    {
        return Environment.UserName;
    }

    /// <summary>
    /// Gets the domain name of the user currently logged into the operating system.
    /// </summary>
    /// <returns>The domain name of the current user.</returns>
    public static string DomainName()
    {
        return Environment.UserDomainName;
    }

    /// <summary>
    /// Gets the machine name of the computer on which the application is running.
    /// </summary>
    /// <returns>The name of the machine.</returns>
    public static string MachineName()
    {
        return Environment.MachineName;
    }
}
