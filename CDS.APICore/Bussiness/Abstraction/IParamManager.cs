namespace CDS.APICore.Bussiness.Abstraction
{
    public interface IParamManager
    {
        void SetValue(string key, string value);

        string GetValue(string key);
    }
}
