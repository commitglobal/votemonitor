import React, { useState, useEffect } from "react";
import { View } from "tamagui";
import { AVPlaybackStatus, AVPlaybackStatusSuccess, Audio } from "expo-av";
import { Icon } from "./Icon";
import { Typography } from "./Typography";

interface AudioPlayerProps {
  uri: string;
}

// Type guard to check if status is AVPlaybackStatusSuccess
function isAVPlaybackStatusSuccess(status: AVPlaybackStatus): status is AVPlaybackStatusSuccess {
  return (status as AVPlaybackStatusSuccess).isLoaded === true;
}

/**
 * TODO: This component would need support for integrating plyaback controls.
 * For now, it just plays the audio file.
 * Recommended: react-native track player
 */
const AudioPlayer = (props: AudioPlayerProps) => {
  const { uri } = props;
  const [sound, setSound] = useState<Audio.Sound | undefined>(undefined);
  const [status, setStatus] = useState<AVPlaybackStatus | undefined>(undefined);

  useEffect(() => {
    async function loadSound() {
      const { sound } = await Audio.Sound.createAsync({ uri }, { shouldPlay: false });
      sound.setOnPlaybackStatusUpdate((status) => setStatus(status));
      setSound(sound);
    }
    loadSound();

    return () => {
      if (sound) {
        sound.unloadAsync();
      }
    };
  }, [uri]);

  if (status === undefined) {
    return <Typography fontWeight="500"> Loading ... </Typography>;
  }

  if (status && isAVPlaybackStatusSuccess(status) === false) {
    return <Typography fontWeight="500"> Audio Status error </Typography>;
  }

  const handlePlayPause = async () => {
    if (status) {
      if (status.isPlaying) {
        await sound?.pauseAsync();
      } else {
        await sound?.playAsync();
      }
    }
  };

  return (
    <View>
      {status.isPlaying ? (
        <Icon icon="eye" onPress={handlePlayPause} />
      ) : (
        <Icon icon="eyeOff" onPress={handlePlayPause} />
      )}
    </View>
  );
};

export default AudioPlayer;
