using System.Text.Json;
using System.Text.RegularExpressions;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Memory;

namespace IPBlocker;

public class IPBlocker : BasePlugin
{
    public override string ModuleName => "Ip blocker in chat";
    public override string ModuleAuthor => "phantom";
    public override string ModuleVersion => "1.0.0";

    private Config _config;

    public override void Load(bool hotReload)
    {
        _config = LoadConfig();

        AddCommandListener("say", OnCommandSay);
        AddCommandListener("say_team", OnCommandSay);
    }

    private HookResult OnCommandSay(CCSPlayerController? player, CommandInfo commandinfo)
    {
        if (player == null) return HookResult.Continue;
        if (!IsIp(commandinfo.ArgString)) return HookResult.Continue;

        player.PrintToChat("IP Block");
        return HookResult.Handled;
    }

    private bool IsIp(string input)
    {
        const string pattern = @"(?:\d{1,3}[^\d\s]?){3}\d{1,3}(?::[0-9]+)?";

        return !Array.Exists(_config.AllowedIPs!, ip => ip == Regex.Match(input, pattern).Value) &&
               Regex.IsMatch(input, pattern);
    }

    private Config LoadConfig()
    {
        var configPath = Path.Combine(ModuleDirectory, "allowedIPs.json");

        if (!File.Exists(configPath)) return CreateConfig(configPath);

        var config = JsonSerializer.Deserialize<Config>(File.ReadAllText(configPath))!;

        return config;
    }

    private Config CreateConfig(string configPath)
    {
        var config = new Config
        {
            AllowedIPs = new[] { "1.1.1.1", "192.168.1.1" }
        };

        File.WriteAllText(configPath,
            JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true }));

        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine("[IPBlocker] The configuration was successfully saved to a file: " + configPath);
        Console.ResetColor();

        return config;
    }
}

public class Config
{
    public string[]? AllowedIPs { get; set; }
}