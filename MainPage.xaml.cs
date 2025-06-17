
using MyEdgeDrop.ViewModel;
using System.Globalization;

namespace MyEdgeDrop
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPagesViewModel vm)
        {
            InitializeComponent();
            BindingContext= vm;
        }

    }

}
