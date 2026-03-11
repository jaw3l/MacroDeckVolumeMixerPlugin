using Newtonsoft.Json;
using SuchByte.MacroDeck.ActionButton;
using SuchByte.MacroDeck.GUI;
using SuchByte.MacroDeck.GUI.CustomControls;
using SuchByte.MacroDeck.Plugins;

namespace VolumeMixerPlugin.Actions;

public class SetFocusedAppVolumeAction : PluginAction
{
    public override string Name => "Set Focused App Volume";
    public override string Description => "Set volume for the currently focused application";
    public override bool CanConfigure => true;

    public override void Trigger(string clientId, ActionButton actionButton)
    {
        var config = GetConfig();
        if (config == null) return;

        var focused = VolumeMixerPluginMain.Instance?.AudioService?.GetFocusedProcessName();
        if (string.IsNullOrEmpty(focused)) return;

        VolumeMixerPluginMain.Instance?.AudioService?.SetAppVolume(focused, config.Volume);
        VolumeMixerPluginMain.Instance?.UpdateVariables();
    }

    private SetAppVolumeConfig? GetConfig()
    {
        if (string.IsNullOrEmpty(Configuration)) return null;
        try
        {
            return JsonConvert.DeserializeObject<SetAppVolumeConfig>(Configuration);
        }
        catch
        {
            return null;
        }
    }

    public override ActionConfigControl GetActionConfigControl(ActionConfigurator actionConfigurator)
    {
        return new SetFocusedAppVolumeConfigControl(actionConfigurator);
    }
}

public class SetFocusedAppVolumeConfigControl : ActionConfigControl
{
    private readonly NumericUpDown _volumeNumeric;

    public SetFocusedAppVolumeConfigControl(ActionConfigurator actionConfigurator)
    {
        var label = new Label { Text = "Volume (0-100):", Location = new Point(14, 18), AutoSize = true };
        _volumeNumeric = new NumericUpDown
        {
            Location = new Point(150, 14),
            Minimum = 0,
            Maximum = 100,
            Value = 50,
            Width = 80
        };

        Controls.Add(label);
        Controls.Add(_volumeNumeric);
    }

    public override bool OnActionSave()
    {
        var config = new SetAppVolumeConfig
        {
            AppName = "",
            Volume = (float)_volumeNumeric.Value
        };
        return true;
    }
}
