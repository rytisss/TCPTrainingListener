using System;
using System.Text;
using WatsonTcp;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Communication;
using OpenCvSharp;

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
        int port = 5423;
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
            Console.WriteLine("Nothing more to do, closing application!");
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

        if (command == "Iteration")
        {
            string iterationPayload = json["Iteration"].ToString();
            ShowIterationInfo(iterationPayload);
        }
        else if (command == "Predictions")
        {
            string predictionsPayload = json["Predictions"].ToString();
            ShowPredictions(predictionsPayload);
        }
        else if (command == "Statistics")
        {
            string statisticsPayload = json["Statistics"].ToString();
            ShowStatistics(statisticsPayload);
        }
        else if (command == "Error")
        {
            string errorMessage = json["Message"].ToString();
            Console.WriteLine("[Error]: " + errorMessage);
        }
        else if (command == "Warning")
        {
            string warningMessage = json["Message"].ToString();
            Console.WriteLine("[Warning]: " + warningMessage);
        }
        else if (command == "Info")
        {
            string infoMessage = json["Message"].ToString();
            Console.WriteLine("[Info]: " + infoMessage);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="iterationPayload"></param>
    static void ShowIterationInfo(string iterationPayload)
    {
        IterationResults iterationsStatistics = JsonConvert.DeserializeObject<IterationResults>(iterationPayload);
        Console.WriteLine("-----------");
        Console.WriteLine("Model name: {0}", iterationsStatistics.model_name);
        Console.WriteLine("Iteration {0} / {1}, from epoch {2}", iterationsStatistics.iteration, iterationsStatistics.iteration_count, iterationsStatistics.epoch);
        foreach (var entry in iterationsStatistics.parameters)
        {
            Console.WriteLine($"{entry.Key}: {entry.Value}");
        }
        Console.WriteLine("-----------");
    }


    /// <summary>
    /// Extract / deserialized training statistics
    /// </summary>
    /// <param name="statisticsPayload">
    /// JSON as string
    /// </param>
    static void ShowStatistics(string statisticsPayload)
    {
        StatisticalResults statistics = JsonConvert.DeserializeObject<StatisticalResults>(statisticsPayload);
        Console.WriteLine("-----------");
        Console.WriteLine("Model name: {0}", statistics.model_name);
        Console.WriteLine("Received training parameters from epoch {0} / {1}", statistics.epoch, statistics.epoch_count);
        foreach (var entry in statistics.parameters)
        {
            Console.WriteLine($"{entry.Key}: {entry.Value}");
        }
        Console.WriteLine("-----------");
    }

    /// <summary>
    /// Extract / deserialized training prediction and show results in model windows
    /// </summary>
    /// <param name="predictionsPayload">
    /// JSON as string
    /// </param>
    static void ShowPredictions(string predictionsPayload)
    {
        // Deserialization
        var settings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter> { new PredictionConverter() }
        };
        var predictions = JsonConvert.DeserializeObject<PredictionInfo>(predictionsPayload, settings);
        // check type and do the following steps accodingly to deriviation of of prediction object
        if (predictions is SegmentationPrediction)
        {
            Console.WriteLine("Showing prediction from epoch {0}/{1}", predictions.epoch, predictions.epoch_count);
            Console.WriteLine("Model name: {0}", predictions.model_name);
            SegmentationPrediction segmentationPrediction = predictions as SegmentationPrediction;
            // show results in modal windows
            for (int i = 0; i < segmentationPrediction.predictions.Length; i++)
            {
                Cv2.ImShow("image", segmentationPrediction.predictions[i].image);
                Cv2.ImShow("label", segmentationPrediction.predictions[i].label);
                Cv2.ImShow("prediction", segmentationPrediction.predictions[i].prediction);
                Cv2.WaitKey(200); // wait for 200ms
            }
            Cv2.DestroyAllWindows();
            Console.WriteLine("Destroying model windows!");
        }
        else if (predictions is DetectionXAIPrediction)
        {
            Console.WriteLine("Showing prediction from epoch {0}/{1}", predictions.epoch, predictions.epoch_count);
            Console.WriteLine("Model name: {0}", predictions.model_name);
            DetectionXAIPrediction detectionXAIPrediction = predictions as DetectionXAIPrediction;
            // show results in modal windows
            for (int i = 0; i < detectionXAIPrediction.predictions.Length; i++)
            {
                Cv2.ImShow("image", detectionXAIPrediction.predictions[i].image);
                Console.WriteLine("Label - {0}, prediction - {1}", detectionXAIPrediction.predictions[i].label, detectionXAIPrediction.predictions[i].prediction);
                Cv2.WaitKey(200); // wait for 200ms
            }
            Cv2.DestroyAllWindows();
            Console.WriteLine("Destroying model windows!");
        }
    }
}
