#!/usr/bin/python

import getopt
import sys


def main(argv):
    # Process inputs.
    location = ''
    try:
        opts, args = getopt.getopt(argv, "hl:", ["help", "location="])
    except getopt.GetoptError:
        print('usage: RoomTemp.py -l <LocationName>')
        sys.exit(2)

    for opt, arg in opts:
        if opt in ('-h', '--help'):
            print('RoomTemp.py -l <LocationName>')
            sys.exit()
        elif opt in ("-l", "--location"):
            location = arg

    if location == '':
        print("'Location' is a required argument. Please refer to --help for more information.")
        sys.exit(3)

    print('Location is', location)
    sys.exit()


if __name__ == "__main__":
    main(sys.argv[1:])
