using uPLibrary.Networking.M2Mqtt.Messages;
using M2MqttUnity;
using UnityEngine;

namespace ExtralityLab
{
    public class MqttClientExampleSendRGB : M2MqttUnityClient
    {
        [Header("Topic")]
        public string publishTopicName = "myUnityApp/analogRGB";

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

        // Den här kommer SpellManager anropa
        public void TriggerLightningSpell()
        {
            if (client == null)
            {
                Debug.LogWarning("MQTT client not ready yet");
                return;
            }

            string message = "led_on";

            client.Publish(
                publishTopicName,
                System.Text.Encoding.UTF8.GetBytes(message),
                MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE,
                false
            );

            Debug.Log($"MQTT sent -> Topic: {publishTopicName}  Message: led_off");
        }
    }
}