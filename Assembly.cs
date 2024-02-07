using Basm.Binding;
using Basm.Syntax;

namespace Basm;

public sealed class Assembly
{
    private Assembly(SyntaxTree syntaxTree)
    {
        SyntaxTree = syntaxTree;
    }

    public SyntaxTree SyntaxTree { get; }

    public static Assembly Create(SyntaxTree syntaxTree)
    {
        return new Assembly(syntaxTree);
    }

    public IEnumerable<Diagnostic> Assemble(InstructionSet instructionSet)
    {
        var boundAssembly = Binder.BindAssembly(SyntaxTree);

        if (boundAssembly.Diagnostics.Any())
            return boundAssembly.Diagnostics;

        var assembler = new Assembler();

        return assembler.Assemble(boundAssembly);
    }

    public EvaluationResult Evaluate(InstructionSet instructionSet)
    {
        var boundAssembly = Binder.BindAssembly(SyntaxTree);

        if (boundAssembly.Diagnostics.Any())
            return new EvaluationResult(boundAssembly.Diagnostics);

        var evaluator = new Evaluator(boundAssembly, instructionSet);
        var value = evaluator.Evaluate();

        return new EvaluationResult(evaluator.Diagnostics, value);;
    }
}
