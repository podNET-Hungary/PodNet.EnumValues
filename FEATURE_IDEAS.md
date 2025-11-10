# Feature Ideas for PodNet.EnumValues

This document outlines 10 feature ideas that would significantly improve the PodNet.EnumValues product. Each feature is designed to enhance usability, extend functionality, or improve developer experience.

## 1. Reverse Lookup - String to Enum Conversion

### Description
Generate reverse lookup methods that convert string values back to enum values. This would complement the existing `GetValue()` methods with `FromValue()` methods.

### Benefits
- Enables bidirectional mapping between enums and string values
- Essential for parsing user input, API responses, and configuration files
- Reduces boilerplate code for common enum parsing scenarios
- Provides type-safe deserialization

### Example Usage
```csharp
[Values<ColorAttribute>]
public enum Sentiment
{
    [Color("Green")]  Happy,
    [Color("Yellow")] Neutral,
    [Color("Red")]    Sad
}

// Generated method:
Sentiment sentiment = SentimentValueExtensions.FromColor("Green"); // Returns Sentiment.Happy

// With options for case-insensitive matching:
Sentiment sentiment = SentimentValueExtensions.FromColor("green", ignoreCase: true);
```

### Implementation Notes
- Generate a switch expression or dictionary-based lookup for performance
- Support options for case-sensitive/insensitive matching
- Handle duplicate values with clear error messages
- Provide optional default value or throw exception for invalid strings

---

## 2. Support for Non-String Value Types

### Description
Extend the `ValueAttribute` to support additional types beyond strings, such as integers, GUIDs, or custom types that can be represented as constants.

### Benefits
- Enables scenarios where enums map to numeric IDs, color codes (as integers), or other constant values
- More flexible for database mapping scenarios
- Allows using the same pattern for different value types
- Better type safety for specific domains

### Example Usage
```csharp
public class IdAttribute : ValueAttribute<int>
{
    public IdAttribute(int value) : base(value) { }
}

[Values<IdAttribute>]
public enum Status
{
    [Id(100)] Active,
    [Id(200)] Inactive,
    [Id(300)] Pending
}

int statusId = Status.Active.GetId(); // Returns 100
Status status = StatusValueExtensions.FromId(200); // Returns Status.Inactive
```

### Implementation Notes
- Extend `ValueAttribute` to be generic: `ValueAttribute<T>`
- Support types that can be used in attributes (primitives, strings, enums, Type)
- Update generator to handle different value types in switch expressions
- Maintain backward compatibility with existing string-based implementation

---

## 3. JSON Serialization Support

### Description
Provide built-in serialization/deserialization support for popular JSON libraries (System.Text.Json and Newtonsoft.Json) that automatically uses the defined string values.

### Benefits
- Seamless API integration with automatic enum serialization
- Eliminates need for custom JSON converters
- Consistent JSON representation across the application
- Reduces developer effort for API development

### Example Usage
```csharp
[Values<ValueAttribute>]
[JsonConverter(typeof(EnumValuesConverter))]
public enum Priority
{
    [Value("low")] Low,
    [Value("medium")] Medium,
    [Value("high")] High
}

// Serializes as: { "priority": "low" }
// Instead of: { "priority": "Low" } or { "priority": 0 }
```

### Implementation Notes
- Generate JSON converters automatically based on `Values` attributes
- Support both System.Text.Json and Newtonsoft.Json
- Make it opt-in via attribute parameter
- Handle null values and undefined enum values appropriately
- Support all existing configuration options (case sensitivity, etc.)

---

## 4. Display Name and Description Support

### Description
Add support for generating multiple accessors - one for the value and additional ones for display names and descriptions, useful for UI scenarios.

### Benefits
- Separates technical values from human-readable text
- Supports internationalization scenarios
- Enables rich UI generation from enums
- Provides context and documentation in one place

