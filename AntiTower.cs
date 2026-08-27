using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Utils;

namespace AntiTower;

[MinimumApiVersion(132)]
public class AntiTower : BasePlugin
{
    public override string ModuleName => "AntiTower";
    public override string ModuleAuthor => "v4r1able (adapted by zaadrot.uz)";
    public override string ModuleDescription => "Prevents players from climbing a blocked ladder/tower on a specific map";
    public override string ModuleVersion => "1.1.0";

    private const string TargetMap = "awp_lego_2";

    // Триггер-зона (запретное место на лестнице)
    private readonly Vector triggerPosition = new Vector(1709.369263f, 245.855865f, 64.671875f);
    private readonly float triggerRadius = 30f;

    // Куда телепортировать игрока (чуть ниже верха, "до запрета")
    private readonly Vector targetPosition = new Vector(1704.664185f, 244.116699f, 82.937500f);
    private readonly QAngle targetAngle = new QAngle(23.095661f, -84.417412f, 0.0f);

    private bool isOnTargetMap = false;

    public override void Load(bool hotReload)
    {
        isOnTargetMap = string.Equals(Server.MapName, TargetMap, StringComparison.OrdinalIgnoreCase);

        RegisterListener<Listeners.OnMapStart>(mapName =>
        {
            isOnTargetMap = string.Equals(mapName, TargetMap, StringComparison.OrdinalIgnoreCase);
        });

        RegisterListener<Listeners.OnTick>(() =>
        {
            if (!isOnTargetMap)
                return;

            foreach (var player in Utilities.GetPlayers())
            {
                if (player == null || !player.IsValid || player.PlayerPawn == null || !player.PawnIsAlive)
                    continue;

                var pos = player.PlayerPawn.Value.AbsOrigin;
                if (pos == null)
                    continue;

                if (CalculateDistance(pos, triggerPosition) <= triggerRadius)
                {
                    player.PlayerPawn.Value.Teleport(targetPosition, targetAngle, new Vector(0, 0, 0));
                }
            }
        });
    }

    private float CalculateDistance(Vector v1, Vector v2)
    {
        var diffX = v1.X - v2.X;
        var diffY = v1.Y - v2.Y;
        var diffZ = v1.Z - v2.Z;
        return MathF.Sqrt(diffX * diffX + diffY * diffY + diffZ * diffZ);
    }
}
