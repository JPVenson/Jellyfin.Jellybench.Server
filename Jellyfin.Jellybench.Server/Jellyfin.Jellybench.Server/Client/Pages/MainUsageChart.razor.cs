using Blazorise;
using Blazorise.Charts;
using Jellyfin.Jellybench.Server.Client.Api;
using Microsoft.AspNetCore.Components;

namespace Jellyfin.Jellybench.Server.Client.Pages
{
    public partial class MainUsageChart
    {
        [Inject]
        public JellybenchApiClient JellybenchApiClient { get; set; }

        private Chart<int> _chartElement;
        private Int32ChartDataSeries _int32ChartDataSeries;

        protected override async Task OnInitializedAsync()
        {
            _int32ChartDataSeries = await JellybenchApiClient.GetCpuChartValuesAsync();
            await RedrawChart();
        }

        private async Task RedrawChart()
        {
            await _chartElement.Clear();
            await _chartElement.AddLabelsDatasetsAndUpdate(_int32ChartDataSeries.Labels.ToArray(),
                new BarChartDataset<int>()
                {
                    Label = "Cpu Streams",
                    Data = _int32ChartDataSeries.Values.ToList(),
                });
        }

        //async Task HandleRedraw()
        //{
        //    await _chartElement.Clear();
        //    await _chartElement.AddLabelsDatasetsAndUpdate(Labels, GetBarChartDataset());
        //}

        //private BarChartDataset<double> GetBarChartDataset()
        //{
        //    return new()
        //    {
        //        Label = "# of randoms",
        //        Data = RandomizeData(),
        //        BackgroundColor = backgroundColors,
        //        BorderColor = borderColors,
        //        BorderWidth = 1
        //    };
        //}
    }
}
