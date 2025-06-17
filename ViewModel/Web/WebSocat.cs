using System.Diagnostics;
using System.Text.Json;
using static MyEdgeDrop.ViewModel.URL;
using static MyEdgeDrop.ViewModel.MainPagesViewModel;

namespace Drop.ViewModel.Web
{
    internal static class WebSocat
    {

        internal static async Task DownloadFileFromApi(string fileName)
        {
            // 构造完整的下载URL
            string downloadUrl = $"{ServerURL}/download/{fileName}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    Debug.WriteLine("开始下载");
                    // 发起GET请求
                    using (HttpResponseMessage response = await client.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead))
                    {
                        // 检查响应状态码
                        if (response.IsSuccessStatusCode)
                        {
                            // 以写入二进制的方式打开保存文件
                            string downloadsFolderPath = Path.Combine((Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)), "Downloads", fileName);

                            using (FileStream fileStream = new FileStream(downloadsFolderPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                            {
                                // 分块写入文件
                                await response.Content.CopyToAsync(fileStream);
                            }
                        }
                        else
                        {
                            // 打印错误信息
                            Console.WriteLine($"下载失败，状态码: {response.StatusCode}, 错误信息: {await response.Content.ReadAsStringAsync()}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"下载过程中发生错误: {ex.Message}");
                }
            }
        }

        internal static async Task UploadFile(string filePath)
        {
            using (HttpClient client = new HttpClient())
            {
                using (MultipartFormDataContent content = new MultipartFormDataContent())
                {
                    FileStream fileStream = new FileStream(filePath, FileMode.Open); // Move outside using block
                    content.Add(new StreamContent(fileStream), "file", Path.GetFileName(filePath));

                    HttpResponseMessage response = await client.PostAsync(ServerURL + "/upload/", content);

                    fileStream.Dispose(); // Dispose after PostAsync completes

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                    }
                }
            }
        }

        internal static async Task SendMessage(string messages)
        {
            if (messages == string.Empty)
            {
                return;
            }
            // 发送GET请求
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(ServerURL + $"/upload_message?message={messages}");

                // 检查响应状态码
                if (response.IsSuccessStatusCode)
                {
                    // 打印响应内容
                    _ = await response.Content.ReadAsStringAsync();
                }
            }
        }

        internal static async Task<string[]> GetFileList()
        {
            // 目标URL
            string url = ServerURL + "/list_files";

            // 创建HttpClient实例
            using (HttpClient client = new HttpClient())
            {
                // 发送GET请求
                HttpResponseMessage response = await client.GetAsync(url);
                // 打印响应内容
                string responseContent = await response.Content.ReadAsStringAsync();
                string[] list = JsonSerializer.Deserialize<string[]>(responseContent);
                return list;
            }
        }

        internal static async Task<string?> GetMessage()
        {
            // 创建HttpClient实例
            using HttpClient client = new();
            // 发送GET请求
            HttpResponseMessage response = await client.GetAsync(ServerURL + "/get_message");

            // 确保请求成功
            response.EnsureSuccessStatusCode();

            // 读取响应内容
            string responseBody = await response.Content.ReadAsStringAsync();
            responseBody = responseBody.Trim('"');
            return responseBody;
        }
    }
}
