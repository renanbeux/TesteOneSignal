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

namespace TesteOneSignal
{
    public partial class MainPage : ContentPage
    {  
        public MainPage()
        {
            InitializeComponent();
            timePicker.Time = DateTime.Now.TimeOfDay;
            
            if (App.Current.Properties.ContainsKey("userID"))
                lblIdUser.Text = App.Current.Properties["userID"].ToString();
        }

        private void btClicado(object sender, EventArgs e)
        {
            if((dataPicker.Date >= DateTime.Now.Date) && (timePicker.Time > DateTime.Now.TimeOfDay))
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

            if(resultado.Success)
                DisplayAlert("AVISO", "Notificação gerada com sucesso!", "OK");
            else
                DisplayAlert("ERRO", "Erro ao gerar notificação!", "OK");            
        }        
    }
}
