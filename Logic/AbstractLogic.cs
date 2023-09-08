namespace Utils.Logic
{

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