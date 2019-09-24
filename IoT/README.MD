### Python dependencies:

1. [requests](https://pypi.org/project/requests/)
1. [Twisted](https://pypi.org/project/Twisted/) - [related StackOverflow issue](https://stackoverflow.com/questions/29846087/microsoft-visual-c-14-0-is-required-unable-to-find-vcvarsall-bat/51087608#51087608)
1. [python-dateutil](https://pypi.org/project/python-dateutil/)

### Autostart on Raspbian
1. Run `sudo nano /etc/rc.local`
1. Add the following before `exit 0`:
    ```
    sleep 60 && cd /home/pi/Desktop/RoomTemp/IoT/ && python3.6 ./RoomTemp.py -u "https://your-url.com" -k "you-api-key" -l "Location Name" -s "TSYS01"
    ```
