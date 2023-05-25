using System;
using System.Text;
using WatsonTcp;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Communication;
using OpenCvSharp;
using System.Drawing.Printing;

public class CommandAndListeningSample
{
    /// <summary>
    /// Max message length in bytes, will set as it is taken from the post-initialized client
    /// </summary>
    public static int MaxMessageLength = 0;

    static void Main(string[] args)
    {
        // infinitive loop

        try
        {
            TrainingListeningSample();
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception in client sample: " + e.Message);
        }
    }

    static public void TrainingListeningSample()
    {
        // server address
        string serverIP = "127.0.0.1";
        int port = 5423; //Listen on different port than request for processing is made
        string serverAddress = serverIP + ":" + port.ToString();

        // create socket instance
        WatsonTcpClient client = new WatsonTcpClient(serverIP, port);
        
        // get max message length
        MaxMessageLength = client.Settings.StreamBufferSize;

        // set events
        client.Events.ServerConnected += OnConnect;
        client.Events.ServerDisconnected += OnDisconnect;
        client.Events.StreamReceived += OnDataStreamReceived;
        client.Events.ExceptionEncountered += OnException;

        // try to connect
        try
        {
            client.Connect();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine("Nothing more to do, closing application! Press key...");
            Console.ReadKey();
            return; // nothing more to do
        }

        for (;;)
        {
            Console.WriteLine("Press 'q' to quit");
            ConsoleKeyInfo keyInfo = Console.ReadKey();
            if (keyInfo.Key == ConsoleKey.Q)
            {
                Console.WriteLine("Exiting...");
                break;
            }
        }
        // disconnect
        if (client.Connected)
        {
            client.Disconnect();
        }
    }

    static void OnConnect(object sender, ConnectionEventArgs args)
    {
        Console.WriteLine("Server " + args.IpPort + " connected");
    }

    static void OnException(object sender, ExceptionEventArgs args)
    {
        Console.WriteLine("Exception " + args.Exception);
    }

    static void OnDisconnect(object sender, DisconnectionEventArgs e)
    {
        string reason = "";
        switch (e.Reason)
        {
            case DisconnectReason.Removed:
                reason = "kicked";
                break;
            case DisconnectReason.Normal:
                reason = "normal";
                break;
            case DisconnectReason.Timeout:
                reason = "timeout";
                break;
            case DisconnectReason.Shutdown:
                reason = "shutdown";
                break;
        }
        Console.WriteLine("Disconnected from server " + e.IpPort + ", reason - " + reason);
        Environment.Exit(1);
    }

    static void OnDataStreamReceived(object sender, StreamReceivedEventArgs args)
    {
        Console.WriteLine("Message from server [" + args.IpPort + "] received!");
        long bytesRemaining = args.ContentLength;

        byte[] buffer = new byte[MaxMessageLength];
        string message = "";
        using (MemoryStream ms = new MemoryStream())
        {
            int bytesRead = 0;
            while (bytesRemaining > 0)
            {
                bytesRead = args.DataStream.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    ms.Write(buffer, 0, bytesRead);
                    bytesRemaining -= bytesRead;
                }
            }
            message = Encoding.UTF8.GetString(ms.ToArray());
        }
        JObject json = JObject.Parse(message);
        if (!json.ContainsKey("Command"))
        {
            Console.WriteLine("Message does not contain 'Command' field!");
            return;
        }

        string command = (string)json["Command"];
        // Print command
        Console.WriteLine("Command: {0}", command);

        if (command == "Predictions")
        {
            string predictionsPayload = json["Predictions"].ToString();
            GetPredictions(predictionsPayload);
        }
        else if (command == "Statistics")
        {
            string statisticsPayload = json["Statistics"].ToString(); // take the field with values
            GetStatistics(statisticsPayload);
        }
    }

    /// <summary>
    /// Extract / deserialized training statistics
    /// </summary>
    /// <param name="statisticsPayload">
    /// JSON as string
    /// </param>
    static void GetStatistics(string statisticsPayload)
    {
        StatisticalResults statistics = JsonConvert.DeserializeObject<StatisticalResults>(statisticsPayload);
        Console.WriteLine("Received training parameters from epoch {0}", statistics.epoch);
        foreach (var entry in statistics.parameters)
        {
            Console.WriteLine($"{entry.Key}: {entry.Value}");
        }
    }

    /// <summary>
    /// Extract / deserialized training prediction and show results in model windows
    /// </summary>
    /// <param name="predictionsPayload">
    /// JSON as string
    /// </param>
    static void GetPredictions(string predictionsPayload)
    {
        // Deserialization
        var settings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter> { new PredictionConverter() }
        };
        var predictions = JsonConvert.DeserializeObject<PredictionResults>(predictionsPayload, settings);
        // check type and do the following steps accodingly
        // TODO: more TBD with more options
        if (predictions is SegmentationPredictionResults)
        {
            Console.WriteLine("Showing prediction from epoch {0}...", predictions.epoch);
            SegmentationPredictionResults segmentationPrediction = predictions as SegmentationPredictionResults;
            // show results in modal windows
            for (int i = 0; i < segmentationPrediction.predictions.Length; i++)
            {
                Cv2.ImShow("image", segmentationPrediction.predictions[i].image);
                Cv2.ImShow("label", segmentationPrediction.predictions[i].label);
                Cv2.ImShow("prediction", segmentationPrediction.predictions[i].prediction);
                Cv2.WaitKey(1000); // wait for 1000ms
                // show first 5
                if (i >= 5)
                {
                    break;
                }
            }
            Cv2.DestroyAllWindows();
            Console.WriteLine("Destroying model windows!");
        }
    }
}
