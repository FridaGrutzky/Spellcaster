using uPLibrary.Networking.M2Mqtt.Messages;
using M2MqttUnity;
using UnityEngine;

namespace ExtralityLab
{
    public class MqttClientExampleSendRGB : M2MqttUnityClient
    {
        [Header("Topic")]
        public string publishTopicName = "myUnityApp/analogRGB";

        private bool ledIsOn = false;
        private bool musicIsOn = false;

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();
        }

        protected override void OnConnecting()
        {
            base.OnConnecting();
            Debug.Log($"MQTT connecting to {brokerAddress}:{brokerPort}");
        }

        protected override void OnConnected()
        {
            base.OnConnected();
            Debug.Log("MQTT connected");
        }

        private void OnDestroy()
        {
            Disconnect();
        }

        // Lightning spell → togglar lampan
        public void TriggerLightningSpell()
        {
            if (client == null)
            {
                Debug.LogWarning("MQTT client not ready yet");
                return;
            }

            ledIsOn = !ledIsOn;
            string message = ledIsOn ? "led_on" : "led_off";

            client.Publish(
                publishTopicName,
                System.Text.Encoding.UTF8.GetBytes(message),
                MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE,
                false
            );

            Debug.Log($"MQTT sent -> Topic: {publishTopicName} Message: {message}");
        }

        // Circle spell → togglar musik
        public void TriggerCircleSpell()
        {
            if (client == null)
            {
                Debug.LogWarning("MQTT client not ready yet");
                return;
            }

            musicIsOn = !musicIsOn;
            string message = musicIsOn ? "music_on" : "music_off";

            client.Publish(
                publishTopicName,
                System.Text.Encoding.UTF8.GetBytes(message),
                MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE,
                false
            );

            Debug.Log($"MQTT sent -> Topic: {publishTopicName} Message: {message}");
        }
    }
}