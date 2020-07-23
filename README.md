# NtFreX.Audio
![Build and test](https://github.com/NtFreX/NtFreX.Audio/workflows/Build%20and%20test/badge.svg)
![Publish to nuget](https://github.com/NtFreX/NtFreX.Audio/workflows/Publish%20to%20nuget/badge.svg)

This .net core library provides functionality to read audio containers, sample wave data, convert between audio formats and play/record wave data.
The entry point should most of the time be the `AudioEnvironment` class.

This is a takeout of the library architecture. A speciality of this library is the WaveEnumerableAudioContainer which allows you to create audio modification pipelines which do not allocate/copy the whole stream.

![img](./resources/doc/architecture.jpg)

## Demo

For a demo look into the demo/NtFrex.Audio.Console project.

![img](./resources/doc/console.jpg)

## Samples

**Read/Write an audio file**

```
var filePath = "myAudio.wave";
using IStreamAudioContainer audio = await AudioEnvironment.Serializer.FromFileAsync(filePath, cancellationToken);

// other audio types are currently not supported
using var convertedAudio = AudioEnvironment.Converter.Convert<WaveStreamAudioContainer>(audio);
```

Other methods which resolve/write an `IStreamAudioContainer` are:

 - AudioEnvironment.Serializer.FromDataAsync
 - AudioEnvironment.Serializer.FromStreamAsync
 - AudioEnvironment.Serializer.ToStreamAsync
 - AudioEnvironment.Serializer.ToFileAsync
 - AudioEnvironment.Serializer.ToDataAsync

There are serval extension methods which make use of those methods. If you want to build your own container you can use the `WaveEnumerableAudioContainerBuilder`.

**Audio sampling**

```
var newAudio = await AudioEnvironment.Sampler
                                     .SampleRateAudioSampler(WellKnownSampleRate.Hz44100)
                                     .SampleAsync(audio, cancellationToken)
```

The sampler is not executed until the new audio is moved into an in memory container or written into another stream.
Other samplers are available under `AudioEnvironment.Sampler`.
Audio samplers can only be used with wave data.

Other samplers are:

 - BitsPerSampleAudioSampler
 - SampleRateAudioSampler
 - ChannelAudioSampler
 - FromMonoAudioSampler
 - ToMonoAudioSampler
 - ShiftWaveAudioSampler
 - SpeedAudioSampler
 - VolumeAudioSampler
 - FloatToPcmAudioSampler
 - PcmToFloatAudioSampler

**Audio render**

```
var audioPlatform = AudioEnvironment.Platform.Get();
using var device = audioPlatform.AudioDeviceFactory.GetDefaultRenderDevice();

(var context, var client) = await device.RenderAsync(audio, cancellationToken).ConfigureAwait(false);

var totalLength = audio.GetLength().TotalSeconds;
context.PositionChanged.Subscribe((sender, args) => LogProgress(args.Value / totalLength));

await context.EndOfPositionReached.WaitForNextEvent().ConfigureAwait(false);

context.Dispose();
client.Dispose();
```

**Audio capture**

```
var audioPlatform = AudioEnvironment.Platform.Get();
using var device = audioPlatform.AudioDeviceFactory.GetDefaultCaptureDevice();

var format = audioPlatform.AudioClientFactory.GetDefaultFormat(device);

using var sink = new FileAudioSink(file);
await sink.InitializeAsync(format).ConfigureAwait(false);

(var context, var client) = await device.CaptureAsync(format, sink, cancellationToken).ConfigureAwait(false);

await Task.Delay(time).ConfigureAwait(false);

context.Dispose();
client.Dispose();

sink.Finish();
```

## Installation

You need to install the `NtFreX.Audio` nuget package and then addtional nuget packages depending on the platforms you want to use.

 - For Windows the `NtFreX.Audio.Wasapi` package
 - For Linux the `NtFreX.Audio.PulseAudio` package

## TODO

 - [ ] tests
 - [ ] performance
 - [ ] api refinement
 - [ ] remove AsyncEnumerator dependency
 - [ ] warnings and todos
   - [ ] why are some headers big endian instead of little endian as doc says? (WaveAudioContainerSerializer)
   - [ ] cleanup EndianAwareBitConverter
   - [ ] interop cleanup
   - [ ] format type cleanup
 - [ ] Wave audio container
   - [x] basis
   - [x] streamable
   - [x] enumerable
 - [ ] Wave audio samplers
   - [x] basis
   - [x] sample rate
   - [x] speed
   - [ ] channels
     - [x] monto to x
     - [x] x to mono
     - [ ] x to x
   - [ ] bits per sample
   - [ ] volume
 - [ ] audio devices
   - [x] basis
   - [x] platform assembly
     - [x] loading (self contained)
     - [x] packaging
   - [ ] windows multi media
     - [x] basis
     - [x] dispose
     - [ ] get all devices
     - [ ] get all playback devices
     - [ ] get all recording devices
     - [ ] get device properties
     - [ ] get supported audio formats of device
     - [ ] events
   - [ ] ALSA
   - [ ] pulse audio
 - [ ] audio playback
   - [x] basis
   - [ ] wasapi
     - [ ] control
     - [x] channel mapping
     - [x] dispose
     - [ ] events
   - [ ] pulse audio
 - [ ] audio recording
   - [x] basis
   - [ ] wasapi
     - [x] basis
     - [ ] control
   - [ ] pulse audio
 - [ ] audio formats and containers
   - [x] IEEE FLOAT
 - [ ] improve
   - [ ] sample rate/speed interpolation
   - [ ] serializer factory resolve by signature in file or/and file extension
   - [ ] platform assembly loading
 - [ ] more
   - [ ] audio encoders
     - [ ] mp3 (https://docs.microsoft.com/en-us/windows/win32/medfound/windows-media-mp3-decoder)
   - [ ] audio converters
     - [ ] avi
   - [ ] signal modification
     - [ ] generation
     - [ ] splitting
     - [ ] joining
   - [ ] make speed audio sampler  work with factor bigger then 2 or smaller then 0.5 and unify with sample rate audip sampler
   - [ ] validate byterate, blockaligin and chunksize in fmt sub chunk
   - [ ] rifx support
   - [ ] riff sub chunk validation
   - [ ] cleanup casting and allocations
   - [ ] non seekable and non skipable streams?
   - [ ] audio conainer options
   - [ ] fourier
   - [ ] volume normalization
   - [ ] background noise filter
   - [ ] ... 
 - [ ] ...


## Bugs

 - [ ] Fatal error. 0xC0000005 at  NtFreX.Audio.Wasapi.Interop.IMMDevice.Activate (Win 10Pro x64)
   - 1. Run `Local (netcoreapp3.1, win-x86)` publish profile
   - 2. Start executable
   - 3. Choose `PlayAudioDemo`
   - 4. Choose `resources/audio/8-bit Detective.wav`
   - 5. Fatal error occures
 - [x] Progress bar is not filled to completion
  - 1. Start console
  - 2. Play any audio
  - 3. Progress bar will stop short before the total
