using Newtonsoft.Json;
using SuchByte.MacroDeck.ActionButton;
using SuchByte.MacroDeck.GUI;
using SuchByte.MacroDeck.GUI.CustomControls;
using SuchByte.MacroDeck.Plugins;

namespace VolumeMixerPlugin.Actions;

public class VolumeUpMasterAction : PluginAction
{
    public override string Name => "Volume Up Master";
    public override string Description => "Increase system master volume";
    public override bool CanConfigure => true;

    public override void Trigger(string clientId, ActionButton actionButton)
    {
        var config = GetConfig();
        if (config == null) return;

        VolumeMixerPluginMain.Instance?.AudioService?.AdjustMasterVolume(config.Step);
        VolumeMixerPluginMain.Instance?.UpdateVariables();
    }

    public override ActionConfigControl GetActionConfigControl(ActionConfigurator actionConfigurator)
    {
        return new VolumeStepMasterConfigControl(this, actionConfigurator);
    }

    private MasterVolumeStepConfig? GetConfig()
    {
        if (string.IsNullOrEmpty(Configuration)) return null;
        try
        {
            return JsonConvert.DeserializeObject<MasterVolumeStepConfig>(Configuration);
        }
        catch
        {
            return null;
        }
    }
}

public class VolumeDownMasterAction : PluginAction
{
    public override string Name => "Volume Down Master";
    public override string Description => "Decrease system master volume";
    public override bool CanConfigure => true;

    public override void Trigger(string clientId, ActionButton actionButton)
    {
        var config = GetConfig();
        if (config == null) return;

        VolumeMixerPluginMain.Instance?.AudioService?.AdjustMasterVolume(-config.Step);
        VolumeMixerPluginMain.Instance?.UpdateVariables();
    }

    public override ActionConfigControl GetActionConfigControl(ActionConfigurator actionConfigurator)
    {
        return new VolumeStepMasterConfigControl(this, actionConfigurator);
    }

    private MasterVolumeStepConfig? GetConfig()
    {
        if (string.IsNullOrEmpty(Configuration)) return null;
        try
        {
            return JsonConvert.DeserializeObject<MasterVolumeStepConfig>(Configuration);
        }
        catch
        {
            return null;
        }
    }
}

public class MasterVolumeStepConfig
{
    public int Step { get; set; } = 5;
}

public class VolumeStepMasterConfigControl : ActionConfigControl
{
    private readonly NumericUpDown _stepNumericUpDown;
    private readonly PluginAction _action;

    public VolumeStepMasterConfigControl(PluginAction action, ActionConfigurator actionConfigurator)
    {
        _action = action;

        var stepLabel = new Label { Text = "Step (%):", Location = new Point(14, 18), AutoSize = true };
        _stepNumericUpDown = new NumericUpDown { Location = new Point(150, 14), Width = 80, Minimum = 1, Maximum = 100, Value = 5 };
        Controls.Add(stepLabel);
        Controls.Add(_stepNumericUpDown);

        LoadConfig();
    }

    private void LoadConfig()
    {
        if (string.IsNullOrEmpty(_action.Configuration)) return;
        try
        {
            var config = JsonConvert.DeserializeObject<MasterVolumeStepConfig>(_action.Configuration);
            if (config != null && config.Step > 0)
            {
                _stepNumericUpDown.Value = Math.Min(Math.Max(config.Step, 1), 100);
            }
        }
        catch { }
    }

    public override bool OnActionSave()
    {
        var config = new MasterVolumeStepConfig { Step = (int)_stepNumericUpDown.Value };
        _action.Configuration = JsonConvert.SerializeObject(config);
        _action.ConfigurationSummary = $"Step: {config.Step}%";
        return true;
    }
}
