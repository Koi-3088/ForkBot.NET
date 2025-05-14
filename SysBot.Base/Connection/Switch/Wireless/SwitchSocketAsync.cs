using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static SysBot.Base.SwitchOffsetTypeUtil;

namespace SysBot.Base;

/// <summary>
/// Connection to a Nintendo Switch hosting the sys-module via a socket (Wi-Fi).
/// </summary>
/// <remarks>
/// Interactions are performed asynchronously.
/// </remarks>
public sealed class SwitchSocketAsync : SwitchSocket, ISwitchConnectionAsync
{
    private SwitchSocketAsync(IWirelessConnectionConfig cfg) : base(cfg) { }

    public static SwitchSocketAsync CreateInstance(IWirelessConnectionConfig cfg)
    {
        return new SwitchSocketAsync(cfg);
    }

    public override void Connect()
    {
        if (Connected)
        {
            Log("Already connected prior, skipping initial connection.");
            return;
        }

        Log("Connecting to device...");
        IAsyncResult result = Connection.BeginConnect(Info.IP, Info.Port, null, null);
        bool success = result.AsyncWaitHandle.WaitOne(5000, true);
        if (!success || !Connection.Connected)
        {
            InitializeSocket();
            throw new Exception("Failed to connect to device.");
        }
        Connection.EndConnect(result);
        Log("Connected!");
        Label = Name;
    }

    public override void Reset()
    {
        if (Connected)
            Disconnect();
        else
            InitializeSocket();
        Connect();
    }

    public override void Disconnect()
    {
        Log("Disconnecting from device...");
        IAsyncResult result = Connection.BeginDisconnect(false, null, null);
        bool success = result.AsyncWaitHandle.WaitOne(5000, true);
        if (!success || Connection.Connected)
        {
            InitializeSocket();
            throw new Exception("Failed to disconnect from device.");
        }
        Connection.EndDisconnect(result);
        Log("Disconnected! Resetting Socket.");
        InitializeSocket();
    }

    public async ValueTask<int> SendAsync(byte[] buffer, CancellationToken token)
    {
        int total = 0;
        while (total < buffer.Length)
        {
            var chunk = new ArraySegment<byte>(buffer, total, buffer.Length - total);
            int sent = await Connection.SendAsync(chunk, token).ConfigureAwait(false);
            if (sent == 0)
            {
                Log("SendAsync() error: connection closed by the server.");
                break;
            }

            total += sent;
        }

        return total;
    }

    public Task<byte[]> ReadBytesAsync(uint offset, int length, CancellationToken token) => Read(Heap, offset, length, token);
    public Task<byte[]> ReadBytesMainAsync(ulong offset, int length, CancellationToken token) => Read(Main, offset, length, token);
    public Task<byte[]> ReadBytesAbsoluteAsync(ulong offset, int length, CancellationToken token) => Read(Absolute, offset, length, token);

    public Task<byte[]> ReadBytesMultiAsync(IReadOnlyDictionary<ulong, int> offsetSizes, CancellationToken token) => ReadMulti(Heap, offsetSizes, token);
    public Task<byte[]> ReadBytesMainMultiAsync(IReadOnlyDictionary<ulong, int> offsetSizes, CancellationToken token) => ReadMulti(Main, offsetSizes, token);
    public Task<byte[]> ReadBytesAbsoluteMultiAsync(IReadOnlyDictionary<ulong, int> offsetSizes, CancellationToken token) => ReadMulti(Absolute, offsetSizes, token);

    public Task WriteBytesAsync(byte[] data, uint offset, CancellationToken token) => Write(Heap, data, offset, token);
    public Task WriteBytesMainAsync(byte[] data, ulong offset, CancellationToken token) => Write(Main, data, offset, token);
    public Task WriteBytesAbsoluteAsync(byte[] data, ulong offset, CancellationToken token) => Write(Absolute, data, offset, token);

    public async Task<ulong> GetMainNsoBaseAsync(CancellationToken token)
    {
        byte[] baseBytes = await ReadAsync(SwitchCommand.GetMainNsoBase(), token).ConfigureAwait(false);
        return BitConverter.ToUInt64(baseBytes, 0);
    }

