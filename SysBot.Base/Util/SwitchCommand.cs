using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SysBot.Base;

/// <summary>
/// Encodes commands to be sent as a <see cref="byte"/> array to a Nintendo Switch running a sys-module.
/// </summary>
public static class SwitchCommand
{
    private static readonly Encoding Encoder = Encoding.ASCII;

    private static byte[] Encode(string command)
        => Encoder.GetBytes($"{command}\r\n");

    private static string ToHex(byte[] data)
        => string.Concat(data.Select(z => $"{z:X2}"));
    private static string Encode(IEnumerable<long> jumps)
        => string.Concat(jumps.Select(z => $" {z}"));
    private static string Encode(IReadOnlyDictionary<ulong, int> offsetSizeDictionary)
        => string.Concat(offsetSizeDictionary.Select(z => $" 0x{z.Key:X16} {z.Value}"));

    private static string ToHex(ReadOnlySpan<byte> data)
    {
        var result = new StringBuilder(data.Length * 2);
        foreach (var b in data)
            result.Append(b.ToString("X2"));
        return result.ToString();
    }

    /// <summary>
    /// Removes the virtual controller from the bot. Allows physical controllers to control manually.
    /// </summary>
    /// <returns>Encoded command bytes</returns>
    public static byte[] DetachController()
        => Encode("detachController");

    /// <summary>
    /// Configures the sys-botbase parameter to the specified value.
    /// </summary>
    /// <returns>Encoded command bytes</returns>
    public static byte[] Configure(SwitchConfigureParameter p, int ms)
        => Encode($"configure {p} {ms}");

    /*
     *
     * Controller Button Commands
     *
     */

    /// <summary>
    /// Presses and releases a <see cref="SwitchButton"/> for 50ms.
    /// </summary>
    /// <remarks>Press &amp; Release timing is performed by the console automatically.</remarks>
    /// <param name="button">Button to click.</param>
    /// <returns>Encoded command bytes</returns>
    public static byte[] Click(SwitchButton button)
        => Encode($"click {button}");

    /// <summary>
    /// Presses and does NOT release a <see cref="SwitchButton"/>.
    /// </summary>
    /// <param name="button">Button to hold.</param>
    /// <returns>Encoded command bytes</returns>
    public static byte[] Hold(SwitchButton button)
        => Encode($"press {button}");

    /// <summary>
    /// Releases the held <see cref="SwitchButton"/>.
    /// </summary>
    /// <param name="button">Button to release.</param>
    /// <returns>Encoded command bytes</returns>
    public static byte[] Release(SwitchButton button)
        => Encode($"release {button}");

    /*
     *
     * Controller Stick Commands
     *
     */

    /// <summary>
    /// Sets the specified <see cref="stick"/> to the desired <see cref="x"/> and <see cref="y"/> positions.
    /// </summary>
    /// <param name="stick">Stick to reset</param>
    /// <param name="x">X position</param>
    /// <param name="y">Y position</param>
    /// <returns>Encoded command bytes</returns>
    public static byte[] SetStick(SwitchStick stick, short x, short y)
        => Encode($"setStick {stick} {x} {y}");

    /// <summary>
    /// Resets the specified <see cref="stick"/> to (0,0)
    /// </summary>
    /// <param name="stick">Stick to reset</param>

    /// <returns>Encoded command bytes</returns>
    public static byte[] ResetStick(SwitchStick stick)
        => SetStick(stick, 0, 0);

    /*
     *
     * Hid Commands
     *
     */

    /// <summary>
    /// Types a keyboard key.
    /// </summary>
    /// <param name="key">Keyboard key to type</param>
    /// <returns>Encoded command bytes</returns>
    public static byte[] TypeKey(HidKeyboardKey key)
        => Encode($"key {(int)key}");

    /// <summary>
    /// Types multiple keyboard keys.
    /// </summary>
    /// <param name="keys">Keyboard keys to type</param>
    /// <returns>Encoded command bytes</returns>
    public static byte[] TypeMultipleKeys(IEnumerable<HidKeyboardKey> keys)
        => Encode($"key{string.Concat(keys.Select(z => $" {(int)z}"))}");

