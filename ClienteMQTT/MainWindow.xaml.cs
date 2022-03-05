using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using OpenNETCF.MQTT;

namespace ClienteMQTT
{
    /// <summary>
    /// Servidor: isc-server.ddns.net
    /// Servidor: 200.79.179.167
    /// Puerto: 1883
    /// Cliente: ednaGarces
    /// </summary>
    public partial class MainWindow : Window
    {
        MQTTClient mqtt;
        DispatcherTimer timer;
        string Mensaje = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnConectar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txbServidor.Text))
            {
                MessageBox.Show("No se ha ingresado el servidor","Error",MessageBoxButton.OK,MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrEmpty(txbPuerto.Text))
            {
                MessageBox.Show("No se ha ingresado el puerto", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            int puerto = 0;
            if (!int.TryParse(txbPuerto.Text, out puerto))
            {
                MessageBox.Show("El puerto ingresado no es un número", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            mqtt = new MQTTClient(txbServidor.Text, puerto);
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 200);
            timer.Tick += Timer_Tick;
            timer.Start();
            mqtt.Connect(txbCliente.Text);
            mqtt.Connected += Mqtt_Connected;
            mqtt.MessageReceived += Mqtt_MessageReceived;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (mqtt.IsConnected)
            {
                stkSuscripcion.IsEnabled = true;
                stkMensaje.IsEnabled = true;
                if (Mensaje != "")
                {
                    lstMensajes.Items.Add(Mensaje);
                    Mensaje = "";
                }
            }
        }

        private void Mqtt_MessageReceived(string topic, QoS qos, byte[] payload)
        {
            Mensaje= $"[{topic}]: {Encoding.UTF8.GetString(payload)}";
        }

        private void Mqtt_Connected(object sender, EventArgs e)
        {
            MessageBox.Show("Se ha conectado de forma exitosa", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            
        }

        private void btnSuscribir_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txbTopico.Text))
            {
                MessageBox.Show("No se ha ingresado nada en el tópico", "Error",MessageBoxButton.OK,MessageBoxImage.Error);
                return;
            }
            if (mqtt.IsConnected)
            {
                mqtt.Subscriptions.Add(new Subscription(txbTopico.Text));
                MessageBox.Show($"Se ha suscrito al tópico {txbTopico.Text}", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show($"No esta suscrito al tópico {txbTopico.Text}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnEnviar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txbTopico.Text))
            {
                MessageBox.Show("No se ha ingresado nada en el tópico", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrEmpty(txbMensaje.Text))
            {
                MessageBox.Show("No se ha ingresado nada en el mensaje", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (mqtt.IsConnected)
            {
                mqtt.Publish(txbTopico.Text, Encoding.UTF8.GetBytes(txbMensaje.Text), QoS.FireAndForget, false);
                MessageBox.Show($"Se ha suscrito al tópico {txbTopico.Text}", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("El servicio de MQTT no esta conectado...", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void lstMensajes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
