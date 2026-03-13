using SuchByte.MacroDeck.ActionButton;
using SuchByte.MacroDeck.GUI;
using SuchByte.MacroDeck.Plugins;

namespace VolumeMixerPlugin.Actions;

public class MuteMasterAction : PluginAction
{
    public override string Name => "Mute/Unmute Master";
    public override string Description => "Toggle system master mute";
    public override bool CanConfigure => false;

    public override void Trigger(string clientId, ActionButton actionButton)
    {
        VolumeMixerPluginMain.Instance?.AudioService?.ToggleMasterMute();
        VolumeMixerPluginMain.Instance?.UpdateVariables();
    }
}
