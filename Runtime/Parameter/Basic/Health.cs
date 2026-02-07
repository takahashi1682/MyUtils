namespace MyUtils.Parameter.Basic
{
    public interface IHealth : IIntParameter
    {
    }
    
    public class Health : AbstractIntParameter, IHealth
    {
    }
}