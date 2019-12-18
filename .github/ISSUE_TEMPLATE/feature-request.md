---
name: Feature request
about: Suggest an idea for the string rendering library
title: ''
labels: enhancement
assignees: ''

---

**Version info**

 - .NET Core / .NET Framework version:
 - Version: [either NuGet package version / GitHub release version, or commit]

**Is your feature request related to a problem? Please describe.**  
A clear and concise description of what the problem is. Ex. _I'm always frustrated when [...]_

If this is related to a specific expression:

* Provide the `DebugView` of the expression.
* Additionally, if you can, provide the factory-method-formatter output, as follows:
  ```
  // using ExpressionTreeToString

  Expression<Func<bool>> expr = ... // your expression here
  var output = expr.ToString("Factory methods");
  ```

**Describe the solution you'd like**  
A clear and concise description of what you want to happen.

**Describe alternatives you've considered**  
A clear and concise description of any alternative solutions or features you've considered.

**Additional context**  
Add any other context or screenshots about the feature request here.
