namespace TUtils.Countdown
{
    public interface IGameTimerObservable : IBasicTimerObservable
    {
    }

    public interface IGameTimerHandler : IBasicTimerHandler
    {
    }

    public class GameTimer : BasicTimer, IGameTimerObservable, IGameTimerHandler
    {
    }
}