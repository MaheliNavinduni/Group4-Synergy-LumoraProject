using LumoraAcademy.Data;
using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Admin;

public partial class PrintStudentReportPage : ContentPage
{
    public PrintStudentReportPage()
    {
        InitializeComponent();

        BindableLayout.SetItemsSource(RowList, SampleData.ReportRows);
    }

    private async void OnGoBackClicked(object sender, EventArgs e)
    {
        await AppNavigation.GoBackAsync();
    }

    private async void OnPrintClicked(object sender, EventArgs e)
    {
        // TODO: generate a PDF of this report and send it to the printer.
        await DisplayAlert("Print Report", "Printing is not connected yet. This will export the report as a PDF.", "OK");
    }
}
