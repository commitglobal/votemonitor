import * as ImagePicker from "expo-image-picker";
import Toast from "react-native-toast-message";
import { Video, Image } from "react-native-compressor";
import * as Sentry from "@sentry/react-native";

/**
 * 
 * 
 *      USAGE EXAMPLE
 * 
 *    const { uploadCameraOrMedia } = useCamera();

      const { mutate } = addAttachmentMutation();

      const handleUpload = async (type: "library" | "camera") => {
        const cameraResult = await uploadCameraOrMedia(type);

        if (!cameraResult) {
          return;
        }

        mutate(
          {
            electionRoundId: "43b91c74-6d05-4fd1-bd93-dfe203c83c53",
            pollingStationId: "d3e6d2e9-0341-4dde-a58a-142a3f2dd19a",
            cameraResult,
          },
          {
            onSuccess: console.log,
            onError: console.log,
          },
        );
      };
 * 
 * 
 * 
 * 
 */

export type FileMetadata = {
  uri: string;
  name: string;
  type: string;
  size?: number;
};

export const useCamera = () => {
  const [status, requestPermission] = ImagePicker.useCameraPermissions();

  const uploadCameraOrMedia = async (
    type: "library" | "cameraPhoto" | "cameraVideo",
  ): Promise<FileMetadata | undefined | void> => {
    if (!status?.granted) {
      const requestedPermisison = await requestPermission();
      if (!requestedPermisison.granted) {
        return Toast.show({
          type: "error",
          text2:
            "Need permissions to open the camera or to use the photo library. Go to Settings -> VoteMonitor.",
          visibilityTime: 5000,
        });
      }
    }

    const luncher =
      type === "library" ? ImagePicker.launchImageLibraryAsync : ImagePicker.launchCameraAsync;

    const specifiedMediaType = { mediaTypes: ImagePicker.MediaTypeOptions.All };

    if (type === "cameraPhoto") {
      specifiedMediaType.mediaTypes = ImagePicker.MediaTypeOptions.Images;
    }

    if (type === "cameraVideo") {
      specifiedMediaType.mediaTypes = ImagePicker.MediaTypeOptions.Videos;
    }

    const result = await luncher({
      ...(specifiedMediaType || { mediaTypes: ImagePicker.MediaTypeOptions.All }),
      allowsEditing: true,
      aspect: [4, 3],
      quality: 0.1,
      allowsMultipleSelection: false,
      videoQuality: ImagePicker.UIImagePickerControllerQualityType.Low, // TODO: careful here, Medium might be enough
      cameraType: ImagePicker.CameraType.back,
    });

    if (result.canceled) {
      return;
    }

    const file = result.assets[0];
    if (file) {
      let resultCompression = file.uri;

      try {
        if (file.type === "image") {
          resultCompression = await Image.compress(file.uri);
        } else if (file.type === "video") {
          resultCompression = await Video.compress(file.uri, {}, (progress) => {
            console.log("Compression Progress: ", progress);
          });
        }
      } catch (err) {
        Sentry.captureException(err);
      }

      const filename = resultCompression.split("/").pop() || "";

      const toReturn = {
        uri: resultCompression,
        name: filename,
        type: file.mimeType || "",
        size: file.fileSize,
      };
      return toReturn;
    }
    return undefined;
  };

  return {
    uploadCameraOrMedia,
  };
};
