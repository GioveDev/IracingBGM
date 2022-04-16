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

namespace MusicPlayer;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly MediaPlayer mediaPlayer = new();
    private SessionFlag currentFlag;
    private string currentlyPlayingSound;

    public MainWindowViewModel()
    {
        Sim.Instance.Connected += OnConnected;
        Sim.Instance.TelemetryUpdated += OnTelemetryUpdated;
        Sim.Instance.Disconnected += OnDisconnected;

        Sim.Instance.Start();
    }

    public string ConnectionStatus { get; set; } = MusicPlayerConstants.NotConnected;

    private void OnConnected(object? sender, EventArgs e)
    {
        ConnectionStatus = MusicPlayerConstants.Connected;
        OnPropertyChanged(nameof(ConnectionStatus));
    }

    private void OnDisconnected(object? sender, EventArgs e)
    {
        ConnectionStatus = MusicPlayerConstants.NotConnected;
        OnPropertyChanged(nameof(ConnectionStatus));
        mediaPlayer.Stop();
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
                PlaySound(MusicPlayerConstants.FinishSound);
                break;
        }
    }

    private void PlaySound(string fileName)
    {
        if (fileName != this.currentlyPlayingSound)
        {
            this.currentlyPlayingSound = fileName;
            var directoryInfo = new DirectoryInfo("Sounds");
            this.mediaPlayer.Open(
                new Uri(directoryInfo
                    .GetFiles(fileName + "*")
                    .Single()
                    .FullName));
            this.mediaPlayer.Play();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}