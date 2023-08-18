# TCPTrainingListener
TCP socket client sample in C# to listen to neural network training events.  
When the training process is started, this sample is capable of connecting to the TCP server and listening to its events.
At the moment 4 types of messages are supported:  
- Iteration
- Predictions
- Statistics
- Error / Warning / Info (messages with text information that can be treated as logs)

# Launch  

