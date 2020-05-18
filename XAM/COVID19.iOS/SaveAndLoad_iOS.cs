using System;
using Xamarin.Forms;
using System.IO;
using System.Threading.Tasks;

using System.Linq;

using Foundation;

using System.Collections.Generic;
using Perimeter.iOS;
using Perimeter.Interfaces;

[assembly: Dependency (typeof(SaveAndLoad_iOS))]

namespace Perimeter.iOS
{
	public class SaveAndLoad_iOS : ISaveAndLoad
	{
		public static string DocumentsPath {
			get {
                //var documentsDirUrl = NSFileManager.DefaultManager.GetUrls (NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User).Last ();
                var documentsDirUrl = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                return documentsDirUrl;
			}
		}

		#region ISaveAndLoad implementation

		public async Task SaveTextAsync (string filename, string text)
		{
            try
            {
                string path = CreatePathToFile(filename);
                System.IO.File.WriteAllText(path,text);
                //using (StreamWriter sw = File.CreateText(path))
                //{
                //    await sw.WriteAsync(text);
                //}

            }
            catch (Exception ex)
            {
                
            }
			    
		}

		public async Task<string> LoadTextAsync (string filename)
		{
            string result = "";
            try
            {

                string path = CreatePathToFile(filename);

                using (StreamReader sr = File.OpenText(path))
                {
                    result = await sr.ReadToEndAsync();
                }

            }
            catch (Exception ex)
            {

                
            }
            return result;

		}

        public async Task SaveTextAsync(string filename, string text,string username)
        {
            try
            {
                string path = CreatePathToFile(filename,username);
                System.IO.File.WriteAllText(path, text);
                //using (StreamWriter sw = File.CreateText(path))
                //{
                //    await sw.WriteAsync(text);
                //}

            }
            catch (Exception ex)
            {

            }

        }

        public async Task<string> LoadTextAsync(string filename,string username)
        {
            string result = "";
            try
            {

                string path = CreatePathToFile(filename,username);

                using (StreamReader sr = File.OpenText(path))
                {
                    result = await sr.ReadToEndAsync();
                }

            }
            catch (Exception ex)
            {


            }
            return result;

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


        public bool FileExists (string filename)
		{
			return File.Exists (CreatePathToFile (filename));
		}

		#endregion

		static string CreatePathToFile(string fileName)
		{
			return Path.Combine (DocumentsPath, fileName);
		}
        static string CreatePathToFile(string fileName,string userName)
        {
            return Path.Combine(DocumentsPath, fileName, userName);
        }

        public IEnumerable<string> GetAllFiles()
        {
            IEnumerable<string> result = null;
                        
            result = Directory.EnumerateFiles(Path.Combine(DocumentsPath, "*.xml"));

            return result;
        }


        public IEnumerable<string> GetProfileFiles(string username)
        {
            IEnumerable<string> result = null;

            var docsPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "Profiles", username);

            result = Directory.EnumerateFiles(Path.Combine(docsPath, "*.xml"));

            return result;
        }

        public IEnumerable<string> GetAllFiles(string filter)
        {
            IEnumerable<string> result = null;

            result = Directory.EnumerateFiles(DocumentsPath, filter);

            return result;
        }
    }
}