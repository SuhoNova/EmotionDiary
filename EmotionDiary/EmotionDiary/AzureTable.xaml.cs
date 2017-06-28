using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmotionDiary.Model;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.Maps;

namespace EmotionDiary
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AzureTable : ContentPage
	{
	    MobileServiceClient client = AzureManager.AzureManagerInstance.AzureClient;

        public AzureTable ()
		{
			InitializeComponent ();
        }
	    async void Handle_ClickedAsync(object sender, System.EventArgs e)
	    {
	        List<EmotionDiaryModel> emotionDiaryHistory = await AzureManager.AzureManagerInstance.GetEmotionHistory();
            EmotionHistoryList.ItemsSource = emotionDiaryHistory;
	    }
    }
}