    /*
     *
     * Memory I/O Commands
     *
     */

    /// <summary>
    /// Requests the Bot to send <see cref="count"/> bytes from <see cref="offset"/>.
    /// </summary>
    /// <param name="offset">Address of the data</param>
    /// <param name="count">Amount of bytes</param>
    /// <returns>Encoded command bytes</returns>
    public static byte[] Peek(uint offset, int count)
        => Encode($"peek 0x{offset:X8} {count}");

    /// <summary>
    /// Requests the Bot to send concat bytes from offsets of sizes in the <see cref="offsetSizeDictionary"/> relative to the heap.
    /// </summary>
    /// <param name="offsetSizeDictionary">Dictionary of offset and sizes to be looked up</param>
    /// <returns>Encoded command bytes</returns>
    public static byte[] PeekMulti(IReadOnlyDictionary<ulong, int> offsetSizeDictionary)
        => Encode($"peekMulti{Encode(offsetSizeDictionary)}");

    /// <summary>
    /// Sends the Bot <see cref="data"/> to be written to <see cref="offset"/>.
    /// </summary>
    /// <param name="offset">Address of the data</param>
    /// <param name="data">Data to write</param>
    /// <returns>Encoded command bytes</returns>
    public static byte[] Poke(uint offset, ReadOnlySpan<byte> data)
        => Encode($"poke 0x{offset:X8} 0x{ToHex(data)}");

    /// <summary>
    /// Requests the Bot to send <see cref="count"/> bytes from absolute <see cref="offset"/>.
    /// </summary>
    /// <param name="offset">Absolute address of the data</param>
    /// <param name="count">Amount of bytes</param>
    /// <returns>Encoded command bytes</returns>
    public static byte[] PeekAbsolute(ulong offset, int count)
        => Encode($"peekAbsolute 0x{offset:X16} {count}");

    /// <summary>
    /// Requests the Bot to send concat bytes from offsets of sizes in the <see cref="offsetSizeDictionary"/> in absolute space.
    /// </summary>
    /// <param name="offsetSizeDictionary">Dictionary of offset and sizes to be looked up</param>
    /// <returns>Encoded command bytes</returns>
    public static byte[] PeekAbsoluteMulti(IReadOnlyDictionary<ulong, int> offsetSizeDictionary)
        => Encode($"peekAbsoluteMulti{Encode(offsetSizeDictionary)}");

    /// <summary>
    /// Sends the Bot <see cref="data"/> to be written to absolute <see cref="offset"/>.
    /// </summary>
    /// <param name="offset">Absolute address of the data</param>
    /// <param name="data">Data to write</param>
    /// <returns>Encoded command bytes</returns>
    public static byte[] PokeAbsolute(ulong offset, ReadOnlySpan<byte> data)
        => Encode($"pokeAbsolute 0x{offset:X16} 0x{ToHex(data)}");

    /// <summary>
    /// Requests the Bot to send <see cref="count"/> bytes from main <see cref="offset"/>.
    /// </summary>
    /// <param name="offset">Main NSO address of the data</param>
    /// <param name="count">Amount of bytes</param>
    /// <returns>Encoded command bytes</returns>
    public static byte[] PeekMain(ulong offset, int count)
        => Encode($"peekMain 0x{offset:X16} {count}");

    /// <summary>
    /// Requests the Bot to send concat bytes from offsets of sizes in the <see cref="offsetSizeDictionary"/> relative to the main region.
    /// </summary>
    /// <param name="offsetSizeDictionary">Dictionary of offset and sizes to be looked up</param>
    /// <returns>Encoded command bytes</returns>
    public static byte[] PeekMainMulti(IReadOnlyDictionary<ulong, int> offsetSizeDictionary)
        => Encode($"peekMainMulti{Encode(offsetSizeDictionary)}");

    /// <summary>
    /// Sends the Bot <see cref="data"/> to be written to main <see cref="offset"/>.
    /// </summary>
    /// <param name="offset">Main NSO address of the data</param>
    /// <param name="data">Data to write</param>
    /// <returns>Encoded command bytes</returns>
    public static byte[] PokeMain(ulong offset, ReadOnlySpan<byte> data)
        => Encode($"pokeMain 0x{offset:X16} 0x{ToHex(data)}");

