---
name: Bug report
about: Report a bug in the string rendering library
title: ''
labels: bug
assignees: ''

---

**Describe the bug**  
A clear and concise description of what the bug is.

**To Reproduce**  
Provide a code example that causes the bug.

If a specific expression is causing the bug, also provide the `DebugView` of the expression.

Additionally, if you can, provide the factory-method-formatter output, as follows:
```
// using ExpressionTreeToString

Expression<Func<bool>> expr = ... // your expression here
var output = expr.ToString("Factory methods");

// include the contents of output in this bug report
```

**Expected behavior**  
A clear and concise description of what you expected to happen.

**Version info**

- .NET Core / .NET Framework version:
- Version: [either NuGet package version / GitHub release version, or commit]

**Additional context**  
Add any other context about the problem here.
