using System.Collections.Generic;
using System.Threading.Tasks;

namespace Perimeter.Interfaces
{
    /// <summary>
    /// Define an API for loading and saving a text file. Reference this interface
    /// in the common code, and implement this interface in the app projects for
    /// iOS, Android and WinPhone. Remember to use the 
    ///     [assembly: Dependency (typeof (SaveAndLoad_IMPLEMENTATION_CLASSNAME))]
    /// attribute on each of the implementations.
    /// </summary>
    public interface ISaveAndLoad
	{
        Task SaveTextAsync(string filename, string text,string username);
        Task<string> LoadTextAsync(string filename,string username);
        Task SaveTextAsync(string filename, string text);
        Task<string> LoadTextAsync(string filename);
        bool SaveBinaryAsync(string filename, byte[] data);
        byte[] LoadBinaryAsync(string filename);
        bool FileExists (string filename);
        IEnumerable<string> GetAllFiles();

        IEnumerable<string> GetAllFiles(string filter);

        bool DeleteFile(string filename);

    }
}

