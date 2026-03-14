using Newtonsoft.Json;
using SuchByte.MacroDeck.ActionButton;
using SuchByte.MacroDeck.GUI;
using SuchByte.MacroDeck.GUI.CustomControls;
using SuchByte.MacroDeck.Plugins;

namespace VolumeMixerPlugin.Actions;

public class SetMasterVolumeAction : PluginAction
{
    public override string Name => "Set Master Volume";
    public override string Description => "Set the system master volume";
    public override bool CanConfigure => true;

    public override void Trigger(string clientId, ActionButton actionButton)
    {
        var config = GetConfig();
        if (config == null) return;

        VolumeMixerPluginMain.Instance?.AudioService?.SetMasterVolume(config.Volume);
        VolumeMixerPluginMain.Instance?.UpdateVariables();
    }

    public override ActionConfigControl GetActionConfigControl(ActionConfigurator actionConfigurator)
    {
        return new SetMasterVolumeConfigControl(this, actionConfigurator);
    }

    private MasterVolumeConfig? GetConfig()
    {
        if (string.IsNullOrEmpty(Configuration)) return null;
        try
        {
            return JsonConvert.DeserializeObject<MasterVolumeConfig>(Configuration);
        }
        catch
        {
            return null;
        }
    }
}

public class MasterVolumeConfig
{
    public float Volume { get; set; } = 50;
}

public class SetMasterVolumeConfigControl : ActionConfigControl
{
    private readonly NumericUpDown _volumeNumeric;
    private readonly PluginAction _action;

    public SetMasterVolumeConfigControl(PluginAction action, ActionConfigurator actionConfigurator)
    {
        _action = action;

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

        LoadConfig();
    }

    private void LoadConfig()
    {
        if (string.IsNullOrEmpty(_action.Configuration)) return;
        try
        {
            var config = JsonConvert.DeserializeObject<MasterVolumeConfig>(_action.Configuration);
            if (config != null)
            {
                _volumeNumeric.Value = (decimal)config.Volume;
            }
        }
        catch { }
    }

    public override bool OnActionSave()
    {
        var config = new MasterVolumeConfig { Volume = (float)_volumeNumeric.Value };
        _action.Configuration = JsonConvert.SerializeObject(config);
        _action.ConfigurationSummary = $"Volume: {config.Volume}%";
        return true;
    }
}
