namespace Whaally.Domain;

[Obsolete("Use the `DomainContext` instead. This class will be removed in the next major release")]
public class Domain : DomainContext
{
    public Domain(IServiceProvider services) : base(services)
    {
    }
}