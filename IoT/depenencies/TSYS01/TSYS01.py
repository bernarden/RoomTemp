# Copyright (c) 2016 Ian Dobbie (ian.doibbie@bioch.ox.ac.uk)
# V 1.0 20160815 IMD
# https://github.com/iandobbie/TSYS01.py/blob/master/TSYS01.py
#
# This is a simple library to access the TSY01 i2c sensor board on a
# Raspberry Pi (tested on a 3)

# Default i2c address is 0x76 i2c bus is 1 on a RPi 3. Only alternative
# is 0x77 as we have a single address pin on this chip.


import time
import struct
try:
    import smbus
    SIMULATION = False
except ImportError:
    try:
        import smbus2 as smbus
        SIMULATION = False
    except ImportError:
        import random
        from depenencies.rpi_hardware import smbus
        SIMULATION = True


# i2c commands
# 0x1E     Reset
# 0x48     start ADC temp conversion
# 0x00     Read ADC temp result
# 0xA0,0xA2,...0xAE   read prom addresses 0-7

ADDR_DEFAULT = 0x76

# addresses of the calibration coefficients.
# TSY01_COEFF0 = 0xAA
# TSY01_COEFF1 = 0xA8
# TSY01_COEFF2 = 0xA6
# TSY01_COEFF3 = 0xA4
# TSY01_COEFF4 = 0xA2

# The following are taken from the Arduino library at:
# https://github.com/Ell-i/ELL-i-KiCAD-Boards/blob/master/TSYS01/Arduino/Tsys01.h
# Made by Apocalyt, pre-calculated constants of powers of 10
# This resolves loss of accuracy in calculation of the temperature values resulting from
# 10^(-21/4) / 256.0
TSYS_POW_A = 0.0000056234132519034908039495103977648123146825104309869166408 / 256.0
# 10^(-16/3) / 256.0
TSYS_POW_B = 0.0000046415888336127788924100763509194465765513491250112436376 / 256.0
# 10^(-11/2) / 256.0
TSYS_POW_C = 0.0000031622776601683793319988935444327185337195551393252168268 / 256.0
# 10^(-6/1) / 256.0
TSYS_POW_D = 0.000001 / 256.0
# 10^(-2/1)
TSYS_POW_E = 0.01


# main class.
class TSYS01(object):
    """Class to read the TSY01 i2c temperature sensing board"""

    def __init__(self, address=ADDR_DEFAULT, i2c=1):
        """Initialise TSY01 device, resets and reads calibration data as well"""
        if address != 0x76 and address != 0x77:
            print("Error: TSY01 only supports address 0x76 or 0x77")
            return

        self.coefficients = [0] * 5
        self.address = address
        self.bus = smbus.SMBus(i2c)

        # reset board to get it into a know state.
        self.reset()

        # read a store calibration data for later calculation
        self.read_calibration()

    def reset(self):
        if SIMULATION:
            return

        """Reset the device"""
        self.bus.write_byte_data(self.address, 0, 0x1E)

    def read_calibration(self):
        if SIMULATION:
            return

        """Reads calibration from the sensor and stores it on self.cal"""
        for i in range(5):
            calibration_data = self.bus.read_word_data(self.address, (0xAA - i * 2))
            self.coefficients[i] = struct.unpack("<H", struct.pack(">H", calibration_data))[0]

    def read_temperature(self):
        if SIMULATION:
            return round(random.uniform(10, 35), 3)

        """Reads the adc values and returns the temp in C"""
        # first start  temperature conversion
        self.bus.write_byte(self.address, 0x48)
        # let ADC conversion happen, max time is 10ms
        time.sleep(0.01)
        # then read output data
        output = self.bus.read_i2c_block_data(self.address, 0x00, 3)
        adc = output[2] + 256 * output[1] + (256 ** 2) * output[0]

        term1 = -2.0 * self.coefficients[4] * ((TSYS_POW_A * adc) ** 4)
        term2 = 4.0 * self.coefficients[3] * ((TSYS_POW_B * adc) ** 3)
        term3 = -2.0 * self.coefficients[2] * ((TSYS_POW_C * adc) ** 2)
        term4 = 1 * self.coefficients[1] * (TSYS_POW_D * adc)
        term5 = -1.5 * self.coefficients[0] * TSYS_POW_E
        return term1 + term2 + term3 + term4 + term5
