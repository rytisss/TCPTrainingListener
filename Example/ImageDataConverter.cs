using Newtonsoft.Json;
using OpenCvSharp;
using System;
using System.IO;

public class ImageDataConverter : JsonConverter<Mat>
{
    public override Mat ReadJson(JsonReader reader, Type objectType, Mat existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        string base64String = reader.Value.ToString();
        byte[] bytes = Convert.FromBase64String(base64String);
        MemoryStream imageStream = new MemoryStream(bytes);

        // Create an OpenCvSharp Mat object from the image stream
        Mat mat = Cv2.ImDecode(imageStream.ToArray(), ImreadModes.Unchanged);
        return mat;
    }

    public override void WriteJson(JsonWriter writer, Mat value, JsonSerializer serializer)
    {
        // Convert the Mat to a byte array
        byte[] bytes = value.ToBytes();

        // Convert the byte array to a base64 string
        string base64String = Convert.ToBase64String(bytes);

        // Write the base64 string to the JSON writer
        writer.WriteValue(base64String);
    }
}