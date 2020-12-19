#include <Arduino.h>
#include <OSCArduino.h>
#include <OSCMessageConsumer.h>
#include <OSCStructMessage.h>
#include <IMessage.h>
#include <ESP8266WiFi.h>
#include <WiFiUdp.h>

#define FULLPOWER 0x1
#define FULLSTOP 0x0

struct ControlMessage {
  uint32_t speed;
};

class OSCControl : public OSC::MessageConsumer
{
private:
  OSC::StructMessage<ControlMessage, uint32_t> _message;
public:
  const char *address()
  {
    return TRAIN_NAME;
  }

  OSC::IMessage *message() {
    return &_message;
  }

  void callbackMessage() {
    Serial.println("Message received.");
    
    analogWrite(5, _message.messageStruct.speed);
  }
};

WiFiUDP udp;
OSC::Arduino<1, 0> osc;

OSCControl control;

void setup()
{
  pinMode(5, OUTPUT);

  Serial.begin(9600);
  Serial.println("Hi!");
  
  WiFi.begin(WIFI_SSID, WIFI_PWD);

  bool ledStatus = false;
  while (WiFi.status() != WL_CONNECTED)
  {
    // Blink the LED
    digitalWrite(5, ledStatus); // Write LED high/low
    ledStatus = (ledStatus == HIGH) ? LOW : HIGH;
    delay(100);
  }

  digitalWrite(5, FULLSTOP);

  Serial.println("WiFi connected");  
  Serial.println("IP address: ");
  Serial.println(WiFi.localIP());

  udp.begin(15670);

  osc.addConsumer(&control);
  osc.bindUDP(&udp, IPAddress(192, 168, 2, 10), 15670);
}

void loop()
{
  osc.loop();

  yield();
}