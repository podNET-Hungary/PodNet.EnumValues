using PodNet.EnumValues.CodeFixes;
using PodNet.EnumValues.Generator;

namespace PodNet.EnumValues.Tests;

[TestClass]
public static class EnumValuesGeneratorTests
{
    [AssemblyInitialize]
#pragma warning disable IDE0060 // Remove unused parameter
    public static void AssemblyInitialize(TestContext testContext)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        // Need to trigger loading the EnumValues.Core assembly containing the marker attributes to
        // prevent compilation errors when it's not being referenced correctly at other parts of the code.
        // User code doesn't have to do this, obviously.
        _ = typeof(ValuesAttribute<ValueAttribute>).Assembly;
    }

    [TestClass] public class _1_DefaultHappyPath : EnumValuesTestCase<Cases._1_DefaultHappyPath>;
    [TestClass] public class _2_DefaultUndefinedValue() : EnumValuesTestCase<Cases._2_DefaultUndefinedValue>.WithCodeFix<MissingEnumValueCodeFix>([EnumValuesGenerator.MissingEnumValueDescriptor]);
    [TestClass] public class _3_UserSuppliedDescendantAttribute : EnumValuesTestCase<Cases._3_UserSuppliedDescendantAttribute>;
    [TestClass] public class _4_MultipleLookups : EnumValuesTestCase<Cases._4_MultipleLookups>;
    [TestClass] public class _5_Configuration_1_DefaultCase : EnumValuesTestCase<Cases._5_Configuration._1_DefaultCase>;
    [TestClass] public class _6_Flags_1_DefaultCase : EnumValuesTestCase<Cases._6_Flags._1_DefaultCase>;
    [TestClass] public class _6_Flags_2_FlagsAttribute() : EnumValuesTestCase<Cases._6_Flags._2_FlagsAttribute>(ignoreSources: true);
    [TestClass] public class _6_Flags_3_Property() : EnumValuesTestCase<Cases._6_Flags._3_Property>(ignoreSources: true);
    [TestClass] public class _6_Flags_4_CustomSeparator() : EnumValuesTestCase<Cases._6_Flags._4_CustomSeparator>(ignoreSources: true);
    [TestClass] public class _6_Flags_5_MissingFlagWarning() : EnumValuesTestCase<Cases._6_Flags._5_MissingFlagWarning>([EnumValuesGenerator.UndefinedEnumFlagMemberDescriptor], true);
    [TestClass] public class _6_Flags_6_MissingNoneWarning() : EnumValuesTestCase<Cases._6_Flags._6_MissingNoneWarning>([EnumValuesGenerator.UndefinedEnumFlagMemberDescriptor], true);
    [TestClass] public class _7_GenericEnumIsNotSupportedWarning() : EnumValuesTestCase<Cases._7_GenericEnumIsNotSupportedWarning>.WithCodeFix<EnumTypeInGenericCodeFix>([EnumValuesGenerator.EnumTypesInGenericsNotSupportedDescriptor], true);
    [TestClass] public class _8_EnumAliasesCanBeUnstableWarning() : EnumValuesTestCase<Cases._8_EnumAliasesCanBeUnstableWarning>([EnumValuesGenerator.RemoveDuplicateEnumValueDescriptor], true);
}
