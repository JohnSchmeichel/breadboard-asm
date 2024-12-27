# Breadboard Assembler

Breadboard Assembler is a simple assembly language interpreter and assembler for educational purposes.
It allows you to write, assemble, and execute assembly code for a simulated instruction set architecture.

This project is inspired by the 8-bit breadboard computer by Ben Eater.
For more information, visit [eater.net/8bit](https://eater.net/8bit).

## Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Building the Project

To build the project, run the following command in the root directory:

```sh
dotnet build
```

### Running the Assembler

To run the assembler, use the following command:

```sh
./basm <path-to-assembly-file>
```

For example, to run the `sample.basm` file:

```sh
./basm assembly/sample.basm
```

### Verbose Mode

To enable verbose mode, use the `-v` flag:

```sh
./basm -v <path-to-assembly-file>
```

### Evaluate Mode

To evaluate the assembly code, use the `eval` argument:

```sh
./basm eval <path-to-assembly-file>
```

## Assembly Language Syntax

The assembly language supports the following instructions:

- `NOP` - No operation
- `LDA` - Load accumulator
- `STA` - Store accumulator
- `ADD` - Add to accumulator
- `SUB` - Subtract from accumulator
- `LDI` - Load immediate
- `ADI` - Add immediate
- `SBI` - Subtract immediate
- `CMP` - Compare
- `CPI` - Compare immediate
- `JMP` - Jump
- `JC` - Jump if carry
- `JZ` - Jump if zero
- `OUT` - Output
- `HLT` - Halt

## Example Assembly Code

Here is an example of a simple assembly program that adds two numbers and outputs the result:

```assembly
.code
    LDA x
    ADD y
    OUT
    HLT

.data
    x: 27
    y: 9
```

## Contributing

Contributions are welcome! Please open an issue or submit a pull request.
For larger changes please start a discussion before making changes.
