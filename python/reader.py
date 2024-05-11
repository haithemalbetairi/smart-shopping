from mfrc522 import MFRC522
from machine import Pin, I2C
from ssd1306 import SSD1306_I2C
from connecter import connect
import framebuf
import utime

connect()

# RFID module GPIO pins for mfrc522.py
# miso = 4  # MISO (Main in Sub out)
# cs   = 5  # SDA or CS (Chip Select)
# sck  = 6  # SCK (Clock signal)
# mosi = 7  # MOSI (Main out Sub in)
# rst  = 22 # RST (Reset)

# initialize RFID reader
reader = MFRC522(spi_id=0,sck=6,miso=4,mosi=7,cs=5,rst=22)

# initialize OLED display
WIDTH = 128
HEIGHT = 64

i2c = I2C(0, scl=Pin(1), sda=Pin(0), freq=200000)
oled = SSD1306_I2C(WIDTH, HEIGHT, i2c)
# Clear screen
oled.fill(0)

# Reset Pico LED
led = Pin('LED', Pin.OUT)
led.on()

print("Bring tag closer...")
print("")

while True:
    reader.init()
    (stat, tag_type) = reader.request(reader.REQIDL)
    led.toggle()
    oled.fill(0)
    if stat == reader.OK:
        (stat, uid) = reader.SelectTagSN()
        (yr, mth, day, hr, mn, sec, ms, us) = utime.localtime()
        if stat == reader.OK:
            card = int.from_bytes(bytes(uid),"little",False)
            print(f"[{yr}-{mth:02d}-{day:02d} {hr:02d}:{mn:02d}:{sec:02d}] CARD ID: "+str(card))
            oled.text("CARD ID: "+str(card), 0, 10)
            oled.show()
    utime.sleep_ms(500)
    oled.text("", 0, 10)
    oled.show()