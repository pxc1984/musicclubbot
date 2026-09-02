namespace CuMusicClub.Web.Endpoints.v1.Users;

public static partial class Users
{
    public static void Map(RouteGroupBuilder group)
    {
        group.RequireAuthorization();

        group.MapGet("/{userId:guid}", Get);
    }
}
