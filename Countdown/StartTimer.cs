namespace MyUtils.Countdown
{
    public interface IStartTimerObservable : IBasicTimerObservable
    {
    }

    public interface IStartTimerHandler : IBasicTimerHandler
    {
    }

    public class StartTimer : BasicTimer, IStartTimerObservable, IStartTimerHandler
    {
    }
}