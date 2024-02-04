#include <SoftwareSerial.h>   // 引用程式庫

/*Bluetooth*/
SoftwareSerial BT(8, 9); // 接收腳, 傳送腳

bool RightStart = false;
bool CollectDone = false;

unsigned char byteBuffer[3];
const unsigned char StartByte = 0xFA;
const unsigned char CmdSendByteA0 = 0xA0;
const unsigned char CmdSendByteA1 = 0xA1;
const unsigned char EndByte = 0xFF;

int readCount = 0;

/*IR*/
const int PinA0  = A0;
const int PinA1  = A1;
int RawDataIR  = 0;
int CalculateValIR  = 0;

void setup() 
{
  Serial.begin(9600);   // 與電腦序列埠連線
  Serial.println("BT is ready!");

  // 設定藍牙模組的連線速率
  // 如果是HC-05，請改成38400
  BT.begin(9600);

  pinMode(PinA0, INPUT);
  pinMode(PinA1, INPUT);
}

void loop() 
{
  //testGP2Y0A21();
  bluetoothPart();
  delay(10); 
  
}

void bluetoothPart()
{
  // 若收到「序列埠監控視窗」的資料，則送到藍牙模組
  /*
  if (Serial.available())
  {
    val = Serial.read();
    BT.print(val);
  }
  */
  // 若收到藍牙模組的資料
  if (BT.available()) 
  {
    unsigned char byteVal = BT.read();
    //Serial.print("Data:");
    //Serial.println(byteVal);
    if (readCount == 0 && byteVal == StartByte)
    {
      RightStart = true;
    }

    if (RightStart)
    {
      //Serial.println("Get Data!!");
      byteBuffer[readCount] = byteVal; 
      readCount++;
      //Serial.println(readCount);
      if (readCount == 3)
      {
        Serial.print("All Data:");
        Serial.print(byteBuffer[0]);
        Serial.print(",");
        Serial.print(byteBuffer[1]);
        Serial.print(",");
        Serial.println(byteBuffer[2]);
        if (byteBuffer[1] == CmdSendByteA0)
        {
          int irData = testGP2Y0A21(CmdSendByteA0);
          BT.write(irData);
          Serial.print("Send Data A0: ");
          Serial.print(CalculateValIR);
        }  
        else if (byteBuffer[1] == CmdSendByteA1)
        {
          int irData = testGP2Y0A21(CmdSendByteA1);
          BT.write(irData);
          Serial.print("Send Data A1: ");
          Serial.print(CalculateValIR);
        }  
        
        resetTrigger();
        }
      }
    }
  }  


void resetTrigger()
{
  RightStart = false;
  CollectDone = false;
  memset(byteBuffer, 0, sizeof(byteBuffer));
  readCount = 0;
}

int testGP2Y0A21(int Pin) 
{ /* function testGP2Y0A21 */
  ////Read distance sensor
  RawDataIR = analogRead(Pin);
  CalculateValIR = distRawToPhys(RawDataIR);
  //Serial.print(gp2y0a21Val); 
  //Serial.print(F(" - "));
  //Serial.println(distRawToPhys(RawDataIR));
  if (CalculateValIR < 200)
  {
    //Serial.println(F("Obstacle detected"));
  } 
  else 
  {
    //Serial.println(F("No obstacle"));
  }
  return CalculateValIR;
}

int distRawToPhys(int raw) 
{ /* function distRawToPhys */
  ////IR Distance sensor conversion rule
  float Vout = float(raw) * 0.0048828125; // Conversion analog to voltage
  int phys = 13 * pow(Vout, -1); // Conversion volt to distance

  return phys;
}
