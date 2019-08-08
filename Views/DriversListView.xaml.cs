using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace SCCRaceMode.Views
{
    public class DriversListView : UserControl
    {
        public DriversListView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}