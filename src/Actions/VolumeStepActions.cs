using Newtonsoft.Json;
using SuchByte.MacroDeck.ActionButton;
using SuchByte.MacroDeck.GUI;
using SuchByte.MacroDeck.GUI.CustomControls;
using SuchByte.MacroDeck.Plugins;
using MacroDeckRoundedComboBox = SuchByte.MacroDeck.GUI.CustomControls.RoundedComboBox;

namespace VolumeMixerPlugin.Actions;

public class VolumeUpAction : PluginAction
{
    public override string Name => "Volume Up";
    public override string Description => "Increase app volume by selected percentage";
    public override bool CanConfigure => true;

    internal string? _trackedAppName;

    public override void Trigger(string clientId, ActionButton actionButton)
    {
        var config = GetConfig();
        if (config == null || string.IsNullOrEmpty(config.AppName)) return;

        VolumeMixerPluginMain.Instance?.AudioService?.AdjustAppVolume(config.AppName, 5);
        VolumeMixerPluginMain.Instance?.UpdateVariables();
    }

    public override void OnActionButtonLoaded()
    {
        var config = GetConfig();
        var appName = string.IsNullOrWhiteSpace(config?.AppName) ? null : config!.AppName;
        VolumeMixerPluginMain.TrackAppUsage(appName, _trackedAppName);
        _trackedAppName = appName;
    }

    public override void OnActionButtonDelete()
    {
        VolumeMixerPluginMain.UntrackAppUsage(_trackedAppName);
        _trackedAppName = null;
    }

    public override ActionConfigControl GetActionConfigControl(ActionConfigurator actionConfigurator)
    {
        return new VolumeStepConfigControl(this, actionConfigurator);
    }

    private VolumeStepConfig? GetConfig()
    {
        if (string.IsNullOrEmpty(Configuration)) return null;
        try
        {
            return JsonConvert.DeserializeObject<VolumeStepConfig>(Configuration);
        }
        catch
        {
            return null;
        }
    }
}

public class VolumeDownAction : PluginAction
{
    public override string Name => "Volume Down";
    public override string Description => "Decrease app volume by selected percentage";
    public override bool CanConfigure => true;

    internal string? _trackedAppName;

    public override void Trigger(string clientId, ActionButton actionButton)
    {
        var config = GetConfig();
        if (config == null || string.IsNullOrEmpty(config.AppName)) return;

        VolumeMixerPluginMain.Instance?.AudioService?.AdjustAppVolume(config.AppName, -5);
        VolumeMixerPluginMain.Instance?.UpdateVariables();
    }

    public override void OnActionButtonLoaded()
    {
        var config = GetConfig();
        var appName = string.IsNullOrWhiteSpace(config?.AppName) ? null : config!.AppName;
        VolumeMixerPluginMain.TrackAppUsage(appName, _trackedAppName);
        _trackedAppName = appName;
    }

    public override void OnActionButtonDelete()
    {
        VolumeMixerPluginMain.UntrackAppUsage(_trackedAppName);
        _trackedAppName = null;
    }

    public override ActionConfigControl GetActionConfigControl(ActionConfigurator actionConfigurator)
    {
        return new VolumeStepConfigControl(this, actionConfigurator);
    }

    private VolumeStepConfig? GetConfig()
    {
        if (string.IsNullOrEmpty(Configuration)) return null;
        try
        {
            return JsonConvert.DeserializeObject<VolumeStepConfig>(Configuration);
        }
        catch
        {
            return null;
        }
    }
}

public class VolumeStepConfig
{
    public string AppName { get; set; } = "";
    public int Step { get; set; } = 5;
}

public class VolumeStepConfigControl : ActionConfigControl
{
    private readonly MacroDeckRoundedComboBox _appComboBox;
    private readonly NumericUpDown _stepNumericUpDown;
    private readonly PluginAction _action;

    public VolumeStepConfigControl(PluginAction action, ActionConfigurator actionConfigurator)
    {
        _action = action;

        var label = new Label { Text = "App Name:", Location = new Point(14, 18), AutoSize = true };
        _appComboBox = new MacroDeckRoundedComboBox { Location = new Point(150, 14), Width = 200, DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList };

        var refreshButton = new Button { Location = new Point(360, 12), Width = 30, Height = 26, Text = "↻" };
        refreshButton.Click += (sender, e) => PopulateApps();

        Controls.Add(label);
        Controls.Add(_appComboBox);
        Controls.Add(refreshButton);

        var stepLabel = new Label { Text = "Step (%):", Location = new Point(14, 50), AutoSize = true };
        _stepNumericUpDown = new NumericUpDown { Location = new Point(150, 46), Width = 80, Minimum = 1, Maximum = 100, Value = 5 };
        Controls.Add(stepLabel);
        Controls.Add(_stepNumericUpDown);

        PopulateApps();
        LoadConfig();
    }

    private void PopulateApps()
    {
        var apps = VolumeMixerPluginMain.Instance?.AudioService?.GetActiveAppNames() ?? new List<string>();
        _appComboBox.Items.Clear();
        foreach (var app in apps)
            _appComboBox.Items.Add(app);
    }

    private void LoadConfig()
    {
        if (string.IsNullOrEmpty(_action.Configuration)) return;
        try
        {
            var config = JsonConvert.DeserializeObject<VolumeStepConfig>(_action.Configuration);
            if (config != null)
            {
                if (config.Step > 0)
                {
                    _stepNumericUpDown.Value = Math.Min(Math.Max(config.Step, 1), 100);
                }

                if (!string.IsNullOrEmpty(config.AppName))
                {
                    if (!_appComboBox.Items.Contains(config.AppName))
                    {
                        _appComboBox.Items.Add(config.AppName);
                    }
                    _appComboBox.SelectedItem = config.AppName;
                }
            }
        }
        catch
        {
        }
    }

    public override bool OnActionSave()
    {
        var config = new VolumeStepConfig { AppName = _appComboBox.SelectedItem?.ToString() ?? "", Step = (int)_stepNumericUpDown.Value };
        _action.Configuration = JsonConvert.SerializeObject(config);
        _action.ConfigurationSummary = config.AppName;

        var newAppName = string.IsNullOrWhiteSpace(config.AppName) ? null : config.AppName;
        if (_action is VolumeUpAction up)
        {
            VolumeMixerPluginMain.TrackAppUsage(newAppName, up._trackedAppName);
            up._trackedAppName = newAppName;
        }
        else if (_action is VolumeDownAction down)
        {
            VolumeMixerPluginMain.TrackAppUsage(newAppName, down._trackedAppName);
            down._trackedAppName = newAppName;
        }

        return true;
    }

}
