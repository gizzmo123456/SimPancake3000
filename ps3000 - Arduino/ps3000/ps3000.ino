#include "MPU6050_tockn.h"
#include "ADXL345_Lib.h"
#include "Wire.h"

#define MUX_ADDR    0x70


// Debuging
bool DEBUG = false;                // forces print to console    // Also displays data normalized (if it been called)
const int DEBUG_INTERVALS = 200;   //millis
unsigned long DEBUG_LAST_INTERVAL = 0;

// Frying pan
MPU6050 MPU[3](Wire);  //TODO: needs to be array, futhermore tocknMPU will also need modifing to support mutiple devices

//Jug
ADXL345 jug_adxl(Wire);

//Hob ehisk togle
const int toggleSwitch_outputPin = 8;
const int toggleSwitch_inputPin  = 10;
const int hob_outputPin = 9;
const int whisk_outputPin = 11;
const int toggle_analogReadPin = A2;

bool pin9_isHigh = true;
// both share the same analog port so they are togled
int whiskValue = 0;
int hobValue = 0;   //Hob 2

// Whisk
const int whisking_switch_lowValue = 10;   // a value above this is considered high value
int whisking_lastWasLow = false;

const int whisking_valueChanged_interval = 250;             //ms
unsigned long whisking_nextInterval = 200;

bool whisking;

// Fire Alarm
const int fireAlarm_pin = 6;//A0;
unsigned long fireAlarm_length = 3350;

unsigned long fireAlarm_cookedPancake_length = 100;

unsigned long fireAlarm_endTime = 0;

// serial
int incomingByte = 0;
const int OUTPUT_BUFFER_SIZE = 5;

void PrintPaddedValue(int num)
{
  //define buffer and the final padded values
  char buff[ OUTPUT_BUFFER_SIZE ];
  char padded[ OUTPUT_BUFFER_SIZE + 1 ];
  
  bool neg = false; //remember if num is negitive

  if(num < 0)       // make shore that the number is positive since we are converting unsigned int to char
  {
    num = -num;
    neg = true;
  }
  
  sprintf(buff, "%.5u", num); //Convert num to chars

  // padd the buffer 
  for(int i = 0; i < OUTPUT_BUFFER_SIZE; i++)
    padded[i] = buff[i];

  padded[OUTPUT_BUFFER_SIZE] = '\0';
  if(neg) padded[0] = '-';  

  Serial.print( String(padded) ); // Output the padded value to serial console :)
  
  if (DEBUG)
    Serial.print("#");
}

void MUX_select(int8_t i2cBus)
{
  
  if (i2cBus > 7) return;
  Wire.beginTransmission(MUX_ADDR);
  Wire.write(1 << i2cBus);
  Wire.endTransmission(true);  
}

void setup() 
{
  //Setup Serial and Wire
  Wire.begin();
  Serial.begin(250000);//9600 just doesnt cut it if we want to maintaince somwhere around 30fps (anythink above 74880 seams a lil to fast)
  
  jug_adxl.begin();   //TODO this dont work if wire.begin is not called but if any MPU functions are called after Wire.begin is called serial does not work... hmmm..
  
  for(int i = 0; i < 3; i++)
  {
    MUX_select(i);
    MPU[i].begin();
  }

  // set up the fire alarm pins
  pinMode(fireAlarm_pin, OUTPUT);
  digitalWrite(fireAlarm_pin, LOW);

  // set up the toggle output pins
  pinMode(toggleSwitch_outputPin, OUTPUT);
  pinMode(toggleSwitch_inputPin, INPUT);
  pinMode(hob_outputPin, OUTPUT);
  pinMode(whisk_outputPin, OUTPUT);

  digitalWrite(toggleSwitch_outputPin, HIGH);
  digitalWrite(hob_outputPin, HIGH);
  digitalWrite(whisk_outputPin, LOW);
  
  //??
  pinMode(A4, INPUT);
  pinMode(13, INPUT);
  
}

