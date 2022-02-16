/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

//from WindowBase.dll
using System.IO.Packaging;

namespace Infosys.Compress_DeCompress
{
    public class Compression
    {
        private const long BUFFER_SIZE = 4096;
        /// <summary>
        /// Adds the file to the given target compressed file. If target compressed file exists, then it adds to it otherwise it creates it.
        /// </summary>
        /// <param name="targetCompressedFileName">The target compressed file</param>
        /// <param name="fileToBeAdded">The file to be added</param>
        /// <param name="relativeTargetFolder">The relative path of the file in the target compressed file to be maintained</param>
        /// <returns>Result depicting if the execution succeeded or failed with error message (if any)</returns>
        public Result AddFile(string targetCompressedFileName, string fileToBeAdded, string relativeTargetFolder)
        {
            Result result = new Result();
            result.IsSuccess = true;
            try
            {
                using (Package zip = System.IO.Packaging.Package.Open(targetCompressedFileName, FileMode.OpenOrCreate))
                {
                    string destFilename = "";
                    if (relativeTargetFolder != null && relativeTargetFolder != "")
                        destFilename = ".\\" + relativeTargetFolder + "\\" + Path.GetFileName(fileToBeAdded);
                    // destFilename = Path.Combine(".", relativeTargetFolder, Path.GetFileName(fileToBeAdded));
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
        /// Compresses and adds the provided file to the steram provided
        /// </summary>
        /// <param name="outputStream">The stream to hold the compressed file</param>
        /// <param name="fileToBeAdded">The file to be added</param>
        /// <param name="relativeTargetFolder">The relative path of the file in the target compressed file to be maintained</param>
        /// <returns>Result depicting if the execution suceeded or failed with error message (if any)</returns>
        public Result AddFile(Stream outputStream, string fileToBeAdded, string relativeTargetFolder)
        {
            Result result = new Result();
            result.IsSuccess = true;
            try
            {
                using (Package zip = System.IO.Packaging.Package.Open(outputStream, FileMode.OpenOrCreate))
                {
                    string destFilename = "";
                    if (relativeTargetFolder != null && relativeTargetFolder != "")
                    {
                        if (relativeTargetFolder.EndsWith("\\"))
                        {
                            destFilename = ".\\" + relativeTargetFolder  + Path.GetFileName(fileToBeAdded);
                        }
                        else
                            destFilename = ".\\" + relativeTargetFolder + "\\" + Path.GetFileName(fileToBeAdded);
                    }
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
        /// Adds the file to the given target compressed file. If target compressed file exists, then it adds to it otherwise it creates it.
        /// It also maintains the folder structure in the target compressed file as the source.
        /// </summary>
        /// <param name="targetCompressedFileName">The target compressed file</param>
        /// <param name="completeFilePath">The file to be added</param>
        /// <returns>Result depicting if the execution suceeded or failed with error message (if any)</returns>
        public Result AddFileWithLocation(string targetCompressedFileName, string completeFilePath)
        {
            Result result = new Result();
            result.IsSuccess = true;
            try
            {
                string fileToBeAdded = Path.GetFileName(completeFilePath);
                string relativeTargetFolder = completeFilePath.Replace(Path.GetPathRoot(completeFilePath), "").Replace(fileToBeAdded, "");
                result = AddFile(targetCompressedFileName, fileToBeAdded, relativeTargetFolder);              
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
        public Result AddFolder(string targetCompressedFileName, string folderToBeAdded, string startingFolderInZip ="")
        {
            Result result = new Result();
            result.IsSuccess = true;
            try
            {
                //string relativeTargetFolder = folderToBeAdded.Replace(Path.GetPathRoot(folderToBeAdded), "");
                string relativeTargetFolder ="";
                if(string.IsNullOrEmpty(startingFolderInZip))
                    relativeTargetFolder = folderToBeAdded.Replace(Path.GetDirectoryName(folderToBeAdded) + "\\","");
                else
                    relativeTargetFolder = startingFolderInZip + "\\" + folderToBeAdded.Replace(Path.GetDirectoryName(folderToBeAdded) + "\\", "");
                if (Directory.Exists(folderToBeAdded))
                {
                    foreach (string file in Directory.GetFiles(folderToBeAdded))
                    {
                        result = AddFile(targetCompressedFileName, file, relativeTargetFolder);
                    }
                    foreach (string folder in Directory.GetDirectories(folderToBeAdded))
                    {
                        result = AddFolder(targetCompressedFileName, folder, relativeTargetFolder);
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
        /// Compresses and adds the folder, its sub-folders and files to the given stream.
        /// </summary>
        /// <param name="outputStream">The stream to hold the compressed folder</param>
        /// <param name="folderToBeAdded">The folder to be added</param>
        /// <returns>Result depicting if the execution suceeded or failed with error message (if any)</returns>
        public Result AddFolder(Stream outputStream, string folderToBeAdded)
        {
            Result result = new Result();
            result.IsSuccess = true;
            try
            {
                string relativeTargetFolder = folderToBeAdded.Replace(Path.GetPathRoot(folderToBeAdded), "");
                if (Directory.Exists(folderToBeAdded))
                {
                    foreach (string file in Directory.GetFiles(folderToBeAdded))
                    {
                        result = AddFile(outputStream, file, relativeTargetFolder);
                    }
                    foreach (string folder in Directory.GetDirectories(folderToBeAdded))
                    {
                        result = AddFolder(outputStream, folder);
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
        /// Get the access to the stream to the target compressed file
        /// </summary>
        /// <param name="targetCompressedFileName">The target compressed file</param>
        /// <param name="outputStream">The stream to the target compressed file</param>
        /// <returns>Result depicting if the execution suceeded or failed with error message (if any)</returns>
        public Result GetStream(string targetCompressedFileName, System.IO.Stream outputStream)
        {
            Result result = new Result();
            result.IsSuccess = true;
            try
            {
                FileStream stream = new FileStream(targetCompressedFileName, FileMode.Open);
                CopyStream(stream, outputStream);
                outputStream.Position = 0;
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
        /// Writes the stream to a compressed file. If target compressed file exists, then it adds to it otherwise it creates it.
        /// </summary>
        /// <param name="inputStream">The input stream holding the compressed file</param>
        /// <param name="targetCompressedFileName">The destination compressed physical file</param>
        /// <returns>Result depicting if the execution suceeded or failed with error message (if any)</returns>
        public Result WriteToFile(Stream inputStream, string targetCompressedFileName)
        {
            Result result = new Result();
            result.IsSuccess = true;
            try
            {
                inputStream.Position = 0;
                FileStream stream = new FileStream(targetCompressedFileName, FileMode.OpenOrCreate);
                CopyStream(inputStream, stream);
                inputStream.Position = 0;
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
        /// Extracts the files and folders in the provided compressed file
        /// </summary>
        /// <param name="compressedFileName">The compressed file</param>
        /// <param name="extractToFolder">The destination for extraction</param>
        /// <param name="overWriteExistingFile">If true and same file found, then it will be over written</param>
        /// <returns>Result depicting if the execution suceeded or failed with error message (if any)</returns>
        public Result Extract(string compressedFileName, string extractToFolder, bool overWriteExistingFile)
        {
            Result result = new Result();
            result.IsSuccess = true;
            try
            {
                using (Package zip = System.IO.Packaging.Package.Open(compressedFileName, FileMode.Open))
                {
                    foreach (PackagePart part in zip.GetParts())
                    {
                        string destination = extractToFolder + part.Uri.ToString().Replace(@"/", "\\");
                        string directoryName = Path.GetDirectoryName(destination);
                        if (!Directory.Exists(directoryName))
                            Directory.CreateDirectory(directoryName);

                        if (File.Exists(destination) && !overWriteExistingFile)
                            continue;
                        string fileName = Path.GetFileName(destination);
                        using (FileStream destFile = new FileStream(destination, FileMode.OpenOrCreate))
                        {
                            CopyStream(part.GetStream(), destFile);
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

        //private void CopyStream(System.IO.FileStream inputStream, System.IO.Stream outputStream)
        /// <summary>
        /// Copies the content from one stream (source:inputStream) to another (target:outputStream)
        /// </summary>
        /// <param name="inputStream">Source stream</param>
        /// <param name="outputStream">Target stream</param>
        private void CopyStream(System.IO.Stream inputStream, System.IO.Stream outputStream)
        {
            long bufferSize = inputStream.Length < BUFFER_SIZE ? inputStream.Length : BUFFER_SIZE;
            byte[] buffer = new byte[bufferSize];
            int bytesRead = 0; long bytesWritten = 0;
            while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                outputStream.Write(buffer, 0, bytesRead);
                bytesWritten += bufferSize;
            }
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
    }
}