    public async Task<ulong> GetHeapBaseAsync(CancellationToken token)
    {
        var baseBytes = await ReadAsync(SwitchCommand.GetHeapBase(), token).ConfigureAwait(false);
        return BitConverter.ToUInt64(baseBytes, 0);
    }

    public async Task<string> GetTitleID(CancellationToken token)
    {
        var bytes = await ReadAsync(SwitchCommand.GetTitleID(), token).ConfigureAwait(false);
        return BitConverter.ToUInt64(bytes, 0).ToString("X16").Trim();
    }

    public async Task<string> GetBotbaseVersion(CancellationToken token)
    {
        var bytes = await ReadAsync(SwitchCommand.GetBotbaseVersion(), token).ConfigureAwait(false);
        return Encoding.ASCII.GetString(bytes).Trim('\0');
    }

    public async Task<string> GetGameInfo(string info, CancellationToken token)
    {
        var bytes = await ReadAsync(SwitchCommand.GetGameInfo(info), token).ConfigureAwait(false);
        return Encoding.ASCII.GetString(bytes).Trim('\0', '\n');
    }

    public async Task<bool> IsProgramRunning(ulong pid, CancellationToken token)
    {
        var bytes = await ReadAsync(SwitchCommand.IsProgramRunning(pid), token).ConfigureAwait(false);
        return ulong.TryParse(Encoding.ASCII.GetString(bytes).Trim(), out var value) && value == 1;
    }

    private async Task<byte[]> Read(ICommandBuilder b, ulong offset, int length, CancellationToken token)
    {
        var cmd = b.Peek(offset, length);
        return await ReadAsync(cmd, token).ConfigureAwait(false);
    }

    private async Task<byte[]> ReadMulti(ICommandBuilder b, IReadOnlyDictionary<ulong, int> offsetSizes, CancellationToken token)
    {
        var cmd = b.PeekMulti(offsetSizes);
        return await ReadAsync(cmd, token).ConfigureAwait(false);
    }

    private async Task Write(ICommandBuilder b, byte[] data, ulong offset, CancellationToken token)
    {
        var cmd = b.Poke(offset, data);
        await SendAsync(cmd, token).ConfigureAwait(false);
    }

    public async Task<byte[]> ReadAsync(byte[] command, CancellationToken token)
    {
        List<byte> data = [];
        await SendAsync(command, token).ConfigureAwait(false);
        await Task.Delay(Connection.ReceiveBufferSize / DelayFactor + BaseDelay, token).ConfigureAwait(false);

        do
        {
            int available = Connection.Available;
            if (available <= 0)
                break;

            byte[] buffer = new byte[available];
            try
            {
                int received = await Connection.ReceiveAsync(buffer, token).ConfigureAwait(false);
                if (received == 0)
                    break;

                data.AddRange(buffer);
            }
            catch (Exception ex)
            {
                LogError($"Socket exception thrown while receiving data:\n{ex.Message}");
                return [];
            }

        } while (data.Last() != (byte)'\n');

        // Remove the last byte, which is always a terminator
        if (data.Count > 0 && data.Last() == (byte)'\n')
            data.RemoveAt(data.Count - 1);

        return [.. data];
    }

    public async Task<byte[]> PointerPeek(int size, IEnumerable<long> jumps, CancellationToken token)
    {
        return await ReadAsync(SwitchCommand.PointerPeek(jumps, size), token).ConfigureAwait(false);
    }

    public async Task PointerPoke(byte[] data, IEnumerable<long> jumps, CancellationToken token)
    {
        await SendAsync(SwitchCommand.PointerPoke(jumps, data), token).ConfigureAwait(false);
    }

    public async Task<ulong> PointerAll(IEnumerable<long> jumps, CancellationToken token)
    {
        var offsetBytes = await ReadAsync(SwitchCommand.PointerAll(jumps), token).ConfigureAwait(false);
        return BitConverter.ToUInt64(offsetBytes, 0);
    }

    public async Task<ulong> PointerRelative(IEnumerable<long> jumps, CancellationToken token)
    {
        var offsetBytes = await ReadAsync(SwitchCommand.PointerRelative(jumps), token).ConfigureAwait(false);
        return BitConverter.ToUInt64(offsetBytes, 0);
    }
}
