namespace VSharp

open Microsoft.Z3
open VSharp.Core

type public ISolver<'TAst, 'TResult> =
    abstract member Encode : term -> 'TAst
    abstract member Solve : term list -> 'TResult

type public ISmtSolver<'TAst> =
    inherit ISolver<'TAst, SmtResult>

type public IZ3Solver =
    inherit ISmtSolver<AST>

type public Z3Solver() =
    interface IZ3Solver with
        member x.Encode t = Z3.encodeTerm t
        member x.Solve terms = Z3.solveFP terms

type public Z3Simplifier() =
    interface IPropositionalSimplifier with
        member x.Simplify t = Z3.simplifyPropositional t

type public SmtSolverWrapper<'TAst>(solver : ISmtSolver<'TAst>) =
    interface VSharp.Core.ISolver with
        override x.Solve term =
            match solver.Solve [term] with
            | SmtSat _ -> VSharp.Core.Sat
            | SmtUnsat -> VSharp.Core.Unsat
            | SmtUnknown _ -> VSharp.Core.Unknown
        override x.SolvePathCondition term pc =
            // TODO: solving should be incremental!
            match solver.Solve (term::pc) with
            | SmtSat _ -> VSharp.Core.Sat
            | SmtUnsat -> VSharp.Core.Unsat
            | SmtUnknown _ -> VSharp.Core.Unknown
