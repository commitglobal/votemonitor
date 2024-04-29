import { useEffect, useState } from "react";
import { Audio } from "expo-av";
import SuperJSON from "superjson";

const useAudio = () => {
  const [recording, setRecording] = useState<any>();
  const [nowPlaying, setNowPlaying] = useState<Audio.Sound>();
  const [permissionResponse, requestPermission] = Audio.usePermissions();

  useEffect(() => {
    return nowPlaying
      ? () => {
          console.log("Unloading Sound");
          nowPlaying.unloadAsync();
        }
      : undefined;
  }, [nowPlaying]);

  async function startRecording() {
    try {
      if (permissionResponse?.status !== "granted") {
        console.log("Requesting permission..");
        await requestPermission();
      }
      await Audio.setAudioModeAsync({
        allowsRecordingIOS: true,
        playsInSilentModeIOS: true,
      });

      console.log("Starting recording..");
      const { recording } = await Audio.Recording.createAsync(
        Audio.RecordingOptionsPresets.HIGH_QUALITY,
      );
      setRecording(recording);
      console.log("Recording started");
    } catch (err) {
      console.error("Failed to start recording", err);
    }
  }

  async function stopRecording() {
    console.log("Stopping recording..");
    setRecording(undefined);
    await recording.stopAndUnloadAsync();
    await Audio.setAudioModeAsync({
      allowsRecordingIOS: false,
    });
    const uri = recording.getURI();
    console.log("Recording stopped and stored at", uri);
    return uri;
  }

  async function playSound(uri: any) {
    const { sound } = await Audio.Sound.createAsync({ uri });
    setNowPlaying(sound);

    const data = await sound.playAsync();

    sound.setOnPlaybackStatusUpdate(_onPlaybackStatusUpdate);

    console.log(SuperJSON.stringify(data));
    console.log(SuperJSON.stringify(sound));
  }

  const _onPlaybackStatusUpdate = (playbackStatus: any) => {
    if (!playbackStatus.isLoaded) {
      // Update your UI for the unloaded state
      if (playbackStatus.error) {
        console.log(`Encountered a fatal error during playback: ${playbackStatus.error}`);
        // Send Expo team the error on Slack or the forums so we can help you debug!
      }
    } else {
      // Update your UI for the loaded state
      console.log("🔉 Loaded");
      if (playbackStatus.isPlaying) {
        // Update your UI for the playing state
        console.log("🔉 Is Playing");
      } else {
        // Update your UI for the paused state
        console.log("🔉 Is Paused");
      }

      if (playbackStatus.isBuffering) {
        // Update your UI for the buffering state
        console.log("🔉 Is Buffering");
      }

      if (playbackStatus.didJustFinish && !playbackStatus.isLooping) {
        // The player has just finished playing and will stop. Maybe you want to play something else?
        console.log("🔉 didJustFinish & !playbackStatus.isLooping");
      }
    }
  };

  return {
    startRecording,
    stopRecording,
    playSound,
    isRecording: !!recording,
    isPlaying: !!nowPlaying,
  };
};

export default useAudio;
