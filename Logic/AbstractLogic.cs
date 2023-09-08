namespace Utils.Logic
{

    /// <summary>
    /// I find myself always writing this pattern where I need to create
    /// some sort of script that will need to be setup, disposed and updated
    /// so I just included it here.
    /// This is useful for state machines, or any other logic that needs to
    /// follow this pattern.
    /// </summary>
    public abstract class AbstractLogic
    {
        protected bool IsSetup;
        protected readonly string Name;

        public AbstractLogic(string name)
        {
            this.Name = name;
            IsSetup = false;
        }

        public void Activate()
        {
            if (IsSetup)
            {
                return;
            }

            OnSetup();

            IsSetup = true;
        }

        public void Deactivate()
        {
            if (!IsSetup)
            {
                return;
            }

            OnDispose();
            IsSetup = false;
        }

        public void Update()
        {
            if (!IsSetup)
            {
                return;
            }
            OnUpdate();
        }

        protected abstract void OnSetup();

        protected abstract void OnUpdate();

        protected abstract void OnDispose();

        public override string ToString()
        {
            return Name;
        }
    }
}