### Example Usage
```csharp
public class UiAttribute : ValueAttribute
{
    public string DisplayName { get; set; }
    public string Description { get; set; }
    
    public UiAttribute(string value, string displayName = null, string description = null) 
        : base(value)
    {
        DisplayName = displayName ?? value;
        Description = description ?? string.Empty;
    }
}

[Values<UiAttribute>]
public enum FileFormat
{
    [Ui("pdf", "PDF Document", "Portable Document Format")]
    Pdf,
    
    [Ui("docx", "Word Document", "Microsoft Word Format")]
    Word
}

// Generated methods:
string value = FileFormat.Pdf.GetUi();              // "pdf"
string display = FileFormat.Pdf.GetUiDisplayName(); // "PDF Document"
string desc = FileFormat.Pdf.GetUiDescription();    // "Portable Document Format"
```

### Implementation Notes
- Analyze custom attribute properties and generate additional accessors
- Support multiple property types
- Generate IntelliSense documentation for all accessors
- Keep method naming consistent and predictable

---

## 5. Validation and Constraint Attributes

### Description
Add validation capabilities to ensure enum values meet certain criteria, with compile-time warnings or runtime validation methods.

### Benefits
- Catches configuration errors at compile time
- Ensures consistency across enum declarations
- Provides validation methods for user input
- Self-documenting constraints

### Example Usage
```csharp
[Values<ValueAttribute>]
[ValidateUnique] // Ensures no duplicate values
[ValidateNonEmpty] // Ensures all values are non-empty strings
public enum ErrorCode
{
    [Value("ERR_001")] InvalidInput,
    [Value("ERR_002")] DatabaseError,
    [Value("ERR_003")] NetworkTimeout
}

// Generated validation method:
bool isValid = ErrorCodeValidator.IsValidErrorCode("ERR_001"); // true
string[] allValid = ErrorCodeValidator.GetAllValidErrorCodes(); // ["ERR_001", "ERR_002", "ERR_003"]
```

### Implementation Notes
- Create validation attributes that analyzers can check
- Generate validation helper methods
- Provide compile-time diagnostics for common errors
- Support custom validation rules

---

## 6. Grouped/Categorized Enums

### Description
Support for categorizing or grouping enum values with additional metadata, enabling filtering and organizational capabilities.

### Benefits
- Better organization for large enums
- Enables filtering by category
- Useful for UI grouping and permissions
- Self-documenting enum organization

### Example Usage
```csharp
public class CategoryAttribute : Attribute
{
    public string Category { get; }
    public CategoryAttribute(string category) => Category = category;
}

[Values<ValueAttribute>]
public enum Permission
{
    [Value("user.read"), Category("User")]
    UserRead,
    
    [Value("user.write"), Category("User")]
    UserWrite,
    
    [Value("admin.read"), Category("Admin")]
    AdminRead,
    
    [Value("admin.write"), Category("Admin")]
    AdminWrite
}

// Generated methods:
IEnumerable<Permission> userPerms = Permission.GetValuesByCategory("User");
string category = Permission.UserRead.GetCategory(); // "User"
```

### Implementation Notes
- Support custom grouping attributes
- Generate filtering methods by group
- Integrate with existing Values pattern
- Support multiple categorization schemes

---

## 7. Code Generation Templates and Customization

### Description
Allow developers to customize the generated code through templates or configuration files, including method signatures, null handling, and error messages.

### Benefits
- Greater flexibility for different coding styles
- Team-specific conventions
- Better integration with existing codebases
- Custom error handling strategies

### Example Usage
```csharp
[Values<ValueAttribute>(
    NullHandling = NullHandling.ReturnNull,  // Instead of throwing
    MethodPrefix = "To",                      // ToValue() instead of GetValue()
    GenerateAsyncMethods = true,              // For database lookups
    CustomErrorMessage = "Invalid {EnumType} value: {Value}"
)]
public enum Status
{
    [Value("active")] Active,
    [Value("inactive")] Inactive
}
```

### Implementation Notes
- Extend ValuesAttribute with more configuration options
- Support custom templates via additional files
- Generate async variants when specified
- Allow customization of exception types and messages

---

## 8. Source Generator Incremental Build Optimization

