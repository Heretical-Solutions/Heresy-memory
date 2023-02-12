namespace HereticalSolutions.MVVM
{
    public interface IPropertyModifiedNotifiable<T>
    {
        T Value { set; }
    }
}