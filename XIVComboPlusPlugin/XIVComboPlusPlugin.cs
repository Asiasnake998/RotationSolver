using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.JobGauge;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Party;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using XIVComboPlus;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace XIVComboPlus;

public sealed class XIVComboPlusPlugin : IDalamudPlugin, IDisposable
{
    private const string _command = "/pcombo";

    private const string _lockCommand = "/tauto";

    private readonly WindowSystem windowSystem;

    private readonly ConfigWindow configWindow;
    //private readonly SystemSound sound;
    public string Name => "XIV Combo Plus";

    private static Framework _framework;


    public XIVComboPlusPlugin(DalamudPluginInterface pluginInterface, Framework framework, CommandManager commandManager, SigScanner sigScanner)
    {
        pluginInterface.Create<Service>(Array.Empty<object>());
        Service.Configuration = pluginInterface.GetPluginConfig() as PluginConfiguration ?? new PluginConfiguration();
        Service.Address = new PluginAddressResolver();
        Service.Address.Setup();
        Service.IconReplacer = new IconReplacer();
        configWindow = new ConfigWindow();
        windowSystem = new WindowSystem(Name);
        windowSystem.AddWindow(configWindow);
        //sound = new SystemSound();
        Service.Interface.UiBuilder.OpenConfigUi += OnOpenConfigUi;
        Service.Interface.UiBuilder.Draw += windowSystem.Draw;

        TargetHelper.Init(sigScanner);

        CommandInfo val = new CommandInfo(new CommandInfo.HandlerDelegate(OnCommand));
        val.HelpMessage = "��һ�����Ե����������õĴ���";
        val.ShowInHelp = true;
        commandManager.AddHandler(_command, val);

        CommandInfo lockInfo = new CommandInfo(new CommandInfo.HandlerDelegate(TargetObject));
        lockInfo.HelpMessage = "������Ҫ�ĵ��ˡ�";
        lockInfo.ShowInHelp = true;
        commandManager.AddHandler(_lockCommand, lockInfo);

        _framework = framework;
        framework.Update += TargetHelper.Framework_Update;
    }


    public void Dispose()
    {
        Service.CommandManager.RemoveHandler(_command);
        Service.CommandManager.RemoveHandler(_lockCommand);
        Service.Interface.UiBuilder.OpenConfigUi -= OnOpenConfigUi;
        Service.Interface.UiBuilder.Draw -= windowSystem.Draw;
        Service.IconReplacer.Dispose();
        _framework.Update -= TargetHelper.Framework_Update;
    }

    private void OnOpenConfigUi()
    {
        configWindow.IsOpen = true;
    }

    internal void TargetObject(string command, string arguments)
    {
        string[] array = arguments.Split();

        if(array.Length > 1)
        {
            if (array[1].Contains('B'))
            {
                Service.Configuration.IsTargetBoss = true;
            }

            if (array[1].Contains('S'))
            {
                Service.Configuration.IsTargetBoss = false;
            }
        }


        uint inputActions = TargetHelper.GetActionsByName(array[0]).RowId;
        uint remapAction = Service.IconReplacer.RemapActionID(inputActions);
        TargetHelper.SetTarget(TargetHelper.GetBestTarget(Service.DataManager.GetExcelSheet<Action>().GetRow(remapAction)));
    }

    private void OnCommand(string command, string arguments)
    {
        //string[] values = IconReplacer.CustomCombos.Select(c => c.ComboFancyName).ToArray();

        string[] array = arguments.Split();
        switch (array[0])
        {
            case "setall":
                {

                    foreach (var item in IconReplacer.CustomCombos)
                    {
                        item.IsEnabled = true;
                    }
                    Service.ChatGui.Print("All SET");
                    Service.Configuration.Save();
                    break;
                }
            case "unsetall":
                {
                    foreach (var item in IconReplacer.CustomCombos)
                    {
                        item.IsEnabled = false;
                    }
                    Service.ChatGui.Print("All UNSET");
                    Service.Configuration.Save();
                    break;
                }
            case "set":
                {
                    string text3 = array[1].ToLowerInvariant();
                    for (int i = 0; i < IconReplacer.CustomCombos.Length; i++)
                    {
                        var value = IconReplacer.CustomCombos[i];
                        if (value.ComboFancyName.ToLowerInvariant() == text3)
                        {
                            value.IsEnabled = true;
                            Service.ChatGui.Print($"{value} SET");
                            break;
                        }
                    }
                    Service.Configuration.Save();
                    break;
                }
            case "toggle":
                {
                    string text = array[1].ToLowerInvariant();
                    for (int i = 0; i < IconReplacer.CustomCombos.Length; i++)
                    {
                        var customComboPreset2 = IconReplacer.CustomCombos[i];
                        if (customComboPreset2.ComboFancyName.ToLowerInvariant() == text)
                        {
                            customComboPreset2.IsEnabled = !customComboPreset2.IsEnabled;
                            Service.ChatGui.Print(customComboPreset2.ComboFancyName + " " + (customComboPreset2.IsEnabled ? "SET": "UNSET"));
                        }
                    }
                    Service.Configuration.Save();
                    break;
                }
            //case "dot":
            //    if (Service.Configuration.EnabledActions.Contains(CustomComboPreset.SCHDotFeature))
            //    {
            //        Service.Configuration.EnabledActions.Remove(CustomComboPreset.SCHDotFeature);
            //    }
            //    if (!Service.Configuration.EnabledActions.Contains(CustomComboPreset.SCHDotFeature))
            //    {
            //        Service.Configuration.EnabledActions.Add(CustomComboPreset.SCHDotFeature);
            //    }
            //    if (Service.Configuration.EnabledActions.Contains(CustomComboPreset.ASTdotFeature))
            //    {
            //        Service.Configuration.EnabledActions.Remove(CustomComboPreset.ASTdotFeature);
            //    }
            //    if (!Service.Configuration.EnabledActions.Contains(CustomComboPreset.ASTdotFeature))
            //    {
            //        Service.Configuration.EnabledActions.Add(CustomComboPreset.ASTdotFeature);
            //    }
            //    Service.Configuration.Save();
            //    break;
            case "unset":
                {
                    string text2 = array[1].ToLowerInvariant();
                    for (int i = 0; i < IconReplacer.CustomCombos.Length; i++)
                    {
                        var value = IconReplacer.CustomCombos[i];
                        if (value.ComboFancyName.ToLowerInvariant() == text2)
                        {
                            value.IsEnabled = true;
                            Service.ChatGui.Print($"{value} UNSET");
                            break;
                        }
                    }
                    Service.Configuration.Save();
                    break;
                }
            //case "list":
            //    switch (array.Length > 1 ? array[1].ToLowerInvariant() : "all")
            //    {
            //        case "set":
            //            foreach (bool item3 in from preset in values
            //                                   select Service.Configuration.IsEnabled(preset))
            //            {
            //                Service.ChatGui.Print(item3.ToString());
            //            }
            //            break;
            //        case "unset":
            //            foreach (bool item4 in from preset in values
            //                                   select !Service.Configuration.IsEnabled(preset))
            //            {
            //                Service.ChatGui.Print(item4.ToString());
            //            }
            //            break;
            //        case "all":
            //            {
            //                for (int i = 0; i < values.Length; i++)
            //                {
            //                    Service.ChatGui.Print(values[i]);
            //                }
            //                break;
            //            }
            //        default:
            //            Service.ChatGui.PrintError("Available list filters: set, unset, all");
            //            break;
            //    }
            //    break;
            default:
                configWindow.Toggle();
                break;
        }
        Service.Configuration.Save();
    }
}