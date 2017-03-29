namespace JW8307A
{
    public class SerialPortEventArgs
    {
        public delegate void ReadValue(byte[] data);

        public delegate void GetConnect(byte[] data);
    }
}