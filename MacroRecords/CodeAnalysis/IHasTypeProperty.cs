namespace RhoMicro.CodeAnalysis
{
    internal interface IHasTypeProperty
    {
        void SetTypeProperty(System.String propertyName, System.Object type);
        System.Object GetTypeProperty(System.String propertyName);
    }
}
