using System.Collections.Immutable;
using Basm.Binding;

namespace Basm;

internal class Evaluator
{
    private readonly Diagnostics diagnostics = [];
    private readonly BoundAssembly assembly;
    private readonly InstructionSet instructionSet;

    public Evaluator(BoundAssembly assembly, InstructionSet instructionSet)
    {
        this.assembly = assembly;
        this.instructionSet = instructionSet;
    }

    public IEnumerable<Diagnostic> Diagnostics => diagnostics;

    public object? Evaluate()
    {
        var statements = assembly.CodeStatements.ToImmutableArray();
        var labels = new Dictionary<BoundLabel, int>();

        for (int i = 0; i < statements.Length; i++)
        {
            if (statements[i] is BoundLabel label)
                labels[label] = i + 1;
        }

        instructionSet.Initialize(assembly.DataVariables);

        var index = 0;

        while (index < statements.Length)
        {
            var statement = statements[index];

            switch (statement)
            {
                case BoundLabel:
                    index++;
                    break;

                case BoundInstruction instruction:
                    var result = instructionSet.Evaluate(instruction);

                    switch (result.IsHalt)
                    {
                        case false when result == default:
                            index += 1;
                            break;
                        case false when result.IsJump:
                            index = labels[result.Label];
                            break;
                        case false when result.IsInvalid:
                            index += 1;
                            diagnostics.Report(result.Diagnostic);
                            break;
                        case true:
                            return result.Value;
                    }
                    break;
            }
        }

        return null;
    }
}