### Description
Optimize the source generator for better incremental build performance, especially in large solutions with many enums.

### Benefits
- Faster development cycle
- Better IDE responsiveness
- Reduced build times in CI/CD
- Better developer experience

### Technical Improvements
- Implement more granular incremental generator stages
- Cache intermediate results more effectively
- Optimize syntax tree traversal
- Reduce allocations in hot paths
- Better diagnostics for generator performance

### Implementation Notes
- Profile current generator performance
- Identify bottlenecks in the pipeline
- Implement proper caching strategies
- Add telemetry for performance monitoring
- Document performance characteristics

---

## 9. Integration with Entity Framework and ORMs

### Description
Provide built-in support for Entity Framework and other ORMs with automatic value converters that use the defined string values for database storage.

### Benefits
- Consistent enum representation in databases
- No need for manual value converters
- Type-safe database queries
- Simplified database migrations

### Example Usage
```csharp
[Values<ValueAttribute>]
[GenerateEfValueConverter] // Generates EF Core value converter
public enum Status
{
    [Value("active")] Active,
    [Value("inactive")] Inactive
}

// In DbContext:
protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
{
    // Auto-registers all generated converters
    configurationBuilder.UseEnumValuesConverters();
}

// Database stores "active" instead of "Active" or 0
```

### Implementation Notes
- Generate value converters for EF Core
- Support common ORM patterns
- Handle null values appropriately
- Provide migration support
- Generate both read and write converters

---

## 10. Enhanced IDE Support and Tooling

### Description
Improve the developer experience with better IDE integration, including quick actions, refactoring support, and enhanced IntelliSense.

### Benefits
- Faster development workflow
- Fewer errors through better tooling
- Better discoverability of features
- Improved code maintainability

### Features
- **Quick Actions**:
  - "Add missing [Value] attributes" for all enum members
  - "Convert to [Flags]" with automatic power-of-two values
  - "Generate reverse lookup method"
  
- **Refactoring Support**:
  - Rename value strings when renaming enum members (with confirmation)
  - Extract enum to separate file with all attributes
  - Convert between different attribute types

- **Enhanced IntelliSense**:
  - Show all possible values in autocomplete
  - Display value strings in tooltips for enum members
  - Warning tooltips for missing or duplicate values

- **Code Lens**:
  - Show number of usages for each enum value
  - Display defined string value next to enum member

### Implementation Notes
- Extend existing analyzer infrastructure
- Add code fix providers for common scenarios
- Implement refactoring providers
- Enhance diagnostic messages with more context
- Add completion providers for better IntelliSense

---

## Priority Recommendation

Based on impact and implementation complexity, the recommended priority order is:

1. **Reverse Lookup (Feature #1)** - High impact, moderate complexity, frequently requested
2. **JSON Serialization Support (Feature #3)** - High impact, moderate complexity, common use case
3. **Display Name and Description (Feature #4)** - High impact, low complexity, UI scenarios
4. **Enhanced IDE Support (Feature #10)** - High impact, high complexity, best developer experience
5. **Validation Attributes (Feature #5)** - Medium impact, low complexity, prevents errors
6. **EF/ORM Integration (Feature #9)** - High impact (for users), moderate complexity, database scenarios
7. **Non-String Values (Feature #2)** - Medium impact, high complexity, niche use cases
8. **Grouped/Categorized Enums (Feature #6)** - Medium impact, moderate complexity, organizational benefits
9. **Code Generation Templates (Feature #7)** - Medium impact, high complexity, advanced scenarios
10. **Build Performance (Feature #8)** - Medium impact, high complexity, optimization

## Conclusion

These features would significantly enhance PodNet.EnumValues by:
- Expanding functionality (reverse lookup, JSON support, ORM integration)
- Improving developer experience (IDE tooling, validation, templates)
- Supporting more use cases (non-string values, categorization, display names)
- Optimizing performance (incremental builds)

Each feature is designed to maintain backward compatibility while adding valuable new capabilities to the library.
