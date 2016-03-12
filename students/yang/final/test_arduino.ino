int val=0;
int prev;

void setup() {

  Serial.begin(9600);
  
}

// the loop routine runs over and over again forever:
void loop() {
 
  prev=val;
  val= analogRead(A0);
 


if(val>prev){
    Serial.println('1');
    delay(100);
    }else{Serial.println('0');
    delay(100);
    }








  
}



