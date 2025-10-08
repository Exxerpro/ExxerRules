namespace IndTrace.Models.UnitTests.S7.Rx
{
    // Interfaces to abstract dependencies
    public interface IS7MultiVar
    {
        void Add(int operand, int type, int dbNo, int start, int length, ref byte[] buffer);

        int Write();
    }

    public interface IValueConverter
    {
        bool TryWriteToBuffer(byte[] buffer, object value, Type type, VariableAddress address);
    }
}
