#include <SoftwareSerial.h>   // 引用程式庫

/*Bluetooth*/
SoftwareSerial BT(8, 9); // 接收腳, 傳送腳
char val;  // 儲存接收資料的變數

/*IR*/
const int gp2y0a21Pin  = A0;
int gp2y0a21Val  = 0;

void setup() 
{
  Serial.begin(9600);   // 與電腦序列埠連線
  Serial.println("BT is ready!");

  // 設定藍牙模組的連線速率
  // 如果是HC-05，請改成38400
  BT.begin(9600);

  pinMode(gp2y0a21Pin, INPUT);
}

void loop() 
{
  //testGP2Y0A21();
  bluetoothPart();
  delay(400);

  
  
}


void bluetoothPart()
{
  // 若收到「序列埠監控視窗」的資料，則送到藍牙模組
  if (Serial.available())
  {
    val = Serial.read();
    BT.print(val);
  }

  // 若收到藍牙模組的資料，則送到「序列埠監控視窗」
  if (BT.available()) 
  {
    val = BT.read();
    Serial.print(val);
  }

  
}

void testGP2Y0A21() 
{ /* function testGP2Y0A21 */
  ////Read distance sensor
  gp2y0a21Val = analogRead(gp2y0a21Pin);
  Serial.print(gp2y0a21Val); 
  Serial.print(F(" - "));
  Serial.println(distRawToPhys(gp2y0a21Val));
  if (gp2y0a21Val < 200)
  {
    Serial.println(F("Obstacle detected"));
  } 
  else 
  {
    Serial.println(F("No obstacle"));
  }
}

int distRawToPhys(int raw) 
{ /* function distRawToPhys */
  ////IR Distance sensor conversion rule
  float Vout = float(raw) * 0.0048828125; // Conversion analog to voltage
  int phys = 13 * pow(Vout, -1); // Conversion volt to distance

  return phys;
}
