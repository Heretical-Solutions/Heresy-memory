namespace HereticalSolutions.MVVM
{
    public interface IViewModel : ILifetimeable
    {
        #region Get observable

        bool GetObservable<T>(string key, out IObservableProperty<T> observable);

        #endregion

        #region Get command

        CommandDelegate GetCommand(string key);
        
        CommandWithArgsDelegate GetCommandWithArguments(string key);
        
        #endregion
    }
}