void loop()
{
  MUX_select(0);
  MPU[0].update();
  MUX_select(1);
  MPU[1].update();
  MUX_select(2);
  MPU[2].update();
  
  jug_adxl.update();
  //UpdateHobWhiskToggle();
  UpdateWhisking();
  
  //Check that Serial is available and read any incoming bytes
  if(Serial.available() > 0 || (DEBUG && millis() > (DEBUG_LAST_INTERVAL + DEBUG_INTERVALS)))
  {
    incomingByte = Serial.read();

    if(incomingByte == 'd' || incomingByte == 'D')  //DEBUG INPUT, D == true
    {
      DEBUG =  incomingByte == 'D';
    }
    else if(incomingByte == 'N' || incomingByte == 'n' )        // Normalize Devices
    { 
      for(int i = 0; i < 3; i++)
        MPU[i].normalize();

      jug_adxl.Normalize();
    }
    else if( DEBUG || incomingByte == 'I' || incomingByte == 'i' )
    {
      printGroupValues();
    }
    
    if(incomingByte == 'f')
    {
      fireAlarm_endTime = millis() + fireAlarm_length;
      digitalWrite(fireAlarm_pin, HIGH);//fireAlarm_vol);
    }
    else if(incomingByte == 'e')
    {
      fireAlarm_endTime = millis() + fireAlarm_cookedPancake_length;
      digitalWrite(fireAlarm_pin, HIGH);
    }
        
    DEBUG_LAST_INTERVAL = millis();
    
  }
  
  if( fireAlarm_endTime > 0 && millis() > fireAlarm_endTime )
  {
    // Stop fire alarm
    //analogWrite(fireAlarm_pin, 0);
    digitalWrite(fireAlarm_pin, LOW);
    fireAlarm_endTime = 0;
  }
  
}

void printGroupValues()
{
    // Print Group Data
    // Hob[n] panX#panY#onStove#hobNob#...
    MUX_select(0);
    PrintPaddedValue( (MPU[0].getAngleX( true )) );
    PrintPaddedValue( (MPU[0].getAngleY( true )) );
    PrintPaddedValue( analogRead(A3) );   //LDR (27k ristor)    //TODO: need to be in array
    PrintPaddedValue( analogRead(A0) );  // Hob Nob             //TODO: needs to be in an array.

    //////// IMPORTENT ///////////////////////////
    // for some unknowen reason if i plug MPU[1] into pins 2 on the MUX none of my 3 MPU's show up
    // So iv just fliped them round in the code :)
    //////////////////////////////////////////////
    MUX_select(2);  
    PrintPaddedValue( (MPU[2].getAngleX( true )) );
    PrintPaddedValue( (MPU[2].getAngleY( true )) );
    PrintPaddedValue( analogRead(A4) );   //LDR (27k ristor)    //TODO: need to be in array
    PrintPaddedValue( analogRead(A1) );  // Hob Nob             //TODO: needs to be in an array.
    
    MUX_select(1);
    PrintPaddedValue( (MPU[1].getAngleX( true )) );
    PrintPaddedValue( (MPU[1].getAngleY( true )) );
    PrintPaddedValue( analogRead(A5) );   //LDR (27k ristor)    //TODO: need to be in array
    PrintPaddedValue( analogRead(A2) );   //hobValue );  // Hob Nob             //TODO: needs to be in an array.
    
    // Print sigleData
    // d ...#Whisking#
    PrintPaddedValue( jug_adxl.GetGyro_z() );
    PrintPaddedValue( whisking );        // whisk rt tilt switch (1k ristor)
    PrintPaddedValue( digitalRead(toggleSwitch_inputPin) == HIGH );

    Serial.print( "\r\n" );
    
}

void UpdateHobWhiskToggle()
{
  //Toggle the values depending on the toggle switch
  if(digitalRead(toggleSwitch_inputPin) == HIGH && !pin9_isHigh)
  {
    whiskValue = analogRead(toggle_analogReadPin);
    digitalWrite(hob_outputPin, HIGH);
    digitalWrite(whisk_outputPin, LOW);
    pin9_isHigh = true;
  }
  else if(digitalRead(toggleSwitch_inputPin) == LOW && pin9_isHigh)
  {
    hobValue = floor(((float)analogRead(toggle_analogReadPin)));// / 613.f) * 1023);
    digitalWrite(hob_outputPin, LOW);
    digitalWrite(whisk_outputPin, HIGH);
    pin9_isHigh = false; 
  }

  // update the hob / whisk value
  if(pin9_isHigh)
    // the value from this hobNob is lower due to a pull-down ristor, so by working out its percentage we can bring it inline whit the out two :D
    hobValue = floor(((float)analogRead(toggle_analogReadPin)));// / 613.f) * 1023);
   else
    whiskValue = analogRead(toggle_analogReadPin);

}

void UpdateWhisking()
{

  int currentValue = whiskValue;
  bool valueIsLow = currentValue < whisking_switch_lowValue;
  
  if(valueIsLow != whisking_lastWasLow)
  {
    whisking = true;
    whisking_nextInterval = millis() + whisking_valueChanged_interval;
  }
  else if(millis() >= whisking_nextInterval)
  {
    whisking = false;
    whisking_nextInterval = millis() + whisking_valueChanged_interval; 
  }

  whisking_lastWasLow = valueIsLow;
  
}
