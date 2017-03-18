using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ParseMediabaseData.AppCode.Utils
{
    public class FileHelper
    {
        public static void ArchiveFiles(string sourceDir, string targetDir, string filePattern = "*.*", bool createSubFolder = false, string subFolder = "" )
        {
            string finalTargetDir = targetDir;

            if (!Directory.Exists(sourceDir))
            {
                throw new DirectoryNotFoundException(string.Format("Source directory \"{0}\" doesn't exist", sourceDir));
            }

            if (createSubFolder)
            {
                if(String.IsNullOrEmpty(subFolder))
                {
                    subFolder = DateTime.Today.ToString("yyyy-MM-dd");
                    finalTargetDir = Path.Combine(targetDir, subFolder);
                }
                else
                {
                    finalTargetDir = Path.Combine(targetDir, subFolder);
                }
            }

            if(!Directory.Exists(finalTargetDir))
            {
                Directory.CreateDirectory(finalTargetDir);
            }

            //Copy Files and delete the old one

            var sourceFiles = Directory.EnumerateFiles(sourceDir, filePattern);

            foreach(string filename in sourceFiles)
            {
                string targetFile = Path.Combine(finalTargetDir, Path.GetFileName(filename));
                //Move the file
                try
                {
                    File.Move(filename, targetFile);
                }
                catch(Exception)
                {
                    try
                    {
                        File.Copy(filename, targetFile, true);
                        File.Delete(filename);

                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
        }

        public static void CleanUpFiles(string directory, string filePattern = "*.*")
        {
            if (!Directory.Exists(directory))
            {
                throw new DirectoryNotFoundException(string.Format("Directory \"{0}\" doesn't exist", directory));
            }

            //Copy Files and delete the old one

            var files = Directory.EnumerateFiles(directory, filePattern);

            foreach (string filename in files)
            {
               
                //Move the file
                try
                {
                    File.Delete(filename);
                }
                catch (Exception)
                {
                    //Ignore
                }
            }
        }
    }
}
