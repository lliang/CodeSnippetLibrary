using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace CodeSnippetLibrary.Net
{
    public class FTPClient
    {
        private readonly string _serverIp;
        private readonly string _userName;
        private readonly string _password;
        private const int BufferSize = 4096; //4kb

        /// <summary>
        /// 构造FTP实例
        /// </summary>
        /// <param name="ftpServerIp">FTP服务器地址</param>
        /// <param name="ftpUserId">用户名</param>
        /// <param name="ftpPassword">密码</param>
        public FTPClient(string ftpServerIp, string ftpUserId, string ftpPassword)
        {
            _serverIp = ftpServerIp;
            _userName = ftpUserId;
            _password = ftpPassword;
        }

        /// <summary>
        /// 创建FTP连接请求
        /// </summary>
        /// <param name="path">访问路径</param>
        private FtpWebRequest CreateFtpRequest(string path)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(path));
            request.UseBinary = true;
            request.Credentials = new NetworkCredential(_userName, _password);
            return request;
        }

        /// <summary>
        /// 验证服务器
        /// </summary>
        /// <returns>返回一个Boolean值标识是否通过服务器验证</returns>
        public bool TestFtp()
        {
            try
            {
                List();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 获取路径下的文件列表
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="method">命令</param>
        /// <returns></returns>
        private string[] List(string path, string method)
        {
            List<string> fileNames = new List<string>();
            FtpWebRequest request = CreateFtpRequest(path);
            request.Method = method;
            using (WebResponse response = request.GetResponse())
            {
                Stream responseStream = response.GetResponseStream();
                if (responseStream != null)
                {
                    using (StreamReader reader = new StreamReader(responseStream, Encoding.UTF8))
                    {
                        while (!reader.EndOfStream)
                        {
                            fileNames.Add(reader.ReadLine());
                        }
                    }
                }
            }
            return fileNames.ToArray();
        }

        /// <summary>
        /// 获取路径下的文件列表
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public string[] List(string path)
        {
            return List("ftp://" + _serverIp + "/" + path, WebRequestMethods.Ftp.ListDirectory);
        }

        /// <summary>
        /// 获取根目录下的文件列表
        /// </summary>
        /// <returns></returns>
        public string[] List()
        {
            return List("");
        }

        /// <summary>
        /// 向目标目录上传文件
        /// </summary>
        /// <param name="filename">上传的文件名</param>
        /// <param name="folder">上传的目录</param>
        public void Upload(string filename, string folder)
        {
            FileInfo fileInfo = new FileInfo(filename);
            string uri;
            if (string.IsNullOrEmpty(folder))
            {
                uri = "ftp://" + _serverIp + "/" + fileInfo.Name;
            }
            else
            {
                uri = "ftp://" + _serverIp + "/" + folder + (folder.EndsWith("/") ? string.Empty : "/") + fileInfo.Name;
            }
            FtpWebRequest request = CreateFtpRequest(uri);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.ContentLength = fileInfo.Length;
            byte[] buffer = new byte[BufferSize];
            using (FileStream fs = fileInfo.OpenRead())
            {
                using (Stream requestStream = request.GetRequestStream())
                {
                    int contentLength = fs.Read(buffer, 0, BufferSize);
                    while (contentLength != 0)
                    {
                        requestStream.Write(buffer, 0, contentLength);
                        contentLength = fs.Read(buffer, 0, BufferSize);
                    }
                    requestStream.Flush();
                }
            }
        }

        /// <summary>
        /// 向目标目录上传流
        /// </summary>
        /// <param name="filename">上传的文件名，不包括路径，包括扩展名</param>
        /// <param name="content">文件的流内容</param>
        /// <param name="folder">上传的目录</param>
        public void Upload(string filename, Stream content, string folder)
        {
            string uri;
            if (string.IsNullOrEmpty(folder))
            {
                uri = "ftp://" + _serverIp + "/" + filename;
            }
            else
            {
                uri = "ftp://" + _serverIp + "/" + folder + (folder.EndsWith("/") ? string.Empty : "/") + filename;
            }
            FtpWebRequest request = CreateFtpRequest(uri);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.ContentLength = content.Length;
            byte[] buffer = new byte[BufferSize];
            using (Stream requestStream = request.GetRequestStream())
            {
                int contentLength = content.Read(buffer, 0, BufferSize);
                while (contentLength != 0)
                {
                    requestStream.Write(buffer, 0, contentLength);
                    contentLength = content.Read(buffer, 0, BufferSize);
                }
                requestStream.Flush();
            }
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="ftpFilePath">文件路径（包括文件名）</param>
        /// <param name="localFilePath">下载后文件名</param>
        public void Download(string ftpFilePath, string localFilePath)
        {
            if (File.Exists(localFilePath))
            {
                throw new Exception(localFilePath + "已存在，无法继续下载。");
            }
            string url = "ftp://" + _serverIp + "/" + ftpFilePath;
            FtpWebRequest request = CreateFtpRequest(url);
            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    if (responseStream == null)
                    {
                        return;
                    }

                    byte[] buffer = new byte[BufferSize];
                    using (FileStream fileStream = new FileStream(localFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        int readCount = responseStream.Read(buffer, 0, BufferSize);
                        while (readCount > 0)
                        {
                            fileStream.Write(buffer, 0, readCount);
                            readCount = responseStream.Read(buffer, 0, BufferSize);
                        }
                        fileStream.Flush();
                    }
                }
            }
        }

        /// <summary>
        /// 删除一个文件
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <returns>返回一个Boolean值表示是否删除成功</returns>
        public void DeleteFile(string fileName)
        {
            string uri = "ftp://" + _serverIp + "/" + fileName;
            FtpWebRequest request = CreateFtpRequest(uri);
            request.KeepAlive = false;
            request.Method = WebRequestMethods.Ftp.DeleteFile;
            using (request.GetResponse()) { }
        }

        /// <summary>
        /// 目录是否存在
        /// </summary>
        /// <param name="dir">目标目录</param>
        /// <returns>返回一个Boolean值表示是否存在目录</returns>
        public bool IsDirectoryExists(string dir)
        {
            if (dir.StartsWith("/"))
            {
                dir = dir.Substring(1);
            }
            if (dir.EndsWith("/"))
            {
                dir = dir.Substring(0, dir.Length - 1);
            }
            int dirIndex = dir.LastIndexOf('/');
            try
            {
                if (dirIndex >= 0)
                {
                    string[] serverDirs = GetDirectories(dir.Substring(0, dirIndex));
                    for (int i = 0; i < serverDirs.Length; i++)
                    {
                        serverDirs[i] = serverDirs[i].ToLower();
                    }
                    return serverDirs.Contains(dir.Substring(dir.LastIndexOf('/') + 1).ToLower());
                }
                var dirs = GetDirectories();
                for (int i = 0; i < dirs.Length; i++)
                {
                    dirs[i] = dirs[i].ToLower();
                }
                return dirs.Contains(dir.ToLower());
            }
            catch (WebException ex)
            {
                var webResponse = ex.Response as FtpWebResponse;
                if (webResponse == null)
                {
                    throw;
                }
                FtpWebResponse response = webResponse;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    return false;
                }
                throw;
            }
        }

        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="dir">目录名称，支持多级</param>
        public void MakeDirectory(string dir)
        {
            if (IsDirectoryExists(dir))
            {
                return;
            }
            string[] targetNames = dir.Split('/');
            string path = "/";
            foreach (string targetName in targetNames)
            {
                if (string.IsNullOrEmpty(targetName))
                {
                    break;
                }
                if (!IsDirectoryExists(path + targetName))
                {
                    string uri = _serverIp + path + targetName;
                    FtpWebRequest request = CreateFtpRequest("ftp://" + uri.Replace("//", "/"));
                    request.Method = WebRequestMethods.Ftp.MakeDirectory;
                    using (request.GetResponse()) { }
                }
                path += targetName + "/";
            }
        }

        /// <summary>
        /// 删除目录，必须为空目录，否则会抛出异常
        /// </summary>
        /// <param name="dirName">目录路径</param>
        /// <returns>返回一个Boolean值表示是否删除成功</returns>
        public void DeleteDirectory(string dirName)
        {
            string uri = "ftp://" + _serverIp + "/" + dirName;
            FtpWebRequest request = CreateFtpRequest(uri);
            request.Method = WebRequestMethods.Ftp.RemoveDirectory;
            using (request.GetResponse()) { }
        }

        /// <summary>
        /// 获取文件大小
        /// </summary>
        /// <param name="filename">文件路径</param>
        /// <returns>返回一个long值表示文件大小</returns>
        public long GetFileSize(string filename)
        {
            long fileSize;
            string uri = "ftp://" + _serverIp + "/" + filename;
            FtpWebRequest request = CreateFtpRequest(uri);
            request.Method = WebRequestMethods.Ftp.GetFileSize;
            using (WebResponse response = request.GetResponse())
            {
                fileSize = response.ContentLength;
            }
            return fileSize;
        }

        /// <summary>
        /// 重命名文件或文件夹
        /// </summary>
        /// <param name="currentPath">目标文件或文件夹完整路径</param>
        /// <param name="newName">新名称</param>
        /// <returns>返回一个Boolean值表示重命名是否成功</returns>
        public void Rename(string currentPath, string newName)
        {
            string uri = "ftp://" + _serverIp + "/" + currentPath;
            FtpWebRequest request = CreateFtpRequest(uri);
            request.Method = WebRequestMethods.Ftp.Rename;
            request.RenameTo = newName;
            using (request.GetResponse()) { }
        }

        /// <summary>
        /// 获得文件明细
        /// </summary>
        /// <returns>返回一个string[]，表示获取的文件名称数组</returns>
        public string[] ListDirectoryDetails()
        {
            return List("ftp://" + _serverIp + "/", WebRequestMethods.Ftp.ListDirectoryDetails);
        }

        /// <summary>
        /// 获得文件明细
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>返回一个string[]，表示获取的文件名称数组</returns>
        public string[] ListDirectoryDetails(string path)
        {
            return List("ftp://" + _serverIp + "/" + path, WebRequestMethods.Ftp.ListDirectoryDetails);
        }

        /// <summary>
        /// 获取所有文件夹
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>返回一个string[]，表示获取的文件夹名称数组</returns>
        public string[] GetDirectories(string path)
        {
            List<string> list = new List<string>();
            string[] allstr = ListDirectoryDetails(path);
            foreach (string s in allstr)
            {
                try
                {
                    if (s.Substring(0, 1).ToLower() != "d" && !s.ToLower().Contains("<dir>"))
                    {
                        continue;
                    }
                    if (s.Substring(s.LastIndexOf(' ')) != " ." && s.Substring(s.LastIndexOf(' ')) != " ..")
                    {
                        list.Add(s.Substring(s.LastIndexOf(' ') + 1));
                    }
                }
                catch
                {
                    return null;
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// 获取根目录的所有文件夹
        /// </summary>
        /// <returns></returns>
        public string[] GetDirectories()
        {
            List<string> list = new List<string>();
            string[] allstr = ListDirectoryDetails();
            foreach (string s in allstr)
            {
                try
                {
                    if (s.Substring(0, 1).ToLower() != "d" && !s.ToLower().Contains("<dir>"))
                    {
                        continue;
                    }
                    if (s.Substring(s.LastIndexOf(' ')) != " ." && s.Substring(s.LastIndexOf(' ')) != " ..")
                    {
                        list.Add(s.Substring(s.LastIndexOf(' ') + 1));
                    }
                }
                catch
                {
                    return null;
                }
            }
            return list.ToArray();
        }
    }
}
