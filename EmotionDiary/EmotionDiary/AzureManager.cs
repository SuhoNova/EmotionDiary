using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EmotionDiary.Model;
using Microsoft.WindowsAzure.MobileServices;

namespace EmotionDiary
{
    public class AzureManager
    {
        private static AzureManager instance;
        private MobileServiceClient client;
        private IMobileServiceTable<EmotionDiaryModel> emotionTable;

        private AzureManager()
        {
            this.client = new MobileServiceClient("https://emotiondiary.azurewebsites.net");
            this.emotionTable = this.client.GetTable<EmotionDiaryModel>();
        }

        public MobileServiceClient AzureClient
        {
            get { return client; }
        }

        public static AzureManager AzureManagerInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AzureManager();
                }

                return instance;
            }
        }
        public async Task<List<EmotionDiaryModel>> GetEmotionHistory()
        {
            return await this.emotionTable.ToListAsync();
        }
        public async Task PostHotDogInformation(EmotionDiaryModel emotionDiaryModel)
        {
            await this.emotionTable.InsertAsync(emotionDiaryModel);
        }
    }
}
