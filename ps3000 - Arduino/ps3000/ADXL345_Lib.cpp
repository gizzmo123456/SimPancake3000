#include "ADXL345_Lib.h"
#include "Wire.h"

ADXL345::ADXL345(TwoWire &w)
{
  wire = &w;  
}

void ADXL345::begin()
{

	wire->beginTransmission(ADXL345_ADDR);
	wire->write(0x2D);   // gets the register of the chip
	wire->write(0);      // reset the byts back to 0
	wire->endTransmission();

	wire->beginTransmission(ADXL345_ADDR);
	wire->write(0x2D);
	wire->write(16);     // set to measure
	wire->endTransmission();

	wire->beginTransmission(ADXL345_ADDR);
	wire->write(0x2D);
	wire->write(8);      // set to sleep
	wire->endTransmission();
	
}

void ADXL345::update()
{
	// Define the XYZ register
	int xyzReg = 0x32;
	wire->beginTransmission(ADXL345_ADDR);
	wire->write(xyzReg);     
	wire->endTransmission();

	wire->beginTransmission(ADXL345_ADDR);
	wire->requestFrom(ADXL345_ADDR, 6);      // wait for a responce of 6 bytes
	
	// read from wire byte by byte
	int i = 0;
	while(wire->available())
	{
		gyro_values[i] = wire->read();
		i++;
	}
  
   wire->endTransmission();
	
	// redefine values into int
	gyro_x = (((int)gyro_values[1]) << 8) | gyro_values[0];
	gyro_y = (((int)gyro_values[3]) << 8) | gyro_values[2];
	gyro_z = (((int)gyro_values[5]) << 8) | gyro_values[4];
  
}

void ADXL345::Normalize()
{
  gyroOffset_x = gyro_x;
  gyroOffset_y = gyro_y;
  gyroOffset_z = gyro_z;
}
