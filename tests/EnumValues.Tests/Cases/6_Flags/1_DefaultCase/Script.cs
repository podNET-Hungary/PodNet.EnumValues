using System.Reflection;
using System.Runtime.Loader;

Assert.AreEqual("RW", Permissions.ReadWrite.GetValue());
Assert.AreEqual("R|W", Permissions.ReadWrite.GetValue("|"));
Assert.AreEqual("A", (Permissions.Read | Permissions.Write | Permissions.Execute).GetValue());
Assert.AreEqual("R|W|X", (Permissions.Read | Permissions.Write | Permissions.Execute).GetValue("|"));
Assert.AreEqual("RWX8", ((Permissions)15).GetValue());
Assert.AreEqual("R|W|X|8", ((Permissions)15).GetValue("|"));
Assert.AreEqual("!", Permissions.Invalid.GetValue());
Assert.AreEqual("-", Permissions.None.GetValue());
Assert.AreEqual("R | W | X | 8 | 16 | 32 | 64 | 128", ((Permissions)255).GetValue(" | "));
Assert.ThrowsException<ArgumentOutOfRangeException>(() => ((Permissions)(-2)).GetValue());