const int button01 = 2;
const int button02 = 3;
const int button03 = 4;
const int button04 = 5;
const int button05 = 6;
const int button06 = 7;
const int button07 = 8;
const int button08 = 9;
const int button09 = 10;
const int button10 = 11;
const int button11 = 12;


void setup() {
  // put your setup code here, to run once:
Serial.begin(9600);
pinMode(button01, INPUT);
pinMode(button02, INPUT);
pinMode(button03, INPUT);
pinMode(button04, INPUT);
pinMode(button05, INPUT);
pinMode(button06, INPUT);
pinMode(button07, INPUT);
pinMode(button08, INPUT);
pinMode(button09, INPUT);
pinMode(button10, INPUT);
pinMode(button11, INPUT);

digitalWrite(button01, HIGH);
digitalWrite(button02, HIGH);
digitalWrite(button03, HIGH);
digitalWrite(button04, HIGH);
digitalWrite(button05, HIGH);
digitalWrite(button06, HIGH);
digitalWrite(button07, HIGH);
digitalWrite(button08, HIGH);
digitalWrite(button09, HIGH);
digitalWrite(button10, HIGH);
digitalWrite(button11, HIGH);
}

void loop() {
  // put your main code here, to run repeatedly:

  String myString = "";
  if(digitalRead(button01) == LOW)
  {
    myString += "1";
  }
  else
  {
    myString += "0";
  }

  if(digitalRead(button02) == LOW)
 {
    myString += "1";
  }
  else
  {
    myString += "0";
  }

  if(digitalRead(button03) == LOW)
  {
    myString += "1";
  }
  else
  {
    myString += "0";
  }

  if(digitalRead(button04) == LOW)
  {
    myString += "1";
  }
  else
  {
    myString += "0";
  }

  if(digitalRead(button05) == LOW)
  {
    myString += "1";
  }
  else
  {
    myString += "0";
  }
  
  if(digitalRead(button06) == LOW)
 {
    myString += "1";
  }
  else
  {
    myString += "0";
  }
  
  if(digitalRead(button07) == LOW)
 {
    myString += "1";
  }
  else
  {
    myString += "0";
  }

  if(digitalRead(button08) == LOW)
  {
    myString += "1";
  }
  else
  {
    myString += "0";
  }

  if(digitalRead(button09) == LOW)
  {
    myString += "1";
  }
  else
  {
    myString += "0";
  }

  if(digitalRead(button10) == LOW)
  {
    myString += "1";
  }
  else
  {
    myString += "0";
  }

  if(digitalRead(button11) == LOW)
 {
    myString += "1";
  }
  else
  {
    myString += "0";
  }
    Serial.println(myString);
    Serial.flush();
    delay(5);
}
