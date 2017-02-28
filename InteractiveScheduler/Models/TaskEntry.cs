using Common.TaskScheduler.Configurations;

namespace InteractiveScheduler.Models
{
    public abstract class AbstractOperation
    {
        public abstract string Name { get; }
        public AbstractConfiguration Value { get; protected set; }
    }

    public class EditExistingOperation:AbstractOperation
    {
        public EditExistingOperation(AbstractConfiguration configuration)
        {
            Value = configuration;
        }

        public override string Name => $"Edit {Value.TaskName}";
    }

    public class AddNewOperation : AbstractOperation
    {
        public override string Name => "Add new scheduled task";

        public AddNewOperation(AbstractConfiguration configuration)
        {
            Value = configuration;
        }
    }
}
