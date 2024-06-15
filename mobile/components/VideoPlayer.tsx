import React from "react";
import { Video, ResizeMode } from "expo-av";
import { View } from "tamagui";

interface VideoPlayerProps {
  uri: string;
}
const VideoPlayer = (props: VideoPlayerProps) => {
  const { uri } = props;
  const video = React.useRef<Video>(null);

  return (
    <Video
      ref={video}
      style={{ width: "100%", height: 500, alignSelf: "center" }}
      source={{ uri }}
      useNativeControls
      resizeMode={ResizeMode.CONTAIN}
      isLooping
    />
  );
};
export default VideoPlayer;
