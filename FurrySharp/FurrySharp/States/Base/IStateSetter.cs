namespace FurrySharp.States
{
    public interface IStateSetter
    {
        public void CreateAndSetState<T>() where T : State, new();
    }
}

