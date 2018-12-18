using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Com.OneSignal;
using Com.OneSignal.Abstractions;
using System.Net.Http;
using System.Net;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Amazon;

namespace TesteOneSignal
{
    public partial class MainPage : ContentPage
    {
        // JWT Exemplo de Token
        //https://www.codeproject.com/Tips/1208535/Create-and-Consume-JWT-Tokens-in-Csharp


        string awsCredentials;
        string awsRegion;

        public MainPage()
        {
            InitializeComponent();

            awsCredentials = "";
            awsRegion = "";

            timePicker.Time = DateTime.Now.TimeOfDay;

            if (App.Current.Properties.ContainsKey("userID"))
                lblIdUser.Text = App.Current.Properties["userID"].ToString();
        }

        private void btClicado(object sender, EventArgs e)
        {
            if ((dataPicker.Date >= DateTime.Now.Date) && (timePicker.Time > DateTime.Now.TimeOfDay))
                EnviarNotificacao();
            else
                DisplayAlert("AVISO", "Data e horário do passado, altere para data e horário futuro!", "OK");
        }

        private void EnviarNotificacao()
        {
            string dataAgendamento = dataPicker.Date.ToString("yyyy-MM-dd") + " " + timePicker.Time.ToString("hh\\:mm\\:ss") + dataPicker.Date.ToString("zzz");
            dataAgendamento = Convert.ToDateTime(dataAgendamento).ToUniversalTime().ToString();

            var notification = new Dictionary<string, object>
            {
                ["large_icon"] = "ic_stat_onesignal_default.png",
                ["android_background_layout"] = new Dictionary<string, string>() { { "image", "onesignal_bgimage_default_image.jpg" }, { "headings_color", "FFFF0000" }, { "contents_color", "FF0000FF" } },
                ["subtitle"] = new Dictionary<string, string>() { { "en", "Titulo para ios 10+" } },
                ["headings"] = new Dictionary<string, string>() { { "en", "Titulo fica aqui" } },
                ["contents"] = new Dictionary<string, string>() { { "en", txtNotificacao.Text } },
                ["include_player_ids"] = new List<string>() { lblIdUser.Text }, // Use your own or get it the devices by calling OneSignal.GetIdsAvailable
                ["send_after"] = dataAgendamento
            };

            var resultado = OneSignal.Current.PostNotificationAsync(notification).GetAwaiter().GetResult();

            if (resultado.Success)
                DisplayAlert("AVISO", "Notificação gerada com sucesso!", "OK");
            else
                DisplayAlert("ERRO", "Erro ao gerar notificação!", "OK");
        }

        /*----------------------------------- CRUD -----------------------------------------------*/
        // https://docs.aws.amazon.com/pt_br/mobile/sdkforxamarin/developerguide/dynamodb-integration-lowlevelapi.html

        private async void SalvarItem()
        {
            // Create a client
            AmazonDynamoDBClient client = new AmazonDynamoDBClient(awsCredentials, awsRegion);

            // Define item attributes
            Dictionary<string, AttributeValue> attributes = new Dictionary<string, AttributeValue>
            {
                // Author is hash-key
                ["Author"] = new AttributeValue { S = "Mark Twain" },
                ["Title"] = new AttributeValue { S = "The Adventures of Tom Sawyer" },
                ["PageCount"] = new AttributeValue { N = "275" },
                ["Price"] = new AttributeValue { N = "10.00" },
                ["Id"] = new AttributeValue { N = "10" },
                ["ISBN"] = new AttributeValue { S = "111-1111111" }
            };

            // Create PutItem request
            PutItemRequest request = new PutItemRequest
            {
                TableName = "Books",
                Item = attributes
            };

            // Issue PutItem request
            var response = await client.PutItemAsync(request);
        }
        
        private async void UpdateItem()
        {
            AmazonDynamoDBClient client = new AmazonDynamoDBClient(awsCredentials, awsRegion);

            Dictionary<string, AttributeValue> key = new Dictionary<string, AttributeValue>
            {
                { "Id", new AttributeValue { N = "10" } }
            };

            // Define attribute updates
            Dictionary<string, AttributeValueUpdate> updates = new Dictionary<string, AttributeValueUpdate>();
            // Add a new string to the item's Genres SS attribute
            updates["Genres"] = new AttributeValueUpdate()
            {
                Action = AttributeAction.ADD,
                Value = new AttributeValue { SS = new List<string> { "Bildungsroman" } }
            };

            UpdateItemRequest request = new UpdateItemRequest
            {
                TableName = "Books",
                Key = key,
                AttributeUpdates = updates
            };

            var response = await client.UpdateItemAsync(request);
        }

        private async void DeleteItem()
        {
            AmazonDynamoDBClient client = new AmazonDynamoDBClient(awsCredentials, awsRegion);

            Dictionary<string, AttributeValue> key = new Dictionary<string, AttributeValue>
            {
                { "Id", new AttributeValue { N = "10" } }
            };

            DeleteItemRequest request = new DeleteItemRequest
            {
                TableName = "Books",
                Key = key
            };

            var response = await client.DeleteItemAsync(request);
        }

        private async void GetItem()
        {
            AmazonDynamoDBClient client = new AmazonDynamoDBClient(awsCredentials, awsRegion);

            Dictionary<string, AttributeValue> key = new Dictionary<string, AttributeValue>
            {
                { "Id", new AttributeValue { N = "10" } }
            };

            GetItemRequest request = new GetItemRequest
            {
                TableName = "Books",
                Key = key,
            };

            var result = await client.GetItemAsync(request);

            Console.WriteLine("Item:");
            Dictionary<string, AttributeValue> item = result.Item;
            foreach (var keyValuePair in item)
            {
                Console.WriteLine("Author := {0}", item["Author"]);
                Console.WriteLine("Title := {0}", item["Title"]);
                Console.WriteLine("Price:= {0}", item["Price"]);
                Console.WriteLine("PageCount := {0}", item["PageCount"]);
            }
        }

        private async void GetItemBy(string campo)
        {
            using (var client = new AmazonDynamoDBClient(awsCredentials, awsRegion))
            {
                var queryResponse = await client.QueryAsync(new QueryRequest()
                {
                    TableName = "Books",
                    IndexName = "Author-Title-index",
                    KeyConditionExpression = "Author = :v_Id",
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                        {
                          ":v_Id", new AttributeValue { S = campo }
                        }
                    }
                });
                queryResponse.Items.ForEach((i) => { Console.WriteLine(i["Title"].S); });
            }
        }

        public async void GetAllFromTable(string tabela)
        {
            using (var client = new AmazonDynamoDBClient(awsCredentials, awsRegion))
            {
                var queryResponse = await client.ScanAsync(new ScanRequest()
                {
                    TableName = tabela
                });
                queryResponse.Items.ForEach((i) => { Console.WriteLine(i["Title"].S); });
            }
        }

    }
}
