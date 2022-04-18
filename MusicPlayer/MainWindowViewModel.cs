using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using iRacingSdkWrapper;
using iRacingSdkWrapper.Bitfields;
using iRacingSimulator;
using MusicPlayer.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MusicPlayer;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly MediaPlayer mediaPlayer = new();
    private SessionFlag currentFlag;
    private string currentlyPlayingSound;
    private dynamic configuration;

    public MainWindowViewModel()
    {
        InitializeMediaPlayerVolume();
        Sim.Instance.Connected += OnConnected;
        Sim.Instance.TelemetryUpdated += OnTelemetryUpdated;
        Sim.Instance.Disconnected += OnDisconnected;

        Sim.Instance.Start();
    }

    public string ConnectionStatus { get; set; } = MusicPlayerConstants.NotConnected;

    public double Volume
    {
        get { return Math.Round(this.mediaPlayer.Volume * 100, 3); }
        set
        {
            var dividedValue = Math.Round(value / 100, 3);
            this.mediaPlayer.Volume = dividedValue;
            OnPropertyChanged(nameof(this.Volume));
        }
    }

    public void SaveVolume()
    {
        this.configuration.Volume = this.Volume;
        string updatedConfiguration = JsonConvert.SerializeObject(this.configuration);
        File.WriteAllText(MusicPlayerConstants.AppSettingsFileName,
            JToken.Parse(updatedConfiguration).ToString(Formatting.Indented));
    }

    private void OnConnected(object? sender, EventArgs e)
    {
        this.ConnectionStatus = MusicPlayerConstants.Connected;
        OnPropertyChanged(nameof(ConnectionStatus));
    }

    private void OnDisconnected(object? sender, EventArgs e)
    {
        this.ConnectionStatus = MusicPlayerConstants.NotConnected;
        OnPropertyChanged(nameof(ConnectionStatus));
        this.mediaPlayer.Stop();
    }

    private void OnTelemetryUpdated(object? sender, SdkWrapper.TelemetryUpdatedEventArgs e)
    {
        var isOnTrack = e.TelemetryInfo.IsOnTrack.Value;

        switch (isOnTrack)
        {
            case false:
                this.PlaySound(MusicPlayerConstants.MenuSound);
                break;
            case true:
                this.HandlePlayerIsOnTrack(e);
                this.CheckUpdatedFlag(e);
                break;
        }
    }

    private void HandlePlayerIsOnTrack(SdkWrapper.TelemetryUpdatedEventArgs eventArgs)
    {
        if (eventArgs.TelemetryInfo.SessionState.Value == SessionStates.GetInCar)
        {
            PlaySound(MusicPlayerConstants.StartSound);
            return;
        }

        if (currentlyPlayingSound == MusicPlayerConstants.MenuSound)
        {
            mediaPlayer.Stop();
            currentlyPlayingSound = string.Empty;
        }
    }

    private void CheckUpdatedFlag(SdkWrapper.TelemetryUpdatedEventArgs eventArgs)
    {
        var sessionFlag = eventArgs.TelemetryInfo.SessionFlags.Value;

        if (sessionFlag == currentFlag) return;

        this.currentFlag = sessionFlag;
        switch (sessionFlag)
        {
            case { } a when a.Contains(SessionFlags.Repair):
                PlaySound(MusicPlayerConstants.FailSound);
                break;
            case { } a when a.Contains(SessionFlags.Disqualify):
                PlaySound(MusicPlayerConstants.FailSound);
                break;
            case { } a when a.Contains(SessionFlags.Checkered):
                if (eventArgs.TelemetryInfo.LapDistPct.Value < 0.4 &&
                    eventArgs.TelemetryInfo.PlayerTrackSurface.Value != TrackSurfaces.InPitStall)
                    PlaySound(MusicPlayerConstants.FinishSound);
                break;
        }
    }

    private void PlaySound(string folderName)
    {
        if (folderName != this.currentlyPlayingSound)
        {
            this.currentlyPlayingSound = folderName;
            var directoryInfo = new DirectoryInfo("Sounds\\" + folderName);
            var files = directoryInfo.GetFiles();

            if (files.Length == 0) return;

            this.mediaPlayer.Open(
                new Uri(files
                    .ElementAt(new Random().Next(0, files.Length))
                    .FullName));
            this.mediaPlayer.Play();
            this.mediaPlayer.MediaEnded += OnMediaEnded;
        }
    }

    private void OnMediaEnded(object? sender, EventArgs e)
    {
        if (currentlyPlayingSound != MusicPlayerConstants.MenuSound) return;
        this.currentlyPlayingSound = string.Empty;
        this.PlaySound(MusicPlayerConstants.MenuSound);
    }

    private void InitializeMediaPlayerVolume()
    {
        var content = File.ReadAllText(MusicPlayerConstants.AppSettingsFileName);
        this.configuration = JsonConvert.DeserializeObject<dynamic>(content);

        this.mediaPlayer.Volume = double.Parse(configuration.Volume.ToString()) / 100;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}