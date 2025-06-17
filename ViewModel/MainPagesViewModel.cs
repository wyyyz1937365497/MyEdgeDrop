using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace MyEdgeDrop.ViewModel
{


    public partial class MainPagesViewModel : ObservableObject
    {
        private string FPath;
        private string Name;
        public Dictionary<string, string> FileDic = new Dictionary<string, string>();
        static string ServerURL = "http://10.168.1.107:19255";
        public MainPagesViewModel()
        {
            Files = [];
        }

        [ObservableProperty]
        string gmes;

        [ObservableProperty]
        string smes;

        [ObservableProperty]
        ObservableCollection<string> files;

        static async Task<string?> GetMessage()
        {
            // 创建HttpClient实例
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // 发送GET请求
                    HttpResponseMessage response = await client.GetAsync(ServerURL + "/get_message");

                    // 确保请求成功
                    response.EnsureSuccessStatusCode();

                    // 读取响应内容
                    string responseBody = await response.Content.ReadAsStringAsync();
                    responseBody = responseBody.Trim('"');
                    // 输出响应内容
                    return responseBody;
                }
                catch (HttpRequestException e)
                {
                    // 处理请求异常
                    Console.WriteLine("Exception Caught!");
                    Console.WriteLine("Message :{0} ", e.Message);
                    return null;
                }
            }
        }

        static async Task SendMessage(string messages)
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

        static async Task UploadFile(string filePath)
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


        [RelayCommand]
        async Task Send()
        {
            await SendMessage(Smes);
            Smes = string.Empty;
            if (FileDic.Count != 0)
            {
                foreach (var fp in FileDic.Keys)
                {
                    await UploadFile(FileDic[fp]);
                    FileDic.Remove(fp);
                    File.Delete(fp);
                }
                DeleteFile();
            }
        }
        [RelayCommand]
        async Task Get()
        {
            Gmes = await GetMessage();
        }

        [RelayCommand]
        async Task<FileResult?> PickAndShow()
        {
            try
            {
                var pickedFiles = await FilePicker.Default.PickMultipleAsync();
                foreach (var file in pickedFiles)
                {
                    FPath = file.FullPath;
                    Name = Path.GetFileName(FPath);
                    FileDic.Add(Name, FPath);
                    Files.Add(Name);
                }
            }
            catch
            {
                return null;
                // The user canceled or something went wrong
            }

            return null;
        }
        [RelayCommand]
        void DeleteFile()
        {
            FileDic.Clear();
            Files.Clear();
        }

        [RelayCommand]
        async Task Setting()
        {
            string result = await Application.Current.MainPage.DisplayPromptAsync("请输入服务器的IP+端口", "现为："+ ServerURL);
        }
    }
}

