using System;

namespace Dissonance.Engine;

[AttributeUsage(AttributeTargets.Method)]
public class SystemAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Method)]
public sealed class WorldSystemAttribute : SystemAttribute { }

[AttributeUsage(AttributeTargets.Method)]
public sealed class EntitySystemAttribute : SystemAttribute { }

[AttributeUsage(AttributeTargets.Method)]
public sealed class MessageSystemAttribute : SystemAttribute { }

[AttributeUsage(AttributeTargets.Method)]
public sealed class IgnoredSystemAttribute : Attribute { }
