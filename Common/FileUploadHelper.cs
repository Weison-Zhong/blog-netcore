namespace Blog2022_netcore.Common
{
    public class FileUploadHelper
    {
        private readonly IHostEnvironment _hostEnvironment;
        private string filePath = "uploadtest/";

        public FileUploadHelper(IHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
        }
        public string ImageUpload(IFormFile formFile)
        {
            var currentDate = DateTime.Now;
            var webRootPath = _hostEnvironment.ContentRootPath;
            try
            {
                //创建存储文件夹   (原来是创建每日存储文件夹)
                if (!Directory.Exists(webRootPath + filePath))
                {
                    Directory.CreateDirectory(webRootPath + filePath);
                }

                if (formFile != null)
                {
                    //文件后缀
                    var fileExtension = Path.GetExtension(formFile.FileName);//获取文件格式，拓展名

                    //判断文件大小
                    var fileSize = formFile.Length;

                    if (fileSize > 1024 * 1024 * 3)
                    {
                        return "文件不能大于3M";
                    }
                    //保存的文件名称(以名称和保存时间命名)
                    var saveName = formFile.FileName.Substring(0, formFile.FileName.LastIndexOf('.')) + "_" + currentDate.ToString("HHmmss") + fileExtension;

                    //文件保存
                    using (var fs = System.IO.File.Create(webRootPath + filePath + saveName))
                    {
                        formFile.CopyTo(fs);
                        fs.Flush();
                    }

                    //完整的文件路径
                    return Path.Combine("http://www.weison-zhong.cn/", filePath, saveName);
                }
                else
                {
                    return "上传失败，未检测上传的文件信息";
                }

            }
            catch (Exception ex)
            {
                return $"文件保存失败，异常信息为：{ex.Message}";
            }
        }

        public string SingleFileUpload(IFormFile formFile)
        {

            var webRootPath = _hostEnvironment.ContentRootPath;
            try
            {
                //创建存储文件夹   (原来是创建每日存储文件夹)
                if (!Directory.Exists(webRootPath + filePath))
                {
                    Directory.CreateDirectory(webRootPath + filePath);
                }

                if (formFile != null)
                {
                    //文件后缀
                    var fileExtension = Path.GetExtension(formFile.FileName);//获取文件格式，拓展名

                    //判断文件大小
                    var fileSize = formFile.Length;

                    if (fileSize > 1024 * 1024 * 10)
                    {
                        return "文件不能大于10M";
                    }
                    //保存的文件名称(以名称和保存时间命名)
                    var saveName = formFile.FileName.Substring(0, formFile.FileName.LastIndexOf('.')) + fileExtension;

                    //文件保存
                    using (var fs = System.IO.File.Create(webRootPath + filePath + saveName))
                    {
                        formFile.CopyTo(fs);
                        fs.Flush();
                    }

                    //完整的文件路径
                    return Path.Combine("http://www.weison-zhong.cn/", filePath, saveName);
                }
                else
                {
                    return "上传失败，未检测上传的文件信息";
                }

            }
            catch (Exception ex)
            {
                return $"文件保存失败，异常信息为：{ex.Message}";
            }
        }
    }
}
