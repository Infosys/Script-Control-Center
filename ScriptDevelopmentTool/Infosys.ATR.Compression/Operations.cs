/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.Packaging;
using System.IO;

namespace Infosys.ATR.Packaging
{
    public class Operations
    {
        private const long BUFFER_SIZE = 4096;
        private static string startingFolder = "";
        static Package zip1;      

        /// <summary>
        /// Interface to create iapd package. Given the qualified file name for the python(.py) file, this compresses and packages all files and folders contained at the same location as the .py file.
        /// the final package's name is same as the .py file name with ext .iapd
        /// </summary>
        /// <param name="pyFullyqualifiedFile">the fully qualified file name for the python(.py) file.</param>
        /// <param name="packageFileExtension">the fully qualified file exetention package file.</param>
        /// <returns>Result depicting if the execution suceeded or failed with error message (if any)</returns>
        public static Result Package(string pyFullyqualifiedFile, string packageFileExtension = "iapd")    
        {
            Result result = null;
            //get the folder conatining the py file
            if (File.Exists(pyFullyqualifiedFile))
            {
                string folder = Path.GetDirectoryName(pyFullyqualifiedFile);
                startingFolder = folder;
                string packageName = Path.GetFileNameWithoutExtension(pyFullyqualifiedFile) + "." + packageFileExtension;
                result = AddFolder(Path.Combine(folder, packageName), folder);
                result.PackagePath = Path.Combine(folder, packageName);
            }
            return result;
        }

