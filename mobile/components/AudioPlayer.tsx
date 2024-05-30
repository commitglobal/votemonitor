import React, { useState, useEffect } from "react";
import { TouchableOpacity } from "react-native";
import { View } from "tamagui";
import { Audio } from "expo-av";
import { Icon } from "./Icon";

interface AudioPlayerProps {
  uri: string;
}

// TODO: Integrate react-native-track-player
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
        console.log("Unloading Sound");
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
      <TouchableOpacity onPress={handlePlayPause}>
        <Icon icon={status.isPlaying ? "eye" : "eyeOff"} />
        {/* <Slider value={[status.positionMillis || 0]} max={status.durationMillis || 1}>
          <Slider.Track>
            <Slider.TrackActive />
          </Slider.Track>
        </Slider> */}
      </TouchableOpacity>
    </View>
  );
};

export default AudioPlayer;
