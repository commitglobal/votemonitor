import React from "react";
import { Video, ResizeMode } from "expo-av";
import { View } from "tamagui";

interface VideoPlayerProps {
  uri: string;
}
const VideoPlayer = (props: VideoPlayerProps) => {
  const { uri } = props;
  const video = React.useRef<Video>(null);
  const [_status, setStatus] = React.useState({});

  return (
    <View>
      <Video
        ref={video}
        style={{ width: "100%", height: 500, alignSelf: "center" }}
        source={{ uri }}
        useNativeControls
        resizeMode={ResizeMode.CONTAIN}
        isLooping
        onPlaybackStatusUpdate={(status) => setStatus(() => status)}
      />
    </View>
  );
};
export default VideoPlayer;
