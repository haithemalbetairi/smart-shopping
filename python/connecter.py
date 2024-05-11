from machine import Pin
import network
import secrets
import urequests
import utime

def connect():
    # LED GPIO
    led = Pin("LED", Pin.OUT)
    led.off()
    
    # wireless connection configuration
    wlan = network.WLAN(network.STA_IF)
    wlan.active(True)
    wlan.connect(secrets.SSID, secrets.PASSWORD)
    
    while wlan.isconnected() == False:
        # local timestamp
        (year, month, day, hour, minute, sec, m_sec, u_sec) = utime.localtime() 
        print(f"[{year}-{month}-{day} {hour}:{minute}:{sec:02d}] Waiting to connect...")
        utime.sleep(1)
        
    ip = wlan.ifconfig()[0]
    print(f"Connected on {ip}")
    led.on()
    return ip

#if wlan.isconnected():
    #print("Successfully connected to " + secrets.SSID)
    #led.on()
    #print("Astronauts currently on the International Space Station")
    #astronauts = urequests.get("http://api.open-notify.org/astros.json").json()
    #number = astronauts["number"]
    #for i in range(number):
        #print(str(i+1) + ") "+ astronauts["people"][i]["name"])
    

