namespace MyUtils.Parameter.Basic
{
    public interface ILevel : IIntParameter
    {
    }

    public class Level : AbstractIntParameter, ILevel
    {
    }
}