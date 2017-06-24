using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using EmotionDiary.Model;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Geolocator;
using Xamarin.Forms;
using Xamarin.Forms.Maps;


namespace EmotionDiary
{
    public partial class CognitiveEmotion : ContentPage
	{
		public CognitiveEmotion ()
		{
			InitializeComponent ();
		}

	    private async void loadCamera(object sender, EventArgs e)
	    {

            // Check if Camera is available
	        await CrossMedia.Current.Initialize();

	        if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
	        {
	            await DisplayAlert("No Camera", ":( No camera available.", "OK");
	            return;
	        }

            // Check if gps is on 
	        var locator = CrossGeolocator.Current;
	        if (!locator.IsGeolocationEnabled)
	        {
	            await DisplayAlert("No Location", "GPS is needed for this to work", "OK");
	            return;
	        }

            // Take a photo
            MediaFile file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
	        {
	            PhotoSize = PhotoSize.Medium,
	            Directory = "Sample",
	            Name = $"{DateTime.UtcNow}.jpg"
	        });

            // Diplay the image of the photo
	        if (file == null)
	            return;

	        image.Source = ImageSource.FromStream(() =>
	        {
	            return file.GetStream();
	        });

            EmotionLabel.Text = "Analyzing selfie...";

            await EvaluateEmotionRequest(file);
	    }

        async Task PostEmotionLocationAsync(string emotionResult, double emotionScore)
        {
            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 50;

            var position = await locator.GetPositionAsync(10000);

            EmotionDiaryModel model = new EmotionDiaryModel()
            {
                EmotionResult = emotionResult,
                EmotionScore = emotionScore,
                Longitude = (float)position.Longitude,
                Latitude = (float)position.Latitude,
                Date = DateTime.UtcNow
            };

            //Geocoder geoCoder = new Geocoder();

            //var positionForGeocoder = new Position(position.Latitude, position.Longitude);
            //var possibleAddresses = await geoCoder.GetAddressesForPositionAsync(positionForGeocoder);
            //var address = "";
            //foreach (var tempAddress in possibleAddresses)
            //    address = tempAddress;

            EmotionLabel.Text = "You feel " + emotionResult + " with a score of " + emotionScore ; //address;

            await AzureManager.AzureManagerInstance.PostHotDogInformation(model);
        }

        static byte[] GetImageAsByteArray(MediaFile file)
	    {
	        var stream = file.GetStream();
	        BinaryReader binaryReader = new BinaryReader(stream);
	        return binaryReader.ReadBytes((int)stream.Length);
	    }

	    async Task EvaluateEmotionRequest(MediaFile file)
	    {
	        var client = new HttpClient();

	        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "1c36e4946d994473b89f5edea07f11f6");

	        string url = "https://westus.api.cognitive.microsoft.com/emotion/v1.0/recognize?";

	        HttpResponseMessage response;

            byte[] byteData = GetImageAsByteArray(file);

	        using (var content = new ByteArrayContent(byteData))
	        {

	            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
	            response = await client.PostAsync(url, content);


	            if (response.IsSuccessStatusCode)
	            {
	                var responseString = await response.Content.ReadAsStringAsync();
                    var emotions = JsonConvert.DeserializeObject<RootObject[]>(responseString);

	                double emotionScore = emotions[0].scores.Values.Max(m => m);
                    // Comparing float numbers using ==  is bad due to possible precision loss
                    var emotionResult = emotions[0].scores.FirstOrDefault(s => Math.Abs(s.Value - emotionScore) < 0.001).Key;
                    //EmotionLabel.Text = "You feel " + emotionResult + "";

                    await PostEmotionLocationAsync(emotionResult, emotionScore);
                }
	            else
	            {
	                await DisplayAlert("Failed", "Couldn't reach the API! Wait or complain to the developer.", "OK");
                }

	        }
	        //Get rid of file once we have finished using it
            file.Dispose();
        }


    }
}