#include <WiFiS3.h>
#include <PubSubClient.h>
#include "analogWave.h"

// ---------- WIFI ----------
const char ssid[] = "dsv-extrality-lab";
const char pass[] = "expiring-unstuck-slider";

// ---------- MQTT ----------
const char broker[] = "inf-liva-server-eth.dsv.local.su.se";
const int port = 1883;
const char mqttUser[] = "esp32";
const char mqttPass[] = "extrality";

// ---------- CLIENTS ----------
WiFiClient wifiClient;
PubSubClient mqttClient(wifiClient);

// ---------- DEVICE ID & TOPICS ----------
String deviceID = "";
String topicLED;
String topicMusic;

// ---------- LED & MUSIC ----------
#define LED_PIN 9
analogWave wave(DAC);

bool musicState = false;
int melody[] = {3000,3200,3400,3800,3400,3200,3000,2600,
                3000,3400,4000,3800,3400,3000,2600,2400};
int melodyLength = 16;
int noteIndex = 0;
unsigned long previousNoteMillis = 0;
unsigned long noteDuration = 120;

// ---------- GET MAC ----------
String getDeviceID() {
  byte mac[6];
  WiFi.macAddress(mac);
  char macStr[13];
  sprintf(macStr,"%02X%02X%02X%02X%02X%02X",
          mac[0],mac[1],mac[2],mac[3],mac[4],mac[5]);
  String macString = String(macStr);
  String last4 = macString.substring(macString.length()-4);
  last4.toLowerCase();
  Serial.print("Device ID: ");
  Serial.println(last4);
  return last4;
}

// ---------- MQTT CALLBACK ----------
void mqttCallback(char* topic, byte* payload, unsigned int length) {
  String message;
  for(unsigned int i=0; i<length; i++){
    message += (char)payload[i];
  }
  message.trim();
  message.toLowerCase();

  Serial.print("MQTT message: "); Serial.println(message);

  if(message == "true" || message == "led_on"){
    digitalWrite(LED_PIN,HIGH);
  }
  else if(message == "false" || message == "led_off"){
    digitalWrite(LED_PIN,LOW);
  }
  else if(message == "music_on"){
    musicState = true;
  }
  else if(message == "music_off"){
    musicState = false;
    wave.stop();
  }
  else{
    Serial.println("Unknown command");
  }
}

// ---------- CONNECT TO WIFI ----------
void connectWiFi() {
WiFi.begin(ssid, pass);
Serial.print("Connecting to WiFi");
while (WiFi.status() != WL_CONNECTED || WiFi.localIP() == IPAddress(0,0,0,0)) {
  Serial.print(".");
  delay(1000);
}
Serial.println();
Serial.print("Connected! IP: ");
Serial.println(WiFi.localIP());;
}

// ---------- CONNECT TO MQTT ----------
void connectMQTT() {
  mqttClient.setServer(broker, port);
  mqttClient.setCallback(mqttCallback);

  Serial.print("Connecting to MQTT broker...");
  while(!mqttClient.connected()){
    String clientID = "Arduino-" + deviceID;
    if(mqttClient.connect(clientID.c_str(), mqttUser, mqttPass)){
      Serial.println("Connected to MQTT broker!");
      mqttClient.subscribe(topicLED.c_str());
    } else {
      Serial.print(".");
      delay(1000);
    }
  }
}

// ---------- SETUP ----------
void setup() {
  Serial.begin(115200);
  delay(1000);

  pinMode(LED_PIN, OUTPUT);
  digitalWrite(LED_PIN,LOW);
  wave.sine(10);

  // WiFi
  connectWiFi();

  // Device ID och topics
  deviceID = getDeviceID();
  topicLED = deviceID + "/D13_led";
  topicMusic = deviceID + "/music";

  // MQTT
  connectMQTT();
}

// ---------- LOOP ----------
void loop() {
  if(!mqttClient.connected()){
    connectMQTT();
  }
  mqttClient.loop();

  // Musikspelning
  if(musicState){
    unsigned long currentMillis = millis();
    if(currentMillis - previousNoteMillis >= noteDuration){
      wave.freq(melody[noteIndex]);
      noteIndex++;
      noteIndex %= melodyLength;
      previousNoteMillis = currentMillis;
    }
  }

  delay(10);
}

/*Kod för att få fram Arduinons MAC address
#include <WiFiS3.h>

void setup() {
  Serial.begin(115200);
  delay(2000);

  uint8_t mac[6];
  WiFi.macAddress(mac);

  Serial.print("MAC Address: ");
  for (int i = 0; i < 6; i++) {
    if (mac[i] < 16) Serial.print("0");
    Serial.print(mac[i], HEX);
    if (i < 5) Serial.print(":");
  }
  Serial.println();
}

void loop() {}
*/ 
