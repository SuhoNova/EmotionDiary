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
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


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

            EmotionLabel.Text = "";

            await EvaluateEmotionRequest(file);
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

                    var emotions = JsonConvert.DeserializeObject<RootObject>(responseString);

                    //double max = responseModel.Predictions.Max(m => m.Probability);
	                var scores = emotions.scores;
	                var highestScore = scores.Values.Max(s => s);
	                var highestEmotion = scores.FirstOrDefault(s => s.Value == highestScore).Key;

                    //TagLabel.Text = (max >= 0.5) ? "Hotdog" : "Not hotdog";
	                EmotionLabel.Text = "You feel " + highestEmotion;

                }

	            //Get rid of file once we have finished using it
	            file.Dispose();
	        }
	    }


    }
}