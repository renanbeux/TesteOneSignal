using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Com.OneSignal;
using Com.OneSignal.Abstractions;

namespace TesteOneSignal
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            if (App.Current.Properties.ContainsKey("userID"))
                lblIdUser.Text = App.Current.Properties["userID"].ToString();
        }

        private void btClicado(object sender, EventArgs e)
        {
            EnviarNotificacao();
        }

        private static string oneSignalDebugMessage;

        private void EnviarNotificacao()
        {
            var notification = new Dictionary<string, object>();
            notification["small_icon"] = "wind.png";
            notification["subtitle"] = new Dictionary<string, string>() { { "en", "Titulo para ios 10+" } };
            notification["headings"] = new Dictionary<string, string>() { { "en", "Titulo fica aqui" } };
            notification["contents"] = new Dictionary<string, string>() { { "en", txtNotificacao.Text } };
            notification["android_background_layout"] = new Dictionary<string, string>() { { "image", "bgImage.jpg" }, { "headings_color", "FFFF0000" }, { "contents_color", "FF0000FF" } };            

            // Just an example userId, use your own or get it the devices by calling OneSignal.GetIdsAvailable
            notification["include_player_ids"] = new List<string>() { lblIdUser.Text };

            // Example of scheduling a notification in the future.
            notification["send_after"] = System.DateTime.Now.ToUniversalTime().AddSeconds(10).ToString("U");

            OneSignal.Current.PostNotification(notification, (responseSuccess) => {
                oneSignalDebugMessage = "Notification posted successful! Funcionou!.\n" + Json.Serialize(responseSuccess);
            }, (responseFailure) => {
                oneSignalDebugMessage = "Notification failed to post: ERRO!\n" + Json.Serialize(responseFailure);
            });

            lblAviso.Text = oneSignalDebugMessage;
        }
    }
}