        /// <summary>
        /// Interface to a package using just the provided single file
        /// </summary>
        /// <param name="fullyqualifiedFile">the file to be added to the package to be created</param>
        /// <param name="packageExt">the extension or type of the package without ".", e.g.- iapd, iapw, etc</param>
        /// <returns></returns>
        public static Result PackageJustFile(string fullyqualifiedFile, string packageExt)
        {
            Result result = new Result();
            try
            {
                //get the folder conatining the py file
                if (File.Exists(fullyqualifiedFile))
                {
                    string folder = Path.GetDirectoryName(fullyqualifiedFile);
                    string packageName = Path.Combine(folder, Path.GetFileNameWithoutExtension(fullyqualifiedFile) + "." + packageExt);
                    using (Package zip = System.IO.Packaging.Package.Open(packageName, FileMode.OpenOrCreate))
                    {
                        string destFilename = ".\\" + Path.GetFileName(fullyqualifiedFile);
                        Uri uri = PackUriHelper.CreatePartUri(new Uri(destFilename, UriKind.Relative));
                        if (zip.PartExists(uri))
                        {
                            zip.DeletePart(uri);
                        }
                        PackagePart part = zip.CreatePart(uri, "", CompressionOption.Normal);
                        using (FileStream fileStream = new FileStream(fullyqualifiedFile, FileMode.Open, FileAccess.Read))
                        {
                            using (Stream dest = part.GetStream())
                            {
                                CopyStream(fileStream, dest);
                            }
                        }
                    }
                    result.PackagePath = Path.Combine(folder, packageName);
                    result.IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                if (ex.InnerException != null)
                {
                    result.Message += ". Inner Exception Message- " + ex.InnerException.Message;
                }
            }
            return result;
        }

        /// <summary>
        /// Interface to create the iapw package.
        /// </summary>
        /// <param name="xamlName">the name of the xaml to be created inside the iapw package</param>
        /// <param name="xamlStream">the stream of the xaml file</param>
        /// <param name="dependencyFolderPath">the optional folder having the dependent files and folders</param>
        /// <returns>Result depicting if the execution suceeded or failed with error message (if any)</returns>
        public static Result Package(string xamlName, Stream xamlStream, string dependencyFolderPath="")
        {
            Result result = null;
            if (!string.IsNullOrEmpty(xamlName) && xamlStream.Length > 0)
            {
                //get the xaml string from the stream
                string xamlString = StreamToString(xamlStream);
                xamlStream.Close();

                //if not dependency folder is provided then dependency folder is same as the local execution path
                if (string.IsNullOrEmpty(dependencyFolderPath))
                {
                    dependencyFolderPath = GetAppPath();
                    //then create a empty folder by the name as the iapw package
                    string newFolder= Path.Combine(dependencyFolderPath, "iapw");
                    if(System.IO.Directory.Exists(newFolder))
                        System.IO.Directory.Delete(newFolder);
                    System.IO.Directory.CreateDirectory(newFolder);                    
                    dependencyFolderPath = newFolder;
                }
                //then write the stream to this folder
                string destination = Path.Combine(dependencyFolderPath, xamlName + ".xaml");
                if (File.Exists(destination))
                    File.Delete(destination);
                //using (FileStream destFile = new FileStream(destination, FileMode.OpenOrCreate))
                //{
                //    CopyStream(xamlStream, destFile);
                //}
                //write the xaml string
                File.WriteAllText(destination, xamlString);

                startingFolder = dependencyFolderPath;
                string packageName = xamlName + ".iapw";
                result = AddFolder(Path.Combine(dependencyFolderPath, packageName), dependencyFolderPath);
                result.PackagePath = Path.Combine(dependencyFolderPath, packageName);
            }
            else
            {
                result = new Result();
                result.IsSuccess = false;
                result.Message = "Invalid XAML name or/and stream";
            }
            return result;
        }

        /// <summary>
        /// Method to return the file content as stream from the IAPD/IAPW package. Make sure to call ClosePackage once stream is read.
        /// </summary>
        /// <param name="iapdOrIapwStream">the iapd package stream</param>
        /// <param name="filenameWithrelativepath">The file to be looked in to the iapd package. Please make sure it stafrts with a back slash.</param>
        /// <returns>the asked file content as stream</returns>
        public static Stream ExtractFile(Stream iapdOrIapwStream, string filenameWithrelativepath)
        {
            Stream file = null;
            iapdOrIapwStream.Position = 0;
            zip1 = System.IO.Packaging.Package.Open(iapdOrIapwStream);
            foreach (PackagePart part in zip1.GetParts())
            {
                string pathPart = part.Uri.ToString().Replace(@"/", "\\");
                if (pathPart.ToLower() == filenameWithrelativepath.ToLower())
                {
                    file = part.GetStream();
                    break;
                }
            }            
            return file;
        }

        /// <summary>
        /// Method to return the file content as stream from the IAPD/IAPW package. Make sure to call ClosePackage once stream is read.
        /// </summary>
        /// <param name="iapdFileWithpath">iapd file path</param>
        /// <param name="filenameWithrelativepath">the file to be looked in to the iapd package</param>
        /// <returns>the asked file content as stream</returns>
        public static Stream ExtractFile(string iapdFileWithpath, string filenameWithrelativepath)
        {
            Stream file = ExtractFile(File.OpenRead(iapdFileWithpath), filenameWithrelativepath);
            return file;
        }

        /// <summary>
        /// TO close the underlying package stream. This is to be used only with the ExtractFile interface
        /// </summary>
        public static void ClosePackage()
        {
            if (zip1 != null)
                zip1.Close();
        }

        /// <summary>
        /// Extracts the files and folders in the provided iapd package
        /// </summary>
        /// <param name="package">The package name with location</param>
        /// <param name="extractToFolder">The destination for extraction</param>
        /// <returns>Result depicting if the execution suceeded or failed with error message (if any)</returns>
        public static Result Unpackage(string package, string extractToFolder)
        {
            bool overWriteExistingFile = true; //if true and same file found, then it will be over written
            Result result = new Result();
            result.IsSuccess = true;
            try
            {
                using (Package zip = System.IO.Packaging.Package.Open(package, FileMode.Open))
                {
                    //create the final extraction folder same as the iapd package name
                    extractToFolder = Path.Combine(extractToFolder, Path.GetFileNameWithoutExtension(package));
                    foreach (PackagePart part in zip.GetParts())
                    {
                        //Console.WriteLine("Extraction of " + part.Uri.ToString());
                        string destination = extractToFolder + Uri.UnescapeDataString(part.Uri.ToString().Replace(@"/", "\\"));
                        string directoryName = Path.GetDirectoryName(destination);
                        if (!Directory.Exists(directoryName))
                        {
                            var dir = Directory.CreateDirectory(directoryName);
                            //if(dir.Exists)
                            //    Console.WriteLine("created " + directoryName);
                        }

                        if (File.Exists(destination) && !overWriteExistingFile)
                            continue;
                        string fileName = Path.GetFileName(destination);
                        using (FileStream destFile = new FileStream(destination, FileMode.OpenOrCreate))
                        {
                            CopyStream(part.GetStream(), destFile);
                            //Console.WriteLine("Copied " + destination);
                        }
                    }

                    result.PackagePath = extractToFolder;
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                if (ex.InnerException != null)
                {
                    result.Message += ". Inner Exception Message- " + ex.InnerException.Message;
                }
            }
            return result;
        }

        /// <summary>
        /// Adds the file to the given target compressed file. If target compressed file exists, then it adds to it otherwise it creates it.
        /// </summary>
        /// <param name="targetCompressedFileName">The target compressed file</param>
        /// <param name="fileToBeAdded">The file to be added</param>
        /// <param name="relativeTargetFolder">The relative path of the file in the target compressed file to be maintained</param>
        /// <returns>Result depicting if the execution suceeded or failed with error message (if any)</returns>
        private static Result AddFile(string targetCompressedFileName, string fileToBeAdded, string relativeTargetFolder)
        {
            Result result = new Result();
            result.IsSuccess = true;
            try
            {
                using (Package zip = System.IO.Packaging.Package.Open(targetCompressedFileName, FileMode.OpenOrCreate))
                {
                    string destFilename = "";
                    if (!string.IsNullOrEmpty(relativeTargetFolder))
                        destFilename = "." + relativeTargetFolder + "\\" + Path.GetFileName(fileToBeAdded);
                    else
                        destFilename = ".\\" + Path.GetFileName(fileToBeAdded);
                    Uri uri = PackUriHelper.CreatePartUri(new Uri(destFilename, UriKind.Relative));
                    if (zip.PartExists(uri))
                    {
                        zip.DeletePart(uri);
                    }
                    PackagePart part = zip.CreatePart(uri, "", CompressionOption.Normal);

                    using (FileStream fileStream = new FileStream(fileToBeAdded, FileMode.Open, FileAccess.Read))
                    {
                        using (Stream dest = part.GetStream())
                        {
                            CopyStream(fileStream, dest);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                if (ex.InnerException != null)
                {
                    result.Message += ". Inner Exception Message- " + ex.InnerException.Message;
                }
            }
            return result;
        }

        /// <summary>
        /// Adds the folder, its sub-folders and files to the given target compressed file. If target compressed file exists, then it adds to it otherwise it creates it.
        /// </summary>
        /// <param name="targetCompressedFileName">The target compressed file</param>
        /// <param name="folderToBeAdded">The folder to be added</param>
        /// <returns>Result depicting if the execution suceeded or failed with error message (if any)</returns>
        private static Result AddFolder(string targetCompressedFileName, string folderToBeAdded)
        {
            Result result = new Result();
            result.IsSuccess = true;
            try
            {
                //string relativeTargetFolder = folderToBeAdded.Replace(Path.GetPathRoot(folderToBeAdded), "");
                string relativeTargetFolder = folderToBeAdded.Replace(startingFolder, "");
                if (Directory.Exists(folderToBeAdded))
                {
                    foreach (string file in Directory.GetFiles(folderToBeAdded))
                    {
                        if (!Path.GetExtension(file).Equals(".iapd") || !Path.GetExtension(file).Equals(".iapw")) //i.e. exclude any iapd/ iapw package
                            result = AddFile(targetCompressedFileName, file, relativeTargetFolder);
                    }
                    foreach (string folder in Directory.GetDirectories(folderToBeAdded))
                    {
                        result = AddFolder(targetCompressedFileName, folder);
                    }
                }
            }
            catch (Exception ex)
            {

                result.IsSuccess = false;
                result.Message = ex.Message;
                if (ex.InnerException != null)
                {
                    result.Message += ". Inner Exception Message- " + ex.InnerException.Message;
                }
            }
            return result;
        }

        /// <summary>
        /// Copies the content from one stream (source) to another (target)
        /// </summary>
        /// <param name="inputStream">Source stream</param>
        /// <param name="outputStream">Target stream</param>
        private static void CopyStream(System.IO.Stream inputStream, System.IO.Stream outputStream)
        {
            inputStream.Position = 0;
            long bufferSize = inputStream.Length < BUFFER_SIZE ? inputStream.Length : BUFFER_SIZE;
            byte[] buffer = new byte[bufferSize];
            int bytesRead = 0; long bytesWritten = 0;
            while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                outputStream.Write(buffer, 0, bytesRead);
                bytesWritten += bufferSize;
            }
        }

        /// <summary>
        /// Method to get the current executing path
        /// </summary>
        /// <returns>current executing path</returns>
        private static string GetAppPath()
        {
            string path;
            path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);

            if (path.Contains(@"file:\\"))
            {
                path = path.Replace(@"file:\\", "");
            }

            else if (path.Contains(@"file:\"))
            {
                path = path.Replace(@"file:\", "");
            }

            return path;
        }

        private static string StreamToString(Stream fileContent)
        {
            fileContent.Position = 0;
            StreamReader reader = new StreamReader(fileContent);
            string fileString = reader.ReadToEnd();
            return fileString;
        }

        /// <summary>
        /// Method to return the list of file content as stream from the IAPD/IAPW package. Make sure to call ClosePackage once stream is read.
        /// </summary>
        /// <param name="iapdOrIapwStream">the iapd package stream</param>
        /// <param name="extension"> it will be extension of file e.g. ".dll",".py",etc The file with specified extension to be looked in to the iapd package. Please make sure it start with a dot "." .</param>
        /// <returns>the asked list file content as stream</returns>
        public static List<Stream> ExtractFiles(Stream iapdOrIapwStream, string extension)
        {
            List<Stream> resourceFiles = new List<Stream>(); 
            iapdOrIapwStream.Position = 0;
            zip1 = System.IO.Packaging.Package.Open(iapdOrIapwStream);
            foreach (PackagePart part in zip1.GetParts())
            {
                string pathPart = part.Uri.ToString().Replace(@"/", "\\");
                if (Path.GetExtension(pathPart.ToLower()).Equals(extension.ToLower()))
                    resourceFiles.Add(part.GetStream());
            }
            return resourceFiles;
        }
    }

    public class Result
    {
        bool isSuccess;

        public bool IsSuccess
        {
            get { return isSuccess; }
            set { isSuccess = value; }
        }

        string message;

        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        public string PackagePath { get; set; }

        public Stream PackageStream { get {
            return new FileStream(PackagePath, FileMode.Open, FileAccess.Read);
        } }
    }
}
