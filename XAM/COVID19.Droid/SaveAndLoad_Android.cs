using Perimeter.Droid;
using Perimeter.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(SaveAndLoad_Android))]

namespace Perimeter.Droid
{
    public class SaveAndLoad_Android : ISaveAndLoad
    {
        private readonly string filePath;
        public SaveAndLoad_Android()
        {
            filePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "TrendeerFiles");

            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);
        }
        public string FilePath
        {
            get
            {
                return filePath;
            }
        }

        #region ISaveAndLoad implementation

        public async Task SaveTextAsync(string filename, string text)
        {
            try
            {

                var path = CreatePathToFile(filename);
                using (StreamWriter sw = File.CreateText(path))
                {
                    await sw.WriteAsync(text);
                }
            }
            catch (Exception ex)
            {


            }

        }

        public async Task<string> LoadTextAsync(string filename)
        {
            try
            {
                var path = CreatePathToFile(filename);
                string result = "";

                if (File.Exists(path))
                {
                    using (StreamReader sr = File.OpenText(path))
                    {
                        result = await sr.ReadToEndAsync();
                    }

                    return result;
                }

            }
            catch (Exception ex)
            {

            }

            return String.Empty;

        }


        public async Task SaveTextAsync(string filename, string text, string username)
        {
            try
            {

                var path = CreatePathToFile(filename, username);
                using (StreamWriter sw = File.CreateText(path))
                {
                    await sw.WriteAsync(text);
                }
            }
            catch (Exception ex)
            {


            }

        }

        public async Task<string> LoadTextAsync(string filename, string username)
        {
            try
            {
                var path = CreatePathToFile(filename, username);
                string result = "";

                if (File.Exists(path))
                {
                    using (StreamReader sr = File.OpenText(path))
                    {
                        result = await sr.ReadToEndAsync();
                    }

                    return result;
                }

            }
            catch (Exception ex)
            {

            }

            return String.Empty;

        }

        public bool SaveBinaryAsync(string filename, byte[] data)
        {
            var path = CreatePathToFile(filename);

            try
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.CreateNew)))
                {
                    writer.Write(data);
                }

                return true;
            }
            catch (Exception ex)
            {

            }

            return false;

        }

        public byte[] LoadBinaryAsync(string filename)
        {
            byte[] result = null;

            try
            {
                var path = CreatePathToFile(filename);

                if (File.Exists(path))
                {
                    result = File.ReadAllBytes(path);

                }

            }
            catch (Exception ex)
            {

            }

            return result;

        }

        public bool FileExists(string filename)
        {
            return File.Exists(CreatePathToFile(filename));
        }

        public bool DeleteFile(string filename)
        {
            bool result = false;

            //if (File.Exists(CreatePathToFile(filename)))
            {
                try
                {
                    if (File.Exists(filename))
                    {
                        File.Delete(filename);
                    }
                    else
                    {
                        IEnumerable<string> files = GetAllFiles(filename);

                        foreach (string file in files)
                        {
                            File.Delete(file);
                        }

                    }


                    result = true;
                }
                catch (Exception ex)
                {
                    result = false;
                }

            }

            return result;
        }

        #endregion

        string CreatePathToFile(string filename)
        {
            var docsPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "TrendeerFiles");

            if (!Directory.Exists(docsPath))
                Directory.CreateDirectory(docsPath);

            return Path.Combine(docsPath, filename);
        }

        string CreatePathToFile(string filename, string username)
        {
            var docsPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "Profiles", username);

            if (!Directory.Exists(docsPath))
                Directory.CreateDirectory(docsPath);

            return Path.Combine(docsPath, filename);
        }

        public IEnumerable<string> GetAllFiles()
        {
            IEnumerable<string> result = null;

            var docsPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "TrendeerFiles");

            result = Directory.EnumerateFiles(docsPath, "*.xml");

            return result;
        }

        public IEnumerable<string> GetProfileFiles(string username)
        {
            IEnumerable<string> result = null;

            var docsPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "Profiles", username);

            result = Directory.EnumerateFiles(docsPath, "*.xml");

            return result;
        }

        public IEnumerable<string> GetAllFiles(string filter)
        {
            IEnumerable<string> result = null;

            var docsPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "TrendeerFiles");

            result = Directory.EnumerateFiles(docsPath, filter);

            return result;
        }

    }
}