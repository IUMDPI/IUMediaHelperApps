using System.Collections.Generic;

namespace Packager.Validators
{
    public interface IValidatorCollection : ICollection<IValidator>
    {
        ValidationResults Validate(object instance);
    }
}