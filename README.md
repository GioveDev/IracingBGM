# IracingBGM
## What it does
Video Demonstration: https://www.youtube.com/watch?v=um4I7KQNjVQ

The application will play audio files of your own choice at various points during an iracing session, namely:

  * Garage Menu: You can relax with some background music as you sit in the garage and watch replays or adjust your setups.
  * Start: This sound will be played when pressing the "Grid" button.
  * Finish: You get to have some tunes playing to celebrate a race finish, will also be played at the end of qualifying and practice sessions.
  * Meatball or Disqualification: If receiving a meatball flag or disqualification wasn't miserable enough already, you can always make it worse with some sad music.

## Prerequisites
.NET 6 Runtime for desktop apps: https://dotnet.microsoft.com/en-us/download/dotnet/6.0/runtime

## Installation
Download the latest installer from the [releases](https://github.com/NickNicks/IracingBGM/releases) page and install it

## Using your own audio files
Inside the installation directory there will be a "Sounds" folder which contains a further four folders corresponding with the four types of audio events mentioned above,
just place your sound files in the appropriate folders and the application will pick them up

You don't need to restart the program to make it recognize new audio files, but audio that is already playing will keep playing.

If there is more than one audio file per folder the application will randomly choose one, good if you have a playlist you want to listen to.

If one of the folders is empty, then no sound will be played in that event, but DO NOT delete the folder themselves!

## Supported file formats
The media player component was provided by microsoft so i assume these are the supported file types, but i don't have time to test them out,
you are free to experiment.

https://support.microsoft.com/en-us/topic/file-types-supported-by-windows-media-player-32d9998e-dc8f-af54-7ba1-e996f74375d9

## Special Thanks
I would like to thank Nick Thissen for creating the amazing Iracing Sdk Wrapper https://github.com/NickThissen/iRacingSdkWrapper that this project is based upon.
