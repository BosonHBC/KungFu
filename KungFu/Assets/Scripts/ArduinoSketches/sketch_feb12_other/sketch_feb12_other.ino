/* FSR simple testing sketch.  <br>Connect one end of FSR to power, the other end to Analog 0.
Then connect one end of a 10K resistor from Analog 0 to ground 
*/
const int button01 = 0; // the FSR and 10K pulldown are connected to a0
const int button02 = 1;
const int button03 = 2;
const int button04 = 3;
const int button05 = 4;
const int button06 = 5;

const int ThreshHold = 625;
 
void setup(void) {
  Serial.begin(9600);   
}
 
void loop(void) {
    String myString = ""; 
    
    // the analog reading from the FSR resistor divider
  if (analogRead(button01) >= ThreshHold) {  
    myString += "1";
  }
  else
  {
    myString += "0";
  }

  if (analogRead(button02) >= ThreshHold) {  
    myString += "1";
  }
  else
  {
    myString += "0";
  }

  if (analogRead(button03) >= ThreshHold) {  
    myString += "1";
  }
  else
  {
    myString += "0";
  }

  if (analogRead(button04) >= ThreshHold) {  
    myString += "1";
  }
  else
  {
    myString += "0";
  }

  if (analogRead(button05) >= ThreshHold) {  
    myString += "1";
  }
  else
  {
    myString += "0";
  }

   if (analogRead(button06) >= ThreshHold) {  
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
