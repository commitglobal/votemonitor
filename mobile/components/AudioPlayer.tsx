import React, { useState, useEffect } from "react";
import { View } from "tamagui";
import { Audio } from "expo-av";
import { Icon } from "./Icon";

interface AudioPlayerProps {
  uri: string;
}

/**
 * TODO: This component would need support for integrating plyaback controls.
 * For now, it just plays the audio file.
 * Recommended: react-native track player
 */
const AudioPlayer = (props: AudioPlayerProps) => {
  const { uri } = props;
  const [sound, setSound] = useState<Audio.Sound | undefined>(undefined);
  const [status, setStatus] = useState<any>({});

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

  const handlePlayPause = async () => {
    if (status.isPlaying) {
      await sound?.pauseAsync();
    } else {
      await sound?.playAsync();
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
