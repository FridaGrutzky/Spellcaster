using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Text;

public class ArduinoMQTTKeyboardM2Mqtt : MonoBehaviour
{
    private MqttClient client;
    private string broker = "test.mosquitto.org";
    private string topic = "Unity/ArduinoSpellCasterControl";

    void Start()
    {
        client = new MqttClient(broker);
        client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
        string clientId = System.Guid.NewGuid().ToString();
        client.Connect(clientId);
        client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
        Debug.Log("Connected to MQTT broker");
    }

    private void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {
        string message = Encoding.UTF8.GetString(e.Message);
        Debug.Log("Received: " + message);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
            PublishMessage("led_on");
        if (Input.GetKeyDown(KeyCode.O))
            PublishMessage("led_off");
        if (Input.GetKeyDown(KeyCode.M))
            PublishMessage("music_on");
        if (Input.GetKeyDown(KeyCode.T))
            PublishMessage("music_off");
    }

    private void PublishMessage(string msg)
    {
        if (client != null && client.IsConnected)
        {
            client.Publish(topic, Encoding.UTF8.GetBytes(msg));
            Debug.Log("Sent: " + msg);
        }
    }

    void OnApplicationQuit()
    {
        if (client != null && client.IsConnected)
            client.Disconnect();
    }
}
