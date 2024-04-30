import * as ImagePicker from "expo-image-picker";

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
};

export const useCamera = () => {
  const [status, requestPermission] = ImagePicker.useCameraPermissions();

  const uploadCameraOrMedia = async (
    type: "library" | "cameraPhoto" | "cameraVideo",
  ): Promise<FileMetadata | undefined> => {
    if (!status?.granted) {
      const requestedPermisison = await requestPermission();
      if (!requestedPermisison.granted) {
        console.error("Need permission to open camera");
        return;
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
      quality: 1,
      allowsMultipleSelection: false,
      videoQuality: ImagePicker.UIImagePickerControllerQualityType.Medium, // TODO: careful here, Medium might be enough
      cameraType: ImagePicker.CameraType.back,
    });

    if (result.canceled) {
      console.log("Closing camera... no picture");
      return;
    }

    console.log(result);

    if (result.assets[0]) {
      const file = result.assets[0];

      const filename = file.uri.split("/").pop() || "";

      const toReturn = {
        uri: result.assets[0].uri,
        name: filename,
        type: file.mimeType || "",
      };
      return toReturn;
    }
    return undefined;
  };

  return {
    uploadCameraOrMedia,
  };
};
