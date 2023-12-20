using Microsoft.Windows.System.Power;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Web.Helpers;

namespace Earthquake_Project
{
    public partial class MainPage : ContentPage
    {
        

        public MainPage()
        {
            InitializeComponent();

            
        }

        // Event handler for the Find button click
        private async void BtnFind_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Use HttpClient to make a request to the USGS earthquake API
                using (HttpClient client = new HttpClient()) 
                {
                    // Construct the URL based on user input
                    string url = $"https://earthquake.usgs.gov/fdsnws/event/1/query?format=geojson&starttime={LblStartDate.Text}&endtime={LblEndDate.Text}&minmagnitude={LblQuakeSize.Text}";
                    HttpResponseMessage response = await client.GetAsync(url);

                    // Check if the API request was successful
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the API response content
                        string content = await response.Content.ReadAsStringAsync();

                        // Parse JSON response
                        JObject jo = JObject.Parse(content);
                        JObject main = JObject.Parse(jo["metadata"].ToString());


                        // Update the global variable NumEarthQuake
                        EarthquakeGV.NumEarthQuake = int.Parse(main["count"].ToString());

                        // Display the number of earthquakes
                        LblEarthquakeResults.Text = $"There were {EarthquakeGV.NumEarthQuake.ToString()} earthquakes during this time.";

                        // Parse earthquake features
                        JArray quakefeature = JArray.Parse(jo["features"].ToString());
                        List<Earthquakes> eqList = new List<Earthquakes>();
                        int idx = 1;

                        // loop through each earthquake feature
                        foreach (var eq in quakefeature) 
                        {
                            {
                                // Parse properties of each earthquake
                                JObject eqJObj = JObject.Parse(eq["properties"].ToString());
                                Earthquakes earthquake = new Earthquakes
                                {
                                    // Extract magnitude and place information
                                    Magnitude = double.Parse(eqJObj["mag"].ToString()),
                                    Place = eqJObj["place"].ToString()
                                };
                                eqList.Add(earthquake);
                                idx++;
                            }

                        }
                        // Display details of a random earthquake
                        Random random = new Random();
                        int randeq = random.Next(1, eqList.Count + 1);
                        Earthquakes displayEQ = eqList[randeq];
                        LblDetails.Text = $"Details of one of them: Place: {displayEQ.Place}, Magnitude: {displayEQ.Magnitude}.";


                    }

                }

            }
            catch (Exception ex)
            {
                // Handle exceptions by displaying an alert
                await DisplayAlert("API Alert",ex.Message,"Close");
            }

        }
    }

}
