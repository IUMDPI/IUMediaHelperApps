namespace Packager.Factories
{
    public abstract class AbstractPodMetadataFactory<T> : IPodMetadataFactory<T> 
    {
        public abstract T Generate(string xml);

    }
}