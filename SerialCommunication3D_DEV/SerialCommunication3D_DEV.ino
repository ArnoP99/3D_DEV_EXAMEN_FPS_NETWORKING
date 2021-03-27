/*
  18/09/2018
  Wim Van Weyenberg
  SerialIO
  Als een 'A' ontvangen wordt gaat de LED op PIN13 aan en als een 'U' ontvangen wordt gaat de LED op PIN13 UIT
  Drukknop op PIN8 indien ingedrukt wordt een 'T' gestuurd

*/

char serialInputChar;
long timer;
void setup()
{
  Serial.begin(9600);
  pinMode(9, INPUT_PULLUP);
  pinMode(11, OUTPUT);
  pinMode(12, OUTPUT);
  pinMode(13, OUTPUT);
  timer = millis();
}

void loop()
{
  if (Serial.available())
  {
    serialInputChar = Serial.read();
    if (serialInputChar == 'X')
    {
      digitalWrite(11, LOW);
      digitalWrite(12, LOW);
      digitalWrite(13, HIGH);
    } else if (serialInputChar == 'Y')
    {
      digitalWrite(11, LOW);
      digitalWrite(12, HIGH);
      digitalWrite(13, HIGH);
    }
    else if (serialInputChar == 'Z')
    {
      digitalWrite(11, HIGH);
      digitalWrite(12, HIGH);
      digitalWrite(13, HIGH);
    } else if (serialInputChar == 'W')
    {
      digitalWrite(11, LOW);
      digitalWrite(12, LOW);
      digitalWrite(13, LOW);
    }
  }
  if (digitalRead(9) == LOW)
  {
    Serial.write('F');
    delay(50); //debounce
    while (digitalRead(9) == LOW);
    delay(50); //debounce
  }
  if (millis() - timer > 500) {
    int potval = map(analogRead(A4), 0, 1023, 0, 5);
    switch (potval) {
      case 0:
        Serial.write('A');
        break;
      case 1:
        Serial.write('B');
        break;
      case 2:
        Serial.write('C');
        break;
      case 3:
        Serial.write('D');
        break;
      case 4:
        Serial.write('E');
        break;
    }
    timer = millis();
  }

}
