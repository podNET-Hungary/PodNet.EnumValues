Assert.AreEqual("R|W", (Permissions.Read | Permissions.Write).GetValue());
Assert.AreEqual("RW", (Permissions.Read | Permissions.Write).GetValue(""));
Assert.AreEqual("R + W", (Permissions.Read | Permissions.Write).GetValue(" + "));
