using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static SysBot.Base.SwitchOffsetTypeUtil;

namespace SysBot.Base;

/// <summary>
/// Connection to a Nintendo Switch hosting the sys-module via USB.
/// </summary>
/// <remarks>
/// Interactions are performed asynchronously.
/// </remarks>
public sealed class SwitchUSBAsync(int Port) : SwitchUSB(Port), ISwitchConnectionAsync
{
    public ValueTask<int> SendAsync(byte[] data, CancellationToken token)
    {
        Debug.Assert(data.Length < MaximumTransferSize);
        var res = Task.Run(() => Send(data), token);
        return new ValueTask<int>(res);
    }

    public Task<byte[]> ReadBytesAsync(uint offset, int length, CancellationToken token) => Task.Run(() => Read(Heap, offset, length), token);
    public Task<byte[]> ReadBytesMainAsync(ulong offset, int length, CancellationToken token) => Task.Run(() => Read(Main, offset, length), token);
    public Task<byte[]> ReadBytesAbsoluteAsync(ulong offset, int length, CancellationToken token) => Task.Run(() => Read(Absolute, offset, length), token);

    public Task<byte[]> ReadBytesMultiAsync(IReadOnlyDictionary<ulong, int> offsetSizes, CancellationToken token) => Task.Run(() => ReadMulti(Heap, offsetSizes), token);
    public Task<byte[]> ReadBytesMainMultiAsync(IReadOnlyDictionary<ulong, int> offsetSizes, CancellationToken token) => Task.Run(() => ReadMulti(Main, offsetSizes), token);
    public Task<byte[]> ReadBytesAbsoluteMultiAsync(IReadOnlyDictionary<ulong, int> offsetSizes, CancellationToken token) => Task.Run(() => ReadMulti(Absolute, offsetSizes), token);

    public Task WriteBytesAsync(byte[] data, uint offset, CancellationToken token) => Task.Run(() => Write(Heap, data, offset), token);
    public Task WriteBytesMainAsync(byte[] data, ulong offset, CancellationToken token) => Task.Run(() => Write(Main, data, offset), token);
    public Task WriteBytesAbsoluteAsync(byte[] data, ulong offset, CancellationToken token) => Task.Run(() => Write(Absolute, data, offset), token);

    public Task<ulong> GetMainNsoBaseAsync(CancellationToken token)
    {
        return Task.Run(() =>
        {
            Send(SwitchCommand.GetMainNsoBase());
            byte[] baseBytes = ReadInternal();
            return BitConverter.ToUInt64(baseBytes, 0);
        }, token);
    }

    public Task<ulong> GetHeapBaseAsync(CancellationToken token)
    {
        return Task.Run(() =>
        {
            Send(SwitchCommand.GetHeapBase());
            byte[] baseBytes = ReadInternal();
            return BitConverter.ToUInt64(baseBytes, 0);
        }, token);
    }

    public Task<string> GetTitleID(CancellationToken token)
    {
        return Task.Run(() =>
        {
            Send(SwitchCommand.GetTitleID());
            byte[] baseBytes = ReadInternal();
            return BitConverter.ToUInt64(baseBytes, 0).ToString("X16").Trim();
        }, token);
    }

    public Task<string> GetBotbaseVersion(CancellationToken token)
    {
        return Task.Run(() =>
        {
            Send(SwitchCommand.GetBotbaseVersion());
            byte[] baseBytes = ReadInternal();
            return Encoding.UTF8.GetString(baseBytes).Trim('\0');
        }, token);
    }

    public Task<string> GetGameInfo(string info, CancellationToken token)
    {
        return Task.Run(() =>
        {
            Send(SwitchCommand.GetGameInfo(info));
            byte[] baseBytes = ReadInternal();
            return Encoding.UTF8.GetString(baseBytes).Trim('\0');
        }, token);
    }

    public Task<bool> IsProgramRunning(ulong pid, CancellationToken token)
    {
        return Task.Run(() =>
        {
            Send(SwitchCommand.IsProgramRunning(pid));
            byte[] baseBytes = ReadInternal();
            return baseBytes.Length == 1 && BitConverter.ToBoolean(baseBytes, 0);
        }, token);
    }

    public Task<byte[]> PointerPeek(int size, IEnumerable<long> jumps, CancellationToken token)
    {
        return Task.Run(() =>
        {
            Send(SwitchCommand.PointerPeek(jumps, size));
            return ReadInternal();
        }, token);
    }

    public Task PointerPoke(byte[] data, IEnumerable<long> jumps, CancellationToken token)
    {
        return Task.Run(() => Send(SwitchCommand.PointerPoke(jumps, data)), token);
    }

    public Task<ulong> PointerAll(IEnumerable<long> jumps, CancellationToken token)
    {
        return Task.Run(() =>
        {
            Send(SwitchCommand.PointerAll(jumps));
            byte[] baseBytes = ReadInternal();
            return BitConverter.ToUInt64(baseBytes, 0);
        }, token);
    }

    public Task<ulong> PointerRelative(IEnumerable<long> jumps, CancellationToken token)
    {
        return Task.Run(() =>
        {
            Send(SwitchCommand.PointerRelative(jumps));
            byte[] baseBytes = ReadInternal();
            return BitConverter.ToUInt64(baseBytes, 0);
        }, token);
    }
}
