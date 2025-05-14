using System;
using System.Collections.Generic;

namespace SysBot.Base;

/// <summary>
/// Different offset types that can be pointed to for read/write requests.
/// </summary>
public enum SwitchOffsetType
{
    /// <summary>
    /// Heap base offset
    /// </summary>
    Heap,

    /// <summary>
    /// Main NSO base offset
    /// </summary>
    Main,

    /// <summary>
    /// Raw offset (arbitrary)
    /// </summary>
    Absolute,
}

public interface ICommandBuilder
{
    SwitchOffsetType Type { get; }

    byte[] Peek(ulong offset, int length);
    byte[] PeekMulti(IReadOnlyDictionary<ulong, int> offsets);
    byte[] Poke(ulong offset, ReadOnlySpan<byte> data);
}

public static class SwitchOffsetTypeUtil
{
    public static readonly HeapCommand Heap = new();
    public static readonly MainCommand Main = new();
    public static readonly AbsoluteCommand Absolute = new();
}

/// <summary>
/// Heap base offset
/// </summary>
public sealed class HeapCommand : ICommandBuilder
{
    public SwitchOffsetType Type => SwitchOffsetType.Heap;

    public byte[] Peek(ulong offset, int length) => SwitchCommand.Peek((uint)offset, length);

    public byte[] PeekMulti(IReadOnlyDictionary<ulong, int> offsets) => SwitchCommand.PeekMulti(offsets);

    public byte[] Poke(ulong offset, ReadOnlySpan<byte> data) => SwitchCommand.Poke((uint)offset, data);
}

/// <summary>
/// Main NSO base offset
/// </summary>
public sealed class MainCommand : ICommandBuilder
{
    public SwitchOffsetType Type => SwitchOffsetType.Main;

    public byte[] Peek(ulong offset, int length) => SwitchCommand.PeekMain(offset, length);

    public byte[] PeekMulti(IReadOnlyDictionary<ulong, int> offsets) => SwitchCommand.PeekMainMulti(offsets);

    public byte[] Poke(ulong offset, ReadOnlySpan<byte> data) => SwitchCommand.PokeMain(offset, data);
}

/// <summary>
/// Raw offset (arbitrary)
/// </summary>
public sealed class AbsoluteCommand : ICommandBuilder
{
    public SwitchOffsetType Type => SwitchOffsetType.Absolute;

    public byte[] Peek(ulong offset, int length) => SwitchCommand.PeekAbsolute(offset, length);

    public byte[] PeekMulti(IReadOnlyDictionary<ulong, int> offsets) => SwitchCommand.PeekAbsoluteMulti(offsets);

    public byte[] Poke(ulong offset, ReadOnlySpan<byte> data) => SwitchCommand.PokeAbsolute(offset, data);
}
