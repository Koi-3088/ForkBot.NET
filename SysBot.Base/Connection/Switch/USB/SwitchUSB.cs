using LibUsbDotNet;
using LibUsbDotNet.Main;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace SysBot.Base;

/// <summary>
/// Abstract class representing the communication over USB.
/// </summary>
public abstract class SwitchUSB : IConsoleConnection
{
    public string Name { get; }
    public string Label { get; set; }
    public bool Connected { get; protected set; }
    private readonly int Port;

    protected SwitchUSB(int port)
    {
        Port = port;
        Name = Label = $"USB-{port}";
    }

    public void Log(string message) => LogInfo(message);
    public void LogInfo(string message) => LogUtil.LogInfo(message, Label);
    public void LogError(string message) => LogUtil.LogError(message, Label);

    private UsbDevice? SwDevice;
    private UsbEndpointReader? reader;
    private UsbEndpointWriter? writer;

    public int MaximumTransferSize { get; set; } = 1024;
    public int BaseDelay { get; set; } = 10;
    public int DelayFactor { get; set; } = 256;

    private readonly Lock _sync = new();
    private static readonly Lock _registry = new();

    public void Reset()
    {
        Disconnect();
        Connect();
    }

    public void Connect()
    {
        SwDevice = TryFindUSB();
        if (SwDevice == null)
            throw new Exception("USB device not found.");
        if (SwDevice is not IUsbDevice usb)
            throw new Exception("Device is using a WinUSB driver. Use libusbK and create a filter.");

        lock (_sync)
        {
            if (!usb.UsbRegistryInfo.IsAlive)
                usb.ResetDevice();

            if (usb.IsOpen)
                usb.Close();
            usb.Open();

            usb.SetConfiguration(1);
            bool resagain = usb.ClaimInterface(0);
            if (!resagain)
            {
                usb.ReleaseInterface(0);
                usb.ClaimInterface(0);
            }

            reader = SwDevice.OpenEndpointReader(ReadEndpointID.Ep01);
            writer = SwDevice.OpenEndpointWriter(WriteEndpointID.Ep01);
        }
    }

    private UsbDevice? TryFindUSB()
    {
        lock (_registry)
        {
            foreach (var device in UsbDevice.AllLibUsbDevices)
            {
                if (device is not UsbRegistry ur)
                    continue;
                if (ur.Vid != 0x057E)
                    continue;
                if (ur.Pid != 0x3000)
                    continue;

                ur.DeviceProperties.TryGetValue("Address", out var addr);
                if (Port.ToString() != addr?.ToString())
                    continue;

                return ur.Device;
            }
        }
        return null;
    }

    public void Disconnect()
    {
        lock (_sync)
        {
            if (SwDevice is { IsOpen: true } x)
            {
                if (x is IUsbDevice wholeUsbDevice)
                {
                    if (!wholeUsbDevice.UsbRegistryInfo.IsAlive)
                        wholeUsbDevice.ResetDevice();
                    wholeUsbDevice.ReleaseInterface(0);
                }
                x.Close();
            }

            reader?.Dispose();
            writer?.Dispose();
        }
    }

    public int Send(byte[] buffer)
    {
        lock (_sync)
            return SendInternal(buffer);
    }

    protected byte[] Read(ICommandBuilder b, ulong offset, int length)
    {
        var cmd = b.Peek(offset, length);
        SendInternal(cmd);
        return ReadInternal();
    }

    protected byte[] ReadMulti(ICommandBuilder b, IReadOnlyDictionary<ulong, int> offsetSizes)
    {
        var cmd = b.PeekMulti(offsetSizes);
        SendInternal(cmd);
        return ReadInternal();
    }

    protected void Write(ICommandBuilder b, ReadOnlySpan<byte> data, ulong offset)
    {
        var cmd = b.Poke(offset, data);
        SendInternal(cmd);
    }

    protected byte[] ReadInternal()
    {
        lock (_sync)
        {
            if (reader == null)
                throw new Exception("USB device not found or not connected.");

            int total = 0;
            List<byte> data = [];

            do
            {
                Thread.Sleep(1);
                byte[] buffer = new byte[MaximumTransferSize];
                var ec = reader.Read(buffer, 0, buffer.Length, 100, out int received);
                if (ec != ErrorCode.None)
                {
                    Disconnect();
                    throw new Exception(UsbDevice.LastErrorString);
                }

                data.AddRange(buffer.AsSpan(0, received));
                total += received;

            } while (data.Last() != (byte)'\n');

            // Remove the last byte, which is always a terminator
            if (data.Count > 0 && data.Last() == (byte)'\n')
                data.RemoveAt(data.Count - 1);

            return [.. data];
        }
    }

    protected int SendInternal(byte[] buffer)
    {
        lock (_sync)
        {
            if (writer == null)
                throw new Exception("USB device not found or not connected.");

            var ec = writer.Write(buffer, 100, out var sent);
            if (ec != ErrorCode.None || sent != buffer.Length)
            {
                Disconnect();
                throw new Exception(UsbDevice.LastErrorString);
            }

            return sent;
        }
    }
}
