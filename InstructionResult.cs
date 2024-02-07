using System.Diagnostics.CodeAnalysis;
using Basm.Binding;

namespace Basm;

internal readonly struct InstructionResult : IEquatable<InstructionResult>
{
    private readonly bool halt;
    private readonly object? result;

    public InstructionResult(object? value): this()
    {
        halt = true;
        result = value;
    }

    public InstructionResult(BoundLabel? label) : this()
    {
        result = label;
    }

    public InstructionResult(Diagnostic diagnostic) : this()
    {
        result = diagnostic;
    }

    public static InstructionResult Jump(BoundLabel? label)
    {
        return new(label);
    }

    public static InstructionResult Invalid(BoundInstruction instruction)
    {
        return new(new Diagnostic(instruction.Syntax.Mnemonic.Location, $"Unknown mnemonic {instruction.Mnemonic}"));
    }

    public static InstructionResult Halt(object? value)
    {
        return new(value);
    }

    public readonly bool IsHalt => halt;
    [MemberNotNullWhen(true, nameof(Label))]
    public readonly bool IsJump => Label != null;
    [MemberNotNullWhen(true, nameof(Diagnostic))]
    public readonly bool IsInvalid => Diagnostic != null;

    public readonly object? Value => result;
    public readonly BoundLabel? Label => result as BoundLabel;
    public readonly Diagnostic? Diagnostic => result as Diagnostic;

    public bool Equals(InstructionResult other) => halt == other.halt && result == other.result;
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is InstructionResult result && Equals(result);
    public override int GetHashCode() => halt.GetHashCode() ^ result?.GetHashCode() ?? 0;

    public static bool operator ==(InstructionResult left, InstructionResult right) => left.Equals(right);
    public static bool operator !=(InstructionResult left, InstructionResult right) => !left.Equals(right);
}
