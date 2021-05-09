namespace CDS.APICore.Bussiness.Abstraction
{
    public interface IParamManager
    {
        void SetValue(string key, object value);

        T GetValue<T>(string key);
    }
}
