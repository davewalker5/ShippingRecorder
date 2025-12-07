from pathlib import Path
import sys

def convert_to_utf8(input_path: Path, output_path: Path) -> None:
    text = input_path.read_text(encoding="latin1")
    output_path.write_text(text, encoding="utf-8")

def main():
    if len(sys.argv) != 3:
        print(f"Usage: {sys.argv[0]} /path/to/input.txt /path/to/output.txt")
        sys.exit(1)

    input_path = Path(sys.argv[1])
    output_path = Path(sys.argv[2])

    convert_to_utf8(input_path, output_path)


if __name__ == "__main__":
    main()