using System;
using System.Threading;
using LibUsbDotNet;
using LibUsbDotNet.Main;

namespace SysBot.Base
{
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

        public int MaximumTransferSize { get; set; } = 0x1C0;
        public int BaseDelay { get; set; } = 1;
        public int DelayFactor { get; set; } = 1000;

        private readonly object _sync = new();
        private static readonly object _registry = new();

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
                foreach (UsbRegistry ur in UsbDevice.AllLibUsbDevices)
                {
                    if (ur.Vid != 0x057E)
                        continue;
                    if (ur.Pid != 0x3000)
                        continue;

                    ur.DeviceProperties.TryGetValue("Address", out object addr);
                    if (Port.ToString() != addr.ToString())
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
                if (SwDevice != null)
                {
                    if (SwDevice.IsOpen)
                    {
                        if (SwDevice is IUsbDevice wholeUsbDevice)
                            wholeUsbDevice.ReleaseInterface(0);
                        SwDevice.Close();
                    }
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

        protected byte[] Read()
        {
            lock (_sync)
            {
                byte[] sizeOfReturn = new byte[4];
                if (reader == null)
                    throw new Exception("USB device not found or not connected.");

                reader.Read(sizeOfReturn, 5000, out _);
                Thread.Sleep(1);

                var size = BitConverter.ToInt32(sizeOfReturn, 0);
                byte[] buffer = new byte[size];
                var buffSize = reader.ReadBufferSize;
                int transferredSize = 0;

                while (transferredSize < size)
                {
                    reader.Read(buffer, transferredSize, buffSize, 5000, out var lenVal);
                    transferredSize += lenVal;
                    Thread.Sleep(1);
                }
                return buffer;
            }
        }

        protected byte[] Read(ulong offset, int length, Func<ulong, int, byte[]> method)
        {
            lock (_sync)
            {
                var cmd = method(offset, length);
                SendInternal(cmd);
                Thread.Sleep(1);
                return Read();
            }
        }

        protected void Write(byte[] data, ulong offset, Func<ulong, byte[], byte[]> method)
        {
            if (data.Length > MaximumTransferSize)
                WriteLarge(data, offset, method);
            else WriteSmall(data, offset, method);
        }

        protected void WriteSmall(byte[] data, ulong offset, Func<ulong, byte[], byte[]> method)
        {
            lock (_sync)
            {
                var cmd = method(offset, data);
                SendInternal(cmd);
                Thread.Sleep(1);
            }
        }

        private void WriteLarge(byte[] data, ulong offset, Func<ulong, byte[], byte[]> method)
        {
            int byteCount = data.Length;
            for (int i = 0; i < byteCount; i += MaximumTransferSize)
            {
                var slice = data.SliceSafe(i, MaximumTransferSize);
                Write(slice, offset + (uint)i, method);
                Thread.Sleep(MaximumTransferSize / DelayFactor + BaseDelay);
            }
        }

        private int SendInternal(byte[] buffer)
        {
            if (writer == null)
                throw new Exception("USB device not found or not connected.");

            int pack = buffer.Length + 2;
            var ec = writer.Write(BitConverter.GetBytes(pack), 2000, out _);
            if (ec != ErrorCode.None)
            {
                Disconnect();
                throw new Exception(UsbDevice.LastErrorString);
            }
            ec = writer.Write(buffer, 2000, out var l);
            if (ec != ErrorCode.None)
            {
                Disconnect();
                throw new Exception(UsbDevice.LastErrorString);
            }
            return l;
        }
    }
}