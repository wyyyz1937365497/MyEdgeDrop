using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using static Drop.ViewModel.Web.WebSocat;

namespace MyEdgeDrop.ViewModel
{
    public static class URL
    {
        public static string ServerURL { get; set; } = "http://10.168.1.107:19255";
    }

    public partial class MainPagesViewModel : ObservableObject
    {
        private string? FPath;
        private string? Name;
        private Dictionary<string, string> FileDic = new Dictionary<string, string>();
        public string[]? ResFList;
        public IAsyncRelayCommand SendMesCommand;
        public IAsyncRelayCommand GetMesCommand;
        public IAsyncRelayCommand PickAndShowCommand;
        public IAsyncRelayCommand SettingCommand;
        public IAsyncRelayCommand SelectDownloadFileCommand;
        public IAsyncRelayCommand DownLoadFile;

        public MainPagesViewModel()
        {
            Files = [];
            Filelists = [];
            SendMesCommand = new AsyncRelayCommand(SendMes);
            GetMesCommand = new AsyncRelayCommand(GetMes);
            PickAndShowCommand = new AsyncRelayCommand(PickAndShow);
            SettingCommand = new AsyncRelayCommand(Setting);
            SelectDownloadFileCommand = new AsyncRelayCommand(SelectDownloadFile);

        }

        [ObservableProperty]
        string? gmes;

        [ObservableProperty]
        string? smes;

        [ObservableProperty]
        ObservableCollection<string>? files;

        [ObservableProperty]
        ObservableCollection<string>? filelists;

        private async Task SendMes()
        {
            try
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
            catch
            {
                await Application.Current.MainPage.DisplayAlert("发送失败", "请检查后端是否正常运行！", "OK");
            }
        }

        private async Task GetMes()
        {
            try
            {
                Gmes = await GetMessage();
            }
            catch
            {
                await Application.Current.MainPage.DisplayAlert("接收失败", "请检查后端是否正常运行！", "OK");
            }
        }

        private async Task<FileResult?> PickAndShow()
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
            Filelists.Clear();
        }

        private async Task Setting()
        {
            URL.ServerURL = await Application.Current.MainPage.DisplayPromptAsync("请输入服务器的IP+端口", "现为：" + URL.ServerURL);
        }
        [RelayCommand]
        private async Task SelectDownloadFile()
        {
            DeleteFile();
            ResFList = await GetFileList();
            foreach (string file in ResFList)
            {
                Filelists.Add(file);
            }
            ResFList = [];
        }
        private async Task DownLoadFile(string file)
        {
            await Application.Current.MainPage.DisplayPromptAsync("将要下载的文件", "现为：" + file);
            //DownloadFileFromApi(file);
            //Filelists.Remove(file);
        }

    }
}

