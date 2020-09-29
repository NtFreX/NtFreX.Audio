# NtFreX.Audio
![Build and test](https://github.com/NtFreX/NtFreX.Audio/workflows/Build%20and%20test/badge.svg)
![Publish to nuget](https://github.com/NtFreX/NtFreX.Audio/workflows/Publish%20to%20nuget/badge.svg)

This .net core library provides functionality to 

 - read audio containers
 - sample audio
 - convert between audio formats
 - play/record audio

## Supported formats

 - Wave
   - Pcm (8bps, 16bps, 32bps, 64bps)
   - Float (16bps, 32bps, 64bps)

## Demo

For a demo look into the demo/NtFrex.Audio.Console project.

![img](./resources/doc/console.jpg)

## Samples

**Read/Write an audio file**

```
var filePath = "myAudio.wav";
await using IAudioContainer audio = await AudioEnvironment.Serializer.FromFileAsync(filePath);
```

Other methods which resolve/write an `IAudioContainer` are:

 - `AudioEnvironment.Serializer.FromDataAsync`
 - `AudioEnvironment.Serializer.FromStreamAsync`
 - `AudioEnvironment.Serializer.ToStreamAsync`
 - `AudioEnvironment.Serializer.ToFileAsync`
 - `AudioEnvironment.Serializer.ToDataAsync`

There are serval extension methods which make use of those methods. If you want to build your own container you can use the `WaveAudioContainerBuilder` or the `IntermediateAudioContainerBuilder`.

**Convert an audio file**

```
await using var converted = await audio.ConvertAsync<IntermediateEnumerableAudioContainer>();
```

**Audio sampling**

```
await using var newAudio = await AudioEnvironment.Sampler
                                                 .SampleRateAudioSampler(WellKnownSampleRate.Hz44100)
                                                 .SampleAsync(audio, cancellationToken)
```

The sampler is not executed until the new audio is moved into an in memory container or written into another stream.
Other samplers are available under `AudioEnvironment.Sampler`.
Audio samplers can only be used with intermediate containers.

Other samplers are:

 - `BitsPerSampleAudioSampler`
 - `SampleRateAudioSampler`
 - `ChannelAudioSampler`
 - `FromMonoAudioSampler`
 - `ToMonoAudioSampler`
 - `ShiftWaveAudioSampler`
 - `SpeedAudioSampler`
 - `VolumeAudioSampler`
 - `FloatToPcmAudioSampler`
 - `PcmToFloatAudioSampler`

**Audio render**

```
var audioPlatform = AudioEnvironment.Platform.Get();
using var device = audioPlatform.AudioDeviceFactory.GetDefaultRenderDevice();

await using var context = await device.RenderAsync(audio);

var totalLength = audio.GetLength().TotalSeconds;
context.PositionChanged.Subscribe((sender, args) => LogProgress(args.Value / totalLength));

await context.EndOfPositionReached.NextEvent();
```

**Audio capture**

```
var audioPlatform = AudioEnvironment.Platform.Get();
using var device = audioPlatform.AudioDeviceFactory.GetDefaultCaptureDevice();

var format = audioPlatform.AudioClientFactory.GetDefaultFormat(device);

await using var sink = await FileAudioSink.CreateAsync(file, format);

await using var context = await device.CaptureAsync(format, sink);

await Task.Delay(time);
```

## Installation

You need to install the `NtFreX.Audio` nuget package and then addtional nuget packages depending on the platforms you want to use.

 - For Windows 7 and upwards the `NtFreX.Audio.Wasapi` package

## Architecture
 
The entry point should most of the time be the `AudioEnvironment` class.
