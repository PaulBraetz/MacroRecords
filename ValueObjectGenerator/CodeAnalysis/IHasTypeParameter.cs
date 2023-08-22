namespace RhoMicro.CodeAnalysis
{
    internal interface IHasTypeParameter
    {
        void SetTypeParameter(System.String parameterName, System.Object type);
        System.Object GetTypeParameter(System.String parameterName);
    }
}
