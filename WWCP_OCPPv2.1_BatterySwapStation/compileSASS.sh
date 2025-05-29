#!/bin/bash

# Determine the directory where the script is located
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

# Ensure jq is installed
if ! command -v jq &> /dev/null
then
    echo "Error: 'jq' is not installed. Please install it to use this script."
    exit 1
fi

# Ensure sass is installed
if ! command -v sass &> /dev/null
then
    echo "Error: 'sass' is not installed. Please install it to use this script."
    exit 1
fi

# Path to the compilerconfig.json file
CONFIG_FILE="$SCRIPT_DIR/compilerconfig.json"

# Check if compilerconfig.json exists
if [ ! -f "$CONFIG_FILE" ]; then
    echo "Error: $CONFIG_FILE not found."
    exit 1
fi

# Read the number of entries in the array
NUM_ENTRIES=$(jq '. | length' "$CONFIG_FILE")

# Loop through each entry
for (( i=0; i<$NUM_ENTRIES; i++ ))
do
    INPUT_FILE=$(jq -r ".[$i].inputFile" "$CONFIG_FILE")
    OUTPUT_FILE=$(jq -r ".[$i].outputFile" "$CONFIG_FILE")

    # Adjust paths to be relative to SCRIPT_DIR
    INPUT_PATH="$SCRIPT_DIR/$INPUT_FILE"
    OUTPUT_PATH="$SCRIPT_DIR/$OUTPUT_FILE"

    echo "Compiling $INPUT_PATH to $OUTPUT_PATH..."

    # Create output directory if it doesn't exist
    OUTPUT_DIR=$(dirname "$OUTPUT_PATH")
    if [ ! -d "$OUTPUT_DIR" ]; then
        mkdir -p "$OUTPUT_DIR"
    fi

    # Compile to regular CSS
    sass --no-source-map "$INPUT_PATH" "$OUTPUT_PATH"

    # Check if the compilation was successful
    if [ $? -ne 0 ]; then
        echo "Error: Failed to compile $INPUT_PATH to $OUTPUT_PATH"
        exit 1
    fi

    # Compile to minified CSS
    MINIFIED_OUTPUT_PATH="${OUTPUT_PATH%.css}.min.css"

    echo "Compiling $INPUT_PATH to $MINIFIED_OUTPUT_PATH..."

    sass --style=compressed --no-source-map "$INPUT_PATH" "$MINIFIED_OUTPUT_PATH"

    # Check if the minification was successful
    if [ $? -ne 0 ]; then
        echo "Error: Failed to compile $INPUT_PATH to $MINIFIED_OUTPUT_PATH"
        exit 1
    fi
done

echo "All SCSS files have been compiled successfully."
