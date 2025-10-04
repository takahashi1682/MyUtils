namespace TUtils.Countdown
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