    /*
     *
     * Pointer Commands
     *
     */

    /// <summary>
    /// Requests the Bot to send <see cref="count"/> bytes from pointer traversals defined by <see cref="jumps"/>
    /// </summary>
    /// <param name="jumps">All traversals in the pointer expression</param>
    /// <param name="count">Amount of bytes</param>
    /// <returns>Encoded command bytes</returns>
    public static byte[] PointerPeek(IEnumerable<long> jumps, int count)
        => Encode($"pointerPeek {count}{Encode(jumps)}");

    /// <summary>
    /// Sends the Bot <see cref="data"/> to be written to the offset at the end pointer traversals defined by <see cref="jumps"/>.
    /// </summary>
    /// <param name="jumps">All traversals in the pointer expression</param>
    /// <param name="data">Data to write</param>
    /// <returns>Encoded command bytes</returns>
    public static byte[] PointerPoke(IEnumerable<long> jumps, byte[] data)
        => Encode($"pointerPoke 0x{ToHex(data)}{Encode(jumps)}");

    /// <summary>
    /// Requests the Bot to solve the pointer traversals defined by <see cref="jumps"/> and send the final absolute offset.
    /// </summary>
    /// <param name="jumps">All traversals in the pointer expression</param>
    /// <returns>Encoded command bytes</returns>
    public static byte[] PointerAll(IEnumerable<long> jumps)
        => Encode($"pointerAll{Encode(jumps)}");

    /// <summary>
    /// Requests the Bot to solve the pointer traversals defined by <see cref="jumps"/> and send the final offset relative to the heap region.
    /// </summary>
    /// <param name="jumps">All traversals in the pointer expression</param>
    /// <returns>Encoded command bytes</returns>
    public static byte[] PointerRelative(IEnumerable<long> jumps)
        => Encode($"pointerRelative{Encode(jumps)}");

    /*
     *
     * Process Info Commands
     *
     */

    /// <summary>
    /// Requests the main NSO base of attached process.
    /// </summary>
    /// <returns>Encoded command bytes</returns>
    public static byte[] GetMainNsoBase()
        => Encode("getMainNsoBase");

    /// <summary>
    /// Requests the heap base of attached process.
    /// </summary>
    /// <returns>Encoded command bytes</returns>
    public static byte[] GetHeapBase()
        => Encode("getHeapBase");

    /// <summary>
    /// Requests the title id of attached process.
    /// </summary>
    /// <returns>Encoded command bytes</returns>
    public static byte[] GetTitleID()
        => Encode("getTitleID");

    /// <summary>
    /// Requests the build id of attached process.
    /// </summary>
    /// <returns>Encoded command bytes</returns>
    public static byte[] GetBuildID()
        => Encode("getBuildID");

    /// <summary>
    /// Requests the sys-botbase or usb-botbase version.
    /// </summary>
    /// <returns>Encoded command bytes</returns>
    public static byte[] GetBotbaseVersion()
        => Encode("getVersion");

    /// <summary>
    /// Receives requested information about the currently running game application.
    /// </summary>
    /// <param name="info">Valid parameters and their return types: icon (byte[]), version (string), rating (int), author (string), name (string)</param>
    /// <returns>Encoded command bytes</returns>
    public static byte[] GetGameInfo(ReadOnlySpan<char> info)
        => Encode($"game {info}");

    /// <summary>
    /// Toggles the screen display On/Off, useful for saving power if not needed.
    /// </summary>
    /// <param name="state">Screen state ON</param>
    /// <returns>Encoded command bytes</returns>
    public static byte[] SetScreen(ScreenState state)
        => Encode($"screen{(state == ScreenState.On ? "On" : "Off")}");

    /// <summary>
    /// Checks if a process is running.
    /// </summary>
    /// <param name="pid">Process ID</param>
    /// <returns>Encoded command bytes</returns>
    public static byte[] IsProgramRunning(ulong pid)
        => Encode($"isProgramRunning 0x{pid:x16}");
}
