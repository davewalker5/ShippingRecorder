#!/usr/bin/env python3

import csv
import re
import sys
from pathlib import Path

# Pattern for lines of interest:
#
# Optional leading spaces
# 2-letter country code
# 1 or more spaces
# 3-letter port location code
# 1 or more spaces
# Port name
# 1 or more spaces plus the rest of the line
#
LINE_RE = re.compile(r"""^\s*([A-Z]{2})\s+([A-Z0-9]{3})\s+(.+)$""")

def extract_codes(input_path: Path, ports_csv: Path, country_codes_csv: Path) -> None:
    ports_rows = dict()
    country_codes = []

    with input_path.open("r", encoding="utf-8") as f:
        for line in f:
            # Only the first 46 characters are significant
            prefix = line[:46].rstrip("\n")

            # Check the line matches the required pattern for a port definition
            match = LINE_RE.match(prefix)
            if not match:
                continue

            # Extract the country code, location code and name
            country_code = f"{match.group(1)}"
            unlocode = f"{country_code}{match.group(2)}"
            name = match.group(3).strip()
            if unlocode not in ports_rows:
                ports_rows[unlocode] = (unlocode, name)

            if country_code not in country_codes:
                country_codes.append(country_code)

    # Write the port definitions CSV
    with ports_csv.open("w", newline="", encoding="utf-8") as out_f:
        writer = csv.writer(out_f, quoting=csv.QUOTE_ALL)
        writer.writerow(["code", "name"])
        writer.writerows(list(ports_rows.values()))

    # Write the country codes CSV
    country_codes_rows = [[code] for code in sorted(country_codes)]
    with country_codes_csv.open("w", newline="", encoding="utf-8") as out_f:
        writer = csv.writer(out_f, quoting=csv.QUOTE_ALL)
        writer.writerow(["code"])
        writer.writerows(country_codes_rows)


def main():
    if len(sys.argv) != 4:
        print(f"Usage: {sys.argv[0]} /path/to/input.txt /path/to/ports.csv /path/to/country-codes.csv")
        sys.exit(1)

    input_path = Path(sys.argv[1])
    ports_csv = Path(sys.argv[2])
    country_codes_csv = Path(sys.argv[3])

    extract_codes(input_path, ports_csv, country_codes_csv)


if __name__ == "__main__":
    main()