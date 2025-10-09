using Microsoft.VisualStudio.TestTools.UnitTesting;

// Ejecuta pruebas en paralelo a nivel de método.
// Workers = 0 => usa tantos workers como núcleos disponibles.
[assembly: Parallelize(Workers = 0, Scope = ExecutionScope.MethodLevel)]
