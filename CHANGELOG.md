##  (2026-09-07)

### ⚠ BREAKING CHANGES

* feat!: add the print statement and static modifier
* feat!: bounded strings now allocate a fixed size on the call stack

### Code Refactoring

* refactor(codegen): fully qualify Int128 types with System namespace

### Continuous Integration

* ci: add workflow to publish compiler to NuGet

### Documentation

* docs: add beautifully formatted ErrorCodes class summary
* docs: define the philosophy of Hazel

### Features

* feat: add bounded string type and improve variable assignment
* feat: add runtime exception BoundedStringOverflowException
* feat: add runtime folder and make runtime always emitted for built-in types
* feat: add type modifiers sealed and abstract and make integer sizing explicit
* feat: bounded string conversions and fix access modifier error message
* feat: improve compile-time bounded-string guarantees and fixes
* feat: make the Hazel.Runtime and sub-namespaces reserved
* feat: standard library and bounded strings import
* feat(extensions): add Visual Studio extension for syntax highlighting
