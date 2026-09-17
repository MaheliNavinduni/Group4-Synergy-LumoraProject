using LumoraAcademy.Data;

namespace LumoraAcademy.Controls;

public partial class AttendanceTableView : ContentView
{
    public AttendanceTableView()
    {
        InitializeComponent();

        BindableLayout.SetItemsSource(RowList, SampleData.AttendanceRecords);
    }
}
