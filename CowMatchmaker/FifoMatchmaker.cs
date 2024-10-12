

namespace CowMatchmaker;

public class FifoMatchmaker : IMatchmaker
{
    private Player? _queuedPlayer;

    public FifoMatchmaker()
    {
        _queuedPlayer = null;
    }

    public FifoMatchmaker(Player player)
    {
        _queuedPlayer = player;
    }

    public int GetPlayerCount()
    {
        return _queuedPlayer is null ? 0 : 1;
    }

    public MatchmakerResult HandleCommand(MatchmakerCommand command)
    {
        return command switch
        {
            JoinMatchmaking c => HandleJoinMatchmaking(c),
            LeaveMatchmaking c => HandleLeaveMatchmaking(c),
            _ => new UnknownCommand(command.GetType().Name),
        };
    }

    private MatchmakerResult HandleJoinMatchmaking(JoinMatchmaking c)
    {
        if (_queuedPlayer is null)
        {
            _queuedPlayer = c.Player;
            return new PlayerJoinedMatchmaking(c.Player);
        }
        else if (_queuedPlayer.Id == c.Player.Id)
        {
            return new PlayerAlreadyInMatchmaking(c.Player);
        }
        else
        {
            var result = new MatchMade(_queuedPlayer, c.Player);
            _queuedPlayer = null;

            return result;
        }
    }

    private MatchmakerResult HandleLeaveMatchmaking(LeaveMatchmaking c)
    {
        if (_queuedPlayer is not null && _queuedPlayer.Id == c.Player.Id)
        {
            _queuedPlayer = null;
            return new PlayerLeftMatchmaking(c.Player);
        }

        return new MustBeInMatchmakingToLeave(c.Player);
    }

}