cd demo\NtFrex.Audio.Console

dotnet publish -p:PublishProfile=Local_netcoreapp3_1_linux_x64_Debug
docker build -t ntfrex.audio -f Dockerfile .
docker run -it --rm ntfrex.audio
