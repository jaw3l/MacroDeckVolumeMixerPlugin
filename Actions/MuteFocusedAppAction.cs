using SuchByte.MacroDeck.ActionButton;
using SuchByte.MacroDeck.GUI;
using SuchByte.MacroDeck.Plugins;

namespace VolumeMixerPlugin.Actions;

public class MuteFocusedAppAction : PluginAction
{
    public override string Name => "Mute/Unmute Focused App";
    public override string Description => "Toggle mute for the currently focused application";
    public override bool CanConfigure => false;

    public override void Trigger(string clientId, ActionButton actionButton)
    {
        var focused = VolumeMixerPluginMain.Instance?.AudioService?.GetFocusedProcessName();
        if (string.IsNullOrEmpty(focused)) return;

        VolumeMixerPluginMain.Instance?.AudioService?.ToggleAppMute(focused);
        VolumeMixerPluginMain.Instance?.UpdateVariables();
    }
}
