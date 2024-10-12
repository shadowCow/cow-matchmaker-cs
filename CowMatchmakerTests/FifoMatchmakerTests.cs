using System.Security.Cryptography;
using CowMatchmaker;

namespace CowMatchmakerTests;

public class FifoMatchmakerTests
{
    [Fact]
    public void JoiningAnEmptyQueue_DoesNotMakeMatch()
    {
        var matchmaker = Given.AnEmptyMatchmaker();
        var player = Given.Player1;

        var result = When.PlayerAttemptsToJoinMatchmaking(player, matchmaker);

        Then.ResultIsPlayerJoinedMatchmaking(result, player);
        Then.MatchmakerHasPlayerCount(matchmaker, 1);
    }

    [Fact]
    public void JoiningAPopulatedQueue_MakesMatch()
    {
        var player1 = Given.Player1;
        var player2 = Given.Player2;
        var matchmaker = Given.APopulatedMatchmaker(player1);

        var result = When.PlayerAttemptsToJoinMatchmaking(player2, matchmaker);

        Then.ResultIsMatchMade(result, player1, player2);
        Then.MatchmakerHasPlayerCount(matchmaker, 0);
    }

    [Fact]
    public void JoiningAQueueYouAreIn_Fails()
    {
        var player1 = Given.Player1;
        var matchmaker = Given.APopulatedMatchmaker(player1);

        var result = When.PlayerAttemptsToJoinMatchmaking(player1, matchmaker);

        Then.ResultIsPlayerAlreadyInMatchmaking(result, player1);
        Then.MatchmakerHasPlayerCount(matchmaker, 1);
    }

    [Fact]
    public void LeavingAQueueYouAreIn_Succeeds()
    {
        var player1 = Given.Player1;
        var matchmaker = Given.APopulatedMatchmaker(player1);

        var result = When.PlayerAttemptsToLeaveMatchmaking(player1, matchmaker);

        Then.ResultIsPlayerLeftMatchmaking(result, player1);
        Then.MatchmakerHasPlayerCount(matchmaker, 0);
    }

    [Fact]
    public void LeavingAnEmptyQueue_Fails()
    {
        var player1 = Given.Player1;
        var matchmaker = Given.AnEmptyMatchmaker();

        var result = When.PlayerAttemptsToLeaveMatchmaking(player1, matchmaker);

        Then.ResultIsMustBeInMatchmakingToLeave(result, player1);
        Then.MatchmakerHasPlayerCount(matchmaker, 0);
    }

    [Fact]
    public void LeavingAQueueYouAreNotIn_Fails()
    {
        var player1 = Given.Player1;
        var player2 = Given.Player2;
        var matchmaker = Given.APopulatedMatchmaker(player2);

        var result = When.PlayerAttemptsToLeaveMatchmaking(player1, matchmaker);

        Then.ResultIsMustBeInMatchmakingToLeave(result, player1);
        Then.MatchmakerHasPlayerCount(matchmaker, 1);
    }
}

static class Given
{
    internal static readonly Player Player1 = new("p1");
    internal static readonly Player Player2 = new("p2");

    internal static FifoMatchmaker AnEmptyMatchmaker()
    {
        var m = new FifoMatchmaker();
        Assert.Equal(0, m.GetPlayerCount());
        
        return m;
    }

    internal static FifoMatchmaker APopulatedMatchmaker(Player player)
    {
        var m = new FifoMatchmaker(player);
        Assert.Equal(1, m.GetPlayerCount());

        return m;
    }
}

static class When
{
    internal static MatchmakerResult PlayerAttemptsToJoinMatchmaking(Player player, FifoMatchmaker matchmaker)
    {
        return matchmaker.HandleCommand(new JoinMatchmaking(player));
    }

    internal static object PlayerAttemptsToLeaveMatchmaking(Player player, FifoMatchmaker matchmaker)
    {
        return matchmaker.HandleCommand(new LeaveMatchmaking(player));
    }
}

static class Then
{
    internal static void ResultIsPlayerJoinedMatchmaking(MatchmakerResult result, Player player)
    {
        var r = Assert.IsType<PlayerJoinedMatchmaking>(result);
        Assert.Equal(player, r.Player);
    }

    internal static void ResultIsMatchMade(MatchmakerResult result, Player player1, Player player2)
    {
        var r = Assert.IsType<MatchMade>(result);
        Assert.Equal(player1, r.Player1);
        Assert.Equal(player2, r.Player2);
    }

    internal static void MatchmakerHasPlayerCount(IMatchmaker matchmaker, int count)
    {
        Assert.Equal(count, matchmaker.GetPlayerCount());
    }

    internal static void ResultIsPlayerLeftMatchmaking(object result, Player player)
    {
        var r = Assert.IsType<PlayerLeftMatchmaking>(result);
        Assert.Equal(player, r.Player);
    }

    internal static void ResultIsMustBeInMatchmakingToLeave(object result, Player player)
    {
        var r = Assert.IsType<MustBeInMatchmakingToLeave>(result);
        Assert.Equal(player, r.Player);
    }

    internal static void ResultIsPlayerAlreadyInMatchmaking(MatchmakerResult result, Player player)
    {
        var r = Assert.IsType<PlayerAlreadyInMatchmaking>(result);
        Assert.Equal(player, r.Player);
    }
}