namespace CowMatchmaker;

public interface IMatchmaker
{
    MatchmakerResult HandleCommand(MatchmakerCommand command);
    int GetPlayerCount();
}

public abstract record MatchmakerCommand;
public sealed record JoinMatchmaking(Player Player) : MatchmakerCommand;
public sealed record LeaveMatchmaking(Player Player) : MatchmakerCommand;

public abstract record MatchmakerResult;

public abstract record MatchmakerEvent : MatchmakerResult;
public sealed record PlayerJoinedMatchmaking(Player Player) : MatchmakerEvent;
public sealed record PlayerLeftMatchmaking(Player Player) : MatchmakerEvent;
public sealed record MatchMade(Player Player1, Player Player2) : MatchmakerEvent;

public abstract record MatchmakerError : MatchmakerResult;
public sealed record MatchmakingIsFull() : MatchmakerError;
public sealed record PlayerAlreadyInMatchmaking(Player Player) : MatchmakerError;
public sealed record MustBeInMatchmakingToLeave(Player Player) : MatchmakerError;
public sealed record UnknownCommand(string CommandName) : MatchmakerError;
