using MyEdgeDrop.ViewModel;
using System.Diagnostics;
using System.Threading.Tasks;
using static Drop.ViewModel.Web.WebSocat;
namespace MyEdgeDrop
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPagesViewModel vm)
        {
            InitializeComponent();
            BindingContext= vm;
        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Debug.WriteLine(e.Item);
            DownloadFileFromApi((e.Item).ToString());

        }

    }

}
