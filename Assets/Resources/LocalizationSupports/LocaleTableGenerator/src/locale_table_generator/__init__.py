import csv
import argparse
from .app import PrototypeTableProcessor
from os import path

def main():
    # Init
    proc = PrototypeTableProcessor()

    # Args
    parser = argparse.ArgumentParser(description="Generate a locale table from a csv file.")
    parser.add_argument("-i", "--input", help="Input files")
    parser.add_argument("-o", "--output", help="Output file", default="")
    args = parser.parse_args()

    _in = args.input
    _out = args.output

    if _in is None:
        parser.print_help()
        return

    # Processing
    files = args.input.split(",")
    for file in files:
        if not path.exists(file):
            raise FileNotFoundError(f"File '{file}' does not exist.")
        with open(file, "r") as f:
            reader = csv.reader(f)
            table = list(reader)
            proc.feed(table)
    
    # Output
    if args.output == "":
        for row in proc.render():
            print(row)
        return
    
    with open(args.output, "w") as f:
        writer = csv.writer(f)
        writer.writerows(proc.render())

if __name__ == "__main__":
    main()
