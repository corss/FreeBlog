<%@ WebHandler Language="C#" Class="upload_ajax" %>
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;

public class upload_ajax : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        //取得处事类型
        string action = context.Request.QueryString["action"];

        switch (action)
        {
            case "EditorFile": //编辑器文件
                EditorFile(context);
                break;
            case "ManagerFile": //管理文件
                ManagerFile(context);
                break;
            default: //普通上传
                UpLoadFile(context);
                break;
        }
    }
    //文件存储路径
    private string Files = "../../UploadFiles/";

    #region 上传文件处理===================================
    private void UpLoadFile(HttpContext context)
    {
        string _delfile = context.Request.QueryString["DelFilePath"], _filetypes = context.Request.QueryString["filetypes"];
        HttpPostedFile _upfile = context.Request.Files["Filedata"];
        bool _iswater = false; //默认不打水印
        bool _isthumbnail = false; //默认不生成缩略图

        if (context.Request.QueryString["IsWater"] == "1")
            _iswater = true;
        if (context.Request.QueryString["IsThumbnail"] == "1")
            _isthumbnail = true;
        if (_upfile == null)
        {
            context.Response.Write("{\"status\": 0, \"msg\": \"请选择要上传文件！\"}");
            return;
        }
        string msg = fileSaveAs(_filetypes, _upfile, _isthumbnail, _iswater);
        //删除已存在的旧文件，旧文件不为空且应是上传文件，防止跨目录删除
        if (!string.IsNullOrEmpty(_delfile) && _delfile.IndexOf("../") == -1
            && _delfile.ToLower().StartsWith(Files))
        {
            DeleteUpFile(_delfile);
        }
        //返回成功信息
        context.Response.Write(msg);
        context.Response.End();
    }
    #endregion

    #region 编辑器上传处理===================================
    private void EditorFile(HttpContext context)
    {
        string _filetypes = context.Request.QueryString["_filetypes"];
        bool _iswater = false; //默认不打水印
        if (context.Request.QueryString["IsWater"] == "1")
            _iswater = true;
        HttpPostedFile imgFile = context.Request.Files["imgFile"];
        if (imgFile == null)
        {
            showError(context, "请选择要上传文件！");
            return;
        }
        string remsg = fileSaveAs(_filetypes, imgFile, false, _iswater);
        Dictionary<string, object> dic = DataRowFromJSON(remsg);
        string status = dic["status"].ToString();
        string msg = dic["msg"].ToString();
        if (status == "0")
        {
            showError(context, msg);
            return;
        }
        string filePath = dic["path"].ToString(); //取得上传后的路径
        Hashtable hash = new Hashtable();
        hash["error"] = 0;
        hash["url"] = filePath;
        context.Response.AddHeader("Content-Type", "text/html; charset=UTF-8");
        context.Response.Write(ObjectToJSON(hash));
        context.Response.End();
    }
    //显示错误
    private void showError(HttpContext context, string message)
    {
        Hashtable hash = new Hashtable();
        hash["error"] = 1;
        hash["message"] = message;
        context.Response.AddHeader("Content-Type", "text/html; charset=UTF-8");
        context.Response.Write(ObjectToJSON(hash));
        context.Response.End();
    }
    #endregion

    #region 浏览文件处理=====================================
    private void ManagerFile(HttpContext context)
    {
        //根目录路径，相对路径
        String rootPath = Files; //站点目录+上传目录
        //根目录URL，可以指定绝对路径，比如 http://www.yoursite.com/attached/
        String rootUrl = Files;
        //图片扩展名
        String fileTypes = "gif,jpg,jpeg,png,bmp";

        String currentPath = "";
        String currentUrl = "";
        String currentDirPath = "";
        String moveupDirPath = "";

        String dirPath = GetMapPath(rootPath);
        String dirName = context.Request.QueryString["dir"];

        //根据path参数，设置各路径和URL
        String path = context.Request.QueryString["path"];
        path = String.IsNullOrEmpty(path) ? "" : path;
        if (path == "")
        {
            currentPath = dirPath;
            currentUrl = rootUrl;
            currentDirPath = "";
            moveupDirPath = "";
        }
        else
        {
            currentPath = dirPath + path;
            currentUrl = rootUrl + path;
            currentDirPath = path;
            moveupDirPath = Regex.Replace(currentDirPath, @"(.*?)[^\/]+\/$", "$1");
        }

        //排序形式，name or size or type
        String order = context.Request.QueryString["order"];
        order = String.IsNullOrEmpty(order) ? "" : order.ToLower();

        //不允许使用..移动到上一级目录
        if (Regex.IsMatch(path, @"\.\."))
        {
            context.Response.Write("Access is not allowed.");
            context.Response.End();
        }
        //最后一个字符不是/
        if (path != "" && !path.EndsWith("/"))
        {
            context.Response.Write("Parameter is not valid.");
            context.Response.End();
        }
        //目录不存在或不是目录
        if (!Directory.Exists(currentPath))
        {
            context.Response.Write("Directory does not exist.");
            context.Response.End();
        }

        //遍历目录取得文件信息
        string[] dirList = Directory.GetDirectories(currentPath);
        string[] fileList = Directory.GetFiles(currentPath);

        switch (order)
        {
            case "size":
                Array.Sort(dirList, new NameSorter());
                Array.Sort(fileList, new SizeSorter());
                break;
            case "type":
                Array.Sort(dirList, new NameSorter());
                Array.Sort(fileList, new TypeSorter());
                break;
            case "name":
            default:
                Array.Sort(dirList, new NameSorter());
                Array.Sort(fileList, new NameSorter());
                break;
        }

        Hashtable result = new Hashtable();
        result["moveup_dir_path"] = moveupDirPath;
        result["current_dir_path"] = currentDirPath;
        result["current_url"] = currentUrl;
        result["total_count"] = dirList.Length + fileList.Length;
        List<Hashtable> dirFileList = new List<Hashtable>();
        result["file_list"] = dirFileList;
        for (int i = 0; i < dirList.Length; i++)
        {
            DirectoryInfo dir = new DirectoryInfo(dirList[i]);
            Hashtable hash = new Hashtable();
            hash["is_dir"] = true;
            hash["has_file"] = (dir.GetFileSystemInfos().Length > 0);
            hash["filesize"] = 0;
            hash["is_photo"] = false;
            hash["filetype"] = "";
            hash["filename"] = dir.Name;
            hash["datetime"] = dir.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss");
            dirFileList.Add(hash);
        }
        for (int i = 0; i < fileList.Length; i++)
        {
            FileInfo file = new FileInfo(fileList[i]);
            Hashtable hash = new Hashtable();
            hash["is_dir"] = false;
            hash["has_file"] = false;
            hash["filesize"] = file.Length;
            hash["is_photo"] = (Array.IndexOf(fileTypes.Split(','), file.Extension.Substring(1).ToLower()) >= 0);
            hash["filetype"] = file.Extension.Substring(1);
            hash["filename"] = file.Name;
            hash["datetime"] = file.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss");
            dirFileList.Add(hash);
        }
        context.Response.AddHeader("Content-Type", "application/json; charset=UTF-8");
        context.Response.Write(ObjectToJSON(result));
        context.Response.End();
    }

    #region Helper
    public class NameSorter : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null && y == null)
            {
                return 0;
            }
            if (x == null)
            {
                return -1;
            }
            if (y == null)
            {
                return 1;
            }
            FileInfo xInfo = new FileInfo(x.ToString());
            FileInfo yInfo = new FileInfo(y.ToString());

            return xInfo.FullName.CompareTo(yInfo.FullName);
        }
    }

    public class SizeSorter : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null && y == null)
            {
                return 0;
            }
            if (x == null)
            {
                return -1;
            }
            if (y == null)
            {
                return 1;
            }
            FileInfo xInfo = new FileInfo(x.ToString());
            FileInfo yInfo = new FileInfo(y.ToString());

            return xInfo.Length.CompareTo(yInfo.Length);
        }
    }

    public class TypeSorter : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null && y == null)
            {
                return 0;
            }
            if (x == null)
            {
                return -1;
            }
            if (y == null)
            {
                return 1;
            }
            FileInfo xInfo = new FileInfo(x.ToString());
            FileInfo yInfo = new FileInfo(y.ToString());

            return xInfo.Extension.CompareTo(yInfo.Extension);
        }
    }
    #endregion
    #endregion

    /// <summary>
    /// 文件上传方法
    /// </summary>
    /// <param name="postedFile">文件流</param>
    /// <param name="isThumbnail">是否生成缩略图</param>
    /// <param name="isWater">是否打水印</param>
    /// <returns>上传后文件信息</returns>
    public string fileSaveAs(string filetypes, HttpPostedFile postedFile, bool isThumbnail, bool isWater)
    {
        try
        {
            string fileExt = GetFileExt(postedFile.FileName); //文件扩展名，不含“.”
            int fileSize = postedFile.ContentLength; //获得文件大小，以字节为支部
            string fileName = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf(@"\") + 1); //取得原文件名
            string newFileName = GetRamCode() + "." + fileExt; //随机生成新的文件名
            string newThumbnailFileName = "thumb_" + newFileName; //随机生成缩略图文件名
            string upLoadPath = GetUpLoadPath(); //上传目录相对路径
            string fullUpLoadPath = GetMapPath(upLoadPath); //上传目录的物理路径
            string newFilePath = upLoadPath.TrimStart("../".ToCharArray()) + newFileName; //上传后的路径
            string newThumbnailPath = upLoadPath + newThumbnailFileName; //上传后的缩略图路径

            //检查文件扩展名是否合法
            if (!IsFileExt(filetypes, fileExt))
            {
                return "{\"status\": 0, \"msg\": \"不允许上传" + fileExt + "类型的文件！\"}";
            }
            //检查文件大小是否合法
            //if (!CheckFileSize(fileExt, fileSize))
            //{
            //    return "{\"status\": 0, \"msg\": \"文件超过限制的大小！\"}";
            //}
            //检查上传的物理路径是否存在，不存在则创建
            if (!Directory.Exists(fullUpLoadPath))
            {
                Directory.CreateDirectory(fullUpLoadPath);
            }

            //保存文件
            postedFile.SaveAs(fullUpLoadPath + newFileName);

            newThumbnailPath = newFilePath;
            //处理完毕，返回JOSN格式的文件信息
            return "{\"status\": 1, \"msg\": \"上传文件成功！\", \"name\": \""
                + fileName + "\", \"path\": \"" + newFilePath + "\", \"thumb\": \""
                + newThumbnailPath + "\", \"size\": " + fileSize + ", \"ext\": \"" + fileExt + "\"}";
        }
        catch
        {
            return "{\"status\": 0, \"msg\": \"上传过程中发生意外错误！\"}";
        }
    }

    /// <summary>
    /// 保存远程文件到本地
    /// </summary>
    /// <param name="fileUri">URI地址</param>
    /// <returns>上传后的路径</returns>
    public string remoteSaveAs(string fileUri)
    {
        WebClient client = new WebClient();
        string fileExt = string.Empty; //文件扩展名，不含“.”
        if (fileUri.LastIndexOf(".") == -1)
        {
            fileExt = "gif";
        }
        else
        {
            fileExt = GetFileExt(fileUri);
        }
        string newFileName = GetRamCode() + "." + fileExt; //随机生成新的文件名
        string upLoadPath = GetUpLoadPath(); //上传目录相对路径
        string fullUpLoadPath = GetMapPath(upLoadPath); //上传目录的物理路径
        string newFilePath = upLoadPath.TrimStart("../".ToCharArray()) + newFileName; //上传后的路径
        //检查上传的物理路径是否存在，不存在则创建
        if (!Directory.Exists(fullUpLoadPath))
        {
            Directory.CreateDirectory(fullUpLoadPath);
        }

        try
        {
            client.DownloadFile(fileUri, fullUpLoadPath + newFileName);
        }
        catch
        {
            return string.Empty;
        }
        client.Dispose();
        return newFilePath;
    }

    #region 私有方法
    #region 获得当前绝对路径
    /// <summary>
    /// 获得当前绝对路径
    /// </summary>
    /// <param name="strPath">指定的路径</param>
    /// <returns>绝对路径</returns>
    public static string GetMapPath(string strPath)
    {
        if (strPath.ToLower().StartsWith("http://"))
        {
            return strPath;
        }
        if (HttpContext.Current != null)
        {
            return HttpContext.Current.Server.MapPath(strPath);
        }
        else //非web程序引用
        {
            strPath = strPath.Replace("/", "\\");
            if (strPath.StartsWith("\\"))
            {
                strPath = strPath.Substring(strPath.IndexOf('\\', 1)).TrimStart('\\');
            }
            return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strPath);
        }
    }
    #endregion
    #region 生成日期随机码
    /// <summary>
    /// 生成日期随机码
    /// </summary>
    /// <returns></returns>
    public static string GetRamCode()
    {
        #region
        return DateTime.Now.ToString("yyyyMMddHHmmssffff");
        #endregion
    }
    #endregion
    /// <summary>
    /// 返回文件扩展名，不含“.”
    /// </summary>
    /// <param name="_filepath">文件全名称</param>
    /// <returns>string</returns>
    public static string GetFileExt(string _filepath)
    {
        if (string.IsNullOrEmpty(_filepath))
        {
            return "";
        }
        if (_filepath.LastIndexOf(".") > 0)
        {
            return _filepath.Substring(_filepath.LastIndexOf(".") + 1); //文件扩展名，不含“.”
        }
        return "";
    }
    /// <summary>
    /// 返回上传目录相对路径
    /// </summary>
    /// <param name="fileName">上传文件名</param>
    private string GetUpLoadPath()
    {
        string path = Files; //站点目录+上传目录
        switch (2)
        {
            case 1: //按年月日每天一个文件夹
                path += DateTime.Now.ToString("yyyyMMdd");
                break;
            default: //按年月/日存入不同的文件夹
                path += DateTime.Now.ToString("yyyyMM") + "/" + DateTime.Now.ToString("dd");
                break;
        }
        return path + "/";
    }
    private bool IsFileExt(string _filetypes, string _fileExt)
    {
        if (!string.IsNullOrWhiteSpace(_filetypes))
        {
            ArrayList al = new ArrayList();
            foreach (string v in _filetypes.Split(','))
            {
                al.Add(v);
            }
            if (al.Contains(_fileExt.ToLower()))
            {
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// 是否为图片文件
    /// </summary>
    /// <param name="_fileExt">文件扩展名，不含“.”</param>
    private bool IsImage(string _fileExt)
    {
        ArrayList al = new ArrayList();
        al.Add("bmp");
        al.Add("jpeg");
        al.Add("jpg");
        al.Add("gif");
        al.Add("png");
        if (al.Contains(_fileExt.ToLower()))
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 删除上传的文件(及缩略图)
    /// </summary>
    /// <param name="_filepath"></param>
    public static void DeleteUpFile(string _filepath)
    {
        if (string.IsNullOrEmpty(_filepath))
        {
            return;
        }
        string fullpath = GetMapPath(_filepath); //原图
        if (File.Exists(fullpath))
        {
            File.Delete(fullpath);
        }
    }
    /// <summary> 
    /// 对象转JSON 
    /// </summary> 
    /// <param name="obj">对象</param> 
    /// <returns>JSON格式的字符串</returns> 
    public static string ObjectToJSON(object obj)
    {
        JavaScriptSerializer jss = new JavaScriptSerializer();
        try
        {
            byte[] b = Encoding.UTF8.GetBytes(jss.Serialize(obj));
            return Encoding.UTF8.GetString(b);
        }
        catch (Exception ex)
        {

            throw new Exception("JSONHelper.ObjectToJSON(): " + ex.Message);
        }
    }

    /// <summary> 
    /// 将JSON文本转换成数据行 
    /// </summary> 
    /// <param name="jsonText">JSON文本</param> 
    /// <returns>数据行的字典</returns>
    public static Dictionary<string, object> DataRowFromJSON(string jsonText)
    {
        return JSONToObject<Dictionary<string, object>>(jsonText);
    }
    /// <summary> 
    /// JSON文本转对象,泛型方法 
    /// </summary> 
    /// <typeparam name="T">类型</typeparam> 
    /// <param name="jsonText">JSON文本</param> 
    /// <returns>指定类型的对象</returns> 
    public static T JSONToObject<T>(string jsonText)
    {
        JavaScriptSerializer jss = new JavaScriptSerializer();
        try
        {
            return jss.Deserialize<T>(jsonText);
        }
        catch (Exception ex)
        {
            throw new Exception("JSONHelper.JSONToObject(): " + ex.Message);
        }
    }
    #endregion

    public bool IsReusable { get { return false; } }
}