Whaally Domain Benchmarks
=========================

The Whaally Domain library heavily relies on type checking for most of its structure. For the few areas where the static type system is not sufficient we've implemented behaviour using reflection to close the gap.

While this primarily provides a risk for the structural integrity of the software, it also incurs a certain performance penalty, which we're trying to quantify in this project.

This benchmark project solely focuses on the internals of the library without any externalities.