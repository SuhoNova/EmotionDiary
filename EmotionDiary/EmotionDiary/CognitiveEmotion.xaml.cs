using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EmotionDiary
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
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

	        file.Dispose();
        }
	}
}