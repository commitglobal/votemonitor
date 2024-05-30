import React from "react";
import { Video, ResizeMode } from "expo-av";
import { View } from "tamagui";
import Button from "./Button";

interface VideoPlayerProps {
  uri: string;
}
const VideoPlayer = (props: VideoPlayerProps) => {
  const { uri } = props;
  const video = React.useRef<Video>(null);
  const [status, setStatus] = React.useState({});

  return (
    <View backgroundColor="red">
      <Video
        ref={video}
        style={{ flex: 1, alignSelf: "center" }}
        source={{ uri: uri }}
        useNativeControls
        resizeMode={ResizeMode.CONTAIN}
        isLooping
        onPlaybackStatusUpdate={(status) => setStatus(() => status)}
      />
      <View>
        <Button
          onPress={() =>
            (status as any).isPlaying ? video.current?.pauseAsync() : video.current?.playAsync()
          }
        >
          Play / Pause
        </Button>
      </View>
    </View>
  );
};

export default VideoPlayer;
